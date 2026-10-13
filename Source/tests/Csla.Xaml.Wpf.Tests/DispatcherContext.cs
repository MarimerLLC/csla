//-----------------------------------------------------------------------
// <copyright file="DispatcherContext.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Runs test code on a WPF dispatcher thread</summary>
//-----------------------------------------------------------------------

using System.Runtime.ExceptionServices;
using System.Windows.Threading;

namespace Csla.Xaml.Wpf.Tests
{
  /// <summary>
  /// Runs test code on an STA thread with a WPF Dispatcher and
  /// DispatcherSynchronizationContext, as a WPF UI thread would,
  /// so async continuations return to the thread that owns
  /// the DependencyObjects under test.
  /// </summary>
  internal static class DispatcherContext
  {
    public static void Run(Func<Task> test)
    {
      ExceptionDispatchInfo failure = null;
      var thread = new Thread(() =>
      {
        var dispatcher = Dispatcher.CurrentDispatcher;
        // exceptions from async void methods are raised on the dispatcher
        dispatcher.UnhandledException += (_, e) =>
        {
          failure ??= ExceptionDispatchInfo.Capture(e.Exception);
          e.Handled = true;
        };
        SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(dispatcher));

        var frame = new DispatcherFrame();
        var task = test();
        task.ContinueWith(t =>
        {
          if (t.Exception != null)
            failure ??= ExceptionDispatchInfo.Capture(t.Exception.InnerException ?? t.Exception);
          frame.Continue = false;
        }, TaskScheduler.FromCurrentSynchronizationContext());
        Dispatcher.PushFrame(frame);
        dispatcher.InvokeShutdown();
      });
      thread.SetApartmentState(ApartmentState.STA);
      thread.Start();
      thread.Join();
      failure?.Throw();
    }

    /// <summary>
    /// Waits, while still pumping the dispatcher, until
    /// the condition is true.
    /// </summary>
    public static async Task WaitUntilAsync(Func<bool> condition, int timeoutMilliseconds = 5000)
    {
      var start = Environment.TickCount64;
      while (!condition())
      {
        if (Environment.TickCount64 - start > timeoutMilliseconds)
          throw new TimeoutException("Condition was not met before the timeout");
        await Task.Delay(10);
      }
    }
  }
}
