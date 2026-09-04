//-----------------------------------------------------------------------
// <copyright file="SeverityTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

using Csla.Testing;
using Csla.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.ValidationRules
{
  [TestClass]
  public class SeverityTests
  {
    private static CslaTestHost _testHost;

    [ClassInitialize]
    public static void ClassInitialize(TestContext testContext)
    {
      _testHost = CslaTestHost.Create();
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
      _testHost?.Dispose();
    }

    [TestInitialize]
    public void Initialize()
    {
      TestResults.Reinitialise();
    }

    [TestMethod]
    public void AllThree()
    {
      IDataPortal<SeverityRoot> dataPortal = _testHost.GetDataPortal<SeverityRoot>();

      SeverityRoot root = dataPortal.Create();
      root.Validate();

      Assert.AreEqual(3, root.BrokenRulesCollection.Count, "3 rules should be broken (total)");
      
      Assert.IsFalse(root.IsValid, "Object should not be valid");

      Assert.AreEqual(1, root.BrokenRulesCollection.ErrorCount, "Only one rule should be broken");
      Assert.AreEqual("Always error", root.BrokenRulesCollection.GetFirstBrokenRule("Test").Description, "'Always error' should be broken (GetFirstBrokenRule)");
      Assert.AreEqual("Always error", root.BrokenRulesCollection.GetFirstMessage("Test", Rules.RuleSeverity.Error).Description, "'Always error' should be broken");

      Assert.AreEqual(1, root.BrokenRulesCollection.WarningCount, "Only one warning should be broken");
      Assert.AreEqual("Always warns", root.BrokenRulesCollection.GetFirstMessage("Test", Rules.RuleSeverity.Warning).Description, "'Always warns' should be broken");

      Assert.AreEqual(1, root.BrokenRulesCollection.InformationCount, "Only one info should be broken");
      Assert.AreEqual("Always info", root.BrokenRulesCollection.GetFirstMessage("Test", Rules.RuleSeverity.Information).Description, "'Always info' should be broken");
    }

    [TestMethod]
    public void NoError()
    {
      IDataPortal<NoErrorRoot> dataPortal = _testHost.GetDataPortal<NoErrorRoot>();

      NoErrorRoot root = dataPortal.Create();
      root.Validate();
      Assert.AreEqual(2, root.BrokenRulesCollection.Count, "2 rules should be broken (total)");

      Assert.IsTrue(root.IsValid, "Object should be valid");
      Assert.AreEqual(0, root.BrokenRulesCollection.ErrorCount, "No rules (errors) should be broken");

      Assert.AreEqual(1, root.BrokenRulesCollection.WarningCount, "Only one warning should be broken");
      Assert.AreEqual("Always warns", root.BrokenRulesCollection.GetFirstMessage("Test", Rules.RuleSeverity.Warning).Description, "'Always warns' should be broken");

      Assert.AreEqual(1, root.BrokenRulesCollection.InformationCount, "Only one info should be broken");
      Assert.AreEqual("Always info", root.BrokenRulesCollection.GetFirstMessage("Test", Rules.RuleSeverity.Information).Description, "'Always info' should be broken");
    }
  }
}