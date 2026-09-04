//-----------------------------------------------------------------------
// <copyright file="RequiredRuleTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Tests for the Required rule on value type properties</summary>
//-----------------------------------------------------------------------

using Csla.Testing;
using Csla.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.ValidationRules
{
  /// <summary>
  /// Tests the Required rule against value type properties.
  /// </summary>
  /// <remarks>
  /// Regression coverage for issue #4856, where adding a Required rule to an int
  /// property caused the rule engine to throw while creating the RuleContext.
  /// </remarks>
  [TestClass]
  public class RequiredRuleTests
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
    public void SettingIntPropertyWithRequiredRuleRunsRules()
    {
      IDataPortal<HasRequiredValueTypeRules> dataPortal = _testHost.GetDataPortal<HasRequiredValueTypeRules>();

      HasRequiredValueTypeRules root = dataPortal.Create();
      root.Id = 5;

      Assert.AreEqual(5, root.Id);
      Assert.IsNull(root.BrokenRulesCollection.GetFirstBrokenRule(HasRequiredValueTypeRules.IdProperty.Name), "Id should not be broken");
    }

    /// <summary>
    /// The Required rule fails when the value is null or its string form is
    /// null/empty/whitespace, so zero satisfies the rule on a non-nullable int.
    /// </summary>
    [TestMethod]
    public void RequiredRuleOnIntPropertyIsSatisfiedByZero()
    {
      IDataPortal<HasRequiredValueTypeRules> dataPortal = _testHost.GetDataPortal<HasRequiredValueTypeRules>();

      HasRequiredValueTypeRules root = dataPortal.Create();
      root.CheckRules();

      Assert.AreEqual(0, root.Id);
      Assert.IsNull(root.BrokenRulesCollection.GetFirstBrokenRule(HasRequiredValueTypeRules.IdProperty.Name), "Id should not be broken");
    }

    [TestMethod]
    public void RequiredRuleOnNullableIntPropertyIsBrokenWhenNull()
    {
      IDataPortal<HasRequiredValueTypeRules> dataPortal = _testHost.GetDataPortal<HasRequiredValueTypeRules>();

      HasRequiredValueTypeRules root = dataPortal.Create();
      root.CheckRules();

      Assert.IsNull(root.NullableId);
      Assert.AreEqual(1, root.BrokenRulesCollection.ErrorCount, "Only NullableId should be broken");
      Assert.AreEqual("NullableId required", root.BrokenRulesCollection.GetFirstBrokenRule(HasRequiredValueTypeRules.NullableIdProperty.Name).Description);
    }

    [TestMethod]
    public void RequiredRuleOnNullableIntPropertyIsSatisfiedByValue()
    {
      IDataPortal<HasRequiredValueTypeRules> dataPortal = _testHost.GetDataPortal<HasRequiredValueTypeRules>();

      HasRequiredValueTypeRules root = dataPortal.Create();
      root.CheckRules();
      root.NullableId = 1;

      Assert.AreEqual(0, root.BrokenRulesCollection.ErrorCount, "No rule should be broken");
      Assert.IsTrue(root.IsValid);
    }
  }
}
