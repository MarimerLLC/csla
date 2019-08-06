//-----------------------------------------------------------------------
// <copyright file="BusyLockTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Threading;
using UnitDriven;
using Csla.Core;
using System.ComponentModel;

#if NUNIT
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
using TestSetup = NUnit.Framework.SetUpAttribute;
#elif MSTEST
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace Csla.Test.Threading
{
  [TestClass]
  public class BusyLockTests
  {
    private class BusyBO : INotifyBusy
    {
      private bool _busy;

      public bool IsBusy
      {
        get { return _busy; }
      }

      public bool IsSelfBusy
      {
        get { return _busy; }
      }

      public void MarkBusy(bool busy)
      {
        _busy = busy;
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
      worker.DoWork += (o, e) => 
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
        worker.DoWork += (o, e) => busy.MarkBusy(false);
        worker.RunWorkerAsync();
      }
      Assert.IsFalse(busy.IsBusy);
    }
  }
}