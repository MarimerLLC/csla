//-----------------------------------------------------------------------
// <copyright file="CslaTaskFactoryTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------using System;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Security.Principal;
using System.Threading;
using Csla.Threading;
#if MSTEST
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif
using UnitDriven;

namespace Csla.Test.Threading
{
  /// <summary>
  ///This is a test class for BackgroundWorkerTest and is intended
  ///to contain all BackgroundWorkerTest Unit Tests
  ///</summary>
  [TestClass]
  public class CslaTaskSchedulerTests : TestBase
  {
    private IPrincipal _originalPrincipal;
    private CultureInfo _originalCulture;
    private  CultureInfo _originalUICulture;
    
    [TestInitialize]
    public void Setup()
    {
      _originalPrincipal = Csla.ApplicationContext.User;
      _originalCulture = Thread.CurrentThread.CurrentCulture;
      _originalUICulture = Thread.CurrentThread.CurrentUICulture;
    }

    [TestCleanup]
    public void Cleanup()
    {
      Csla.ApplicationContext.User = _originalPrincipal;

      Thread.CurrentThread.CurrentCulture = _originalCulture;
      Thread.CurrentThread.CurrentUICulture = _originalUICulture;
    }

    [TestMethod]
    public void SchdulerTakesContextToBackgroundThread()
    {
      Csla.ApplicationContext.User = new MyPrincipal();
      Csla.ApplicationContext.ClientContext["BWTEST"] = "TEST";
      Csla.ApplicationContext.GlobalContext["BWTEST"] = "TEST";
      Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("FR");
      Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("FR");

      var UIThreadid = Thread.CurrentThread.ManagedThreadId;
      var cslaScheduler = new CslaTaskScheduler();
      var factory = new TaskFactory(cslaScheduler);

      using (UnitTestContext context = GetContext())
      {
        var task1 = new Task(() =>
          {
            Thread.Sleep(20);

            context.Assert.IsFalse(Thread.CurrentThread.ManagedThreadId == UIThreadid);

            // make sure that user, clientcontext, globalcontext, currentCulture and currentUIculture are sent 
            context.Assert.IsTrue(Csla.ApplicationContext.User is MyPrincipal);
            context.Assert.AreEqual("TEST", Csla.ApplicationContext.GlobalContext["BWTEST"]);
            context.Assert.AreEqual("TEST", Csla.ApplicationContext.ClientContext["BWTEST"]);
            context.Assert.AreEqual("FR", Thread.CurrentThread.CurrentCulture.Name.ToUpper());
            context.Assert.AreEqual("FR", Thread.CurrentThread.CurrentUICulture.Name.ToUpper());
          });

        task1.ContinueWith((o) =>
          {
            context.Assert.IsFalse(Thread.CurrentThread.ManagedThreadId == UIThreadid);

            // make sure that user, clientcontext, globalcontext, currentCulture and currentUIculture are sent 
            context.Assert.IsTrue(Csla.ApplicationContext.User is MyPrincipal);
            context.Assert.AreEqual("TEST", Csla.ApplicationContext.GlobalContext["BWTEST"]);
            context.Assert.AreEqual("TEST", Csla.ApplicationContext.ClientContext["BWTEST"]);
            context.Assert.AreEqual("FR", Thread.CurrentThread.CurrentCulture.Name.ToUpper());
            context.Assert.AreEqual("FR", Thread.CurrentThread.CurrentUICulture.Name.ToUpper());
          }, cslaScheduler);

        var task2 = new Task(() =>
          {
            Thread.Sleep(25);

            context.Assert.IsFalse(Thread.CurrentThread.ManagedThreadId == UIThreadid);

            // make sure that user, clientcontext, globalcontext, currentCulture and currentUIculture are sent 
            context.Assert.IsTrue(Csla.ApplicationContext.User is MyPrincipal);
            context.Assert.AreEqual("TEST", Csla.ApplicationContext.GlobalContext["BWTEST"]);
            context.Assert.AreEqual("TEST", Csla.ApplicationContext.ClientContext["BWTEST"]);
            context.Assert.AreEqual("FR", Thread.CurrentThread.CurrentCulture.Name.ToUpper());
            context.Assert.AreEqual("FR", Thread.CurrentThread.CurrentUICulture.Name.ToUpper());
          });

        task1.Start(cslaScheduler);
        task2.Start(cslaScheduler);

        Task.WaitAll(task1, task2);

        var task3 = new Task(() => context.Assert.Success());
        task3.Start(cslaScheduler);

        Task.WaitAll(task3);

        context.Complete();
      }

    }
  }
}
