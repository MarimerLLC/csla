//-----------------------------------------------------------------------
// <copyright file="BusyLock.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implementation of a lock that waits while</summary>
//-----------------------------------------------------------------------
using System;
using Csla.Core;
using System.Threading;

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
    public static void WaitOne(INotifyBusy obj)
    {
      BusyLocker locker = new BusyLocker(obj, TimeSpan.FromMilliseconds(Timeout.Infinite));
      locker.WaitOne();
    }

    /// <summary>
    /// Wait until the specified object is not busy 
    /// (IsBusy is false).
    /// </summary>
    /// <param name="obj">Target object.</param>
    /// <param name="timeout">Timeout value.</param>
    public static void WaitOne(INotifyBusy obj, TimeSpan timeout)
    {
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
    private ManualResetEvent _event = new ManualResetEvent(false);
    private INotifyBusy _target;
    private TimeSpan _timeout;

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="target">Target object.</param>
    /// <param name="timeout">Timeout value.</param>
    public BusyLocker(INotifyBusy target, TimeSpan timeout)
    {
      _event.Reset(); // set the event to non-signaled by default.
      _target = target;
      _timeout = timeout;
    }

    /// <summary>
    /// Waits for the target object to become not busy.
    /// </summary>
    public void WaitOne()
    {
      try
      {
        _target.BusyChanged += new BusyChangedEventHandler(notify_BusyChanged);

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
        _target.BusyChanged -= new BusyChangedEventHandler(notify_BusyChanged);
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
