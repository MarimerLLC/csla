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
using Csla.Configuration;
using Csla.TestHelpers;
using Microsoft.Extensions.DependencyInjection;

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
      _testDIContext = TestDIContextFactory.CreateContext(
        options => options.DataPortal(dp => dp.AddServerSideDataPortal(
          cfg => cfg.RegisterObjectFactoryLoader<ObjectFactoryLoader<RootFactory>>())
        ));
    }

    [TestInitialize]
    public void Initialize()
    {
      TestResults.Reinitialise();
    }

    /// <summary>
    /// Always make sure to cleanup after each test 
    /// </summary>
    [TestCleanup]
    public void Cleanup()
    {
      // TODO: Is any of this cleanup still required? Probably not
      //Csla.ApplicationContext.DataPortalProxy = "Local";
      //Csla.DataPortal.ResetProxyType();
    }

    [TestMethod]
    [TestCategory("SkipWhenLiveUnitTesting")]
    public void Create()
    {
      TestDIContext testDIContext = TestDIContextFactory.CreateContext(
      // TODO: What proxy can we use for this test? Old one was Remoting, now retired
      //  options => options.Services.AddTransient<DataPortalClient.IDataPortalProxy, Testing.Business.TestProxies.AppDomainProxy>(), 
        opts => opts.DataPortal(
          dp => dp.AddServerSideDataPortal(cfg => cfg.RegisterObjectFactoryLoader<ObjectFactoryLoader<RootFactory>>())),
        new System.Security.Claims.ClaimsPrincipal());

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
      // TODO: What proxy can we use for this test? Old one was Remoting, now retired
      //TestDIContext testDIContext = TestDIContextFactory.CreateContext(
      //  options => options.Services.AddTransient<DataPortalClient.IDataPortalProxy, Testing.Business.TestProxies.AppDomainProxy>(), 
      //);
      TestDIContext testDIContext = TestDIContextFactory.CreateContext(
        opts => opts.DataPortal(dp => dp.AddServerSideDataPortal(cfg => cfg.RegisterObjectFactoryLoader<ObjectFactoryLoader<RootFactoryC>>()))
        );

      IDataPortal<Root> dataPortal = testDIContext.CreateDataPortal<Root>();

      var root = dataPortal.Create("abc");
      Assert.AreEqual("Create abc", root.Data, "Data should match");
      Assert.AreEqual(Csla.ApplicationContext.ExecutionLocations.Client, root.Location, "Location should match");
      Assert.IsTrue(root.IsNew, "Should be new");
      Assert.IsTrue(root.IsDirty, "Should be dirty");
    }

    [TestMethod]
    public void CreateWithParam()
    {
      TestDIContext testDIContext = TestDIContextFactory.CreateContext(
      // TODO: What proxy can we use for this test? Old one was Remoting, now retired
      //  options => options.Services.AddTransient<DataPortalClient.IDataPortalProxy, Testing.Business.TestProxies.AppDomainProxy>(), 
        opts => opts.DataPortal(dp => dp.AddServerSideDataPortal(cfg => cfg.RegisterObjectFactoryLoader<ObjectFactoryLoader<RootFactory>>())),
        new System.Security.Claims.ClaimsPrincipal());

      IDataPortal<Root> dataPortal = testDIContext.CreateDataPortal<Root>();

      var root = dataPortal.Create("abc");
      Assert.AreEqual("Create abc", root.Data, "Data should match");
      Assert.AreEqual(Csla.ApplicationContext.ExecutionLocations.Client, root.Location, "Location should match");
      Assert.IsTrue(root.IsNew, "Should be new");
      Assert.IsTrue(root.IsDirty, "Should be dirty");
    }

    // TODO: This test needs to be updated when the factory model is updated
    // to use DI and multi-property criteria
    [TestMethod]
    [ExpectedException(typeof(MissingMethodException))]
    public void CreateMissing()
    {
      TestDIContext testDIContext = TestDIContextFactory.CreateContext(
      // TODO: What proxy can we use for this test? Old one was Remoting, now retired
      //  options => options.Services.AddTransient<DataPortalClient.IDataPortalProxy, Testing.Business.TestProxies.AppDomainProxy>(), 
        opts => opts.DataPortal(dp => dp.AddServerSideDataPortal(cfg => cfg.RegisterObjectFactoryLoader<ObjectFactoryLoader<RootFactory1>>())),
        new System.Security.Claims.ClaimsPrincipal()
        );

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
      IDataPortal<Root> dataPortal = _testDIContext.CreateDataPortal<Root>();

      var root = dataPortal.Fetch();
      Assert.AreEqual("Fetch", root.Data, "Data should match");
      Assert.IsFalse(root.IsNew, "Should not be new");
      Assert.IsFalse(root.IsDirty, "Should not be dirty");
    }

    [TestMethod]
    public void FetchCriteria()
    {
      IDataPortal<Root> dataPortal = _testDIContext.CreateDataPortal<Root>();

      var root = dataPortal.Fetch("abc");
      Assert.AreEqual("abc", root.Data, "Data should match");
      Assert.IsFalse(root.IsNew, "Should not be new");
      Assert.IsFalse(root.IsDirty, "Should not be dirty");
    }

    [TestMethod]
    public void Update()
    {
      IDataPortal<Root> dataPortal = _testDIContext.CreateDataPortal<Root>();

      var root = dataPortal.Fetch();
      root.Data = "abc";

      root = dataPortal.Update(root);
      Assert.AreEqual(TransactionalTypes.Manual, root.TransactionalType, "Transactional type should match");
      Assert.AreEqual("Update", root.Data, "Data should match");
      Assert.IsFalse(root.IsNew, "Should not be new");
      Assert.IsFalse(root.IsDirty, "Should not be dirty");
    }

    [TestMethod]
    public void UpdateTransactionScope()
    {
      TestDIContext testDIContext = TestDIContextFactory.CreateContext(
        opts => opts.DataPortal(dp => dp.AddServerSideDataPortal(cfg => cfg.RegisterObjectFactoryLoader<ObjectFactoryLoader<RootFactory1>>()))
        );
      IDataPortal<Root> dataPortal = testDIContext.CreateDataPortal<Root>();

      var root = dataPortal.Fetch();
      root.Data = "abc";

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
      TestDIContext testDIContext = TestDIContextFactory.CreateContext(
        options => options
        .Data(
          cfg => cfg
          .DefaultTransactionIsolationLevel(TransactionIsolationLevel.RepeatableRead)
          .DefaultTransactionTimeoutInSeconds(45)
        )
        .DataPortal(
          dp => dp.AddServerSideDataPortal(cfg => cfg.RegisterObjectFactoryLoader<ObjectFactoryLoader<RootFactory4>>())
          )
        );
      IDataPortal<Root> dataPortal = testDIContext.CreateDataPortal<Root>();

      var root = dataPortal.Create();
      root.Data = "abc";


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
      TestDIContext testDIContext = TestDIContextFactory.CreateContext(
        options => options
        .Data(
          cfg => cfg
          .DefaultTransactionIsolationLevel(TransactionIsolationLevel.RepeatableRead)
          .DefaultTransactionTimeoutInSeconds(45)
        )
        .DataPortal(
          dp => dp.AddServerSideDataPortal(cfg => cfg.RegisterObjectFactoryLoader<ObjectFactoryLoader<RootFactory5>>()))
        );
      IDataPortal<Root> dataPortal = testDIContext.CreateDataPortal<Root>();

      var root = dataPortal.Create();
      root.Data = "abc";


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
      IDataPortal<Root> dataPortal = _testDIContext.CreateDataPortal<Root>();

      dataPortal.Delete("abc");

      Assert.AreEqual("Delete", TestResults.GetResult("ObjectFactory"), "Data should match");
    }

    [TestMethod]
    public void FetchLoadProperty()
    {
      TestDIContext testDIContext = TestDIContextFactory.CreateContext(
        options => options.DataPortal(
          dp => dp.AddServerSideDataPortal(cfg => cfg.RegisterObjectFactoryLoader<ObjectFactoryLoader<RootFactory3>>()))
        );
      IDataPortal<Root> dataPortal = testDIContext.CreateDataPortal<Root>();

      var root = dataPortal.Fetch();
      Assert.AreEqual("Fetch", root.Data, "Data should match");
      Assert.IsFalse(root.IsNew, "Should not be new");
      Assert.IsFalse(root.IsDirty, "Should not be dirty");
    }

    [TestMethod]
    public void DataPortalExecute_OnCommandObjectWithLocalProxy_CallsFactoryExecute()
    {
      TestDIContext testDIContext = TestDIContextFactory.CreateDefaultContext();
      IDataPortal<CommandObject> dataPortal = testDIContext.CreateDataPortal<CommandObject>();

      var test = CommandObject.Execute(dataPortal);
      // return value is set in Execute method in CommandObjectFactory
      Assert.IsTrue(test);
    }

    [TestMethod]
    [ExpectedException(typeof(DataPortalException))]
    public void DataPortalExecute_OnCommandObjectWithFalseExecuteMethod_ThrowsExeptionMehodNotFound()
    {
      TestDIContext testDIContext = TestDIContextFactory.CreateDefaultContext();
      IDataPortal<CommandObjectMissingFactoryMethod> dataPortal = testDIContext.CreateDataPortal<CommandObjectMissingFactoryMethod>();

      try
      {
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