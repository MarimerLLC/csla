//-----------------------------------------------------------------------
// <copyright file="FluentConfigTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
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
    private CslaConfigurationOptions cslaOptions;

    [TestInitialize]
    public void Initialize()
    {
      var config = new ConfigurationBuilder()
               .AddJsonFile("appsettings.test.json")
               .Build();
      cslaOptions = new CslaConfigurationOptions();
      config.Bind("csla", cslaOptions);
    }

    [TestMethod]
    public void ReadConfigCore()
    {
      Assert.AreEqual(PropertyChangedModes.Windows, Csla.ApplicationContext.PropertyChangedMode);
      Assert.AreEqual("test1,test2", ConfigurationManager.AppSettings["CslaPropertyInfoFactory"], "CslaPropertyInfoFactory");
      Assert.AreEqual("testReader", ConfigurationManager.AppSettings["CslaReader"], "CslaReader");
      Assert.AreEqual("testSerializationFormatter", ConfigurationManager.AppSettings["CslaSerializationFormatter"], "CslaSerializationFormatter");
      Assert.AreEqual("testIsInRoleProvider", ConfigurationManager.AppSettings["CslaIsInRoleProvider"], "CslaIsInRoleProvider");
      Assert.IsInstanceOfType(MobileRequestProcessor.FactoryLoader, typeof(TestMobileFactoryLoader), "CslaMobileFactoryLoader");      
      Assert.AreEqual(PrincipalCache.MaxCacheSize, cslaOptions.PrincipalCacheSize, "CslaPrincipalCacheSize");      
      Assert.IsInstanceOfType(CslaReaderWriterFactory.GetCslaWriter(), typeof(TestCslaWriter), "CslaMobileFactoryLoader");      
      Assert.AreEqual("testDbProvider", ConfigurationManager.AppSettings["dbProvider"], "dbProvider");      
    }

    [TestMethod]
    public void ReadConfigData()
    {
       Assert.AreEqual(Csla.TransactionIsolationLevel.RepeatableRead, Csla.ApplicationContext.DefaultTransactionIsolationLevel, "DefaultTransactionIsolationLevel");
       Assert.AreEqual(60, Csla.ApplicationContext.DefaultTransactionTimeoutInSeconds, "DefaultTransactionTimeoutInSeconds");
    }

    [TestMethod]
    public void DataPortalConfigCore()
    {
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