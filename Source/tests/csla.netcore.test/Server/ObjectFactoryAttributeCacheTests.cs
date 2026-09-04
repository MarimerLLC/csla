//-----------------------------------------------------------------------
// <copyright file="ObjectFactoryAttributeCacheTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Verifies ObjectFactoryAttribute lookups are cached per type</summary>
//-----------------------------------------------------------------------

using Csla.Server;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.Server
{
  [TestClass]
  public class ObjectFactoryAttributeCacheTests
  {
    [ObjectFactory("Csla.Test.Server.ObjectFactoryAttributeCacheTests+SomeFactory, csla.netcore.test")]
    [Serializable]
    private class FactoryRoot : BusinessBase<FactoryRoot>
    {
    }

    [Serializable]
    private class PlainRoot : BusinessBase<PlainRoot>
    {
    }

    private class DerivedFactoryRoot : FactoryRoot
    {
    }

    [TestMethod]
    public void LookupReturnsAttributeForDecoratedType()
    {
      var attribute = ObjectFactoryAttribute.GetObjectFactoryAttribute(typeof(FactoryRoot));

      Assert.IsNotNull(attribute);
      Assert.AreEqual("Csla.Test.Server.ObjectFactoryAttributeCacheTests+SomeFactory, csla.netcore.test", attribute.FactoryTypeName);
    }

    [TestMethod]
    public void LookupReturnsNullForUndecoratedType()
    {
      Assert.IsNull(ObjectFactoryAttribute.GetObjectFactoryAttribute(typeof(PlainRoot)));
      // second call exercises the cached null result
      Assert.IsNull(ObjectFactoryAttribute.GetObjectFactoryAttribute(typeof(PlainRoot)));
    }

    [TestMethod]
    public void LookupIsCachedPerType()
    {
      // Type.GetCustomAttributes materializes a new attribute instance on every
      // call, so reference equality across calls proves the cache is used.
      var first = ObjectFactoryAttribute.GetObjectFactoryAttribute(typeof(FactoryRoot));
      var second = ObjectFactoryAttribute.GetObjectFactoryAttribute(typeof(FactoryRoot));

      Assert.IsNotNull(first);
      Assert.AreSame(first, second);
    }

    [TestMethod]
    public void LookupHonorsInheritedAttribute()
    {
      var attribute = ObjectFactoryAttribute.GetObjectFactoryAttribute(typeof(DerivedFactoryRoot));

      Assert.IsNotNull(attribute);
      Assert.AreEqual("Csla.Test.Server.ObjectFactoryAttributeCacheTests+SomeFactory, csla.netcore.test", attribute.FactoryTypeName);
    }
  }
}
