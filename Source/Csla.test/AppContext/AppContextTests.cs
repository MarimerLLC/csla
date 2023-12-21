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
    [TestMethod()]
    public void ClientContext()
    {
      IDataPortal<ClientContextGetter> dataPortal = _testDIContext.CreateDataPortal<ClientContextGetter>();
      ApplicationContext applicationContext = _testDIContext.CreateTestApplicationContext();

      applicationContext.ClientContext.Add("value", "client context data");
      Assert.AreEqual("client context data", applicationContext.ClientContext["value"], "Matching data not retrieved");

      var obj = dataPortal.Fetch();
      Assert.AreEqual("client context data", obj.ClientContextValue, "value didn't come back from server");
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

    #region FailUpdateContext

    [TestMethod()]
    public void FailUpdateContext()
    {
      TestDIContext testDIContext = TestDIContextFactory.CreateContext(opts => opts.DataPortal(cfg => cfg.DataPortalReturnObjectOnException(true)));
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

    #endregion

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

  [Serializable]
  public class ClientContextGetter : BusinessBase<ClientContextGetter>
  {
    public static readonly PropertyInfo<string> ClientContextValueProperty = RegisterProperty<string>(nameof(ClientContextValue));
    public string ClientContextValue
    {
      get => GetProperty(ClientContextValueProperty);
      set => SetProperty(ClientContextValueProperty, value);
    }

    [Fetch]
    private void Fetch()
    {
      ClientContextValue = ApplicationContext.ClientContext["value"].ToString();
    }
  }
}
