using Csla.Core;
using Csla.Serialization.Mobile;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Server.Tests
{
  [TestClass]
  public class WaitForIdleTests
  {


    [TestMethod]
    public async Task WaitForIdle_Should_WaitForIdle()
    {
      // Arrange
      var target = new FakeBusinessBase
      {
        new FakeEntity { IsBusy = false }
      };

      TimeSpan timeout = TimeSpan.FromSeconds(5);

      // Act
      await target.WaitForIdle(timeout);

      // Assert
      Assert.IsTrue(true);
      // Add assertion for idle state
    }

    [TestMethod]
    public async Task WaitForIdle_Should_WaitForIdleWithCancellation()
    {
      // Arrange
      var target = new FakeBusinessBase
      {
        new FakeEntity { IsBusy = false }
      };
      CancellationToken cancellationToken = new CancellationToken();

      // Act
      await target.WaitForIdle(cancellationToken);

      // Assert
      // Add assertion for idle state
      Assert.IsTrue(true);
    }

    [TestMethod]
    public async Task WaitForIdle_Should_ThrowTimeoutExeption()
    {
      // Arrange
      var target = new FakeBusinessBase
      {
        new FakeEntity { IsBusy = true }
      };

      TimeSpan timeout = TimeSpan.FromSeconds(0);
      // Assert

      await Assert.ThrowsExceptionAsync<TimeoutException>(() => target.WaitForIdle(timeout));
      // Add assertion for idle state
    }

    [TestMethod]
    public async Task WaitForIdle_Should_ThrowCancelledTaskException()
    {
      // Arrange
      var target = new FakeBusinessBase
      {
        new FakeEntity { IsBusy = true }
      };

      var cancellationToken = new CancellationTokenSource();
      cancellationToken.Cancel();

      // Assert
      // Add assertion for idle state
      await Assert.ThrowsExceptionAsync<TaskCanceledException>(() => target.WaitForIdle(cancellationToken.Token));

    }



  }

  public class FakeBusinessBase : DynamicListBase<FakeEntity>
  {

  }

  public class FakeEntity : IEditableBusinessObject, IUndoableObject, ISavable, IMobileObject, IBusinessObject
  {
    public int EditLevelAdded { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public int Identity => throw new NotImplementedException();

    public int EditLevel => throw new NotImplementedException();

    public bool IsValid => throw new NotImplementedException();

    public bool IsSelfValid => throw new NotImplementedException();

    public bool IsDirty => throw new NotImplementedException();

    public bool IsSelfDirty => throw new NotImplementedException();

    public bool IsDeleted => throw new NotImplementedException();

    public bool IsNew => throw new NotImplementedException();

    public bool IsSavable => throw new NotImplementedException();

    public bool IsChild => throw new NotImplementedException();

    public bool IsBusy { get; set; }

    public bool IsSelfBusy => throw new NotImplementedException();

    public event BusyChangedEventHandler BusyChanged;
    public event EventHandler<Core.ErrorEventArgs> UnhandledAsyncException;
    public event EventHandler<SavedEventArgs> Saved;

    public void AcceptChanges(int parentEditLevel, bool parentBindingEdit) => throw new NotImplementedException();
    public void ApplyEdit() => throw new NotImplementedException();
    public void BeginEdit() => throw new NotImplementedException();
    public void CancelEdit() => throw new NotImplementedException();
    public void CopyState(int parentEditLevel, bool parentBindingEdit) => throw new NotImplementedException();
    public void Delete() => throw new NotImplementedException();
    public void DeleteChild() => throw new NotImplementedException();
    public void GetChildren(SerializationInfo info, MobileFormatter formatter) => throw new NotImplementedException();
    public void GetState(SerializationInfo info) => throw new NotImplementedException();
    public object Save() => throw new NotImplementedException();
    public object Save(bool forceUpdate) => throw new NotImplementedException();
    public Task SaveAndMergeAsync() => throw new NotImplementedException();
    public Task SaveAndMergeAsync(bool forceUpdate) => throw new NotImplementedException();
    public Task<object> SaveAsync() => throw new NotImplementedException();
    public Task<object> SaveAsync(bool forceUpdate) => throw new NotImplementedException();
    public void SaveComplete(object newObject) => throw new NotImplementedException();
    public void SetChildren(SerializationInfo info, MobileFormatter formatter) => throw new NotImplementedException();
    public void SetParent(IParent parent) { }
    public void SetState(SerializationInfo info) => throw new NotImplementedException();
    public void UndoChanges(int parentEditLevel, bool parentBindingEdit) => throw new NotImplementedException();
  }

}
