//-----------------------------------------------------------------------
// <copyright file="BackgroundWorkerTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------using System;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
#if MSTEST
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif
using UnitDriven;
using BackgroundWorker = Csla.Threading.BackgroundWorker;

namespace Csla.Test.Threading
{


  /// <summary>
  ///This is a test class for BackgroundWorkerTest and is intended
  ///to contain all BackgroundWorkerTest Unit Tests
  ///</summary>
  [TestClass]
  public class BackgroundWorkerTests : TestBase
  {

    #region Additional test attributes
    // 
    //You can use the following additional attributes as you write your tests:
    private IPrincipal _originalPrincipal;
    private CultureInfo _originalCulture;
    private CultureInfo _originalUICulture;

    [ClassInitialize]
    public static void ClassInit(TestContext context)
    {
      BackgroundWorkerSyncContextHelper.Init();
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
      BackgroundWorkerSyncContextHelper.Cleanup();
    }

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

    #endregion

    /// <summary>
    ///A test for BackgroundWorker Constructor
    ///</summary>
    [TestMethod]
    public void BackgroundWorker_Constructor_DefaultValues()
    {
      using (var context = GetContext())
      {
        BackgroundWorker target = new BackgroundWorker();

        context.Assert.IsFalse(target.WorkerReportsProgress, "WorkerReportsProgress is false by default");
        context.Assert.IsFalse(target.WorkerSupportsCancellation, "WorkerSupportsCancellation is false by default");
        context.Assert.Success();
      }

    }

    /// <summary>
    ///A test for BackgroundWorker normal run 
    ///</summary>
    [TestMethod]
    
    public void BackgroundWorker_RunWorkerAsync_CallsDoWorkAndWorkerCompleted()
    {

      using (UnitTestContext context = GetContext())
      {
        BackgroundWorkerSyncContextHelper.DoTests(() =>
        {

          var UIThreadid = Thread.CurrentThread.ManagedThreadId;

          Csla.ApplicationContext.User = new ClaimsPrincipal();
          Csla.ApplicationContext.ClientContext["BWTEST"] = "TEST";
          Csla.ApplicationContext.GlobalContext["BWTEST"] = "TEST";


          Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("FR");
          Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("FR");

          BackgroundWorker target = new BackgroundWorker();
          bool doWorkCalled = false;

          target.DoWork += (o, e) =>
          {
            doWorkCalled = true;
            context.Assert.IsFalse(Thread.CurrentThread.ManagedThreadId == UIThreadid);

            // make sure that user, clientcontext, globalcontext, currentCulture and currentUIculture are sent 
            context.Assert.IsTrue(Csla.ApplicationContext.User is ClaimsPrincipal);
            context.Assert.AreEqual("TEST", Csla.ApplicationContext.GlobalContext["BWTEST"]);
            context.Assert.AreEqual("TEST", Csla.ApplicationContext.ClientContext["BWTEST"]);
            context.Assert.AreEqual("FR", Thread.CurrentThread.CurrentCulture.Name.ToUpper());
            context.Assert.AreEqual("FR", Thread.CurrentThread.CurrentUICulture.Name.ToUpper());
          };
          target.RunWorkerCompleted += (o, e) =>
          {
            // assert that this callback comes on the "UI" thread 
            context.Assert.IsTrue(Thread.CurrentThread.ManagedThreadId == UIThreadid);
            context.Assert.IsNull(e.Error);
            context.Assert.IsTrue(doWorkCalled, "Do work has been called");
            context.Assert.Success();
          };
          target.RunWorkerAsync(null);


          while (target.IsBusy)
          {
            // this is the equvalent to Application.DoEvents in Windows Forms 
            BackgroundWorkerSyncContextHelper.PumpDispatcher();
          }

          context.Complete();
        });
      }
    }


    /// <summary>
    ///A test for BackgroundWorker when reporting progress
    ///</summary>
    [TestMethod]
    public void BackgroundWorker_RunWorkerAsync_ReportsProgress()
    {
      using (UnitTestContext context = GetContext())
      {
        BackgroundWorkerSyncContextHelper.DoTests(() =>
        {

          var UIThreadid = Thread.CurrentThread.ManagedThreadId;
          int numTimesProgressCalled = 0;

          BackgroundWorker target = new BackgroundWorker();
          target.DoWork += (o, e) =>
                              {
                                // report progress changed 10 times
                                for (int i = 1; i < 11; i++)
                                {
                                  target.ReportProgress(i*10);
                                }
                                e.Result = new object();
                              };
          target.WorkerReportsProgress = true;
          target.ProgressChanged += (o, e) =>
                                      {
                                        context.Assert.IsTrue(Thread.CurrentThread.ManagedThreadId == UIThreadid);
                                        numTimesProgressCalled++;
                                      };
          target.RunWorkerCompleted += (o, e) =>
                                          {
                                            context.Assert.IsTrue(Thread.CurrentThread.ManagedThreadId == UIThreadid);
                                            context.Assert.IsNull(e.Error);
                                            context.Assert.IsTrue(numTimesProgressCalled == 10,"ReportProgress has been called 10 times");
                                            context.Assert.Success();
                                          };
          target.RunWorkerAsync(null);

          while (target.IsBusy)
          {
            // this is the equvalent to Application.DoEvents in Windows Forms 
            BackgroundWorkerSyncContextHelper.PumpDispatcher();
          }

          context.Complete();
        });
      }
    }

    /// <summary>
    ///A test for BackgroundWorker when reporting progress
    ///</summary>
    [TestMethod]
    public void BackgroundWorker_DoWork_ThrowsInvalidOperationExcpetionWhenWorkerReportsProgressIsFalse()
    {
      UnitTestContext context = GetContext();

      int numTimesProgressCalled = 0;

      BackgroundWorker target = new BackgroundWorker();
      target.DoWork += (o, e) =>
      {
        // report progress changed 10 times
        for (int i = 1; i < 11; i++)
        {
          target.ReportProgress(i * 10);
        }
      };
      target.WorkerReportsProgress = false;
      target.ProgressChanged += (o, e) =>
      {
        numTimesProgressCalled++;
      };
      target.RunWorkerCompleted += (o, e) =>
      {
        //  target does not support ReportProgress we shold get a System.InvalidOperationException from DoWork
        context.Assert.IsTrue(e.Error is System.InvalidOperationException);
        context.Assert.Success();
      };
      target.RunWorkerAsync(null);
      context.Complete();
    }

    /// <summary>
    ///A test for BackgroundWorker when reporting progress
    ///</summary>
    [TestMethod]
    [TestCategory("SkipWhenLiveUnitTesting")]
    public void BackgroundWorker_CancelAsync_ReportsCancelledWhenWorkerSupportsCancellationIsTrue()
    {
      UnitTestContext context = GetContext();

      BackgroundWorker target = new BackgroundWorker();
      target.DoWork += (o, e) =>
      {
        // report progress changed 10 times
        for (int i = 1; i < 11; i++)
        {
          Thread.Sleep(100);
          if (target.CancellationPending)
          {
            e.Cancel = true;
            return;
          }
        }
      };
      target.WorkerSupportsCancellation = true;
      target.RunWorkerCompleted += (o, e) =>
      {
        //  target does not support ReportProgress we shold get a System.InvalidOperationException from DoWork
        context.Assert.IsNull(e.Error);
        context.Assert.IsTrue(e.Cancelled);
        context.Assert.Success();
      };
      target.RunWorkerAsync(null);
      target.CancelAsync();
      context.Complete();
    }

    /// <summary>
    ///A test for BackgroundWorker when reporting progress
    ///</summary>
    [TestMethod]
    [ExpectedException(typeof(System.InvalidOperationException))]
    public void BackgroundWorker_CancelAsync_ThrowsInvalidOperationExceptionWhenWorkerSupportsCancellationIsFalse()
    {
      using (UnitTestContext context = GetContext())
      {
        BackgroundWorker target = new BackgroundWorker();
        target.DoWork += (o, e) =>
        {
          for (int i = 1; i < 11; i++)
          {
            Thread.Sleep(10);
          }
        };
        target.WorkerSupportsCancellation = false;
        target.RunWorkerAsync(null);

        try
        {
          target.CancelAsync();   // this call throws exception 
        }
        catch (InvalidOperationException ex)
        {
          context.Assert.Fail(ex);
        }
        context.Complete();
      }
    }
  }
}
