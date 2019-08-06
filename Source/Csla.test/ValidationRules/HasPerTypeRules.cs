//-----------------------------------------------------------------------
// <copyright file="HasPerTypeRules.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>This is needed because in Silverlight the tests cannot be run in separate AppDomains</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using UnitDriven;

#if NUNIT
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#elif MSTEST
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif


namespace Csla.Test.ValidationRules
{
#if TESTING
  [System.Diagnostics.DebuggerStepThrough]
#endif
  [TestClass]
  public class PerTypeTests : TestBase
  {
    [TestMethod]
    public void OnlySharedRules()
    {
      UnitTestContext context = GetContext();
      ApplicationContext.GlobalContext.Clear();
      ApplicationContext.GlobalContext["Shared"] = 0;

      HasOnlyPerTypeRules root = new HasOnlyPerTypeRules();
      root.Validate();
      context.Assert.AreEqual(string.Empty, root.Test, "Test string should be empty");
      context.Assert.AreEqual(1, root.BrokenRulesCollection.Count, "Broken rule count should be 1 first");
      root.Test = "test";
      context.Assert.AreEqual("test", root.Test, "Test string should be 'test'");
      context.Assert.AreEqual(0, root.BrokenRulesCollection.Count, "Broken rule count should be 0");
      root.Test = "big test";
      context.Assert.AreEqual("big test", root.Test, "Test string should be 'big test'");
      context.Assert.AreEqual(1, root.BrokenRulesCollection.Count, "Broken rule count should be 1 last");
      context.Assert.Success();
      context.Complete();
    }

    [TestMethod]
    public void StringRequired()
    {
      UnitTestContext context = GetContext();
      ApplicationContext.GlobalContext.Clear();
      ApplicationContext.GlobalContext["Shared"] = 0;

      HasPerTypeRules root = new HasPerTypeRules();
      root.Validate();
      context.Assert.AreEqual(string.Empty, root.Test, "Test string should be empty");
      context.Assert.AreEqual(1, root.BrokenRulesCollection.Count, "Broken rule count should be 1 first");
      root.Test = "test";
      context.Assert.AreEqual("test", root.Test, "Test string should be 'test'");
      context.Assert.AreEqual(0, root.BrokenRulesCollection.Count, "Broken rule count should be 0");
      root.Test = "big test";
      context.Assert.AreEqual("big test", root.Test, "Test string should be 'big test'");
      context.Assert.AreEqual(1, root.BrokenRulesCollection.Count, "Broken rule count should be 1 last");
      context.Assert.Success();

      context.Complete();
    }

    /// <summary>
    /// This is needed because in Silverlight the tests cannot be run in separate AppDomains
    /// therefore the expected value is different for subsequent runs of this test.
    /// </summary>
    private static bool _initialized = false;

    [TestMethod]
    public void NoDoubleInit()
    {
      UnitTestContext context = GetContext();
      ApplicationContext.GlobalContext.Clear();
      ApplicationContext.GlobalContext["Shared"] = 0;

      HasPerTypeRules2 root = new HasPerTypeRules2();
      root = new HasPerTypeRules2();

      int expected = (_initialized ? 0 : 1);
      int actual = (int)ApplicationContext.GlobalContext["Shared"];
      context.Assert.AreEqual(expected, actual, "Rules should init just once");

      _initialized = true;
      context.Assert.Success();
      context.Complete();
    }

    [TestMethod]
    public void LoadRuleSets()
    {
      UnitTestContext context = GetContext();
      ApplicationContext.GlobalContext.Clear();

      var root = new HasRuleSetRules();

      var d = root.GetDefaultRules();
      context.Assert.AreEqual(1, d.Length);
      
      root.Name = "abc";
      context.Assert.IsTrue(root.IsValid, "valid with name");
      root.Name = "123456";
      context.Assert.IsTrue(root.IsValid, "valid with long name");
      root.Name = "";
      context.Assert.IsFalse(root.IsValid, "not valid with empty name");


      var t = root.GetTestRules();
      context.Assert.AreEqual(2, t.Length);

      root.Name = "abc";
      context.Assert.IsTrue(root.IsValid, "valid with name");
      root.Name = "123456";
      context.Assert.IsFalse(root.IsValid, "not valid with too long name");
      root.Name = "";
      context.Assert.IsFalse(root.IsValid, "not valid with empty name");

      context.Assert.Success();
      context.Complete();
    }
  }
}