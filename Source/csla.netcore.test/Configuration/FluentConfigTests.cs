//-----------------------------------------------------------------------
// <copyright file="FluentConfigTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using Csla.Configuration;
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
  public class FluentConfigTests
  {
    [TestMethod]
    public void FluentConfigCore()
    {
      new Csla.Configuration.CslaConfiguration()
        .PropertyChangedMode(Csla.ApplicationContext.PropertyChangedModes.Windows)
        .PropertyInfoFactory("a,b")
        .RuleSet("abc")
        .UseReflectionFallback(false);

      Assert.AreEqual(Csla.ApplicationContext.PropertyChangedModes.Windows, Csla.ApplicationContext.PropertyChangedMode);
      Assert.AreEqual("a,b", Csla.Configuration.ConfigurationManager.AppSettings["CslaPropertyInfoFactory"]);
      Assert.AreEqual("abc", Csla.ApplicationContext.RuleSet);
      Assert.AreEqual(false, Csla.ApplicationContext.UseReflectionFallback);

      new Csla.Configuration.CslaConfiguration()
        .PropertyChangedMode(Csla.ApplicationContext.PropertyChangedModes.Xaml)
        .PropertyInfoFactory(null)
        .RuleSet(null)
        .UseReflectionFallback(true);
    }

    [TestMethod]
    public void FluentConfigDataPortal()
    {
      new Csla.Configuration.CslaConfiguration()
        .DataPortal.AuthenticationType("custom")
        .DataPortal.AutoCloneOnUpdate(false)
        .DataPortal.DataPortalActivator(new TestActivator())
        .DataPortal.DataPortalProxyFactory("abc")
        .DataPortal.DataPortalReturnObjectOnException(true)
        .DataPortal.DefaultDataPortalProxy("abc", "def")
        .DataPortal.ExceptionInspectorType("abc")
        .DataPortal.FactoryLoaderType("abc")
        .DataPortal.InterceptorType("abc")
        .DataPortal.ServerAuthorizationProvider("abc");

      Assert.AreEqual("custom", Csla.ApplicationContext.AuthenticationType, "AuthenticationType");
      Assert.AreEqual(false, Csla.ApplicationContext.AutoCloneOnUpdate, "AutoCloneOnUpdate");
      Assert.IsInstanceOfType(Csla.ApplicationContext.DataPortalActivator, typeof(TestActivator), "DataPortalActivator");
      Assert.AreEqual("abc", Csla.ApplicationContext.DataPortalProxyFactory, "DataPortalProxyFactory");
      Assert.AreEqual(true, Csla.ApplicationContext.DataPortalReturnObjectOnException, "DataPortalReturnObjectOnException");
      Assert.AreEqual("abc", Csla.ApplicationContext.DataPortalProxy, "DataPortalProxy");
      Assert.AreEqual("def", Csla.ApplicationContext.DataPortalUrlString, "DataPortalUrlString");
      Assert.AreEqual("abc", Csla.Server.DataPortalExceptionHandler.ExceptionInspector, "ExceptionInspector");
      Assert.AreEqual("abc", ConfigurationManager.AppSettings["CslaObjectFactoryLoader"], "CslaObjectFactoryLoader");
      Assert.AreEqual("abc", ConfigurationManager.AppSettings["CslaDataPortalInterceptor"], "InterceptorType");
      Assert.AreEqual("abc", ConfigurationManager.AppSettings["CslaAuthorizationProvider"], "CslaAuthorizationProvider");

      new Csla.Configuration.CslaConfiguration()
        .DataPortal.AuthenticationType(null)
        .DataPortal.AutoCloneOnUpdate(true)
        .DataPortal.DataPortalActivator(null)
        .DataPortal.DataPortalProxyFactory(null)
        .DataPortal.DataPortalReturnObjectOnException(false)
        .DataPortal.DefaultDataPortalProxy(typeof(Csla.DataPortalClient.LocalProxy).AssemblyQualifiedName, null)
        .DataPortal.ExceptionInspectorType(null)
        .DataPortal.FactoryLoaderType(null)
        .DataPortal.InterceptorType(null)
        .DataPortal.ServerAuthorizationProvider(null);
    }

    [TestMethod]
    public void FluentConfigDataPortalDescriptors()
    {
      //TODO: test resource/type mappings
    }

    [TestMethod]
    public void FluentConfigData()
    {
      new CslaConfiguration()
        .Data.DefaultTransactionIsolationLevel(Csla.TransactionIsolationLevel.RepeatableRead)
        .Data.DefaultTransactionTimeoutInSeconds(123);

      Assert.AreEqual(Csla.TransactionIsolationLevel.RepeatableRead, Csla.ApplicationContext.DefaultTransactionIsolationLevel, "DefaultTransactionIsolationLevel");
      Assert.AreEqual(123, Csla.ApplicationContext.DefaultTransactionTimeoutInSeconds, "DefaultTransactionTimeoutInSeconds");

      new CslaConfiguration()
        .Data.DefaultTransactionIsolationLevel(Csla.TransactionIsolationLevel.ReadCommitted)
        .Data.DefaultTransactionTimeoutInSeconds(90);
    }

    [TestMethod]
    public void FluentConfigSecurity()
    {
      new CslaConfiguration()
        .Security.PrincipalCacheMaxCacheSize(123);

      Assert.AreEqual(123, Csla.Security.PrincipalCache.MaxCacheSize, "MaxCacheSize");

      new CslaConfiguration()
        .Security.PrincipalCacheMaxCacheSize(10);
    }
  }

  public class TestActivator : Csla.Server.IDataPortalActivator
  {
    public object CreateInstance(Type requestedType)
    {
      throw new NotImplementedException();
    }

    public void FinalizeInstance(object obj)
    {
      throw new NotImplementedException();
    }

    public void InitializeInstance(object obj)
    {
      throw new NotImplementedException();
    }

    public Type ResolveType(Type requestedType)
    {
      throw new NotImplementedException();
    }
  }
}