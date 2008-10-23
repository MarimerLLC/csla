using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Core;
using System.ComponentModel;
using System.Threading;

namespace Csla.Threading
{
  // NOTE: It is not advisable to call this from a UI thread in silverlight!
  // Most likely it will result in blocking the entire browser.

  public static class BusyLock
  {
    public static void WaitOne(INotifyBusy obj)
    {
      BusyLocker locker = new BusyLocker(obj, TimeSpan.FromMilliseconds(Timeout.Infinite));
      locker.WaitOne();
    }

    public static void WaitOne(INotifyBusy obj, TimeSpan timeout)
    {
      BusyLocker locker = new BusyLocker(obj, timeout);
      locker.WaitOne();
    }
  }

  public class BusyLocker : IDisposable
  {
    private ManualResetEvent _event = new ManualResetEvent(false);
    private INotifyBusy _target;
    private TimeSpan _timeout;

    public BusyLocker(INotifyBusy target, TimeSpan timeout)
    {
      _event.Reset(); // set the event to non-signaled by default.
      _target = target;
      _timeout = timeout;
    }

    public void WaitOne()
    {
      try
      {
        _target.BusyChanged += new BusyChangedEventHandler(notify_BusyChanged);

        // Do nothing if this object is not currently busy
        // otherwise wait for the event to be signaled.
        if (_target.IsBusy)
        {
#if SILVERLIGHT
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

    public void Dispose()
    {
      WaitOne();
    }
  }
}
