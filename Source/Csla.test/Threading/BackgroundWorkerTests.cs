////-----------------------------------------------------------------------
//// <copyright file="BackgroundWorkerTests.cs" company="Marimer LLC">
////     Copyright (c) Marimer LLC. All rights reserved.
////     Website: https://cslanet.com
//// </copyright>
//// <summary>no summary</summary>
////-----------------------------------------------------------------------using System;
//using System;
//using System.ComponentModel;
//using System.Globalization;
//using System.Security.Claims;
//using System.Security.Principal;
//using System.Threading;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using UnitDriven;
//using BackgroundWorker = Csla.Threading.BackgroundWorker;

//namespace Csla.Test.Threading
//{


//  /// <summary>
//  ///This is a test class for BackgroundWorkerTest and is intended
//  ///to contain all BackgroundWorkerTest Unit Tests
//  ///</summary>
//  [TestClass]
//  public class BackgroundWorkerTests : TestBase
//  {

//    #region Additional test attributes
//    // 
//    //You can use the following additional attributes as you write your tests:
//    private IPrincipal _originalPrincipal;
//    private CultureInfo _originalCulture;
//    private CultureInfo _originalUICulture;

//    [ClassInitialize]
//    public static void ClassInit(TestContext context)
//    {
//      BackgroundWorkerSyncContextHelper.Init();
//    }

//    [ClassCleanup]
//    public static void ClassCleanup()
//    {
//      BackgroundWorkerSyncContextHelper.Cleanup();
//    }

//    [TestInitialize]
//    public void Setup()
//    {
//      _originalPrincipal = Csla.ApplicationContext.User;
//      _originalCulture = Thread.CurrentThread.CurrentCulture;
//      _originalUICulture = Thread.CurrentThread.CurrentUICulture;
//    }

//    [TestCleanup]
//    public void Cleanup()
//    {
//      Csla.ApplicationContext.User = _originalPrincipal;

//      Thread.CurrentThread.CurrentCulture = _originalCulture;
//      Thread.CurrentThread.CurrentUICulture = _originalUICulture;
//    }

//    #endregion

//    /// <summary>
//    ///A test for BackgroundWorker Constructor
//    ///</summary>
//    [TestMethod]
//    public void BackgroundWorker_Constructor_DefaultValues()
//    {
//      using (var context = GetContext())
//      {
//        BackgroundWorker target = new BackgroundWorker();

//        Assert.IsFalse(target.WorkerReportsProgress, "WorkerReportsProgress is false by default");
//        Assert.IsFalse(target.WorkerSupportsCancellation, "WorkerSupportsCancellation is false by default");
//        Assert.Success();
//      }

//    }

//    /// <summary>
//    ///A test for BackgroundWorker normal run 
//    ///</summary>
//    [TestMethod]
    
//    public void BackgroundWorker_RunWorkerAsync_CallsDoWorkAndWorkerCompleted()
//    {

//      using (UnitTestContext context = GetContext())
//      {
//        BackgroundWorkerSyncContextHelper.DoTests(() =>
//        {

//          var UIThreadid = Thread.CurrentThread.ManagedThreadId;

//          Csla.ApplicationContext.User = new ClaimsPrincipal();
//          TestResults.Add("BWTEST", "TEST");


//          Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("FR");
//          Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("FR");

//          BackgroundWorker target = new BackgroundWorker();
//          bool doWorkCalled = false;

//          target.DoWork += (o, e) =>
//          {
//            doWorkCalled = true;
//            Assert.IsFalse(Thread.CurrentThread.ManagedThreadId == UIThreadid);

//            // make sure that user, clientcontext, globalcontext, currentCulture and currentUIculture are sent 
//            Assert.IsTrue(Csla.ApplicationContext.User is ClaimsPrincipal);
//            Assert.AreEqual("TEST", TestResults.GetResult("BWTEST"));
//            Assert.AreEqual("FR", Thread.CurrentThread.CurrentCulture.Name.ToUpper());
//            Assert.AreEqual("FR", Thread.CurrentThread.CurrentUICulture.Name.ToUpper());
//          };
//          target.RunWorkerCompleted += (o, e) =>
//          {
//            // assert that this callback comes on the "UI" thread 
//            Assert.IsTrue(Thread.CurrentThread.ManagedThreadId == UIThreadid);
//            Assert.IsNull(e.Error);
//            Assert.IsTrue(doWorkCalled, "Do work has been called");
//            Assert.Success();
//          };
//          target.RunWorkerAsync(null);


//          while (target.IsBusy)
//          {
//            // this is the equvalent to Application.DoEvents in Windows Forms 
//            BackgroundWorkerSyncContextHelper.PumpDispatcher();
//          }

//          context.Complete();
//        });
//      }
//    }


//    /// <summary>
//    ///A test for BackgroundWorker when reporting progress
//    ///</summary>
//    [TestMethod]
//    public void BackgroundWorker_RunWorkerAsync_ReportsProgress()
//    {
//      using (UnitTestContext context = GetContext())
//      {
//        BackgroundWorkerSyncContextHelper.DoTests(() =>
//        {

//          var UIThreadid = Thread.CurrentThread.ManagedThreadId;
//          int numTimesProgressCalled = 0;

//          BackgroundWorker target = new BackgroundWorker();
//          target.DoWork += (o, e) =>
//                              {
//                                // report progress changed 10 times
//                                for (int i = 1; i < 11; i++)
//                                {
//                                  target.ReportProgress(i*10);
//                                }
//                                e.Result = new object();
//                              };
//          target.WorkerReportsProgress = true;
//          target.ProgressChanged += (o, e) =>
//                                      {
//                                        Assert.IsTrue(Thread.CurrentThread.ManagedThreadId == UIThreadid);
//                                        numTimesProgressCalled++;
//                                      };
//          target.RunWorkerCompleted += (o, e) =>
//                                          {
//                                            Assert.IsTrue(Thread.CurrentThread.ManagedThreadId == UIThreadid);
//                                            Assert.IsNull(e.Error);
//                                            Assert.IsTrue(numTimesProgressCalled == 10,"ReportProgress has been called 10 times");
//                                            Assert.Success();
//                                          };
//          target.RunWorkerAsync(null);

//          while (target.IsBusy)
//          {
//            // this is the equvalent to Application.DoEvents in Windows Forms 
//            BackgroundWorkerSyncContextHelper.PumpDispatcher();
//          }

//          context.Complete();
//        });
//      }
//    }

//    /// <summary>
//    ///A test for BackgroundWorker when reporting progress
//    ///</summary>
//    [TestMethod]
//    public void BackgroundWorker_DoWork_ThrowsInvalidOperationExcpetionWhenWorkerReportsProgressIsFalse()
//    {
//      UnitTestContext context = GetContext();

//      int numTimesProgressCalled = 0;

//      BackgroundWorker target = new BackgroundWorker();
//      target.DoWork += (o, e) =>
//      {
//        // report progress changed 10 times
//        for (int i = 1; i < 11; i++)
//        {
//          target.ReportProgress(i * 10);
//        }
//      };
//      target.WorkerReportsProgress = false;
//      target.ProgressChanged += (o, e) =>
//      {
//        numTimesProgressCalled++;
//      };
//      target.RunWorkerCompleted += (o, e) =>
//      {
//        //  target does not support ReportProgress we shold get a System.InvalidOperationException from DoWork
//        Assert.IsTrue(e.Error is System.InvalidOperationException);
//        Assert.Success();
//      };
//      target.RunWorkerAsync(null);
//      context.Complete();
//    }

//    /// <summary>
//    ///A test for BackgroundWorker when reporting progress
//    ///</summary>
//    [TestMethod]
//    [TestCategory("SkipWhenLiveUnitTesting")]
//    public void BackgroundWorker_CancelAsync_ReportsCancelledWhenWorkerSupportsCancellationIsTrue()
//    {
//      UnitTestContext context = GetContext();

//      BackgroundWorker target = new BackgroundWorker();
//      target.DoWork += (o, e) =>
//      {
//        // report progress changed 10 times
//        for (int i = 1; i < 11; i++)
//        {
//          Thread.Sleep(100);
//          if (target.CancellationPending)
//          {
//            e.Cancel = true;
//            return;
//          }
//        }
//      };
//      target.WorkerSupportsCancellation = true;
//      target.RunWorkerCompleted += (o, e) =>
//      {
//        //  target does not support ReportProgress we shold get a System.InvalidOperationException from DoWork
//        Assert.IsNull(e.Error);
//        Assert.IsTrue(e.Cancelled);
//        Assert.Success();
//      };
//      target.RunWorkerAsync(null);
//      target.CancelAsync();
//      context.Complete();
//    }

//    /// <summary>
//    ///A test for BackgroundWorker when reporting progress
//    ///</summary>
//    [TestMethod]
//    [ExpectedException(typeof(System.InvalidOperationException))]
//    public void BackgroundWorker_CancelAsync_ThrowsInvalidOperationExceptionWhenWorkerSupportsCancellationIsFalse()
//    {
//      using (UnitTestContext context = GetContext())
//      {
//        BackgroundWorker target = new BackgroundWorker();
//        target.DoWork += (o, e) =>
//        {
//          for (int i = 1; i < 11; i++)
//          {
//            Thread.Sleep(10);
//          }
//        };
//        target.WorkerSupportsCancellation = false;
//        target.RunWorkerAsync(null);

//        try
//        {
//          target.CancelAsync();   // this call throws exception 
//        }
//        catch (InvalidOperationException ex)
//        {
//          Assert.Fail(ex);
//        }
//        context.Complete();
//      }
//    }
//  }
//}
