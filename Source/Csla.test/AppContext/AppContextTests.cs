//-----------------------------------------------------------------------
// <copyright file="AppContextTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Test to see if contexts get cleared out properly</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Csla.Configuration;
using Csla.TestHelpers;

#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#endif

namespace Csla.Test.AppContext
{
  [TestClass()]
  public class AppContextTests
  {
    private static TestDIContext _testDIContext;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
      _testDIContext = TestDIContextFactory.CreateDefaultContext();
    }

    [TestInitialize]
    public void SetScopedSp()
    {
      TestResults.Reinitialise();
    }

    [TestCleanup]
    public void ClearContextsAfterEachTest()
    {
      TestResults.Reinitialise();
    }

    #region Simple Test

    [TestMethod()]
    public void SimpleTest()
    {
      IDataPortal<SimpleRoot> dataPortal = _testDIContext.CreateDataPortal<SimpleRoot>();

      // TODO: Fix test
      TestResults.Reinitialise();
      //ApplicationContext.ClientContext["v1"] = "client";

      SimpleRoot root = dataPortal.Fetch(new SimpleRoot.Criteria("data"));

      //Assert.AreEqual("client", ApplicationContext.ClientContext["v1"], "client context didn't roundtrip");
      Assert.AreEqual("Fetched", TestResults.GetResult("Root"), "global context missing server value");
    }

    #endregion

    [TestMethod()]
    public void ApplicationContextProperties()
    {
      // TODO: Fix test
      //ApplicationContext.DataPortalProxy = null;
      //Assert.AreEqual("Local", ApplicationContext.DataPortalProxy);
      //Assert.AreEqual("Client", ApplicationContext.ExecutionLocation.ToString());
    }

    #region TestAppContext across different Threads

    // TODO: Is this test relevant anymore? I can't work out how to do this test
    [Ignore]
    [TestMethod]
    public void TestAppContextAcrossDifferentThreads()
    {
      List<AppContextThread> AppContextThreadList = new List<AppContextThread>();
      List<Thread> ThreadList = new List<Thread>();

      TestResults.Reinitialise();

      for (int x = 0; x < 10; x++)
      {
        AppContextThread act = new AppContextThread("Thread: " + x);
        AppContextThreadList.Add(act);

        Thread t = new Thread(new ThreadStart(act.Run));
        t.Name = "Thread: " + x;
        t.Start();
        ThreadList.Add(t);
      }

      Exception ex = null;
      try
      {
        foreach (AppContextThread act in AppContextThreadList)
        {
          //We are accessing the Client/GlobalContext via this thread, therefore
          //it should be removed.
          Assert.AreEqual(true, act.Removed);
        }
        //We are now accessing the shared value. If any other thread
        //loses its Client/GlobalContext this will turn to true
        //Assert.AreEqual(false, AppContextThread.StaticRemoved);
      }
      catch (Exception e)
      {
        ex = e;
      }
      finally
      {
        foreach (AppContextThread act in AppContextThreadList)
          act.Stop();

        foreach (Thread t in ThreadList)
        {
          t.Join();
        }
      }
      if (ex != null) throw ex;
    }

    #endregion

    #region ClientContext

    /// <summary>
    /// Test the Client Context
    /// </summary>
    /// <remarks>
    /// Clearing the GlobalContext clears the ClientContext also? 
    /// Should the ClientContext be cleared explicitly also?
    /// </remarks>
    [TestMethod()]
    public void ClientContext()
    {
      IDataPortal<Basic.Root> dataPortal = _testDIContext.CreateDataPortal<Basic.Root>();
      ApplicationContext applicationContext = _testDIContext.CreateTestApplicationContext();

      TestResults.Reinitialise();

      applicationContext.ClientContext.Add("clientcontext", "client context data");
      Assert.AreEqual("client context data", applicationContext.ClientContext["clientcontext"], "Matching data not retrieved");

      Basic.Root root = dataPortal.Create(new Basic.Root.Criteria());
      root.Data = "saved";
      Assert.AreEqual("saved", root.Data, "Root data should be 'saved'");
      Assert.AreEqual(true, root.IsDirty, "Object should be dirty");
      Assert.AreEqual(true, root.IsValid, "Object should be valid");

      TestResults.Reinitialise();
      root = root.Save();

      Assert.IsNotNull(root, "Root object should not be null");
      Assert.AreEqual("Inserted", TestResults.GetResult("Root"), "Object not inserted");
      Assert.AreEqual("saved", root.Data, "Root data should be 'saved'");
      Assert.AreEqual(false, root.IsNew, "Object should not be new");
      Assert.AreEqual(false, root.IsDeleted, "Object should not be deleted");
      Assert.AreEqual(false, root.IsDirty, "Object should not be dirty");

      //TODO: Is there a modern equivalent of this?
      //Assert.AreEqual("client context data", Csla.ApplicationContext.ClientContext["clientcontext"], "Client context data lost");
      Assert.AreEqual("client context data", TestResults.GetResult("clientcontext"), "Global context data lost");
      Assert.AreEqual("new global value", TestResults.GetResult("globalcontext"), "New global value lost");
    }

    #endregion

    #region Dataportal Events

    // TODO: Is this test relevant any more? These event handlers don't seem to be exposed
    /// <summary>
    /// Test the dataportal events
    /// </summary>
    /// <remarks>
    /// How does the GlobalContext get the keys "dpinvoke" and "dpinvokecomplete"?
    /// 
    /// In the vb version of this test it calls RemoveHandler in VB. Unfortunately removing handlers aren't quite
    /// that easy in C# I had to declare EventHandlers that could be added and removed. Also I found out that the
    /// VB library does not seem to contain the DataPortalInvokeEventHandler object so I put a conditional compile
    /// flag around this method and set a warning message.
    /// </remarks>
    [Ignore]
    [TestMethod()]
    public void DataPortalEvents()
    {
      IDataPortal<Basic.Root> dataPortal = _testDIContext.CreateDataPortal<Basic.Root>();

      TestResults.Reinitialise();
      TestResults.Add("global", "global");

      //dataPortal.DataPortalInvoke += new Action<DataPortalEventArgs>(OnDataPortaInvoke);
      //dataPortal.DataPortalInvokeComplete += new Action<DataPortalEventArgs>(OnDataPortalInvokeComplete);

      Basic.Root root = dataPortal.Fetch(new Basic.Root.Criteria("testing"));

      //dataPortal.DataPortalInvoke -= new Action<DataPortalEventArgs>(OnDataPortaInvoke);
      //dataPortal.DataPortalInvokeComplete -= new Action<DataPortalEventArgs>(OnDataPortalInvokeComplete);

      //Populated in the handlers below
      Assert.AreEqual("global", TestResults.GetResult("ClientInvoke"), "Client invoke incorrect");
      Assert.AreEqual("global", TestResults.GetResult("ClientInvokeComplete"), "Client invoke complete");

      //populated in the Root Dataportal handlers.
      Assert.AreEqual("global", TestResults.GetResult("dpinvoke"), "Server invoke incorrect");
      Assert.AreEqual("global", TestResults.GetResult("dpinvokecomplete"), "Server invoke compelte incorrect");
    }

    private void OnDataPortaInvoke(DataPortalEventArgs e)
    {
      TestResults.Add("ClientInvoke", TestResults.GetResult("global"));
    }

    private void OnDataPortalInvokeComplete(DataPortalEventArgs e)
    {
      TestResults.Add("ClientInvokeComplete", TestResults.GetResult("global"));
    }

    #endregion

    #region FailCreateContext

    /// <summary>
    /// Test the FaileCreate Context
    /// </summary>
    [TestMethod()]
    public void FailCreateContext()
    {
      IDataPortal<ExceptionRoot> dataPortal = _testDIContext.CreateDataPortal<ExceptionRoot>();
      
      TestResults.Reinitialise();

      ExceptionRoot root;
      try
      {
        root = dataPortal.Create(new ExceptionRoot.Criteria());
        Assert.Fail("Exception didn't occur");
      }
      catch (DataPortalException ex)
      {
        Assert.IsNull(ex.BusinessObject, "Business object shouldn't be returned");
        Assert.AreEqual("Fail create", ex.GetBaseException().Message, "Base exception message incorrect");
        Assert.IsTrue(ex.Message.StartsWith("DataPortal.Create failed"), "Exception message incorrect");
      }

      Assert.AreEqual("create", TestResults.GetResult("create"), "GlobalContext not preserved");
    }

    #endregion

    #region FailFetchContext

    [TestMethod()]
    public void FailFetchContext()
    {
      IDataPortal<ExceptionRoot> dataPortal = _testDIContext.CreateDataPortal<ExceptionRoot>();
      
      TestResults.Reinitialise();
      ExceptionRoot root = null;
      try
      {
        root = dataPortal.Fetch(new ExceptionRoot.Criteria("fail"));
        Assert.Fail("Exception didn't occur");
      }
      catch (DataPortalException ex)
      {
        Assert.IsNull(ex.BusinessObject, "Business object shouldn't be returned");
        Assert.AreEqual("Fail fetch", ex.GetBaseException().Message, "Base exception message incorrect");
        Assert.IsTrue(ex.Message.StartsWith("DataPortal.Fetch failed"), "Exception message incorrect");
      }
      catch (Exception ex)
      {
        Assert.Fail("Unexpected exception: " + ex.ToString());
      }

      Assert.AreEqual("create", TestResults.GetResult("create"), "GlobalContext not preserved");
    }

    #endregion

    #region FailUpdateContext

    [TestMethod()]
    public void FailUpdateContext()
    {
      TestDIContext testDIContext = TestDIContextFactory.CreateContext(opts => opts.DataPortal().DataPortalReturnObjectOnException(true));
      IDataPortal<ExceptionRoot> dataPortal = testDIContext.CreateDataPortal<ExceptionRoot>();
      
      try
      {
        TestResults.Reinitialise();

        ExceptionRoot root;
        try
        {
          root = dataPortal.Create(new ExceptionRoot.Criteria());
          Assert.Fail("Create exception didn't occur");
        }
        catch (DataPortalException ex)
        {
          root = (ExceptionRoot)ex.BusinessObject;
          Assert.AreEqual("Fail create", ex.GetBaseException().Message, "Base exception message incorrect");
          Assert.IsTrue(ex.Message.StartsWith("DataPortal.Create failed"), "Exception message incorrect");
        }

        root.Data = "boom";
        try
        {
          root = root.Save();

          Assert.Fail("Insert exception didn't occur");
        }
        catch (DataPortalException ex)
        {
          root = (ExceptionRoot)ex.BusinessObject;
          Assert.AreEqual("Fail insert", ex.GetBaseException().Message, "Base exception message incorrect");
          Assert.IsTrue(ex.Message.StartsWith("DataPortal.Update failed"), "Exception message incorrect");
        }

        Assert.AreEqual("boom", root.Data, "Business object not returned");
        Assert.AreEqual("create", TestResults.GetResult("create"), "GlobalContext not preserved");
      }
      finally
      {
      }
    }

    #endregion

    #region FailDeleteContext

    [TestMethod()]
    public void FailDeleteContext()
    {
      IDataPortal<ExceptionRoot> dataPortal = _testDIContext.CreateDataPortal<ExceptionRoot>();
      TestResults.Reinitialise();

      ExceptionRoot root = null;
      try
      {
        dataPortal.Delete("fail");
        Assert.Fail("Exception didn't occur");
      }
      catch (DataPortalException ex)
      {
        root = (ExceptionRoot)ex.BusinessObject;
        Assert.AreEqual("Fail delete", ex.GetBaseException().Message, "Base exception message incorrect");
        Assert.IsTrue(ex.Message.StartsWith("DataPortal.Delete failed"), "Exception message incorrect");
      }
      Assert.IsNull(root, "Business object returned");
      Assert.AreEqual("create", TestResults.GetResult("create"), "GlobalContext not preserved");
    }

    #endregion

    private Basic.Root GetRoot(string data)
    {
      IDataPortal<Basic.Root> dataPortal = _testDIContext.CreateDataPortal<Basic.Root>();

      return dataPortal.Fetch(new Basic.Root.Criteria(data));
    }

  }
}
