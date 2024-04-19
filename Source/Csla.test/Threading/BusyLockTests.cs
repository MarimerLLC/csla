//-----------------------------------------------------------------------
// <copyright file="BusyLockTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

using Csla.Threading;
using Csla.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.Threading
{
  [TestClass]
  public class BusyLockTests
  {
    private class BusyBO : INotifyBusy
    {
      public bool IsBusy { get; private set; }

      public bool IsSelfBusy
      {
        get { return IsBusy; }
      }

      public void MarkBusy(bool busy)
      {
        IsBusy = busy;
        if (BusyChanged != null)
          BusyChanged(this, new BusyChangedEventArgs("", busy));
      }

      public event BusyChangedEventHandler BusyChanged;      

      public event EventHandler<ErrorEventArgs> UnhandledAsyncException;

      protected virtual void OnUnhandledAsyncException()
      {
        if (UnhandledAsyncException != null)
          UnhandledAsyncException(this, new ErrorEventArgs(null, null));
      }
    }

    [TestMethod]
    public void SimpleLock()
    {
      BusyBO busy = new BusyBO();
      busy.MarkBusy(true);
      System.ComponentModel.BackgroundWorker worker = new System.ComponentModel.BackgroundWorker();
      worker.DoWork += (_, _) => 
      {
        System.Threading.Thread.Sleep(10);
        busy.MarkBusy(false);
      };
      worker.RunWorkerAsync();

      BusyLock.WaitOne(busy);
      Assert.IsFalse(busy.IsBusy);
    }

    [TestMethod]
    public void TestTimeout()
    {
      BusyBO busy = new BusyBO();
      busy.MarkBusy(true);
      BusyLock.WaitOne(busy, TimeSpan.FromMilliseconds(10));
      Assert.IsTrue(busy.IsBusy);
    }

    [TestMethod]
    public void TestBusyLockObject()
    {
      BusyBO busy = new BusyBO();
      busy.MarkBusy(true);
      using (BusyLocker bl = new BusyLocker(busy, TimeSpan.FromSeconds(1)))
      {
        System.ComponentModel.BackgroundWorker worker = new System.ComponentModel.BackgroundWorker();
        worker.DoWork += (_, _) => busy.MarkBusy(false);
        worker.RunWorkerAsync();
      }
      Assert.IsFalse(busy.IsBusy);
    }
  }
}