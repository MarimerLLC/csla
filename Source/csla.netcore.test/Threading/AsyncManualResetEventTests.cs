//-----------------------------------------------------------------------
// <copyright file="AsyncManualResetEventTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Threading.Tasks;

#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#endif 

namespace Csla.Test.Threading
{
  [TestClass]
  public class AsyncManualResetEventTests
  {
    [TestMethod]
    public void SetEvent()
    {
      var e = new Csla.Threading.AsyncManualResetEvent();
      e.Set();
    }

    [TestMethod]
    public void ResetEvent()
    {
      var e = new Csla.Threading.AsyncManualResetEvent();
      e.Reset();
    }

    [TestMethod]
    public void WaitForWork()
    {
      int x = 0;
      var e = new Csla.Threading.AsyncManualResetEvent();
      Task.Run(() =>
      {
        x = 42;
        System.Threading.Thread.Sleep(10);
        e.Set();
      });
      var zx = e.WaitAsync();
      while (!zx.IsCompleted) { }
      Assert.AreEqual(42, x);
    }

    [TestMethod]
    public void SyncWaitForWork()
    {
      int x = 0;
      var e = new Csla.Threading.AsyncManualResetEvent();
      Task.Run(() =>
      {
        x = 42;
        System.Threading.Thread.Sleep(10);
        e.Set();
      });
      e.Wait();
      Assert.AreEqual(42, x);
    }

    [TestMethod]
    [ExpectedException(typeof(TimeoutException))]
    public void EventTimeout()
    {
      var e = new Csla.Threading.AsyncManualResetEvent();
      Task.Run(() =>
      {
        System.Threading.Thread.Sleep(10);
        e.Set();
      });
      e.WaitAsync(new TimeSpan(0));
    }

    [TestMethod]
    [ExpectedException(typeof(TimeoutException))]
    public void SyncEventTimeout()
    {
      var e = new Csla.Threading.AsyncManualResetEvent();
      Task.Run(() =>
      {
        System.Threading.Thread.Sleep(10);
        e.Set();
      });
      e.Wait(new TimeSpan(0));
    }
  }
}
