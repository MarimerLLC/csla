//-----------------------------------------------------------------------
// <copyright file="ObjectFactoryTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Always make sure to cleanup after each test </summary>
//-----------------------------------------------------------------------

using Csla.Configuration;
using Csla.Testing;
using Csla.TestHelpers;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.ObjectFactory
{
  [TestClass]
  public class ObjectFactoryTests
  {
    private static CslaTestHost _testHost;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
      _testHost = CslaTestHost.Create(t => t.ConfigureCsla(options => options.DataPortal(dp => dp.AddServerSideDataPortal(
          cfg => cfg.RegisterObjectFactoryLoader<ObjectFactoryLoader<RootFactory>>())
        )));
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
      _testHost?.Dispose();
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

    // TODO: fix test
    [Ignore]
    [TestMethod]
    [TestCategory("SkipWhenLiveUnitTesting")]
    public void Create()
    {
      using var testHost = CslaTestHost.Create(t => t.ConfigureCsla(// TODO: What proxy can we use for this test? Old one was Remoting, now retired
      //  options => options.Services.AddTransient<DataPortalClient.IDataPortalProxy, Testing.Business.TestProxies.AppDomainProxy>(), 
        opts => opts.DataPortal(
          dp => dp.AddServerSideDataPortal(cfg => cfg.RegisterObjectFactoryLoader<ObjectFactoryLoader<RootFactory>>()))).AsPrincipal(new System.Security.Claims.ClaimsPrincipal()));

      IDataPortal<Root> dataPortal = testHost.GetDataPortal<Root>();

      var root = dataPortal.Create();
      Assert.AreEqual("Create", root.Data, "Data should match");
      Assert.AreEqual(ApplicationContext.ExecutionLocations.Server, root.Location, "Location should match");
      Assert.IsTrue(root.IsNew, "Should be new");
      Assert.IsTrue(root.IsDirty, "Should be dirty");
    }

    [TestMethod]
    public void CreateLocal()
    {
      // TODO: What proxy can we use for this test? Old one was Remoting, now retired
      //CslaTestHost testHost = CslaTestHost.Create(t => t.AsPrincipal(//  options => options.Services.AddTransient<DataPortalClient.IDataPortalProxy, Testing.Business.TestProxies.AppDomainProxy>(), 
      //));
      using var testHost = CslaTestHost.Create(t => t.ConfigureCsla(opts => opts.DataPortal(dp => dp.AddServerSideDataPortal(cfg => cfg.RegisterObjectFactoryLoader<ObjectFactoryLoader<RootFactoryC>>()))));

      IDataPortal<Root> dataPortal = testHost.GetDataPortal<Root>();

      var root = dataPortal.Create("abc");
      Assert.AreEqual("Create abc", root.Data, "Data should match");
      Assert.AreEqual(ApplicationContext.ExecutionLocations.Client, root.Location, "Location should match");
      Assert.IsTrue(root.IsNew, "Should be new");
      Assert.IsTrue(root.IsDirty, "Should be dirty");
    }

    [TestMethod]
    public void CreateWithParam()
    {
      using var testHost = CslaTestHost.Create(t => t.ConfigureCsla(// TODO: What proxy can we use for this test? Old one was Remoting, now retired
      //  options => options.Services.AddTransient<DataPortalClient.IDataPortalProxy, Testing.Business.TestProxies.AppDomainProxy>(), 
        opts => opts.DataPortal(dp => dp.AddServerSideDataPortal(cfg => cfg.RegisterObjectFactoryLoader<ObjectFactoryLoader<RootFactory>>()))).AsPrincipal(new System.Security.Claims.ClaimsPrincipal()));

      IDataPortal<Root> dataPortal = testHost.GetDataPortal<Root>();

      var root = dataPortal.Create("abc");
      Assert.AreEqual("Create abc", root.Data, "Data should match");
      Assert.AreEqual(ApplicationContext.ExecutionLocations.Client, root.Location, "Location should match");
      Assert.IsTrue(root.IsNew, "Should be new");
      Assert.IsTrue(root.IsDirty, "Should be dirty");
    }

    // TODO: fix test
    [Ignore]
    [TestMethod]
    [ExpectedException(typeof(MissingMethodException))]
    public void CreateMissing()
    {
      using var testHost = CslaTestHost.Create(t => t.ConfigureCsla(// TODO: What proxy can we use for this test? Old one was Remoting, now retired
      //  options => options.Services.AddTransient<DataPortalClient.IDataPortalProxy, Testing.Business.TestProxies.AppDomainProxy>(), 
        opts => opts.DataPortal(dp => dp.AddServerSideDataPortal(cfg => cfg.RegisterObjectFactoryLoader<ObjectFactoryLoader<RootFactory1>>()))).AsPrincipal(new System.Security.Claims.ClaimsPrincipal()));

      IDataPortal<Root> dataPortal = testHost.GetDataPortal<Root>();

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
      IDataPortal<Root> dataPortal = _testHost.GetDataPortal<Root>();

      var root = dataPortal.Fetch();
      Assert.AreEqual("Fetch", root.Data, "Data should match");
      Assert.IsFalse(root.IsNew, "Should not be new");
      Assert.IsFalse(root.IsDirty, "Should not be dirty");
    }

    [TestMethod]
    public void FetchCriteria()
    {
      IDataPortal<Root> dataPortal = _testHost.GetDataPortal<Root>();

      var root = dataPortal.Fetch("abc");
      Assert.AreEqual("abc", root.Data, "Data should match");
      Assert.IsFalse(root.IsNew, "Should not be new");
      Assert.IsFalse(root.IsDirty, "Should not be dirty");
    }

    [TestMethod]
    public void Update()
    {
      IDataPortal<Root> dataPortal = _testHost.GetDataPortal<Root>();

      var root = dataPortal.Fetch();
      root.Data = "abc";

      root = dataPortal.Update(root);
      Assert.AreEqual(TransactionalTypes.Manual, root.TransactionalType, "Transactional type should match");
      Assert.AreEqual("Update", root.Data, "Data should match");
      Assert.IsFalse(root.IsNew, "Should not be new");
      Assert.IsFalse(root.IsDirty, "Should not be dirty");
    }

    // TODO: fix test
    [Ignore]
    [TestMethod]
    public void UpdateTransactionScope()
    {
      using var testHost = CslaTestHost.Create(t => t.ConfigureCsla(opts => opts.DataPortal(dp => dp.AddServerSideDataPortal(cfg => cfg.RegisterObjectFactoryLoader<ObjectFactoryLoader<RootFactory1>>()))));
      IDataPortal<Root> dataPortal = testHost.GetDataPortal<Root>();

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

    // TODO: fix test
    [Ignore]
    [TestMethod]
    public void UpdateTransactionScopeUsingCustomTransactionLevelAndTimeout()
    {
      using var testHost = CslaTestHost.Create(t => t.ConfigureCsla(options => options
        .Data(cfg =>
          {
            cfg.DefaultTransactionIsolationLevel = TransactionIsolationLevel.RepeatableRead;
            cfg.DefaultTransactionTimeoutInSeconds = 45;
          }
        )
        .DataPortal(
          dp => dp.AddServerSideDataPortal(cfg => cfg.RegisterObjectFactoryLoader<ObjectFactoryLoader<RootFactory4>>())
          )));
      IDataPortal<Root> dataPortal = testHost.GetDataPortal<Root>();

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


    // TODO: fix test
    [Ignore]
    [TestMethod]
    public void UpdateTransactionScopeUsingDefaultTransactionLevelAndTimeout()
    {
      using var testHost = CslaTestHost.Create(t => t.ConfigureCsla(options => options
        .Data(cfg =>
        {
          cfg.DefaultTransactionIsolationLevel = TransactionIsolationLevel.RepeatableRead;
          cfg.DefaultTransactionTimeoutInSeconds = 45;
        })
        .DataPortal(
          dp => dp.AddServerSideDataPortal(cfg => cfg.RegisterObjectFactoryLoader<ObjectFactoryLoader<RootFactory5>>()))));
      IDataPortal<Root> dataPortal = testHost.GetDataPortal<Root>();

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
      IDataPortal<Root> dataPortal = _testHost.GetDataPortal<Root>();

      dataPortal.Delete("abc");

      Assert.AreEqual("Delete", TestResults.GetResult("ObjectFactory"), "Data should match");
    }

    [TestMethod]
    public void FetchLoadProperty()
    {
      using var testHost = CslaTestHost.Create(t => t.ConfigureCsla(options => options.DataPortal(
          dp => dp.AddServerSideDataPortal(cfg => cfg.RegisterObjectFactoryLoader<ObjectFactoryLoader<RootFactory3>>()))));
      IDataPortal<Root> dataPortal = testHost.GetDataPortal<Root>();

      var root = dataPortal.Fetch();
      Assert.AreEqual("Fetch", root.Data, "Data should match");
      Assert.IsFalse(root.IsNew, "Should not be new");
      Assert.IsFalse(root.IsDirty, "Should not be dirty");
    }

    [TestMethod]
    public void DataPortalExecute_OnCommandObjectWithLocalProxy_CallsFactoryExecute()
    {
      using var testHost = CslaTestHost.Create();
      IDataPortal<CommandObject> dataPortal = testHost.GetDataPortal<CommandObject>();

      var test = CommandObject.Execute(dataPortal);
      // return value is set in Execute method in CommandObjectFactory
      Assert.IsTrue(test);
    }

    // TODO: fix test
    [Ignore]
    [TestMethod]
    [ExpectedException(typeof(DataPortalException))]
    public void DataPortalExecute_OnCommandObjectWithFalseExecuteMethod_ThrowsExeptionMehodNotFound()
    {
      using var testHost = CslaTestHost.Create();
      IDataPortal<CommandObjectMissingFactoryMethod> dataPortal = testHost.GetDataPortal<CommandObjectMissingFactoryMethod>();

      try
      {
        var test = CommandObjectMissingFactoryMethod.Execute(dataPortal);
      }
      catch (DataPortalException ex)
      {
        // inner exception should be System.NotImplementedException and mesaage should contain methodname 
        Assert.AreEqual(typeof(NotImplementedException), ex.InnerException.GetType());
        Assert.IsTrue(ex.InnerException.Message.Contains("ExecuteMissingMethod"));
        // rethrow exception 
        throw;
      }
      Assert.Fail("Should throw exception");
    }

    [TestMethod("The return type of an business object factory type can be of type Task<T> instead of Task<object>.")]
    public async Task BusinessObbjectFactory_ReturnTypesOfAsyncMethods()
    {
      using var testHost = CslaTestHost.Create(t => t.ConfigureCsla(options => options.DataPortal(dp => dp.AddServerSideDataPortal(
          cfg => cfg.RegisterObjectFactoryLoader<ObjectFactoryLoader<RootFactoryAsync>>())
        )));
      
      var obj = await testHost.GetDataPortal<AsyncRootFactoryBO>().CreateAsync();
      using (new AssertionScope())
      {
        obj.Should().NotBeNull();
        obj.Text.Should().NotBeNullOrWhiteSpace();
      }
    }

    #region Multiple parameters and dependency injection (issue #1707)

    private static CslaTestHost CreateFactoryContext<TFactory>(Action<IServiceCollection> configureServices = null)
      where TFactory : class
    {
      return CslaTestHost.Create(t =>
      {
        t.ConfigureCsla(opts => opts.DataPortal(dp => dp.AddServerSideDataPortal(
          cfg => cfg.RegisterObjectFactoryLoader<ObjectFactoryLoader<TFactory>>())));
        t.AsPrincipal(new System.Security.Claims.ClaimsPrincipal());
        if (configureServices is not null)
          t.ConfigureServices(configureServices);
      });
    }

    [TestMethod("Factory Create accepts multiple criteria parameters")]
    public void CreateWithMultipleParameters()
    {
      var testHost = CreateFactoryContext<MultiParamRootFactory>();
      var dataPortal = testHost.GetDataPortal<Root>();

      var root = dataPortal.Create("abc", 5);

      Assert.AreEqual("Create abc 5", root.Data);
      Assert.IsTrue(root.IsNew);
    }

    [TestMethod("Factory Fetch accepts multiple criteria parameters")]
    public void FetchWithMultipleParameters()
    {
      var testHost = CreateFactoryContext<MultiParamRootFactory>();
      var dataPortal = testHost.GetDataPortal<Root>();

      var root = dataPortal.Fetch("abc", 5);

      Assert.AreEqual("Fetch abc 5", root.Data);
      Assert.IsFalse(root.IsNew);
    }

    [TestMethod("Factory method resolves [Inject] parameters from the service provider")]
    public void FetchWithInjectedService()
    {
      var testHost = CreateFactoryContext<InjectRootFactory>(
        services => services.AddTransient<IFactoryTestService, FactoryTestService>());
      var dataPortal = testHost.GetDataPortal<Root>();

      var root = dataPortal.Fetch("id7");

      Assert.AreEqual("id7:injected", root.Data);
    }

    [TestMethod("Factory method supports multiple criteria together with [Inject]")]
    public void CreateWithMultipleParametersAndInjectedService()
    {
      var testHost = CreateFactoryContext<InjectRootFactory>(
        services => services.AddTransient<IFactoryTestService, FactoryTestService>());
      var dataPortal = testHost.GetDataPortal<Root>();

      var root = dataPortal.Create("abc", 5);

      Assert.AreEqual("abc 5 injected", root.Data);
      Assert.IsTrue(root.IsNew);
    }

    [TestMethod("Factory overloads are disambiguated by criteria parameter count")]
    public void FetchOverloadResolution()
    {
      var testHost = CreateFactoryContext<OverloadRootFactory>();
      var dataPortal = testHost.GetDataPortal<Root>();

      Assert.AreEqual("Fetch0", dataPortal.Fetch().Data);
      Assert.AreEqual("Fetch1 abc", dataPortal.Fetch("abc").Data);
      Assert.AreEqual("Fetch2 abc 5", dataPortal.Fetch("abc", 5).Data);
    }

    [TestMethod("Object factory supports constructor injection")]
    public void FetchWithConstructorInjectedService()
    {
      var testHost = CreateFactoryContext<CtorInjectRootFactory>(
        services => services.AddTransient<IFactoryTestService, FactoryTestService>());
      var dataPortal = testHost.GetDataPortal<Root>();

      var root = dataPortal.Fetch();

      Assert.AreEqual("injected", root.Data);
    }

    [TestMethod("Command object factory Execute supports criteria plus [Inject]")]
    public void ExecuteCommandWithInjectedService()
    {
      using var testHost = CslaTestHost.Create(t => t.ConfigureServices(services => services.AddTransient<IFactoryTestService, FactoryTestService>()));
      var dataPortal = testHost.GetDataPortal<InjectCommandObject>();

      var result = InjectCommandObject.Execute(dataPortal);

      Assert.AreEqual("injected", result.Value);
    }

    #endregion
  }
}