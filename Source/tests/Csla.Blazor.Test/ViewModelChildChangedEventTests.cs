using Microsoft.VisualStudio.TestTools.UnitTesting;
using Csla.Blazor.Test.Fakes;
using Csla.TestHelpers;
using System.Threading.Tasks;
using Csla.Core;

namespace Csla.Blazor.Test
{
  [TestClass]
  public class ViewModelChildChangedEventTests
  {
    private static TestDIContext _testDIContext;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
      _testDIContext = TestDIContextFactory.CreateDefaultContext();
    }

    [TestCleanup]
    public void CleanupTests()
    {
      FakeDataStorage.ClearDataStorage();
    }

    [TestMethod]
    public void ViewModel_WithBusinessBase_ShouldHookChildChangedEvent()
    {
      // Arrange
      var appContext = TestDIContextExtensions.CreateTestApplicationContext(_testDIContext);
      var vm = new ViewModel<FakePerson>(appContext);
      bool childChangedCalled = false;
      vm.ModelChildChanged += (sender, e) => childChangedCalled = true;

      // Act - Create and set a FakePerson model (BusinessBase)
      IDataPortal<FakePerson> dataPortal = _testDIContext.CreateDataPortal<FakePerson>();
      var person = dataPortal.Create();
      vm.Model = person;

      // Trigger child changed by modifying email addresses
      var emailAddress = person.EmailAddresses.AddNew();
      emailAddress.EmailAddress = "test@example.com";

      // Assert
      Assert.IsTrue(childChangedCalled, "ModelChildChanged event should be raised for BusinessBase");
    }

    [TestMethod]
    public void ViewModel_WithBusinessListBase_ShouldHookChildChangedEvent()
    {
      // Arrange
      var appContext = TestDIContextExtensions.CreateTestApplicationContext(_testDIContext);
      var vm = new ViewModel<FakePersonEmailAddresses>(appContext);
      bool childChangedCalled = false;
      vm.ModelChildChanged += (sender, e) => childChangedCalled = true;

      // Act - Create and set a FakePersonEmailAddresses model (BusinessListBase)
      IChildDataPortal<FakePersonEmailAddresses> dataPortal = _testDIContext.CreateChildDataPortal<FakePersonEmailAddresses>();
      var emailAddresses = dataPortal.CreateChild();
      vm.Model = emailAddresses;

      // Trigger child changed by adding an item
      var emailAddress = emailAddresses.AddNew();
      emailAddress.EmailAddress = "test@example.com";

      // Assert
      Assert.IsTrue(childChangedCalled, "ModelChildChanged event should be raised for BusinessListBase");
    }

    [TestMethod]
    public void ViewModel_WhenModelChanged_ShouldUnhookOldAndHookNewChildChangedEvent()
    {
      // Arrange
      var appContext = TestDIContextExtensions.CreateTestApplicationContext(_testDIContext);
      var vm = new ViewModel<FakePersonEmailAddresses>(appContext);
      int childChangedCount = 0;
      vm.ModelChildChanged += (sender, e) => childChangedCount++;

      IChildDataPortal<FakePersonEmailAddresses> dataPortal = _testDIContext.CreateChildDataPortal<FakePersonEmailAddresses>();
      var emailAddresses1 = dataPortal.CreateChild();
      var emailAddresses2 = dataPortal.CreateChild();

      // Act - Set first model
      vm.Model = emailAddresses1;
      var email1 = emailAddresses1.AddNew();
      email1.EmailAddress = "test1@example.com";

      // Change model
      vm.Model = emailAddresses2;
      var email2 = emailAddresses2.AddNew();
      email2.EmailAddress = "test2@example.com";

      // Modify old model - should NOT trigger event
      var email3 = emailAddresses1.AddNew();
      email3.EmailAddress = "test3@example.com";

      // Assert
      Assert.AreEqual(2, childChangedCount, "ModelChildChanged should only be raised for current model");
    }
  }
}
