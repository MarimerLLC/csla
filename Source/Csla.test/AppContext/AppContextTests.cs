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
using Csla;
using System.Threading;

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
    #region Simple Test

    [TestMethod()]
    public void SimpleTest()
    {
      ApplicationContext.Clear();
      ApplicationContext.ClientContext["v1"] = "client";
      ApplicationContext.GlobalContext["v2"] = "client";

      SimpleRoot root = SimpleRoot.GetSimpleRoot("data");

      Assert.AreEqual("client", ApplicationContext.ClientContext["v1"], "client context didn't roundtrip");
      Assert.AreEqual("client", ApplicationContext.GlobalContext["v2"], "global context didn't roundtrip");
      Assert.AreEqual("Fetched", ApplicationContext.GlobalContext["Root"], "global context missing server value");
    }

    #endregion

    [TestMethod()]
    public void ApplicationContextProperties()
    {
      ApplicationContext.DataPortalProxy = null;
      Assert.AreEqual("Local", ApplicationContext.DataPortalProxy);
      Assert.AreEqual("Client", ApplicationContext.ExecutionLocation.ToString());
    }

    #region TestAppContext across different Threads
    [TestMethod]
    public void TestAppContextAcrossDifferentThreads()
    {
      List<AppContextThread> AppContextThreadList = new List<AppContextThread>();
      List<Thread> ThreadList = new List<Thread>();

      for (int x = 0; x < 10; x++)
      {
        AppContextThread act = new AppContextThread("Thread: " + x);
        AppContextThreadList.Add(act);

        Thread t = new Thread(new ThreadStart(act.Run));
        t.Name = "Thread: " + x;
        t.Start();
        ThreadList.Add(t);
      }

      ApplicationContext.Clear();
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
      Csla.ApplicationContext.GlobalContext.Clear();

      Csla.ApplicationContext.ClientContext.Add("clientcontext", "client context data");
      Assert.AreEqual("client context data", Csla.ApplicationContext.ClientContext["clientcontext"], "Matching data not retrieved");

      Csla.Test.Basic.Root root = Csla.Test.Basic.Root.NewRoot();
      root.Data = "saved";
      Assert.AreEqual("saved", root.Data, "Root data should be 'saved'");
      Assert.AreEqual(true, root.IsDirty, "Object should be dirty");
      Assert.AreEqual(true, root.IsValid, "Object should be valid");

      Csla.ApplicationContext.GlobalContext.Clear();
      root = root.Save();

      Assert.IsNotNull(root, "Root object should not be null");
      Assert.AreEqual("Inserted", Csla.ApplicationContext.GlobalContext["Root"], "Object not inserted");
      Assert.AreEqual("saved", root.Data, "Root data should be 'saved'");
      Assert.AreEqual(false, root.IsNew, "Object should not be new");
      Assert.AreEqual(false, root.IsDeleted, "Object should not be deleted");
      Assert.AreEqual(false, root.IsDirty, "Object should not be dirty");

      Assert.AreEqual("client context data", Csla.ApplicationContext.ClientContext["clientcontext"], "Client context data lost");
      Assert.AreEqual("client context data", Csla.ApplicationContext.GlobalContext["clientcontext"], "Global context data lost");
      Assert.AreEqual("new global value", Csla.ApplicationContext.GlobalContext["globalcontext"], "New global value lost");
    }
    #endregion

    #region GlobalContext
    /// <summary>
    /// Test the Global Context
    /// </summary>
    [TestMethod()]
    public void GlobalContext()
    {
      Csla.ApplicationContext.GlobalContext.Clear();

      ApplicationContext.GlobalContext["globalcontext"] = "global context data";
      Assert.AreEqual("global context data", ApplicationContext.GlobalContext["globalcontext"], "first");

      Csla.Test.Basic.Root root = Csla.Test.Basic.Root.NewRoot();
      root.Data = "saved";
      Assert.AreEqual("saved", root.Data);
      Assert.AreEqual(true, root.IsDirty);
      Assert.AreEqual(true, root.IsValid);

      Csla.ApplicationContext.GlobalContext.Clear();
      root = root.Save();

      Assert.IsNotNull(root);
      //Assert.IsNotNull(Thread.GetData(Thread.GetNamedDataSlot("Csla.GlobalContext")));
      Assert.AreEqual("Inserted", Csla.ApplicationContext.GlobalContext["Root"]);
      Assert.AreEqual("saved", root.Data);
      Assert.AreEqual(false, root.IsNew);
      Assert.AreEqual(false, root.IsDeleted);
      Assert.AreEqual(false, root.IsDirty);

      Assert.AreEqual("new global value", ApplicationContext.GlobalContext["globalcontext"], "Second");
    }
    #endregion

    #region Dataportal Events
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
    [TestMethod()]
    public void DataPortalEvents()
    {
      ApplicationContext.GlobalContext.Clear();
      ApplicationContext.Clear();
      ApplicationContext.GlobalContext["global"] = "global";

      Csla.DataPortal.DataPortalInvoke += new Action<DataPortalEventArgs>(OnDataPortaInvoke);
      Csla.DataPortal.DataPortalInvokeComplete += new Action<DataPortalEventArgs>(OnDataPortalInvokeComplete);

      Csla.Test.Basic.Root root = Csla.Test.Basic.Root.GetRoot("testing");

      Csla.DataPortal.DataPortalInvoke -= new Action<DataPortalEventArgs>(OnDataPortaInvoke);
      Csla.DataPortal.DataPortalInvokeComplete -= new Action<DataPortalEventArgs>(OnDataPortalInvokeComplete);

      //Populated in the handlers below
      Assert.AreEqual("global", Csla.ApplicationContext.GlobalContext["ClientInvoke"], "Client invoke incorrect");
      Assert.AreEqual("global", Csla.ApplicationContext.GlobalContext["ClientInvokeComplete"], "Client invoke complete");

      //populated in the Root Dataportal handlers.
      Assert.AreEqual("global", Csla.ApplicationContext.GlobalContext["dpinvoke"], "Server invoke incorrect");
      Assert.AreEqual("global", Csla.ApplicationContext.GlobalContext["dpinvokecomplete"], "Server invoke compelte incorrect");
    }

    private void OnDataPortaInvoke(DataPortalEventArgs e)
    {
      Csla.ApplicationContext.GlobalContext["ClientInvoke"] = ApplicationContext.GlobalContext["global"];
    }
    private void OnDataPortalInvokeComplete(DataPortalEventArgs e)
    {
      Csla.ApplicationContext.GlobalContext["ClientInvokeComplete"] = ApplicationContext.GlobalContext["global"];
    }
    #endregion

    #region FailCreateContext
    /// <summary>
    /// Test the FaileCreate Context
    /// </summary>
    [TestMethod()]
    public void FailCreateContext()
    {
      ApplicationContext.GlobalContext.Clear();
      ApplicationContext.Clear();

      ExceptionRoot root;
      try
      {
        root = ExceptionRoot.NewExceptionRoot();
        Assert.Fail("Exception didn't occur");
      }
      catch (DataPortalException ex)
      {
        Assert.IsNull(ex.BusinessObject, "Business object shouldn't be returned");
        Assert.AreEqual("Fail create", ex.GetBaseException().Message, "Base exception message incorrect");
        Assert.IsTrue(ex.Message.StartsWith("DataPortal.Create failed"), "Exception message incorrect");
      }

      Assert.AreEqual("create", ApplicationContext.GlobalContext["create"], "GlobalContext not preserved");
    }
    #endregion

    #region FailFetchContext
    [TestMethod()]
    public void FailFetchContext()
    {
      ApplicationContext.GlobalContext.Clear();
      ExceptionRoot root = null;
      try
      {
        root = ExceptionRoot.GetExceptionRoot("fail");
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

      Assert.AreEqual("create", ApplicationContext.GlobalContext["create"], "GlobalContext not preserved");
    }
    #endregion

    #region FailUpdateContext
    [TestMethod()]
    public void FailUpdateContext()
    {
      try
      {
        ApplicationContext.GlobalContext.Clear();
        ApplicationContext.DataPortalReturnObjectOnException = true;

        ExceptionRoot root;
        try
        {
          root = ExceptionRoot.NewExceptionRoot();
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
        Assert.AreEqual("create", ApplicationContext.GlobalContext["create"], "GlobalContext not preserved");
      }
      finally
      {
        ApplicationContext.DataPortalReturnObjectOnException = false;
      }
    }
    #endregion

    #region FailDeleteContext
    [TestMethod()]
    public void FailDeleteContext()
    {
      ApplicationContext.GlobalContext.Clear();
      ApplicationContext.Clear();

      ExceptionRoot root = null;
      try
      {
        ExceptionRoot.DeleteExceptionRoot("fail");
        Assert.Fail("Exception didn't occur");
      }
      catch (DataPortalException ex)
      {
        root = (ExceptionRoot)ex.BusinessObject;
        Assert.AreEqual("Fail delete", ex.GetBaseException().Message, "Base exception message incorrect");
        Assert.IsTrue(ex.Message.StartsWith("DataPortal.Delete failed"), "Exception message incorrect");
      }
      Assert.IsNull(root, "Business object returned");
      Assert.AreEqual("create", ApplicationContext.GlobalContext["create"], "GlobalContext not preserved");
    }
    #endregion

    [TestCleanup]
    public void ClearContextsAfterEachTest()
    {
      ApplicationContext.GlobalContext.Clear();
      ApplicationContext.Clear();
    }

    [TestInitialize]
    public void SetScopedSp()
    {
      ApplicationContext.GlobalContext.Clear();
      ApplicationContext.Clear();
    }

  }
}
