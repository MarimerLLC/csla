//-----------------------------------------------------------------------
// <copyright file="CreateConstructorTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.ComponentModel;
using UnitDriven;

#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#endif

namespace Csla.Test.MethodCaller
{
  [TestClass]
  public class CreateConstructorTests
  {
    [TestMethod]
    public void CreateInstanceSuccess()
    {
      var t1 = Csla.Reflection.MethodCaller.CreateInstance(typeof(TestClass));
#if MSTEST
      Assert.IsInstanceOfType(t1, typeof(TestClass));
#else
      Assert.IsInstanceOfType(typeof(TestClass), t1);
#endif
    }

    [TestMethod]
#if WINDOWS_PHONE
    [ExpectedException(typeof(MissingMethodException))]
#else
    [ExpectedException(typeof(NotSupportedException))]
#endif
    public void CreateInstanceNoParameterlessConstructorFail()
    {
      Csla.Reflection.MethodCaller.CreateInstance(typeof(Fail1));
    }

    [TestMethod]
#if WINDOWS_PHONE
    [ExpectedException(typeof(MissingMethodException))]
#endif
    public void CreateInstanceNonPublicConstructor()
    {
      Csla.Reflection.MethodCaller.CreateInstance(typeof(NonPublic1));
    }

    [TestMethod]
#if WINDOWS_PHONE
    [ExpectedException(typeof(MethodAccessException))]
#endif
    public void CreateInstanceNonPublicNestedTypeSuccess()
    {
      var instance = (NonPublic2)Csla.Reflection.MethodCaller.CreateInstance(typeof(NonPublic2));
      Assert.IsNotNull(instance);
    }

    [TestMethod]
    [ExpectedException(typeof(NotSupportedException))]
    public void CreateInstanceNotClassFail()
    {
      Csla.Reflection.MethodCaller.CreateInstance(typeof(TestStruct));
    }

    public class Fail1
    {
      public Fail1(int unsupported) { }
    }

    public class NonPublic1
    {
      protected NonPublic1() { }
    }

    private class NonPublic2
    {
      public NonPublic2() { }
    }

    public class TestClass
    {
      public TestClass() { }
      public TestClass(int unsupported) { }
    }

    public struct TestStruct
    {
    }
  }
}