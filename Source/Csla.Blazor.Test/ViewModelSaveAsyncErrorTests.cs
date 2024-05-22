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
      var error = 0;
      vm.Error += (o, e) =>
      {
        error++;
        Assert.IsNotNull(e.Error);
        // these fields should be populated when Error event is triggered
        Assert.IsNotNull(vm.Exception);
        Assert.IsNotNull(vm.ViewModelErrorText);
        Assert.AreSame(e.Error, vm.Exception);
        Assert.AreSame(o, vm);
      };
      person.FirstName = FakePerson.FirstNameFailOnInsertValue;

      // Act
      await vm.SaveAsync();

      // Assert
      Assert.AreEqual(1, error); // Error event should have been triggered only once
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
      vm.Error += (o, e) =>
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

    [TestMethod]
    public async Task SavingSuccess_BusyHelper()
    {
      // Arrange
      IDataPortal<Person> dataPortal;
      Person person;

      // Create an instance of a DataPortal that can be used for instantiating objects
      dataPortal = _testDIContext.CreateDataPortal<Person>();
      person = dataPortal.Create();
      person.Name = "TestTest";
      
      var appCntxt = _testDIContext.CreateTestApplicationContext();
      var vm = new ViewModel<Person>(appCntxt)
      {
        Model = person,
        BusyTimeout = TimeSpan.FromSeconds(3)
      };

      await vm.SaveAsync();
      Assert.IsNull(vm.Exception);
    }

    [TestMethod]
    public async Task SavingFailure_ErrorTimeOut()
    {
      // Arrange
      IDataPortal<Person> dataPortal;
      Person person;

      // Create an instance of a DataPortal that can be used for instantiating objects
      dataPortal = _testDIContext.CreateDataPortal<Person>();
      person = dataPortal.Create();
      person.Name = "TestTest";

      var appCntxt = _testDIContext.CreateTestApplicationContext();
      var vm = new ViewModel<Person>(appCntxt)
      {
        Model = person,
        BusyTimeout = TimeSpan.FromSeconds(1)
      };

      await vm.SaveAsync();
      Assert.IsNotNull(vm.Exception);
      Assert.AreEqual(vm.Exception.Message, "Csla.Blazor.Test.Person.SaveAsync - 00:00:01.");
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
