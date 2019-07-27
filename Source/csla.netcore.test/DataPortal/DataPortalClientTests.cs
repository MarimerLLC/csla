//-----------------------------------------------------------------------
// <copyright file="DataPortalClientTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
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
  public class DataPortalClientTests
  {
    [TestCleanup]
    public void Cleanup()
    {
      ApplicationContext.DataPortalProxy = null;
      ApplicationContext.DataPortalUrlString = null;
      Csla.DataPortalClient.DataPortalProxyFactory.DataPortalTypeProxyDescriptors?.Clear();
    }

    [TestMethod]
    public void ProxyFactoryGetTypeName()
    {
      var name = Csla.DataPortalClient.DataPortalProxyFactory.GetTypeName(typeof(TestType));
      Assert.AreEqual("csla.netcore.test.DataPortal.TestType, csla.netcore.test", name);
    }

    [TestMethod]
    public void ProxyFactoryGetTypeKeyNoResource()
    {
      var name = Csla.DataPortalClient.DataPortalProxyFactory.GetTypeKey(typeof(TestType));
      Assert.AreEqual("csla.netcore.test.DataPortal.TestType, csla.netcore.test", name);
    }

    [TestMethod]
    public void ProxyFactoryGetTypeKeyResource()
    {
      var key = Csla.DataPortalClient.DataPortalProxyFactory.GetTypeKey(typeof(ResourceType));
      Assert.AreEqual(ServerResources.SpecializedAlgorithm, Enum.Parse(typeof(ServerResources), key));
    }

    [TestMethod]
    public void GetProxyDefault()
    {
      Csla.DataPortalClient.DataPortalProxyFactory.DataPortalTypeProxyDescriptors?.Clear();
      var fake = new Csla.DataPortalClient.DataPortalProxyDescriptor
      { ProxyTypeName = typeof(System.String).AssemblyQualifiedName, DataPortalUrl = "https://example.com/fake" };
      Csla.DataPortalClient.DataPortalProxyFactory.AddDescriptor("123", fake);
      Csla.DataPortalClient.DataPortalProxyFactory.AddDescriptor("abc", fake);

      var factory = new Csla.DataPortalClient.DataPortalProxyFactory();
      var proxy = factory.Create(typeof(DefaultType));
      Assert.IsNotNull(proxy, "proxy can't be null");
      Assert.IsInstanceOfType(proxy, typeof(Csla.DataPortalClient.LocalProxy), "should be httpproxy");
    }

    [TestMethod]
    public void GetProxyNoResource()
    {
      Csla.DataPortalClient.DataPortalProxyFactory.DataPortalTypeProxyDescriptors?.Clear();
      var fake = new Csla.DataPortalClient.DataPortalProxyDescriptor
        { ProxyTypeName = typeof(System.String).AssemblyQualifiedName, DataPortalUrl = "https://example.com/fake" };
      Csla.DataPortalClient.DataPortalProxyFactory.AddDescriptor("123", fake);
      Csla.DataPortalClient.DataPortalProxyFactory.AddDescriptor(
        typeof(TestType),
        new Csla.DataPortalClient.DataPortalProxyDescriptor
          { ProxyTypeName = typeof(Csla.DataPortalClient.HttpProxy).AssemblyQualifiedName, DataPortalUrl = "https://example.com/test" });
      Csla.DataPortalClient.DataPortalProxyFactory.AddDescriptor("abc", fake);

      var factory = new Csla.DataPortalClient.DataPortalProxyFactory();
      var proxy = factory.Create(typeof(TestType));
      Assert.IsNotNull(proxy, "proxy can't be null");
      Assert.IsInstanceOfType(proxy, typeof(Csla.DataPortalClient.HttpProxy), "should be httpproxy");
      Assert.AreEqual("https://example.com/test", ((Csla.DataPortalClient.HttpProxy)proxy).DataPortalUrl);
    }

    [TestMethod]
    public void GetProxyResource()
    {
      Csla.DataPortalClient.DataPortalProxyFactory.DataPortalTypeProxyDescriptors?.Clear();
      var fake = new Csla.DataPortalClient.DataPortalProxyDescriptor
        { ProxyTypeName = typeof(System.String).AssemblyQualifiedName, DataPortalUrl = "https://example.com/fake" };
      Csla.DataPortalClient.DataPortalProxyFactory.AddDescriptor("123", fake);
      Csla.DataPortalClient.DataPortalProxyFactory.AddDescriptor(
        (int)ServerResources.SpecializedAlgorithm,
        new Csla.DataPortalClient.DataPortalProxyDescriptor
        { ProxyTypeName = typeof(Csla.DataPortalClient.HttpProxy).AssemblyQualifiedName, DataPortalUrl = "https://example.com/test" });
      Csla.DataPortalClient.DataPortalProxyFactory.AddDescriptor("abc", fake);

      var factory = new Csla.DataPortalClient.DataPortalProxyFactory();
      var proxy = factory.Create(typeof(ResourceType));
      Assert.IsNotNull(proxy, "proxy can't be null");
      Assert.IsInstanceOfType(proxy, typeof(Csla.DataPortalClient.HttpProxy), "should be httpproxy");
      Assert.AreEqual("https://example.com/test", ((Csla.DataPortalClient.HttpProxy)proxy).DataPortalUrl);
    }
  }

  [Serializable]
  public class DefaultType : BusinessBase<DefaultType>
  {

  }

  [Serializable]
  public class TestType : BusinessBase<TestType>
  {

  }

  public enum ServerResources
  {
    LotsOfMemory,
    AccessToLegacyDatabase,
    SpecializedAlgorithm
  }

  [Serializable]
  [DataPortalServerResource((int)ServerResources.SpecializedAlgorithm)]
  public class ResourceType : BusinessBase<ResourceType>
  {

  }
}
