﻿//-----------------------------------------------------------------------
// <copyright file="GetConstructor.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.Reflection
{
  [TestClass]
  public class GetConstructorTests
  {
    [TestMethod]
    public void GetsStaticConstructor()
    {
      var flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
      var obj = new HasStaticCtor();
      var ctor = GetConstructor(obj.GetType(), flags, null, [], null);
      Assert.IsNotNull(ctor);
      Assert.IsTrue(ctor.IsStatic);
    }

    [TestMethod]
    public void GetsInstanceConstructor()
    {
      var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
      var obj = new HasStaticCtor();
      var ctor = GetConstructor(obj.GetType(), flags, null, [], null);
      Assert.IsNotNull(ctor);
      Assert.IsFalse(ctor.IsStatic);
    }

    private static ConstructorInfo GetConstructor(Type t, BindingFlags bindingFlags, object c, Type[] paramTypes, object d)
    {
      if (paramTypes.Length > 0)
        throw new NotImplementedException();
      var ti = t.GetTypeInfo();
      var m = ti.DeclaredConstructors.Where(r => r.GetParameters().Length == 0);
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
