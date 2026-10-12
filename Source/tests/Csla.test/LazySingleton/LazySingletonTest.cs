using Csla.Testing;
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

    private static CslaTestHost _testHost;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
      _testHost = CslaTestHost.Create();
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
      _testHost?.Dispose();
    }

    [TestMethod]
    public void LazySingletonConstructorWithOverloadConstructorCreatesObject()
    {
      var lazy = new LazySingleton<Dictionary<string, object>>(() => new Dictionary<string, object>());
      Assert.IsNotNull(lazy, "new LazySingleton can not be null.");
    }

    [TestMethod]
    public void NullDelegateThrowsArgumentNullException()
    {
      Assert.ThrowsException<ArgumentNullException>(() => new LazySingleton<Dictionary<string, object>>(null));
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

    [TestMethod]
    public void ValueIsCreatedOnlyOnce()
    {
      var calls = 0;
      var lazy = new LazySingleton<Dictionary<string, object>>(() =>
      {
        calls++;
        return new Dictionary<string, object>();
      });

      var first = lazy.Value;
      var second = lazy.Value;

      Assert.AreEqual(1, calls, "Delegate must be invoked exactly once.");
      Assert.AreSame(first, second);
    }

    [TestMethod]
    public void CreateInstanceDICreatesLazySingletonWithDelegate()
    {
      var applicationContext = _testHost.ApplicationContext;
      Func<Dictionary<string, object>> factory = () => new Dictionary<string, object>();

      var lazy = (LazySingleton<Dictionary<string, object>>)applicationContext.CreateInstanceDI(
        typeof(LazySingleton<Dictionary<string, object>>), factory);

      Assert.IsNotNull(lazy.Value);
      Assert.IsTrue(lazy.IsValueCreated);
    }
  }
}
