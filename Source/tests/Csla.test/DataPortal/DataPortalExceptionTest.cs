using Csla.TestHelpers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.DataPortal
{
  [TestClass]
  public class DataPortalExceptionTests
  {
    private static TestDIContext _testDIContext;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
      _testDIContext = TestDIContextFactory.CreateDefaultContext();
    }

    [TestMethod]
    [TestCategory("SkipWhenLiveUnitTesting")]
    public async Task ChildInnerExceptionFlowsFromDataPortal()
    {
      IDataPortal<EditableRoot1> dataPortal = _testDIContext.CreateDataPortal<EditableRoot1>();

      try
      {
        var bo = dataPortal.Create();

        await bo.SaveAsync();
      }
      catch (DataPortalException e)
      {
        Assert.IsInstanceOfType(e.BusinessException, typeof(CustomException));
      }
    }

    [TestMethod]
    [TestCategory("SkipWhenLiveUnitTesting")]
    public async Task ChildInnerExceptionDoesNotGetUnpackedButFlowsFromDataPortal()
    {
      IDataPortal<EditableRoot1> dataPortal = _testDIContext.CreateDataPortal<EditableRoot1>();

      var bo = dataPortal.Create();
      bo.Child.ThrowWithInnerException = true;

      var exception = await FluentActions.Awaiting(() => bo.SaveAsync()).Should().ThrowAsync<DataPortalException>();
      exception.Which.BusinessException.Should().BeOfType<CustomException>();
    }

    [TestMethod]
    [TestCategory("SkipWhenLiveUnitTesting")]
    public async Task ChildCreationExceptionFlowsFromDataPortalAsync()
    {
      IDataPortal<EditableRoot1> dataPortal = _testDIContext.CreateDataPortal<EditableRoot1>();
      string expected = "A suitable constructor for type 'Csla.Test.DataPortal.EditableChild2' could not be located.";

      try
      {
        var bo = await dataPortal.FetchAsync();
      }
      catch (DataPortalException e)
      {
        Assert.AreEqual(e.BusinessException.Message.Substring(0, expected.Length), expected);
      }
    }
  }

  [Serializable]
  public class EditableRoot1 : BusinessBase<EditableRoot1>
  {
    public static readonly PropertyInfo<EditableChild1> ChildProperty =
        RegisterProperty<EditableChild1>(c => c.Child);

    public EditableChild1 Child
    {
      get { return GetProperty(ChildProperty); }
      private set { LoadProperty(ChildProperty, value); }
    }

    [RunLocal]
    [Create]
    protected void DataPortal_Create([Inject] IChildDataPortal<EditableChild1> childDataPortal)
    {
      using (BypassPropertyChecks)
      {
        LoadProperty(ChildProperty, childDataPortal.CreateChild());
      }
      BusinessRules.CheckRules();
    }

    [Fetch]
    protected async Task DataPortal_FetchAsync([Inject] IChildDataPortal<EditableChild2> childDataPortal)
    {
      using (BypassPropertyChecks)
      {
        LoadProperty(ChildProperty, await childDataPortal.FetchChildAsync(1));
      }
      BusinessRules.CheckRules();
    }

    [Transactional(TransactionalTypes.TransactionScope)]
    [Insert]
    protected void DataPortal_Insert()
    {
      using (BypassPropertyChecks)
      {
        FieldManager.UpdateChildren(this);
      }
    }
  }

  [Serializable, CslaImplementProperties]
  public partial class EditableChild1 : BusinessBase<EditableChild1>
  {
    public partial bool ThrowWithInnerException { get; set; }

    #region Data Access

    [CreateChild]
    protected override void Child_Create()
    {
      using (BypassPropertyChecks)
      {
      }
      base.Child_Create();
    }

    [FetchChild]
    private Task Child_FetchAsync(int id)
    {
      using (BypassPropertyChecks)
      {
        throw new CustomException("Fetch not allowed");
      }
    }

    [InsertChild]
    private void Child_Insert(object parent)
    {
      using (BypassPropertyChecks)
      {
        throw new CustomException("Insert not allowed", ThrowWithInnerException ? new InvalidOperationException() : null);
      }
    }

    #endregion

  }

  [Serializable]
  public class EditableChild2 : EditableChild1
  {
    private EditableChild2()
    {
      // Disallow creation of the object, to test what happens in this scenario
    }

    [Fetch]
    private Task Child_FetchAsync(int id)
    {
      return Task.CompletedTask;
    }
  }

  [Serializable]
  public class CustomException : Exception
  {
    public CustomException(string message) : base(message) { }

    public CustomException(string message, Exception? innerException) : base(message, innerException) { }
  }
}