//-----------------------------------------------------------------------
// <copyright file="AsyncManualResetEvent.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Async/await implementation of a ManualResetEvent</summary>
//-----------------------------------------------------------------------

// Code from Stephen Toub @ Microsoft
// https://blogs.msdn.microsoft.com/pfxteam/2012/02/11/building-async-coordination-primitives-part-1-asyncmanualresetevent/

namespace Csla.Threading
{
  /// <summary>
  /// Async/await implementation of a ManualResetEvent
  /// </summary>
  public class AsyncManualResetEvent
  {
    private volatile TaskCompletionSource<bool> _tcs = new TaskCompletionSource<bool>();

    /// <summary>
    /// Get awaitable task for the event
    /// </summary>
    public Task WaitAsync() { return _tcs.Task; }

    /// <summary>
    /// Sets the cancellation token for the event.
    /// </summary>
    /// <param name="ct">The cancellation token to set.</param>
    public void SetCancellationToken(CancellationToken ct)
    {
      ct.Register(() => _tcs.TrySetCanceled(), useSynchronizationContext: false);
    }

    /// <summary>
    /// Set the event, unblocking any code awaiting the event
    /// </summary>
    public void Set() { _tcs.TrySetResult(true); }

    /// <summary>
    /// Suggestion - If the rules throw an exception
    /// let the client know about it if/when they await all rules
    /// </summary>
    /// <param name="ex"></param>
    public void Set(Exception ex) { _tcs.TrySetException(ex); }

    /// <summary>
    /// Reset the event, preparing it for reuse
    /// </summary>
    public void Reset()
    {
      while (true)
      {
        var tcs = _tcs;
        if (!tcs.Task.IsCompleted ||
            Interlocked.CompareExchange(ref _tcs, new TaskCompletionSource<bool>(), tcs) == tcs)
          return;
      }
    }
  }
}
