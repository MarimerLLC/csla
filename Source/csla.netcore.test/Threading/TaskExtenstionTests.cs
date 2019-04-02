//-----------------------------------------------------------------------
// <copyright file="TaskExtenstionTests.cs" company="Marimer LLC">
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
using Csla.Threading;

namespace Csla.Test.Threading
{
  [TestClass]
  public class TaskExtenstionTests
  {
    [TestMethod]
    public void SpinWait()
    {
      int x = 0;
      var task = Task.Run(() => { System.Threading.Thread.Sleep(10); x = 42; });
      task.SpinWait();
      Assert.AreEqual(42, x);
    }

    [TestMethod]
    [ExpectedException(typeof(TimeoutException))]
    public void SpinWaitTimeout()
    {
      var task = Task.Run(() => System.Threading.Thread.Sleep(10));
      task.SpinWait(new TimeSpan(0, 0, 0));
    }

    [TestMethod]
    public void RunWithContext()
    {
      int x = 0;
      ApplicationContext.ClientContext["x"] = 42;
      var task = new Task<int>(() => (int)ApplicationContext.ClientContext["x"]);
      x = task.RunWithContext();
      Assert.AreEqual(42, x);
    }

    [TestMethod]
    [ExpectedException(typeof(TimeoutException))]
    public void RunWithContextTimeout()
    {
      var task = new Task<int>(() => { System.Threading.Thread.Sleep(10); return 42; });
      int x = task.RunWithContext(new TimeSpan(0, 0, 0));
    }
  }
}
