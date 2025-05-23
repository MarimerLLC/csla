//-----------------------------------------------------------------------
// <copyright file="HasPerTypeRules.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>This is needed because in Silverlight the tests cannot be run in separate AppDomains</summary>
//-----------------------------------------------------------------------

using Csla.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.ValidationRules
{
#if TESTING
  [System.Diagnostics.DebuggerStepThrough]
#endif
  [TestClass]
  public class PerTypeTests
  {
    private static TestDIContext _testDIContext;

    [ClassInitialize]
    public static void ClassInitialze(TestContext testContext)
    {
      _testDIContext = TestDIContextFactory.CreateDefaultContext();
    }

    [TestMethod]
    public void OnlySharedRules()
    {
      IDataPortal<HasOnlyPerTypeRules> dataPortal = _testDIContext.CreateDataPortal<HasOnlyPerTypeRules>();

      TestResults.Reinitialise();
      TestResults.Add("Shared", "0");

      HasOnlyPerTypeRules root = dataPortal.Create();
      root.Validate();
      Assert.AreEqual(string.Empty, root.Test, "Test string should be empty");
      Assert.AreEqual(1, root.BrokenRulesCollection.Count, "Broken rule count should be 1 first");
      root.Test = "test";
      Assert.AreEqual("test", root.Test, "Test string should be 'test'");
      Assert.AreEqual(0, root.BrokenRulesCollection.Count, "Broken rule count should be 0");
      root.Test = "big test";
      Assert.AreEqual("big test", root.Test, "Test string should be 'big test'");
      Assert.AreEqual(1, root.BrokenRulesCollection.Count, "Broken rule count should be 1 last");
    }

    [TestMethod]
    public void StringRequired()
    {
      IDataPortal<HasPerTypeRules> dataPortal = _testDIContext.CreateDataPortal<HasPerTypeRules>();
      
      TestResults.Reinitialise();
      TestResults.Add("Shared", "0");

      HasPerTypeRules root = dataPortal.Create();
      root.Validate();
      Assert.AreEqual(string.Empty, root.Test, "Test string should be empty");
      Assert.AreEqual(1, root.BrokenRulesCollection.Count, "Broken rule count should be 1 first");
      root.Test = "test";
      Assert.AreEqual("test", root.Test, "Test string should be 'test'");
      Assert.AreEqual(0, root.BrokenRulesCollection.Count, "Broken rule count should be 0");
      root.Test = "big test";
      Assert.AreEqual("big test", root.Test, "Test string should be 'big test'");
      Assert.AreEqual(1, root.BrokenRulesCollection.Count, "Broken rule count should be 1 last");
    }

    /// <summary>
    /// This is needed because in Silverlight the tests cannot be run in separate AppDomains
    /// therefore the expected value is different for subsequent runs of this test.
    /// </summary>
    private static bool _initialized = false;

    [TestMethod]
    public void NoDoubleInit()
    {
      IDataPortal<HasPerTypeRules2> dataPortal = _testDIContext.CreateDataPortal<HasPerTypeRules2>();

      TestResults.Reinitialise();
      TestResults.Add("Shared", "0");

      HasPerTypeRules2 root = dataPortal.Create();

      int expected = (_initialized ? 0 : 1);
      int actual = int.Parse(TestResults.GetResult("Shared"));
      Assert.AreEqual(expected, actual, "Rules should init just once");

      _initialized = true;
    }

    [TestMethod]
    public void LoadRuleSets()
    {
      IDataPortal<HasRuleSetRules> dataPortal = _testDIContext.CreateDataPortal<HasRuleSetRules>();

      TestResults.Reinitialise();

      var root = dataPortal.Create();

      var d = root.GetDefaultRules();
      Assert.AreEqual(1, d.Length);

      root.Name = "abc";
      Assert.IsTrue(root.IsValid, "valid with name");
      root.Name = "123456";
      Assert.IsTrue(root.IsValid, "valid with long name");
      root.Name = "";
      Assert.IsFalse(root.IsValid, "not valid with empty name");


      var t = root.GetTestRules();
      Assert.AreEqual(2, t.Length);

      root.Name = "abc";
      Assert.IsTrue(root.IsValid, "valid with name");
      root.Name = "123456";
      Assert.IsFalse(root.IsValid, "not valid with too long name");
      root.Name = "";
      Assert.IsFalse(root.IsValid, "not valid with empty name");
    }
  }
}