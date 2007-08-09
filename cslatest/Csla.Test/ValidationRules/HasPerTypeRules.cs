using System;
using System.Collections.Generic;
using System.Text;

#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#endif 

namespace Csla.Test.ValidationRules
{
  [TestClass()]
  public class PerTypeTests
  {
    [TestMethod]
    public void OnlySharedRules()
    {
      ApplicationContext.GlobalContext.Clear();
      ApplicationContext.GlobalContext["Shared"] = 0;

      HasOnlyPerTypeRules root = new HasOnlyPerTypeRules();
      Assert.AreEqual(string.Empty, root.Test, "Test string should be empty");
      Assert.AreEqual(1, root.BrokenRulesCollection.Count, "Broken rule count should be 1 first");
      root.Test = "test";
      Assert.AreEqual("test", root.Test, "Test string should be 'test'");
      Assert.AreEqual(0, root.BrokenRulesCollection.Count, "Broken rule count should be 0");
      root.Test = "big test";
      Assert.AreEqual("big test", root.Test, "Test string should be 'big test'");
      Assert.AreEqual(1, root.BrokenRulesCollection.Count, "Broken rule count should be 1 last");
    }

    [TestMethod()]
    public void StringRequired()
    {
      ApplicationContext.GlobalContext.Clear();
      ApplicationContext.GlobalContext["Shared"] = 0;

      HasPerTypeRules root = new HasPerTypeRules();
      Assert.AreEqual(string.Empty, root.Test, "Test string should be empty");
      Assert.AreEqual(1, root.BrokenRulesCollection.Count, "Broken rule count should be 1 first");
      root.Test = "test";
      Assert.AreEqual("test", root.Test, "Test string should be 'test'");
      Assert.AreEqual(0, root.BrokenRulesCollection.Count, "Broken rule count should be 0");
      root.Test = "big test";
      Assert.AreEqual("big test", root.Test, "Test string should be 'big test'");
      Assert.AreEqual(1, root.BrokenRulesCollection.Count, "Broken rule count should be 1 last");
    }

    [TestMethod()]
    public void NoDoubleInit()
    {
      ApplicationContext.GlobalContext.Clear();
      ApplicationContext.GlobalContext["Shared"] = 0;

      HasPerTypeRules2 root = new HasPerTypeRules2();
      root = new HasPerTypeRules2();
      Assert.AreEqual(1, (int)ApplicationContext.GlobalContext["Shared"], "Rules should init just once");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void BadNonStaticRuleMethod()
    {
      // creating the object should trigger AddBusinessRules()
      // which should fail due to the bad exception
      HasBadSharedRule bad = new HasBadSharedRule();
    }
  }
}
