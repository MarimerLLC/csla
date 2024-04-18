using System;
using System.Collections.Generic;
using UnitDriven;
using Csla.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Csla.Test.LazySingleton
{
  /// <summary>
  ///This is a test class for LazySingletonTest and is intended
  ///to contain all LazySingletonTest Unit Tests
  ///</summary>
  [TestClass]
  public class LazySingletonTest
  {

    private static TestDIContext _testDIContext;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
      _testDIContext = TestDIContextFactory.CreateDefaultContext();
    }

    [TestMethod]
    public void LazySingletonDefaultConstructorCreatesObject()
    {
      var lazy = new LazySingleton<Dictionary<string, object>>();
      Assert.IsNotNull(lazy, "new LazySingleton can not be null.");
    }

    [TestMethod]
    public void LazySingletonConstructorWithOverloadConstructorCreatesObject()
    {
      var lazy = new LazySingleton<Dictionary<string, object>>(() => new Dictionary<string, object>());
      Assert.IsNotNull(lazy, "new LazySingleton can not be null.");
    }

    [TestMethod]
    public void IsValueCreatedWithDefaultConstructorIsFalseTest()
    {
      var lazy = new LazySingleton<Dictionary<string, object>>();
      Assert.IsFalse(lazy.IsValueCreated, "IsValueCreated must be false by default.");
    }


    [TestMethod]
    public void IsValueCreatedWithOverloadConstructorIsFalseTest()
    {
      var lazy = new LazySingleton<Dictionary<string, object>>(() => new Dictionary<string, object>());
      Assert.IsFalse(lazy.IsValueCreated, "IsValueCreated must be false by default.");
    }

    [TestMethod]
    public void ValueIsLazyWithOverloadConstructorCreatesValueTest()
    {
      var lazy = new LazySingleton<Dictionary<string, object>>(() => new Dictionary<string, object>());
      var value = lazy.Value;
      Assert.IsNotNull(lazy.Value, "Value must not be null.");
      Assert.IsTrue(lazy.IsValueCreated);
      Assert.AreEqual(typeof(Dictionary<string, object>), lazy.Value.GetType());
    }

    #region Private Helper Methods

    private LazySingleton<T> CreateLazySingleton<T>() where T:class
    {
      return new LazySingleton<T>();
    }

    #endregion
  }
}
