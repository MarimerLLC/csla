//-----------------------------------------------------------------------
// <copyright file="FluentConfigTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using Csla.Configuration;
using Microsoft.Extensions.Configuration;
using static Csla.ApplicationContext;
using Csla.Server.Hosts.Mobile;
using Csla.Server;
using System;
using Csla.Security;
using Csla.Serialization.Mobile;
using System.Collections.Generic;
using System.IO;
using Csla;

#if !NUNIT

using Microsoft.VisualStudio.TestTools.UnitTesting;

#else
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#endif

namespace csla.netcore.test.Configuration
{
  [TestClass]
  public class ReadConfigFromDotnetCoreSubsystemTests
  {
    [TestMethod]
    public void ReadConfigCore()
    {
      var config = new ConfigurationBuilder()
             .AddJsonFile("appsettings.coresettings.test.json")
             .Build()
             .ConfigureCsla();

      Assert.AreEqual(PropertyChangedModes.Windows,(PropertyChangedModes)Enum.Parse(typeof(PropertyChangedModes), ConfigurationManager.AppSettings["CslaPropertyChangedMode"]));
      Assert.AreEqual("test1,test2", ConfigurationManager.AppSettings["CslaPropertyInfoFactory"], "CslaPropertyInfoFactory");
      Assert.AreEqual("testReader", ConfigurationManager.AppSettings["CslaReader"], "CslaReader");
      Assert.AreEqual("testSerializationFormatter", ConfigurationManager.AppSettings["CslaSerializationFormatter"], "CslaSerializationFormatter");
      Assert.IsInstanceOfType(MobileRequestProcessor.FactoryLoader, typeof(TestMobileFactoryLoader), "CslaMobileFactoryLoader");
      Assert.AreEqual("10", ConfigurationManager.AppSettings["CslaPrincipalCacheSize"], "CslaPrincipalCacheSize");
      Assert.IsInstanceOfType(CslaReaderWriterFactory.GetCslaWriter(), typeof(TestCslaWriter), "CslaWriter");
      Assert.AreEqual("testDbProvider", ConfigurationManager.AppSettings["CslaDbProvider"], "CslaDbProvider");
      Assert.AreEqual("RepeatableRead", ConfigurationManager.AppSettings["CslaDefaultTransactionIsolationLevel"], "DefaultTransactionIsolationLevel");
      Assert.AreEqual("60", ConfigurationManager.AppSettings["CslaDefaultTransactionTimeoutInSeconds"], "DefaultTransactionTimeoutInSeconds");                
    }

    [TestCleanup]
    public void ClearContextsAfterEachTest()
    {
      CleanupContextAndConfigurations();
    }

    private static void CleanupContextAndConfigurations()
    {
      ApplicationContext.AuthenticationType = null;
      ApplicationContext.DataPortalActivator = null;
      ConfigurationManager.AppSettings["CslaDataPortalActivator"] = null;
      ConfigurationManager.AppSettings["CslaDataPortalProxy"] = null;
      ConfigurationManager.AppSettings["CslaMobileFactoryLoader"] = null;
      ConfigurationManager.AppSettings["CslaDataPortalProxyFactory"] = null;
      ConfigurationManager.AppSettings["CslaReader"] = null; FactoryDataPortal.FactoryLoader = null;
      ApplicationContext.DataPortalReturnObjectOnException = true;
      DataPortalExceptionHandler.ExceptionInspector = null;
      Csla.Server.DataPortal.InterceptorType = null;
      ApplicationContext.DataPortalProxyFactory = string.Empty;
      ApplicationContext.DataPortalProxy = null;
      CslaReaderWriterFactory.SetCslaWriterType(null);
      CslaReaderWriterFactory.SetCslaReaderType(null);
      ConfigurationManager.AppSettings["CslaReader"] = null;
      ApplicationContext.RuleSet = null;
      Csla.Configuration.ConfigurationManager.AppSettings["CslaWriter"] = null;
      ApplicationContext.UseReflectionFallback = true;
      ApplicationContext.PropertyChangedMode = ApplicationContext.PropertyChangedModes.Xaml;
      ApplicationContext.DefaultTransactionIsolationLevel = TransactionIsolationLevel.ReadCommitted;
      ApplicationContext.DefaultTransactionTimeoutInSeconds = 90;
      ConfigurationManager.AppSettings["CslaSerializationFormatter"] = "MobileFormatter";
      new Csla.Configuration.CslaConfiguration()
         .PropertyChangedMode(Csla.ApplicationContext.PropertyChangedModes.Xaml)
         .PropertyInfoFactory(null)
         .RuleSet(null)
         .UseReflectionFallback(true);
      ApplicationContext.PropertyChangedMode = ApplicationContext.PropertyChangedModes.Xaml;
      ConfigurationManager.AppSettings["CslaDefaultTransactionIsolationLevel"] = null;
      ApplicationContext.Clear();
    }

    [TestInitialize]
    public void CleanupConfigurationBeforeEachTest()
    {
      CleanupContextAndConfigurations();
    }

    [TestMethod]
    public void DataPortalConfigCore()
    {
      var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.dataportal.test.json")
            .Build()
            .ConfigureCsla();

      Assert.AreEqual("testAuthentication", AuthenticationType, nameof(AuthenticationType));
      Assert.AreEqual(true, AutoCloneOnUpdate, nameof(AutoCloneOnUpdate));
      Assert.IsInstanceOfType(DataPortalActivator, typeof(TestActivator), nameof(DataPortalActivator));
      Assert.AreEqual("testProxyFactory", DataPortalProxyFactory, nameof(DataPortalProxyFactory));
      Assert.AreEqual(true, DataPortalReturnObjectOnException, nameof(DataPortalReturnObjectOnException));
      Assert.AreEqual("testProxy", DataPortalProxy, nameof(DataPortalProxy));
      Assert.AreEqual("testProxyUrl", DataPortalUrlString, nameof(DataPortalUrlString));
      Assert.AreEqual("testExceptionInspector", Csla.Server.DataPortalExceptionHandler.ExceptionInspector, nameof(Csla.Server.DataPortalExceptionHandler.ExceptionInspector));
      Assert.IsInstanceOfType(FactoryDataPortal.FactoryLoader, typeof(TestObjectFactoryLoader), "CslaObjectFactoryLoader");
      Assert.AreEqual("testInterceptor", ConfigurationManager.AppSettings["CslaDataPortalInterceptor"], "InterceptorType");
      Assert.AreEqual("testAuthorizationProvider", ConfigurationManager.AppSettings["CslaAuthorizationProvider"], "CslaAuthorizationProvider");
    }
  }

  public class TestMobileFactoryLoader : IMobileFactoryLoader
  {
    public object GetFactory(string factoryName)
    {
      return null;
    }
  }

  public class TestObjectFactoryLoader : IObjectFactoryLoader
  {
    public object GetFactory(string factoryName)
    {
      return null;
    }

    public Type GetFactoryType(string factoryName)
    {
      return null;
    }
  }

  public class TestCslaWriter : ICslaWriter
  {
    public void Write(Stream serializationStream, List<SerializationInfo> objectData)
    {

    }
  }
}