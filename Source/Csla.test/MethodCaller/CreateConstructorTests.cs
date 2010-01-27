using System;
using System.ComponentModel;
using UnitDriven;

#if !SILVERLIGHT
#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#endif
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
    [System.ComponentModel.Description("This test does not pass, which tells me the Activator is actually faster than the MethodCaller.")]
    public void CreateInstanceWithExpressionsFasterThanActivatorSuccess()
    {
      int times = 100000;
      var start = DateTime.Now;
      for(int x=0;x<times;x++)
        Csla.Reflection.MethodCaller.CreateInstance(typeof(TestClass));
      var end = DateTime.Now;
      var t1 = end - start;

      start = DateTime.Now;
      for (int x = 0; x < times; x++)
        Activator.CreateInstance(typeof(TestClass));
      end = DateTime.Now;
      var t2 = end - start;

      Assert.IsTrue(t1 < t2);
    }

    [TestMethod]
    [ExpectedException(typeof(NotSupportedException))]
    public void CreateInstanceNoParameterlessConstructorFail()
    {
      Csla.Reflection.MethodCaller.CreateInstance(typeof(Fail1));
    }

    [TestMethod]
#if SILVERLIGHT
    [ExpectedException(typeof(NotSupportedException))]
#endif
    public void CreateInstanceNonPublicConstructor()
    {
      Csla.Reflection.MethodCaller.CreateInstance(typeof(NonPublic1));
    }

    [TestMethod]
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