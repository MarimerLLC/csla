using Microsoft.VisualStudio.TestTools.UnitTesting;
using Csla.Blazor.Test.Fakes;
using Microsoft.AspNetCore.Components.Forms;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using System;
using Csla.TestHelpers;
using System.Threading.Tasks;
using Csla.Core;

namespace Csla.Blazor.Test
{
  public class MyViewModel<T> : ViewModel<T>
  {
    public MyViewModel(ApplicationContext context) : base(context) { ManageObjectLifetime = true; }

    public void Cancel()
    {
      DoCancel();
    }
  }

  [TestClass]
  public class ViewModelEditChildListSaveEditLevelTests
  {
    private static TestDIContext _testDIContext;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
      _testDIContext = TestDIContextFactory.CreateDefaultContext();
    }

    [TestMethod]
    public async Task SaveModelChildListChange_ValidateEditLevel()
    {
      // Arrange
      FakePerson person = GetValidFakePerson();
      //EditContext editContext = new EditContext(person);
      //editContext.AddCslaValidation();
      var iuo = person as IUndoableObject;
      var appCntxt = TestDIContextExtensions.CreateTestApplicationContext(_testDIContext);
      var vm = new ViewModel<FakePerson>(appCntxt);
      vm.ManageObjectLifetime = true;
      vm.Model = person;

      Assert.IsTrue(iuo.EditLevel > 0);
      await vm.SaveAsync();
      Assert.IsTrue(iuo.EditLevel == 1);

      // Act
      var eaddr = person.EmailAddresses.AddNew();
      eaddr.EmailAddress = "test@test.ca";
      Assert.IsTrue(iuo.EditLevel > 0);
      await vm.SaveAsync();

      // Assert
      Assert.IsTrue(iuo.EditLevel == 1);

    }

    [TestMethod]
    public async Task SaveThenCancel_ValidatePropertyValue()
    {
      // Arrange
      //FakePerson person = GetValidFakePerson();
      //var iuo = person as IUndoableObject;
      var appCntxt = TestDIContextExtensions.CreateTestApplicationContext(_testDIContext);
      var vm = new MyViewModel<FakePerson>(appCntxt);
      await vm.RefreshAsync(FetchFakePerson);

      // Act
      await vm.SaveAsync();
   
      // Assert
      Assert.IsFalse(vm.Model.IsSavable);
      Assert.IsFalse(vm.Model.IsDirty);
      Assert.IsFalse(vm.Model.IsNew);

      // Act
      string firstName = "SaveThis";
      vm.Model.FirstName = firstName;
      await vm.SaveAsync();

      // Assert
      Assert.IsFalse(vm.Model.IsSavable);
      Assert.IsFalse(vm.Model.IsDirty);
      Assert.IsFalse(vm.Model.IsNew);
      Assert.IsTrue(vm.Model.FirstName == firstName);

      // Act
      string cancelName = "Cancel This";
      vm.Model.FirstName = cancelName;
      vm.Cancel();

      // Assert
      Assert.IsFalse(vm.Model.IsSavable);
      Assert.IsFalse(vm.Model.IsDirty);
      Assert.IsFalse(vm.Model.IsNew);

      Assert.IsTrue(vm.Model.FirstName == firstName);

    }

    #region Helper Methods

    async Task<FakePerson> FetchFakePerson()
    {
      FakePerson person = GetValidFakePerson();
      return await Task.FromResult(person);
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

    private string ConcatenateMessages(IEnumerable<string> messages)
    {
      return string.Join("; ", messages);
    }

    #endregion

  }
}
