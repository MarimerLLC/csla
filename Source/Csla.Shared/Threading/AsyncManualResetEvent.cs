#if !NET40
//-----------------------------------------------------------------------
// <copyright file="AsyncManualResetEvent.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary></summary>
//-----------------------------------------------------------------------
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Csla.Threading
{
  /// <summary>
  /// ManualResetEvent that works with async/await.
  /// </summary>
  /// <remarks>
  /// Based on a blog post by Stephen Toub.
  /// https://blogs.msdn.microsoft.com/pfxteam/2012/02/11/building-async-coordination-primitives-part-1-asyncmanualresetevent/
  /// </remarks>
  public class AsyncManualResetEvent
  {
    private volatile TaskCompletionSource<bool> _completionSource =
      new TaskCompletionSource<bool>();

    /// <summary>
    /// Wait for the event to be set.
    /// </summary>
    public Task WaitAsync() { return _completionSource.Task; }

    /// <summary>
    /// Wait for the event to be set.
    /// </summary>
    /// <param name="t">Timeout value</param>
    public Task WaitAsync(TimeSpan t)
    {
      var tasks = new Task[] { _completionSource.Task, Task.Delay(t) };
      var index = Task.WaitAny(tasks);
      if (index == 1)
        throw new TimeoutException();
      return tasks[0];
    }

    /// <summary>
    /// Wait synchronously (spinwait) for the event to be set.
    /// </summary>
    public void Wait()
    {
      while (!_completionSource.Task.IsCompleted)
      {
        Thread.Sleep(1);
      }
    }

    /// <summary>
    /// Wait synchronously (spinwait) for the event to be set.
    /// </summary>
    /// <param name="t">Timeout value</param>
    public void Wait(TimeSpan t)
    {
      var timeout = DateTime.Now + t;
      while (!_completionSource.Task.IsCompleted)
      {
        if (DateTime.Now > timeout)
          throw new TimeoutException();
        Thread.Sleep(1);
      }
    }

    /// <summary>
    /// Sets the event, releasing waiters.
    /// </summary>
    public void Set()
    {
      _completionSource.TrySetResult(true);
    }

    /// <summary>
    /// Resets the event.
    /// </summary>
    public void Reset()
    {
      while (true)
      {
        var currentCompletionSource = _completionSource;
        if (!currentCompletionSource.Task.IsCompleted ||
            Interlocked.CompareExchange(ref _completionSource, new TaskCompletionSource<bool>(), currentCompletionSource) == currentCompletionSource)
          return;
      }
    }
  }
}
#endif