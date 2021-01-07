using System;
using Csla;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.DataPortal
{
  [TestClass]
  public class DataPortalExceptionTests
  {
    [TestMethod]
    [TestCategory("SkipWhenLiveUnitTesting")]
    public void ChildInnerExceptionFlowsFromDataPortal()
    {
      try
      {
        var bo = EditableRoot1.New();

        bo.Save();
      }
      catch (DataPortalException e)
      {
        Assert.IsInstanceOfType(e.BusinessException, typeof(InvalidOperationException));
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

    public static EditableRoot1 New()
    {
      return Csla.DataPortal.Create<EditableRoot1>();
    }

    [RunLocal]
    protected override void DataPortal_Create()
    {
      using (BypassPropertyChecks)
      {
        Child = EditableChild1.New();
      }
      base.DataPortal_Create();
    }

    [Transactional(TransactionalTypes.TransactionScope)]
    protected override void DataPortal_Insert()
    {
      using (BypassPropertyChecks)
      {
        FieldManager.UpdateChildren(this);
      }
    }
  }

  [Serializable]
  public class EditableChild1 : BusinessBase<EditableChild1>
  {
    #region Factory Methods

    internal static EditableChild1 New()
    {
      return Csla.DataPortal.CreateChild<EditableChild1>();
    }

    #endregion

    #region Data Access

    protected override void Child_Create()
    {
      using (BypassPropertyChecks)
      {
      }
      base.Child_Create();
    }

    private void Child_Insert(object parent)
    {
      using (BypassPropertyChecks)
      {
        throw new InvalidOperationException("Insert not allowed");
      }
    }

    #endregion
  }
}