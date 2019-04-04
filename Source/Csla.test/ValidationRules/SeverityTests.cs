//-----------------------------------------------------------------------
// <copyright file="SeverityTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
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
  [TestClass()]
  public class SeverityTests
  {
    [TestMethod]
    public void AllThree()
    {
      SeverityRoot root = new SeverityRoot();
      root.Validate();

      Assert.AreEqual(3, root.BrokenRulesCollection.Count, "3 rules should be broken (total)");
      
      Assert.IsFalse(root.IsValid, "Object should not be valid");

      Assert.AreEqual(1, root.BrokenRulesCollection.ErrorCount, "Only one rule should be broken");
      Assert.AreEqual("Always error", root.BrokenRulesCollection.GetFirstBrokenRule("Test").Description, "'Always error' should be broken (GetFirstBrokenRule)");
      Assert.AreEqual("Always error", root.BrokenRulesCollection.GetFirstMessage("Test", Csla.Rules.RuleSeverity.Error).Description, "'Always error' should be broken");

      Assert.AreEqual(1, root.BrokenRulesCollection.WarningCount, "Only one warning should be broken");
      Assert.AreEqual("Always warns", root.BrokenRulesCollection.GetFirstMessage("Test", Csla.Rules.RuleSeverity.Warning).Description, "'Always warns' should be broken");

      Assert.AreEqual(1, root.BrokenRulesCollection.InformationCount, "Only one info should be broken");
      Assert.AreEqual("Always info", root.BrokenRulesCollection.GetFirstMessage("Test", Csla.Rules.RuleSeverity.Information).Description, "'Always info' should be broken");
    }

    [TestMethod]
    public void NoError()
    {
      NoErrorRoot root = new NoErrorRoot();
      root.Validate();
      Assert.AreEqual(2, root.BrokenRulesCollection.Count, "2 rules should be broken (total)");

      Assert.IsTrue(root.IsValid, "Object should be valid");
      Assert.AreEqual(0, root.BrokenRulesCollection.ErrorCount, "No rules (errors) should be broken");

      Assert.AreEqual(1, root.BrokenRulesCollection.WarningCount, "Only one warning should be broken");
      Assert.AreEqual("Always warns", root.BrokenRulesCollection.GetFirstMessage("Test", Csla.Rules.RuleSeverity.Warning).Description, "'Always warns' should be broken");

      Assert.AreEqual(1, root.BrokenRulesCollection.InformationCount, "Only one info should be broken");
      Assert.AreEqual("Always info", root.BrokenRulesCollection.GetFirstMessage("Test", Csla.Rules.RuleSeverity.Information).Description, "'Always info' should be broken");
    }
  }
}