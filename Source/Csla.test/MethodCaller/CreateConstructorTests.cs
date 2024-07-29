//-----------------------------------------------------------------------
// <copyright file="CreateConstructorTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

using Csla.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Csla.Test.MethodCaller
{
  [TestClass]
  public class CreateConstructorTests
  {
    [TestMethod]
    public void CreateInstanceSuccess()
    {
      var services = new ServiceCollection();
      services.AddCsla();
      var provider = services.BuildServiceProvider();
      var applicationContext = provider.GetService<ApplicationContext>();
      var t1 = applicationContext.CreateInstance(typeof(TestClass));
      Assert.IsInstanceOfType(t1, typeof(TestClass));
    }

    [TestMethod]
    public void CreateInstanceNonPublicNestedTypeSuccess()
    {
      var services = new ServiceCollection();
      services.AddCsla();
      var provider = services.BuildServiceProvider();
      var applicationContext = provider.GetService<ApplicationContext>();
      var instance = (NonPublic2)applicationContext.CreateInstanceDI(typeof(NonPublic2));
      Assert.IsNotNull(instance);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void CreateInstanceNotClassFail()
    {
      var services = new ServiceCollection();
      services.AddCsla();
      var provider = services.BuildServiceProvider();
      var applicationContext = provider.GetService<ApplicationContext>();
      var obj = applicationContext.CreateInstanceDI(typeof(TestStruct));
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

    public struct TestStruct;
  }
}