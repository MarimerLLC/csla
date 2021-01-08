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
    /// <summary>
    /// Always make sure to cleanup after each test 
    /// </summary>
    [TestCleanup]
    public void Cleanup()
    {
      Csla.ApplicationContext.User = new System.Security.Principal.GenericPrincipal(
          new System.Security.Principal.GenericIdentity(string.Empty), new string[] { });
      Csla.ApplicationContext.DataPortalProxy = "Local";
      Csla.DataPortal.ResetProxyType();
      Csla.Server.FactoryDataPortal.FactoryLoader = null;
      Csla.ApplicationContext.GlobalContext.Clear();
    }

    [TestMethod]
    [TestCategory("SkipWhenLiveUnitTesting")]
    public void Create()
    {
      Csla.ApplicationContext.User = new System.Security.Claims.ClaimsPrincipal();
      Csla.ApplicationContext.DataPortalProxy = "Csla.Testing.Business.TestProxies.AppDomainProxy, Csla.Testing.Business";

      Csla.Server.FactoryDataPortal.FactoryLoader =
          new ObjectFactoryLoader();

      var root = Csla.DataPortal.Create<Root>();
      Assert.AreEqual("Create", root.Data, "Data should match");
      Assert.AreEqual(Csla.ApplicationContext.ExecutionLocations.Server, root.Location, "Location should match");
      Assert.IsTrue(root.IsNew, "Should be new");
      Assert.IsTrue(root.IsDirty, "Should be dirty");
    }

    [TestMethod]
    public void CreateLocal()
    {
      Csla.ApplicationContext.DataPortalProxy = "Csla.Testing.Business.TestProxies.AppDomainProxy, Csla.Testing.Business";
      Csla.Server.FactoryDataPortal.FactoryLoader =
        new ObjectFactoryLoader(8);
      var root = Csla.DataPortal.Create<Root>("abc");
      Assert.AreEqual("Create abc", root.Data, "Data should match");
      Assert.AreEqual(Csla.ApplicationContext.ExecutionLocations.Client, root.Location, "Location should match");
      Assert.IsTrue(root.IsNew, "Should be new");
      Assert.IsTrue(root.IsDirty, "Should be dirty");
    }

    [TestMethod]
    public void CreateWithParam()
    {
      Csla.ApplicationContext.User = new System.Security.Claims.ClaimsPrincipal();
      Csla.ApplicationContext.DataPortalProxy = "Csla.Testing.Business.TestProxies.AppDomainProxy, Csla.Testing.Business";
      Csla.Server.FactoryDataPortal.FactoryLoader =
          new ObjectFactoryLoader();
      var root = Csla.DataPortal.Create<Root>("abc");
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
      Csla.ApplicationContext.User = new System.Security.Claims.ClaimsPrincipal();
      Csla.ApplicationContext.DataPortalProxy = "Csla.Testing.Business.TestProxies.AppDomainProxy, Csla.Testing.Business";
      Csla.Server.FactoryDataPortal.FactoryLoader =
          new ObjectFactoryLoader(1);
      try
      {
        var root = Csla.DataPortal.Create<Root>("abc", 123);
      }
      catch (DataPortalException ex)
      {
        throw ex.BusinessException;
      }
    }

    [TestMethod]
    public void FetchNoCriteria()
    {
      Csla.Server.FactoryDataPortal.FactoryLoader = 
        new ObjectFactoryLoader();
      var root = Csla.DataPortal.Fetch<Root>();
      Assert.AreEqual("Fetch", root.Data, "Data should match");
      Assert.IsFalse(root.IsNew, "Should not be new");
      Assert.IsFalse(root.IsDirty, "Should not be dirty");
    }

    [TestMethod]
    public void FetchCriteria()
    {
      Csla.Server.FactoryDataPortal.FactoryLoader =
        new ObjectFactoryLoader();
      var root = Csla.DataPortal.Fetch<Root>("abc");
      Assert.AreEqual("abc", root.Data, "Data should match");
      Assert.IsFalse(root.IsNew, "Should not be new");
      Assert.IsFalse(root.IsDirty, "Should not be dirty");
    }

    [TestMethod]
    public void Update()
    {
      Csla.Server.FactoryDataPortal.FactoryLoader =
        new ObjectFactoryLoader();
      var root = new Root();
      root.Data = "abc";
      root = Csla.DataPortal.Update<Root>(root);
      Assert.AreEqual(TransactionalTypes.Manual, root.TransactionalType, "Transactional type should match");
      Assert.AreEqual("Update", root.Data, "Data should match");
      Assert.IsFalse(root.IsNew, "Should not be new");
      Assert.IsFalse(root.IsDirty, "Should not be dirty");
    }

    [TestMethod]
    public void UpdateTransactionScope()
    {
      Csla.Server.FactoryDataPortal.FactoryLoader =
        new ObjectFactoryLoader(1);
      var root = new Root();
      root.Data = "abc";
      root = Csla.DataPortal.Update<Root>(root);
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
      ApplicationContext.DefaultTransactionIsolationLevel = TransactionIsolationLevel.RepeatableRead;
      ApplicationContext.DefaultTransactionTimeoutInSeconds = 45;

      Csla.Server.FactoryDataPortal.FactoryLoader =
        new ObjectFactoryLoader(4);
      var root = new Root();
      root.Data = "abc";
      root = Csla.DataPortal.Update<Root>(root);
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
      ApplicationContext.DefaultTransactionIsolationLevel = TransactionIsolationLevel.RepeatableRead;
      ApplicationContext.DefaultTransactionTimeoutInSeconds = 45;
      Csla.Server.FactoryDataPortal.FactoryLoader =
        new ObjectFactoryLoader(5);
      var root = new Root();
      root.Data = "abc";
      root = Csla.DataPortal.Update<Root>(root);
      Assert.AreEqual(TransactionalTypes.TransactionScope, root.TransactionalType, "Transactional type should match");
      Assert.AreEqual("RepeatableRead", root.IsolationLevel, "Transactional isolation should match");
      Assert.AreEqual(45, root.TransactionTimeout, "Transactional timeout should match");

      Assert.AreEqual("Update", root.Data, "Data should match");
      Assert.IsFalse(root.IsNew, "Should not be new");
      Assert.IsFalse(root.IsDirty, "Should not be dirty");
    }

#if DEBUG
    [TestMethod]
    [Ignore]
    public void UpdateEnerpriseServicesTransactionCustomTransactionLevel()
    {
      ApplicationContext.DefaultTransactionIsolationLevel = TransactionIsolationLevel.RepeatableRead;
      Csla.Server.FactoryDataPortal.FactoryLoader =
        new ObjectFactoryLoader(6);
      var root = new Root();
      root.Data = "abc";
      root = Csla.DataPortal.Update<Root>(root);
      Assert.AreEqual(TransactionalTypes.EnterpriseServices, root.TransactionalType, "Transactional type should match");
      Assert.AreEqual("ReadCommitted", root.IsolationLevel, "Transactional isolation should match");

      Assert.AreEqual("Update", root.Data, "Data should match");
      Assert.IsFalse(root.IsNew, "Should not be new");
      Assert.IsFalse(root.IsDirty, "Should not be dirty");
    }

    [TestMethod]
    [Ignore]
    public void UpdateEnerpriseServicesTransactionDefaultTransactionLevel()
    {
      ApplicationContext.DefaultTransactionIsolationLevel = TransactionIsolationLevel.RepeatableRead;

      Csla.Server.FactoryDataPortal.FactoryLoader =
        new ObjectFactoryLoader(7);
      var root = new Root();
      root.Data = "abc";
      
      root = Csla.DataPortal.Update<Root>(root);
      Assert.AreEqual(TransactionalTypes.EnterpriseServices, root.TransactionalType, "Transactional type should match");
      Assert.AreEqual("RepeatableRead", root.IsolationLevel, "Transactional isolation should match");

      Assert.AreEqual("Update", root.Data, "Data should match");
      Assert.IsFalse(root.IsNew, "Should not be new");
      Assert.IsFalse(root.IsDirty, "Should not be dirty");
    }
#endif

    [TestMethod]
    public void Delete()
    {
      Csla.ApplicationContext.GlobalContext.Clear();

      Csla.Server.FactoryDataPortal.FactoryLoader =
        new ObjectFactoryLoader();

      Csla.DataPortal.Delete<Root>("abc");

      Assert.AreEqual("Delete", Csla.ApplicationContext.GlobalContext["ObjectFactory"].ToString(), "Data should match");
    }

    [TestMethod]
    public void FetchLoadProperty()
    {
      Csla.Server.FactoryDataPortal.FactoryLoader =
        new ObjectFactoryLoader(3);
      var root = Csla.DataPortal.Fetch<Root>();
      Assert.AreEqual("Fetch", root.Data, "Data should match");
      Assert.IsFalse(root.IsNew, "Should not be new");
      Assert.IsFalse(root.IsDirty, "Should not be dirty");
    }

    [TestMethod]
    public void DataPortalExecute_OnCommandObjectWithLocalProxy_CallsFactoryExecute()
    {
      Csla.ApplicationContext.GlobalContext.Clear();
      Csla.Server.FactoryDataPortal.FactoryLoader = null;
      var test = CommandObject.Execute();
      // return value is set in Execute method in CommandObjectFactory
      Assert.IsTrue(test);
    }

    [TestMethod]
    [ExpectedException(typeof(DataPortalException))]
    public void DataPortalExecute_OnCommandObjectWithFalseExecuteMethod_ThrowsExeptionMehodNotFound()
    {
      try
      {
        Csla.ApplicationContext.GlobalContext.Clear();
        Csla.Server.FactoryDataPortal.FactoryLoader = null;
        var test = CommandObjectMissingFactoryMethod.Execute();
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