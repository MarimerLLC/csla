using Csla.Blazor.Test.Fakes;
using Csla.Test;
using Csla.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Blazor.Test
{
  [TestClass]
  public class ViewModelSaveAsyncErrorTests
  {
    private static TestDIContext _testDIContext;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
      _testDIContext = TestDIContextFactory.CreateDefaultContext();
    }

    [TestMethod]
    public async Task SavingFailure_ErrorEventIsInvoked()
    {
      // Arrange
      var person = GetValidFakePerson();
      var appCntxt = _testDIContext.CreateTestApplicationContext();
      var vm = new ViewModel<FakePerson>(appCntxt)
      {
        ManageObjectLifetime = true,
        Model = person,
      };
      SetupErrorHandler(vm);
      TestResults.Add("FakePerson", string.Empty);
      // The second insert with the same key will result in an exception, so the insert will fail

      // Act
      await vm.SaveAsync();

      // Assert
      Assert.AreEqual("Invoked", TestResults.GetResult("ErrorHandler"));
      Assert.IsNotNull(vm.Exception);
      Assert.IsNotNull(vm.ViewModelErrorText);
    }

    [TestMethod]
    public async Task SavingSuccess_ErrorEventIsNotInvoked()
    {
      // Arrange
      var person = GetValidFakePerson();
      var appCntxt = _testDIContext.CreateTestApplicationContext();
      var vm = new ViewModel<FakePerson>(appCntxt)
      {
        ManageObjectLifetime = true,
        Model = person,
      };
      SetupErrorHandler(vm);

      // Act
      await vm.SaveAsync();

      // Assert
      Assert.AreEqual("Inserted", TestResults.GetResult("FakePerson"));
      Assert.AreEqual(string.Empty, TestResults.GetResult("ErrorHandler"));
      Assert.IsNull(vm.Exception);
      Assert.IsNull(vm.ViewModelErrorText);
    }

    #region Helper Methods

    private void SetupErrorHandler(ViewModel<FakePerson> vm)
    {
      vm.Error += () =>
      {
        TestResults.Add("ErrorHandler", "Invoked");
      };
    }

    FakePerson GetValidFakePerson()
    {
      IDataPortal<FakePerson> dataPortal;
      FakePerson person;

      // Create an instance of a DataPortal that can be used for instantiating objects
      dataPortal = _testDIContext.CreateDataPortal<FakePerson>();
      person = dataPortal.Create();

      person.FirstName = "John";
      person.LastName = "Smith";
      person.HomeTelephone = "01234 567890";

      return person;
    }

    #endregion

  }

}
