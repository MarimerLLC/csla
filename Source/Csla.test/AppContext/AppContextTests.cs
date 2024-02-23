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
    public void Initialize()
    {
      TestResults.Reinitialise();
    }

    #region Simple Test

    [TestMethod()]
    public void SimpleTest()
    {
      IDataPortal<SimpleRoot> dataPortal = _testDIContext.CreateDataPortal<SimpleRoot>();

      // TODO: How do we do this test in Csla 6?
      //ApplicationContext.ClientContext["v1"] = "client";

      SimpleRoot root = dataPortal.Fetch(new SimpleRoot.Criteria("data"));

      //Assert.AreEqual("client", ApplicationContext.ClientContext["v1"], "client context didn't roundtrip");
      Assert.AreEqual("Fetched", TestResults.GetResult("Root"), "global context missing server value");
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
    [Ignore] // fails on ci build
    [TestMethod()]
    public void ClientContext()
    {
      IDataPortal<Basic.Root> dataPortal = _testDIContext.CreateDataPortal<Basic.Root>();
      ApplicationContext applicationContext = _testDIContext.CreateTestApplicationContext();

      var testContext = "client context data";
      applicationContext.ClientContext.Add("clientcontext", testContext);
      Assert.AreEqual(testContext, applicationContext.ClientContext["clientcontext"], "Matching data not retrieved");

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

    #region FailCreateContext

    /// <summary>
    /// Test the FaileCreate Context
    /// </summary>
    [TestMethod()]
    public void FailCreateContext()
    {
      IDataPortal<ExceptionRoot> dataPortal = _testDIContext.CreateDataPortal<ExceptionRoot>();
      
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

    [TestMethod()]
    public void FailUpdateContext()
    {
      TestDIContext testDIContext = TestDIContextFactory.CreateContext(opts => opts.
        DataPortal(cfg => cfg.
          AddServerSideDataPortal(dpc => dpc.
            DataPortalReturnObjectOnException =true)));
      IDataPortal<ExceptionRoot> dataPortal = testDIContext.CreateDataPortal<ExceptionRoot>();
      
      try
      {
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
      }
      finally
      {
      }
    }

    #region FailDeleteContext

    [TestMethod()]
    public void FailDeleteContext()
    {
      IDataPortal<ExceptionRoot> dataPortal = _testDIContext.CreateDataPortal<ExceptionRoot>();

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
