using Csla.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.Threading
{
  [TestClass]
  public class AsyncManualResetEventTests
  {

    #region SetCancellationToken

    [TestMethod]
    public void SetCancellationToken_CancellationTokenIsCancelled_TaskIsCancelled()
    {
      // Arrange
      var resetEvent = new AsyncManualResetEvent();
      var cancellationTokenSource = new CancellationTokenSource();
      var cancellationToken = cancellationTokenSource.Token;

      // Act
      resetEvent.SetCancellationToken(cancellationToken);
      cancellationTokenSource.Cancel();

      // Assert
      Assert.IsTrue(resetEvent.WaitAsync().IsCanceled);
    }

    [TestMethod]
    public void SetCancellationToken_CancellationTokenIsNotCancelled_TaskIsNotCancelled()
    {
      // Arrange
      var resetEvent = new AsyncManualResetEvent();
      var cancellationTokenSource = new CancellationTokenSource();
      var cancellationToken = cancellationTokenSource.Token;

      // Act
      resetEvent.SetCancellationToken(cancellationToken);

      // Assert
      Assert.IsFalse(resetEvent.WaitAsync().IsCanceled);
    }

    #endregion

    [TestMethod]
    public async Task WaitAsync_ShouldWaitUntilSet()
    {
      // Arrange
      var resetEvent = new AsyncManualResetEvent();

      // Act
      var task = Task.Run(async () =>
      {
        await Task.Delay(100);
        resetEvent.Set();
      });

      var taskr = resetEvent.WaitAsync();
      await taskr;

      // Assert
      Assert.IsTrue(taskr.IsCompleted);
    }

    [TestMethod]
    public async Task SetCancellationToken_ShouldCancelWaitAsync()
    {
      // Arrange
      var resetEvent = new AsyncManualResetEvent();
      var cancellationTokenSource = new CancellationTokenSource();
      resetEvent.SetCancellationToken(cancellationTokenSource.Token);

      // Act
      var task = Task.Run(async () =>
      {
        await Task.Delay(100);
        cancellationTokenSource.Cancel();
      });

      // Assert
      await Assert.ThrowsExceptionAsync<TaskCanceledException>(async () =>
      {
        await resetEvent.WaitAsync();
      });
    }
  }
}
