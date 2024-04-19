//-----------------------------------------------------------------------
// <copyright file="ShortCircuitTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

using Csla.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.ValidationRules
{
  [TestClass()]
  public class ShortCircuitTests
  {
    private static TestDIContext _testDIContext;

    [ClassInitialize]
    public static void ClassInitialize(TestContext testContext)
    {
      _testDIContext = TestDIContextFactory.CreateDefaultContext();
    }

    [TestInitialize]
    public void Initialize()
    {
      TestResults.Reinitialise();
    }

    [TestMethod]
    public void ShortCircuitOnNew()
    {
      IDataPortal<ShortCircuit> dataPortal = _testDIContext.CreateDataPortal<ShortCircuit>();

      ShortCircuit root = dataPortal.Create();
      root.CheckRules();
      Assert.AreEqual(1, root.BrokenRulesCollection.ErrorCount, "Only one rule should be broken");
      Assert.AreEqual("Test required", root.BrokenRulesCollection.GetFirstBrokenRule("Test").Description, "'Test required' should be broken");
    }

    [TestMethod]
    public void ShortCircuitOnPropertySet()
    {
      IDataPortal<ShortCircuit> dataPortal = _testDIContext.CreateDataPortal<ShortCircuit>();

      ShortCircuit root = dataPortal.Create();
      root.CheckRules();
      root.Test = "some data";
      Assert.AreEqual(1, root.BrokenRulesCollection.ErrorCount, "Only one rule should be broken with data");
      Assert.AreEqual("Always error", root.BrokenRulesCollection.GetFirstBrokenRule("Test").Description, "'Always fails' should be broken");
      root.Test = "";
      Assert.AreEqual(1, root.BrokenRulesCollection.ErrorCount, "Only one rule should be broken when empty");
      Assert.AreEqual("Test required", root.BrokenRulesCollection.GetFirstBrokenRule("Test").Description, "'Test required' should be broken");
    }

    [TestMethod]
    public void HigherThreshold()
    {
      IDataPortal<ShortCircuit> dataPortal = _testDIContext.CreateDataPortal<ShortCircuit>();

      ShortCircuit root = dataPortal.Create();
      root.CheckRules();
      Assert.AreEqual(1, root.BrokenRulesCollection.ErrorCount, "Only one rule should be broken");
      Assert.AreEqual("Test required", root.BrokenRulesCollection.GetFirstBrokenRule("Test").Description, "'Test required' should be broken");

      root.Threshold = 100;
      root.CheckRules();
      Assert.AreEqual(2, root.BrokenRulesCollection.ErrorCount, "Two rules should be broken after checkrules");
      root.Test = "some data";
      Assert.AreEqual(1, root.BrokenRulesCollection.ErrorCount, "One rule should be broken with data");
      Assert.AreEqual("Always error", root.BrokenRulesCollection.GetFirstBrokenRule("Test").Description, "'Always fails' should be broken");
      root.Test = "";
      Assert.AreEqual(2, root.BrokenRulesCollection.ErrorCount, "Two rules should be broken when empty");
    }
  }
}