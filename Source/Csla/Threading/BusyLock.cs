//-----------------------------------------------------------------------
// <copyright file="BusyLock.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implementation of a lock that waits while</summary>
//-----------------------------------------------------------------------

using Csla.Core;

namespace Csla.Threading
{
  /// <summary>
  /// Implementation of a lock that waits while
  /// a target object is busy.
  /// </summary>
  /// <remarks>
  /// Do not call this from a Blazor UI thread, as it will block
  /// the entire browser.
  /// </remarks>
  public static class BusyLock
  {
    /// <summary>
    /// Wait until the specified object is not busy 
    /// (IsBusy is false).
    /// </summary>
    /// <param name="obj">Target object.</param>
    /// <exception cref="ArgumentNullException"><paramref name="obj"/> is <see langword="null"/>.</exception>
    public static void WaitOne(INotifyBusy obj)
    {
      WaitOne(obj, Timeout.InfiniteTimeSpan);
    }

    /// <summary>
    /// Wait until the specified object is not busy 
    /// (IsBusy is false).
    /// </summary>
    /// <param name="obj">Target object.</param>
    /// <param name="timeout">Timeout value.</param>
    /// <exception cref="ArgumentNullException"><paramref name="obj"/> is <see langword="null"/>.</exception>
    public static void WaitOne(INotifyBusy obj, TimeSpan timeout)
    {
      Guard.NotNull(obj);

      BusyLocker locker = new BusyLocker(obj, timeout);
      locker.WaitOne();
    }
  }

  /// <summary>
  /// Implementation of a lock that waits while
  /// a target object is busy.
  /// </summary>
  public class BusyLocker : IDisposable
  {
    private readonly ManualResetEvent _event = new(false);
    private readonly INotifyBusy _target;
    private readonly TimeSpan _timeout;

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="target">Target object.</param>
    /// <param name="timeout">Timeout value.</param>
    /// <exception cref="ArgumentNullException"><paramref name="target"/> is <see langword="null"/>.</exception>
    public BusyLocker(INotifyBusy target, TimeSpan timeout)
    {
      _target = target ?? throw new ArgumentNullException(nameof(target));
      _timeout = timeout;
      _event.Reset(); // set the event to non-signaled by default.
    }

    /// <summary>
    /// Waits for the target object to become not busy.
    /// </summary>
    public void WaitOne()
    {
      try
      {
        _target.BusyChanged += notify_BusyChanged;

        // Do nothing if this object is not currently busy
        // otherwise wait for the event to be signaled.
        if (_target.IsBusy)
        {
#if (ANDROID || IOS)
          _event.WaitOne(_timeout);
#else
          _event.WaitOne(_timeout, false);
#endif
        }
      }
      finally
      {
        _target.BusyChanged -= notify_BusyChanged;
        _event.Close();
      }
    }

    private void notify_BusyChanged(object sender, BusyChangedEventArgs e)
    {
      // If the object is not busy then trigger 
      // the event to unblock the calling thread.
      if (!_target.IsBusy)
        _event.Set();
    }

    /// <summary>
    /// Disposes the object.
    /// </summary>
    public void Dispose()
    {
      WaitOne();
    }
  }
}
