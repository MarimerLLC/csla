using FluentAssertions;
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
      target.IsBusy.Should().BeFalse();
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
      target.IsBusy.Should().BeFalse();
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
}
