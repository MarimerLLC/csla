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
    [TestCleanup]
    public void Cleanup()
    {
      new Csla.Configuration.CslaConfiguration()
        .VersionRoutingTag(null)
        .SettingsChanged();
    }

    [TestMethod]
    public void GetRoutingTag()
    {
      var proxy = new Csla.DataPortalClient.HttpProxy();
      var method = proxy.GetType().GetMethods(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
        .Where(r=>r.Name == "GetRoutingToken")
        .FirstOrDefault();
      string result = (string)method.Invoke(proxy, new object[] { typeof(RoutingTest) });
      Assert.AreEqual("mytag", result);
    }

    [TestMethod]
    public void CreateTagNoVersionNoRoute()
    {
      var proxy = new Csla.DataPortalClient.HttpProxy();
      var method = proxy.GetType().GetMethods(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
        .Where(r => r.Name == "CreateOperationTag")
        .FirstOrDefault();
      string result = (string)method.Invoke(proxy, new object[] { "create", "", "" });
      Assert.AreEqual("create", result);
    }

    [TestMethod]
    public void CreateTagNoVersion()
    {
      var proxy = new Csla.DataPortalClient.HttpProxy();
      var method = proxy.GetType().GetMethods(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
        .Where(r => r.Name == "CreateOperationTag")
        .FirstOrDefault();
      string result = (string)method.Invoke(proxy, new object[] { "create", "", "mytag" });
      Assert.AreEqual("create/mytag-", result);
    }

    [TestMethod]
    public void CreateTagNoRoute()
    {
      var proxy = new Csla.DataPortalClient.HttpProxy();
      var method = proxy.GetType().GetMethods(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
        .Where(r => r.Name == "CreateOperationTag")
        .FirstOrDefault();
      string result = (string)method.Invoke(proxy, new object[] { "create", "v1", "" });
      Assert.AreEqual("create/-v1", result);
    }

    [TestMethod]
    public void CreateTag()
    {
      var proxy = new Csla.DataPortalClient.HttpProxy();
      var method = proxy.GetType().GetMethods(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
        .Where(r => r.Name == "CreateOperationTag")
        .FirstOrDefault();
      string result = (string)method.Invoke(proxy, new object[] { "create", "v1", "mytag" });
      Assert.AreEqual("create/mytag-v1", result);
    }
  }

  [Serializable]
  [DataPortalServerRoutingTag("mytag")]
  public class RoutingTest : BusinessBase<RoutingTest>
  {

  }
}
