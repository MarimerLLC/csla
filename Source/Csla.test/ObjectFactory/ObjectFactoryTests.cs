//-----------------------------------------------------------------------
// <copyright file="ObjectFactoryTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Always make sure to cleanup after each test </summary>
//-----------------------------------------------------------------------
using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace Csla.Test.ObjectFactory
{
  [TestClass]
  public class ObjectFactoryTests
  {
    private static TestDIContext _testDIContext;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
      _testDIContext = TestDIContextFactory.CreateDefaultContext();
    }

    /// <summary>
    /// Always make sure to cleanup after each test 
    /// </summary>
    [TestCleanup]
    public void Cleanup()
    {
      // TODO: Is this still required?
      //Csla.ApplicationContext.User = new System.Security.Principal.GenericPrincipal(
      //    new System.Security.Principal.GenericIdentity(string.Empty), new string[] { });
      //Csla.ApplicationContext.DataPortalProxy = "Local";
      //Csla.DataPortal.ResetProxyType();
      //Csla.Server.FactoryDataPortal.FactoryLoader = null;
      TestResults.Reinitialise();
    }

    [TestMethod]
    [TestCategory("SkipWhenLiveUnitTesting")]
    public void Create()
    {
      TestDIContext testDIContext = TestDIContextFactory.CreateContext(new System.Security.Claims.ClaimsPrincipal());
      // TODO: Fix this
      //Csla.ApplicationContext.DataPortalProxy = "Csla.Testing.Business.TestProxies.AppDomainProxy, Csla.Testing.Business";

      //Csla.Server.FactoryDataPortal.FactoryLoader =
      //    new ObjectFactoryLoader();

      IDataPortal<Root> dataPortal = testDIContext.CreateDataPortal<Root>();
      
      var root = dataPortal.Create();
      Assert.AreEqual("Create", root.Data, "Data should match");
      Assert.AreEqual(Csla.ApplicationContext.ExecutionLocations.Server, root.Location, "Location should match");
      Assert.IsTrue(root.IsNew, "Should be new");
      Assert.IsTrue(root.IsDirty, "Should be dirty");
    }

    [TestMethod]
    public void CreateLocal()
    {
      // TODO: Fix this
      //Csla.ApplicationContext.DataPortalProxy = "Csla.Testing.Business.TestProxies.AppDomainProxy, Csla.Testing.Business";
      //Csla.Server.FactoryDataPortal.FactoryLoader =
      //  new ObjectFactoryLoader(8);

      IDataPortal<Root> dataPortal = _testDIContext.CreateDataPortal<Root>();

      var root = dataPortal.Create("abc");
      Assert.AreEqual("Create abc", root.Data, "Data should match");
      Assert.AreEqual(Csla.ApplicationContext.ExecutionLocations.Client, root.Location, "Location should match");
      Assert.IsTrue(root.IsNew, "Should be new");
      Assert.IsTrue(root.IsDirty, "Should be dirty");
    }

    [TestMethod]
    public void CreateWithParam()
    {
      TestDIContext testDIContext = TestDIContextFactory.CreateContext(new System.Security.Claims.ClaimsPrincipal());
      // TODO: Fix this
      //Csla.ApplicationContext.DataPortalProxy = "Csla.Testing.Business.TestProxies.AppDomainProxy, Csla.Testing.Business";
      //Csla.Server.FactoryDataPortal.FactoryLoader =
      //    new ObjectFactoryLoader();

      IDataPortal<Root> dataPortal = testDIContext.CreateDataPortal<Root>();

      var root = dataPortal.Create("abc");
      Assert.AreEqual("Create abc", root.Data, "Data should match");
      Assert.AreEqual(Csla.ApplicationContext.ExecutionLocations.Client, root.Location, "Location should match");
      Assert.IsTrue(root.IsNew, "Should be new");
      Assert.IsTrue(root.IsDirty, "Should be dirty");
    }

    // this test needs to be updated when the factory model is updated
    // to use DI and multi-property criteria
    [Ignore]
    [TestMethod]
    [ExpectedException(typeof(MissingMethodException))]
    public void CreateMissing()
    {
      TestDIContext testDIContext = TestDIContextFactory.CreateContext(new System.Security.Claims.ClaimsPrincipal());
      // TODO: Fix this
      //Csla.ApplicationContext.DataPortalProxy = "Csla.Testing.Business.TestProxies.AppDomainProxy, Csla.Testing.Business";
      //Csla.Server.FactoryDataPortal.FactoryLoader =
      //    new ObjectFactoryLoader(1);

      IDataPortal<Root> dataPortal = testDIContext.CreateDataPortal<Root>();

      try
      {
        var root = dataPortal.Create("abc", 123);
      }
      catch (DataPortalException ex)
      {
        throw ex.BusinessException;
      }
    }

    [TestMethod]
    public void FetchNoCriteria()
    {
      // TODO: Fix this
      //Csla.Server.FactoryDataPortal.FactoryLoader = 
      //  new ObjectFactoryLoader();

      IDataPortal<Root> dataPortal = _testDIContext.CreateDataPortal<Root>();

      var root = dataPortal.Fetch();
      Assert.AreEqual("Fetch", root.Data, "Data should match");
      Assert.IsFalse(root.IsNew, "Should not be new");
      Assert.IsFalse(root.IsDirty, "Should not be dirty");
    }

    [TestMethod]
    public void FetchCriteria()
    {
      // TODO: Fix this
      //Csla.Server.FactoryDataPortal.FactoryLoader =
      //  new ObjectFactoryLoader();

      IDataPortal<Root> dataPortal = _testDIContext.CreateDataPortal<Root>();

      var root = dataPortal.Fetch("abc");
      Assert.AreEqual("abc", root.Data, "Data should match");
      Assert.IsFalse(root.IsNew, "Should not be new");
      Assert.IsFalse(root.IsDirty, "Should not be dirty");
    }

    [TestMethod]
    public void Update()
    {
      // TODO: Fix this
      //Csla.Server.FactoryDataPortal.FactoryLoader =
      //  new ObjectFactoryLoader();
      var root = new Root();
      root.Data = "abc";

      IDataPortal<Root> dataPortal = _testDIContext.CreateDataPortal<Root>();

      root = dataPortal.Update(root);
      Assert.AreEqual(TransactionalTypes.Manual, root.TransactionalType, "Transactional type should match");
      Assert.AreEqual("Update", root.Data, "Data should match");
      Assert.IsFalse(root.IsNew, "Should not be new");
      Assert.IsFalse(root.IsDirty, "Should not be dirty");
    }

    [TestMethod]
    public void UpdateTransactionScope()
    {
      // TODO: Fix this
      //Csla.Server.FactoryDataPortal.FactoryLoader =
      //  new ObjectFactoryLoader(1);
      var root = new Root();
      root.Data = "abc";

      IDataPortal<Root> dataPortal = _testDIContext.CreateDataPortal<Root>();

      root = dataPortal.Update(root);
      Assert.AreEqual(TransactionalTypes.TransactionScope, root.TransactionalType, "Transactional type should match");
      Assert.AreEqual("Serializable", root.IsolationLevel, "Transactional isolation should match");
      Assert.AreEqual(30, root.TransactionTimeout, "Transactional timeout should match");

      Assert.AreEqual("Update", root.Data, "Data should match");
      Assert.IsFalse(root.IsNew, "Should not be new");
      Assert.IsFalse(root.IsDirty, "Should not be dirty");
    }

    [TestMethod]
    public void UpdateTransactionScopeUsingCustomTransactionLevelAndTimeout()
    {
      // TODO: Fix this
      //ApplicationContext.DefaultTransactionIsolationLevel = TransactionIsolationLevel.RepeatableRead;
      //ApplicationContext.DefaultTransactionTimeoutInSeconds = 45;
      //
      //Csla.Server.FactoryDataPortal.FactoryLoader =
      //  new ObjectFactoryLoader(4);
      var root = new Root();
      root.Data = "abc";

      IDataPortal<Root> dataPortal = _testDIContext.CreateDataPortal<Root>();

      root = dataPortal.Update(root);
      Assert.AreEqual(TransactionalTypes.TransactionScope, root.TransactionalType, "Transactional type should match");
      Assert.AreEqual("ReadCommitted", root.IsolationLevel, "Transactional isolation should match");
      Assert.AreEqual(100, root.TransactionTimeout, "Transactional timeout should match");

      Assert.AreEqual("Update", root.Data, "Data should match");
      Assert.IsFalse(root.IsNew, "Should not be new");
      Assert.IsFalse(root.IsDirty, "Should not be dirty");
    }


    [TestMethod]
    public void UpdateTransactionScopeUsingDefaultTransactionLevelAndTimeout()
    {
      // TODO: Fix this
      //ApplicationContext.DefaultTransactionIsolationLevel = TransactionIsolationLevel.RepeatableRead;
      //ApplicationContext.DefaultTransactionTimeoutInSeconds = 45;
      //Csla.Server.FactoryDataPortal.FactoryLoader =
      //  new ObjectFactoryLoader(5);
      var root = new Root();
      root.Data = "abc";

      IDataPortal<Root> dataPortal = _testDIContext.CreateDataPortal<Root>();

      root = dataPortal.Update(root);
      Assert.AreEqual(TransactionalTypes.TransactionScope, root.TransactionalType, "Transactional type should match");
      Assert.AreEqual("RepeatableRead", root.IsolationLevel, "Transactional isolation should match");
      Assert.AreEqual(45, root.TransactionTimeout, "Transactional timeout should match");

      Assert.AreEqual("Update", root.Data, "Data should match");
      Assert.IsFalse(root.IsNew, "Should not be new");
      Assert.IsFalse(root.IsDirty, "Should not be dirty");
    }

    [TestMethod]
    public void Delete()
    {
      TestResults.Reinitialise();

      // TODO: Fix this
      //Csla.Server.FactoryDataPortal.FactoryLoader =
      //  new ObjectFactoryLoader();

      IDataPortal<Root> dataPortal = _testDIContext.CreateDataPortal<Root>();

      dataPortal.Delete("abc");

      Assert.AreEqual("Delete", TestResults.GetResult("ObjectFactory"), "Data should match");
    }

    [TestMethod]
    public void FetchLoadProperty()
    {
      // TODO: Fix this
      //Csla.Server.FactoryDataPortal.FactoryLoader =
      //  new ObjectFactoryLoader(3);

      IDataPortal<Root> dataPortal = _testDIContext.CreateDataPortal<Root>();

      var root = dataPortal.Fetch();
      Assert.AreEqual("Fetch", root.Data, "Data should match");
      Assert.IsFalse(root.IsNew, "Should not be new");
      Assert.IsFalse(root.IsDirty, "Should not be dirty");
    }

    [TestMethod]
    public void DataPortalExecute_OnCommandObjectWithLocalProxy_CallsFactoryExecute()
    {
      TestResults.Reinitialise();
      // TODO: Fix this
      //Csla.Server.FactoryDataPortal.FactoryLoader = null;

      IDataPortal<CommandObject> dataPortal = _testDIContext.CreateDataPortal<CommandObject>();

      var test = CommandObject.Execute(dataPortal);
      // return value is set in Execute method in CommandObjectFactory
      Assert.IsTrue(test);
    }

    [TestMethod]
    [ExpectedException(typeof(DataPortalException))]
    public void DataPortalExecute_OnCommandObjectWithFalseExecuteMethod_ThrowsExeptionMehodNotFound()
    {
      try
      {
        TestResults.Reinitialise();
        // TODO: Fix this
        //Csla.Server.FactoryDataPortal.FactoryLoader = null;

        IDataPortal<CommandObjectMissingFactoryMethod> dataPortal = _testDIContext.CreateDataPortal<CommandObjectMissingFactoryMethod>();

        var test = CommandObjectMissingFactoryMethod.Execute(dataPortal);
      }
      catch (DataPortalException ex)
      {
        // inner exception should be System.NotImplementedException and mesaage should contain methodname 
        Assert.AreEqual(typeof(System.NotImplementedException), ex.InnerException.GetType());
        Assert.IsTrue(ex.InnerException.Message.Contains("ExecuteMissingMethod"));
        // rethrow exception 
        throw;
      }
      Assert.Fail("Should throw exception");
    }
  }
}