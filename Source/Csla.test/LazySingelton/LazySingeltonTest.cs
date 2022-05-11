using System;
using System.Collections.Generic;
using UnitDriven;
using Csla.TestHelpers;

#if NUNIT
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
using TestSetup = NUnit.Framework.SetUpAttribute;
#elif MSTEST
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif
namespace Csla.Test.LazySingelton
{
    
    
    /// <summary>
    ///This is a test class for LazySingeltonTest and is intended
    ///to contain all LazySingeltonTest Unit Tests
    ///</summary>
  [TestClass]
  public class LazySingeltonTest
  {

    private static TestDIContext _testDIContext;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
      _testDIContext = TestDIContextFactory.CreateDefaultContext();
    }

    [TestMethod]
    public void LazySingeltonDefaultConstructorCreatesObject()
    {
      var lazy = new LazySingleton<Dictionary<string, object>>();
      Assert.IsNotNull(lazy, "new LazySingelton can not be null.");
    }

    [TestMethod]
    public void LazySingeltonConstructorWithOverloadConstructorCreatesObject()
    {
      var lazy = new LazySingleton<Dictionary<string, object>>(() => new Dictionary<string, object>());
      Assert.IsNotNull(lazy, "new LazySingelton can not be null.");
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
    public void ValueIsLazyCreatesValueTest()
    {
      var lazy = new LazySingleton<Dictionary<string, object>>();
      var value = lazy.Value;
      Assert.IsNotNull(lazy.Value, "Value must not be null.");
      Assert.IsTrue(lazy.IsValueCreated);
      Assert.AreEqual(typeof(Dictionary<string, object>), lazy.Value.GetType());
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
