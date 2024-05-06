using Csla.Blazor.Test.Fakes;
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
        Model = person,
      };
      var error = false;
      vm.Error += () =>
      {
        error = true;
        // these fields should be populated when Error event is triggered
        Assert.IsNotNull(vm.Exception);
        Assert.IsNotNull(vm.ViewModelErrorText);
      };
      person.FirstName = FakePerson.FirstNameFailOnInsertValue;

      // Act
      await vm.SaveAsync();

      // Assert
      Assert.IsTrue(error); // Error event should have been triggered
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
        Model = person,
      };
      var error = false;
      vm.Error += () =>
      {
        error = true;
      };

      // Act
      await vm.SaveAsync();

      // Assert
      Assert.IsFalse(error); // Error event shouldn't have been triggered
      Assert.IsNull(vm.Exception);
      Assert.IsNull(vm.ViewModelErrorText);
    }

    #region Helper Methods

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
