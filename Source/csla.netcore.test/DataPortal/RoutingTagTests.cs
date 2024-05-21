﻿//-----------------------------------------------------------------------
// <copyright file="RoutingTagTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

using System.Reflection;
using Csla;
using Csla.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace csla.netcore.test.DataPortal
{
  [TestClass]
  public class RoutingTagTests
  {
    private static TestDIContext _testDIContext;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
      _testDIContext = TestDIContextFactory.CreateDefaultContext();
    }

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
      var method = proxy.GetType().BaseType
        .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
        .FirstOrDefault(r => r.Name == "GetRoutingToken");

      string result = (string)method.Invoke(proxy, [typeof(RoutingTest)]);
      Assert.AreEqual("mytag", result);
    }

    [TestMethod]
    public void CreateTagNoVersionNoRoute()
    {
      var proxy = CreateTestHttpProxy();
      var method = proxy.GetType()
        .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
        .First(r => r.Name == "CreateOperationTag");
      string result = (string)method.Invoke(proxy, ["create", "", ""]);
      Assert.AreEqual("create", result);
    }

    [TestMethod]
    public void CreateTagNoVersion()
    {
      var proxy = CreateTestHttpProxy();
      var method = proxy.GetType()
        .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
        .First(r => r.Name == "CreateOperationTag");
      string result = (string)method.Invoke(proxy, ["create", "", "mytag"]);
      Assert.AreEqual("create/mytag-", result);
    }

    [TestMethod]
    public void CreateTagNoRoute()
    {
      var proxy = CreateTestHttpProxy();
      var method = proxy.GetType()
        .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
        .First(r => r.Name == "CreateOperationTag");
      string result = (string)method.Invoke(proxy, ["create", "v1", ""]);
      Assert.AreEqual("create/-v1", result);
    }

    [TestMethod]
    public void CreateTag()
    {
      var proxy = CreateTestHttpProxy();
      var method = proxy.GetType()
        .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
        .First(r => r.Name == "CreateOperationTag");
      string result = (string)method.Invoke(proxy, ["create", "v1", "mytag"]);
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
      HttpClient httpClient;

      var applicationContext = _testDIContext.CreateTestApplicationContext();
      var dataPortalOptions = applicationContext.GetRequiredService<Csla.Configuration.DataPortalOptions>();
      httpClient = new HttpClient();
      proxyOptions = new Csla.Channels.Http.HttpProxyOptions();
      proxy = new Csla.Channels.Http.HttpProxy(applicationContext, httpClient, proxyOptions, dataPortalOptions);

      return proxy;
    }
  }

  [Serializable]
  [DataPortalServerRoutingTag("mytag")]
  public class RoutingTest : BusinessBase<RoutingTest>;
}