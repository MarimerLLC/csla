//-----------------------------------------------------------------------
// <copyright file="RoutingTagTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
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

namespace csla.netcore.test.DataPortal
{
  [TestClass]
  public class RoutingTagTests
  {

    // Not sure what the CSLA 6 equivalent of this is - or, for that matter, why it might be useful.
    // There is no configuration change performed for the tests, so why would we need to clean one up?
    // If there is global state to clean up here, I don't understand how/why
    //[TestCleanup]
    //public void Cleanup()
    //{
    //  new Csla.Configuration.CslaConfiguration()
    //    .VersionRoutingTag(null)
    //    .SettingsChanged();
    //}

    [TestMethod]
    public void GetRoutingTag()
    {
      var proxy = CreateTestHttpProxy();
      var method = proxy.GetType().BaseType.GetMethods(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
        .Where(r => r.Name == "GetRoutingToken")
        .FirstOrDefault();

      string result = (string)method.Invoke(proxy, new object[] { typeof(RoutingTest) });
      Assert.AreEqual("mytag", result);
    }

    [TestMethod]
    public void CreateTagNoVersionNoRoute()
    {
      var proxy = CreateTestHttpProxy();
      var method = proxy.GetType().GetMethods(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
        .Where(r => r.Name == "CreateOperationTag")
        .FirstOrDefault();
      string result = (string)method.Invoke(proxy, new object[] { "create", "", "" });
      Assert.AreEqual("create", result);
    }

    [TestMethod]
    public void CreateTagNoVersion()
    {
      var proxy = CreateTestHttpProxy();
      var method = proxy.GetType().GetMethods(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
        .Where(r => r.Name == "CreateOperationTag")
        .FirstOrDefault();
      string result = (string)method.Invoke(proxy, new object[] { "create", "", "mytag" });
      Assert.AreEqual("create/mytag-", result);
    }

    [TestMethod]
    public void CreateTagNoRoute()
    {
      var proxy = CreateTestHttpProxy();
      var method = proxy.GetType().GetMethods(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
        .Where(r => r.Name == "CreateOperationTag")
        .FirstOrDefault();
      string result = (string)method.Invoke(proxy, new object[] { "create", "v1", "" });
      Assert.AreEqual("create/-v1", result);
    }

    [TestMethod]
    public void CreateTag()
    {
      var proxy = CreateTestHttpProxy();
      var method = proxy.GetType().GetMethods(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
        .Where(r => r.Name == "CreateOperationTag")
        .FirstOrDefault();
      string result = (string)method.Invoke(proxy, new object[] { "create", "v1", "mytag" });
      Assert.AreEqual("create/mytag-v1", result);
    }

    /// <summary>
    /// Create an Http Proxy instance for use in testing
    /// </summary>
    /// <returns>An instance of Csla.Channels.Http.HttpProxy for use in testing</returns>
    private Csla.Channels.Http.HttpProxy CreateTestHttpProxy()
    {
      Csla.Channels.Http.HttpProxy proxy;
      Csla.Channels.Http.HttpProxyOptions proxyOptions;
      System.Net.Http.HttpClient httpClient;

      var applicationContext = ApplicationContextFactory.CreateTestApplicationContext();
      httpClient = new System.Net.Http.HttpClient();
      proxyOptions = new Csla.Channels.Http.HttpProxyOptions();
      proxy = new Csla.Channels.Http.HttpProxy(applicationContext, httpClient, proxyOptions);

      return proxy;
    }
  }

  [Serializable]
  [DataPortalServerRoutingTag("mytag")]
  public class RoutingTest : BusinessBase<RoutingTest>
  {
  }
}