using Csla.Blazor.Test.Fakes;
using Csla.Core;
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
    public async Task SavingWithCancellationToken_Success()
    {
      // Arrange
      var person = GetValidFakePerson();
      var appCntxt = _testDIContext.CreateTestApplicationContext();
      var vm = new ViewModel<FakePerson>(appCntxt)
      {
        Model = person,
      };
      var cancellationToken = new CancellationToken();

      // Act
      await vm.SaveAsync(cancellationToken);

      // Assert

      Assert.IsNull(vm.Exception);
      Assert.IsNull(vm.ViewModelErrorText);
    }

    [TestMethod]
    public async Task SavingWithCancelledCancellationToken_ErrorEventIsInvoked()
    {

      // Arrange
      var person = new FakeBusy();
      person.IsBusy = true;
      person.IsSavable = false;

      var appCntxt = _testDIContext.CreateTestApplicationContext();
      var vm = new ViewModel<FakeBusy>(appCntxt)
      {
        Model = person,
      };

      var cancellationTokenSource = new CancellationTokenSource();
      cancellationTokenSource.Cancel();
      // Act
      await vm.SaveAsync(cancellationTokenSource.Token);

      // Assert
      Assert.IsNotNull(vm.Exception);
      Assert.IsNotNull(vm.ViewModelErrorText);
    }


    private class FakeBusy : Core.ITrackStatus
    {
      public FakeBusy()
      {
      }

      public bool IsValid { get; set; }

      public bool IsSelfValid { get; set; }

      public bool IsDirty { get; set; }

      public bool IsSelfDirty { get; set; }

      public bool IsDeleted { get; set; }

      public bool IsNew { get; set; }

      public bool IsSavable { get; set; }

      public bool IsChild { get; set; }

      private bool _IsBusy;
      public bool IsBusy
      {
        get
        {
          UnhandledAsyncException?.Invoke(this, new Core.ErrorEventArgs(this, new Exception()));
          return _IsBusy;
        }
        set
        {
          _IsBusy = value;
          BusyChanged?.Invoke(value, new BusyChangedEventArgs(nameof(IsBusy), value));
        }
      }

      public bool IsSelfBusy { get; set; }

      public event BusyChangedEventHandler BusyChanged;
      public event EventHandler<Core.ErrorEventArgs> UnhandledAsyncException;
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
