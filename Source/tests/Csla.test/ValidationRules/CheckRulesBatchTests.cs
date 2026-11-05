//-----------------------------------------------------------------------
// <copyright file="CheckRulesBatchTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Tests for the default (batch) BusinessRules.CheckRules()</summary>
//-----------------------------------------------------------------------

using Csla.Configuration;
using Csla.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.ValidationRules
{
  /// <summary>
  /// Tests for the default BusinessRules.CheckRules() behavior: every
  /// property rule runs once, then only the rules invalidated by changed
  /// values rerun (#3736, #4235).
  /// </summary>
  /// <remarks>
  /// The scenarios match CheckRulesBaselineTests, which pins the legacy
  /// (CSLA 10) behavior, so the two classes show each behavior change side
  /// by side. Rule execution order is the order in which rules start.
  /// </remarks>
  [TestClass]
  public class CheckRulesBatchTests
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

    private static T Create<T>(CslaTestHost host = null) where T : RuleLogRoot<T>
    {
      var obj = (host ?? _testHost).GetDataPortal<T>().Create();
      obj.ClearRuleLog();
      return obj;
    }

    // Labels for CreditOrderRoot rules
    private const string SetCheckoutStatus = "OnCreditHold.SetCheckoutStatus";
    private const string Sum = "OrderTotal.Sum";
    private const string SetCreditHold = "OrderTotal.SetCreditHold";
    private const string ProductSign = "ProductAmount.SignAgreement";
    private const string FreightSign = "FreightCharge.SignAgreement";
    private const string TaxSign = "SalesTax.SignAgreement";

    [TestMethod]
    public void CheckRules_RunsRulesInPriorityOrderAcrossProperties()
    {
      // #4235: priority applies across properties. Rules with the same
      // priority run by property (first registration), then registration.
      var root = Create<CrossPropertyPriorityRoot>();

      var affected = root.RunCheckRules();

      root.RuleLog.Should().Equal("B:p-1", "A:p0", "C:p0", "A:p1");
      affected.Should().BeEquivalentTo("A", "B", "C");
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void CheckRules_CreditOrderScenario_RunsEachRuleOnceThenRerunsInvalidatedRules(bool cascadeOnDirtyProperties)
    {
      // The scenario from #3736. Legacy CheckRules() runs 24 (cascade) or 21 rules here.
      var root = Create<CreditOrderRoot>();
      root.CascadeOnDirtyProperties = cascadeOnDirtyProperties;

      var affected = root.RunCheckRules();

      root.RuleLog.Should().Equal(
        // pass 1: every rule once
        SetCheckoutStatus,     // CanCheckOut false -> true
        Sum,
        SetCreditHold,         // OnCreditHold false -> true
        ProductSign, FreightSign, TaxSign,
        // pass 2: OnCreditHold changed; its rules rerun, and so do the
        // OrderTotal rules because SetCreditHold uses it as an input
        SetCheckoutStatus,     // CanCheckOut true -> false
        Sum, SetCreditHold);
        // CanCheckOut changed, but no rule belongs to it or uses it as an input

      root.OnCreditHold.Should().BeTrue();
      root.CanCheckOut.Should().BeFalse();
      root.OrderTotal.Should().Be(115m);
      root.BrokenRulesCollection.Should().BeEmpty();
      affected.Should().BeEquivalentTo(
        "OnCreditHold", "CanCheckOut", "OrderTotal", "ProductAmount", "FreightCharge", "SalesTax");
    }

    [TestMethod]
    public void CheckRules_ReportsBrokenRulesFromEveryProperty()
    {
      var root = Create<CreditOrderRoot>();
      root.LoadSalesTax(-5m);

      root.RunCheckRules();

      root.BrokenRulesCollection.Select(r => r.Property).Should().BeEquivalentTo(
        "ProductAmount", "FreightCharge", "SalesTax");
    }

    [TestMethod]
    public async Task CheckRulesAsync_WithOnlySyncRules_MatchesCheckRules()
    {
      var syncRoot = Create<CreditOrderRoot>();
      syncRoot.CascadeOnDirtyProperties = true;
      var syncAffected = syncRoot.RunCheckRules();

      var asyncRoot = Create<CreditOrderRoot>();
      asyncRoot.CascadeOnDirtyProperties = true;
      var asyncAffected = await asyncRoot.RunCheckRulesAsync();

      asyncRoot.RuleLog.Should().Equal(syncRoot.RuleLog);
      asyncAffected.Should().BeEquivalentTo(syncAffected);
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void CheckRules_DependencyChain(bool cascadeOnDirtyProperties)
    {
      var root = Create<DependencyChainRoot>();
      root.CascadeOnDirtyProperties = cascadeOnDirtyProperties;

      var affected = root.RunCheckRules();

      root.RuleLog.Should().Equal(
        // pass 1
        "N1->N2", "N2->N3", "N3->N4",
        // pass 2: N2 and N3 changed
        "N2->N3", "N3->N4");
      root.N2.Should().Be(1);
      root.N3.Should().Be(2);
      root.N4.Should().Be(3);
      affected.Should().BeEquivalentTo("N1", "N2", "N3", "N4");
    }

    [TestMethod]
    public void CheckRules_ConvergingCycle_WithCascadeOnDirtyProperties_RepeatsUntilSettled()
    {
      var root = Create<ConvergingCycleRoot>();
      root.CascadeOnDirtyProperties = true;

      root.RunCheckRules();

      root.RuleLog.Should().Equal(
        "Total", "Fraction",   // pass 1:  5, 1.25
        "Total", "Fraction",   // pass 2:  6.25, 1.56
        "Total", "Fraction",   // pass 3:  6.56, 1.64
        "Total", "Fraction",   // pass 4:  6.64, 1.66
        "Total", "Fraction",   // pass 5:  6.66, 1.67
        "Total", "Fraction",   // pass 6:  6.67, 1.67 (Fraction unchanged)
        "Total", "Fraction");  // pass 7:  Total changed in pass 6; nothing changes
      root.Total.Should().Be(6.67m);
      root.Fraction.Should().Be(1.67m);
    }

    [TestMethod]
    public void CheckRules_ConvergingCycle_WithoutCascadeOnDirtyProperties_MakesOneRerunPass()
    {
      var root = Create<ConvergingCycleRoot>();
      root.CascadeOnDirtyProperties = false;

      root.RunCheckRules();

      root.RuleLog.Should().Equal(
        "Total", "Fraction",   // pass 1: 5, 1.25
        "Total", "Fraction");  // pass 2: 6.25, 1.56
      root.Total.Should().Be(6.25m);
      root.Fraction.Should().Be(1.56m);
    }

    [TestMethod]
    public void CheckRules_OscillatingCycle_WithoutCascadeOnDirtyProperties_MakesOneRerunPass()
    {
      var root = Create<OscillatingCycleRoot>();
      root.CascadeOnDirtyProperties = false;

      root.RunCheckRules();

      root.RuleLog.Should().Equal("Ping", "Pong", "Ping", "Pong");
      root.Ping.Should().Be(3);
      root.Pong.Should().Be(4);
    }

    [TestMethod]
    public void CheckRules_OscillatingCycle_WithCascadeOnDirtyProperties_Throws()
    {
      // The legacy runner overflows the stack here.
      var root = Create<OscillatingCycleRoot>();
      root.CascadeOnDirtyProperties = true;

      var act = () => root.RunCheckRules();

      act.Should().Throw<InvalidOperationException>()
        .WithMessage("*100 cascade passes*Ping*");
      root.RuleLog.Should().HaveCount(202);  // pass 1 plus 100 rerun passes, 2 rules each
    }

    [TestMethod]
    public void CheckRules_HonorsMaxCheckRulesPasses()
    {
      // The converging cycle needs 6 rerun passes to settle.
      using var host = CslaTestHost.Create(o => o.ConfigureCsla(c => c.Rules(r => r.MaxCheckRulesPasses = 3)));
      var root = Create<ConvergingCycleRoot>(host);
      root.CascadeOnDirtyProperties = true;

      var act = () => root.RunCheckRules();

      act.Should().Throw<InvalidOperationException>()
        .WithMessage("*3 cascade passes*");
    }

    [TestMethod]
    public void MaxCheckRulesPasses_MustBeAtLeastOne()
    {
      var options = new RuleOptions();

      var act = () => options.MaxCheckRulesPasses = 0;

      act.Should().Throw<ArgumentOutOfRangeException>();
      options.MaxCheckRulesPasses.Should().Be(100);
    }

    [TestMethod]
    public void CheckRules_ShortCircuitsPerProperty()
    {
      var root = Create<ShortCircuitBaselineRoot>();

      root.RunCheckRules();

      // Priority 0 rules run first, then priority 1. X:p1 is skipped because
      // X:p0 is broken (ProcessThroughPriority = 0), Y:p1 still runs, and
      // Z:after-stop is skipped because Z:stop stops processing.
      root.RuleLog.Should().Equal("X:p0-broken", "Z:stop", "Y:p1");
      root.BrokenRulesCollection.Select(r => r.Property).Should().Equal("X");
    }

    [TestMethod]
    public void CheckRules_ShortCircuit_HonorsProcessThroughPriority()
    {
      var root = Create<ShortCircuitBaselineRoot>();
      root.ProcessThroughPriority = 1;

      root.RunCheckRules();

      root.RuleLog.Should().Equal("X:p0-broken", "Z:stop", "X:p1", "Y:p1");
    }

    [TestMethod]
    public void CheckRules_HonorsDenyCheckRules_AndDoesNotRerunAffectedProperties()
    {
      var root = Create<RunModeBaselineRoot>();

      root.RunCheckRules();

      // P:deny-check-rules never runs in CheckRules(). Q is an affected property
      // of P:normal, but no value changed, so Q's rules run only once.
      root.RuleLog.Should().Equal("P:normal", "Q:deny-as-affected", "Q:normal");
    }

    [TestMethod]
    public void CheckRules_InputDependencies_RunOnceWhenNoValueChanges()
    {
      var root = Create<InputDependencyRoot>();

      root.RunCheckRules();

      root.RuleLog.Should().Equal("K:rule", "M:cascade-if-dirty", "M2:input-K");
    }

    [TestMethod]
    public void CheckRules_ObjectRuleOutputsInvalidatePropertyRules()
    {
      var root = Create<ObjectRuleBaselineRoot>();

      var affected = root.RunCheckRules();

      root.RuleLog.Should().Equal(
        "Object:sets-P",       // object rules first; P changes
        "Q:input-P", "P:rule", // pass 1
        "Q:input-P", "P:rule");// pass 2: P changed, so P's rules and the rules using P rerun
      root.P.Should().Be(42);
      affected.Should().BeEquivalentTo("P", "Q");
    }

    [TestMethod]
    public async Task CheckRulesAsync_AsyncRuleCompletionCascadesToAffectedProperties()
    {
      var root = Create<AsyncBaselineRoot>();

      await root.RunCheckRulesAsync();

      root.RuleLog.Should().Equal(
        "A:async:start",
        "B:sync",              // the batch continues while A's rule is running
        "A:async:end",
        "B:sync");             // A's completion reruns its affected property B
      root.B.Should().Be(7);
      root.IsBusy.Should().BeFalse();
    }

    [TestMethod]
    public void CheckRules_AfterSerialization_UsesTheRuleRunner()
    {
      var root = Create<CreditOrderRoot>();
      var clone = root.Clone();

      clone.RunCheckRules();

      clone.RuleLog.Should().HaveCount(9);
      clone.OnCreditHold.Should().BeTrue();
    }
  }
}
