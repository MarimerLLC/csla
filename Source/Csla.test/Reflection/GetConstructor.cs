//-----------------------------------------------------------------------
// <copyright file="GetConstructor.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Reflection;
using System.Linq;
#if !NUNIT
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#endif
using Csla.Reflection;

namespace Csla.Test.Reflection
{
  [TestClass]
  public class GetConstructorTests
  {
    [TestMethod]
    public void GetsStaticConstructor()
    {
      var flags = System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic;
      var obj = new HasStaticCtor();
      var ctor = GetConstructor(obj.GetType(), flags, null, new Type[] { }, null);
      Assert.IsNotNull(ctor);
      Assert.IsTrue(ctor.IsStatic);
    }

    [TestMethod]
    public void GetsInstanceConstructor()
    {
      var flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic;
      var obj = new HasStaticCtor();
      var ctor = GetConstructor(obj.GetType(), flags, null, new Type[] { }, null);
      Assert.IsNotNull(ctor);
      Assert.IsFalse(ctor.IsStatic);
    }

    private static ConstructorInfo GetConstructor(Type t, BindingFlags bindingFlags, object c, Type[] paramTypes, object d)
    {
      if (paramTypes.Length > 0)
        throw new NotImplementedException();
      var ti = t.GetTypeInfo();
      var m = ti.DeclaredConstructors.Where(r => r.GetParameters().Count() == 0);
      if (!((bindingFlags & BindingFlags.Instance) > 0))
        m = m.Where(r => r.IsStatic);
      if (!((bindingFlags & BindingFlags.Static) > 0))
        m = m.Where(r => !r.IsStatic);
      if (!((bindingFlags & BindingFlags.Public) > 0))
        m = m.Where(r => !r.IsPublic);
      if (!((bindingFlags & BindingFlags.NonPublic) > 0))
        m = m.Where(r => r.IsPublic);
      return m.FirstOrDefault();
    }

  }

  public class HasStaticCtor
  {
    static HasStaticCtor()
    { }
  }
}
