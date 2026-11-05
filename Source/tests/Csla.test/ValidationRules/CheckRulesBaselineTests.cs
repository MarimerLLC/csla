//-----------------------------------------------------------------------
// <copyright file="CheckRulesBaselineTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Characterization tests for BusinessRules.CheckRules()</summary>
//-----------------------------------------------------------------------

using Csla.Configuration;
using Csla.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.ValidationRules
{
  /// <summary>
  /// Baseline (characterization) tests for BusinessRules.CheckRules().
  /// </summary>
  /// <remarks>
  /// These tests pin down the CSLA 10 execution order, rerun counts, final
  /// property values, broken rules and return values of CheckRules(). They
  /// run with RuleOptions.UseLegacyCheckRules, to show that the legacy rule
  /// runner still matches CSLA 10. See CheckRulesBatchTests for the default
  /// behavior (#3736, #4235). Rule execution order is the order in which
  /// rules start.
  /// </remarks>
  [TestClass]
  public class CheckRulesBaselineTests
  {
    private static CslaTestHost _testHost;

    [ClassInitialize]
    public static void ClassInitialize(TestContext testContext)
    {
      _testHost = CslaTestHost.Create(o => o.ConfigureCsla(c => c.Rules(r => r.UseLegacyCheckRules = true)));
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
      _testHost?.Dispose();
    }

    private static T Create<T>() where T : RuleLogRoot<T>
    {
      var obj = _testHost.GetDataPortal<T>().Create();
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
    public void CheckRules_GroupsRulesByPropertyInRegistrationOrder_PriorityOnlyAppliesWithinAProperty()
    {
      // #4235: priority is not honored across properties today.
      var root = Create<CrossPropertyPriorityRoot>();

      var affected = root.RunCheckRules();

      root.RuleLog.Should().Equal("A:p0", "A:p1", "B:p-1", "C:p0");
      affected.Should().BeEquivalentTo("A", "B", "C");
    }

    [TestMethod]
    public void CheckRules_CreditOrderScenario_WithCascadeOnDirtyProperties()
    {
      // The scenario from #3736.
      var root = Create<CreditOrderRoot>();
      root.CascadeOnDirtyProperties = true;

      var affected = root.RunCheckRules();

      root.RuleLog.Should().Equal(
        // OnCreditHold
        SetCheckoutStatus,
        //   cascade: OnCreditHold is an input of SetCreditHold
        Sum, SetCreditHold,
        //     cascade: OnCreditHold became dirty
        SetCheckoutStatus,
        //       cascade: OnCreditHold is an input of SetCreditHold
        Sum, SetCreditHold,
        // OrderTotal
        Sum, SetCreditHold,
        //   cascade: OnCreditHold is an affected property
        SetCheckoutStatus,
        // ProductAmount
        ProductSign,
        //   cascade: ProductAmount is an input of Sum, FreightSign and TaxSign
        Sum, SetCreditHold, FreightSign, TaxSign,
        // FreightCharge
        FreightSign,
        Sum, SetCreditHold, ProductSign, TaxSign,
        // SalesTax
        TaxSign,
        Sum, SetCreditHold, ProductSign, FreightSign);

      root.OnCreditHold.Should().BeTrue();
      root.CanCheckOut.Should().BeFalse();
      root.OrderTotal.Should().Be(115m);
      root.BrokenRulesCollection.Should().BeEmpty();
      affected.Should().BeEquivalentTo(
        "OnCreditHold", "CanCheckOut", "OrderTotal", "ProductAmount", "FreightCharge", "SalesTax");
    }

    [TestMethod]
    public void CheckRules_CreditOrderScenario_WithoutCascadeOnDirtyProperties()
    {
      var root = Create<CreditOrderRoot>();
      root.CascadeOnDirtyProperties = false;

      var affected = root.RunCheckRules();

      root.RuleLog.Should().Equal(
        // OnCreditHold
        SetCheckoutStatus,
        //   cascade: OnCreditHold is an input of SetCreditHold (sets OnCreditHold, no further cascade)
        Sum, SetCreditHold,
        // OrderTotal
        Sum, SetCreditHold,
        //   cascade: OnCreditHold is an affected property
        SetCheckoutStatus,
        // ProductAmount
        ProductSign,
        Sum, SetCreditHold, FreightSign, TaxSign,
        // FreightCharge
        FreightSign,
        Sum, SetCreditHold, ProductSign, TaxSign,
        // SalesTax
        TaxSign,
        Sum, SetCreditHold, ProductSign, FreightSign);

      root.OnCreditHold.Should().BeTrue();
      root.CanCheckOut.Should().BeFalse();
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
    public void CheckRules_DependencyChain_WithoutCascadeOnDirtyProperties()
    {
      var root = Create<DependencyChainRoot>();
      root.CascadeOnDirtyProperties = false;

      var affected = root.RunCheckRules();

      root.RuleLog.Should().Equal(
        "N1->N2",
        "N2->N3",           // cascade: N2 is affected by N1's rule
        "N2->N3",
        "N3->N4",           // cascade: N3 is affected by N2's rule
        "N3->N4");          // (N4 has no rules)
      root.N2.Should().Be(1);
      root.N3.Should().Be(2);
      root.N4.Should().Be(3);
      affected.Should().BeEquivalentTo("N1", "N2", "N3", "N4");
    }

    [TestMethod]
    public void CheckRules_DependencyChain_WithCascadeOnDirtyProperties()
    {
      var root = Create<DependencyChainRoot>();
      root.CascadeOnDirtyProperties = true;

      var affected = root.RunCheckRules();

      root.RuleLog.Should().Equal(
        "N1->N2",
        "N2->N3",           // cascade: N2 became dirty
        "N3->N4",           //   cascade: N3 became dirty
        "N2->N3",
        "N3->N4",           // cascade: N3 is affected by N2's rule
        "N3->N4");
      root.N2.Should().Be(1);
      root.N3.Should().Be(2);
      root.N4.Should().Be(3);
      affected.Should().BeEquivalentTo("N1", "N2", "N3", "N4");
    }

    [TestMethod]
    public void CheckRules_ConvergingCycle_WithCascadeOnDirtyProperties_RecursesUntilSettled()
    {
      var root = Create<ConvergingCycleRoot>();
      root.CascadeOnDirtyProperties = true;

      root.RunCheckRules();

      root.RuleLog.Should().Equal(
        // Total: each rule makes the other's input dirty, recursing one level per change
        "Total",               // 5
        "Fraction",            // 1.25
        "Total",               // 6.25
        "Fraction",            // 1.56
        "Total",               // 6.56
        "Fraction",            // 1.64
        "Total",               // 6.64
        "Fraction",            // 1.66
        "Total",               // 6.66
        "Fraction",            // 1.67
        "Total",               // 6.67
        "Fraction",            // 1.67, unchanged, so the recursion stops
        // Fraction
        "Fraction",
        "Total");              // cascade: Fraction is an input of Total
      root.Total.Should().Be(6.67m);
      root.Fraction.Should().Be(1.67m);
    }

    [TestMethod]
    public void CheckRules_ConvergingCycle_WithoutCascadeOnDirtyProperties_StopsBeforeSettling()
    {
      var root = Create<ConvergingCycleRoot>();
      root.CascadeOnDirtyProperties = false;

      root.RunCheckRules();

      root.RuleLog.Should().Equal(
        "Total",               // 5
        "Fraction",            // cascade: Total is an input of Fraction -> 1.25
        "Fraction",            // unchanged
        "Total");              // cascade: Fraction is an input of Total -> 6.25
      // Not settled: Fraction should be round(6.25 * 0.25, 2) = 1.56
      root.Total.Should().Be(6.25m);
      root.Fraction.Should().Be(1.25m);
    }

    [TestMethod]
    public void CheckRules_OscillatingCycle_WithoutCascadeOnDirtyProperties_Terminates()
    {
      // With CascadeOnDirtyProperties = true this cycle recurses until the
      // process dies with a StackOverflowException, which can't be caught or
      // tested in-process. Only the bounded, cascade-off behavior is pinned here.
      var root = Create<OscillatingCycleRoot>();
      root.CascadeOnDirtyProperties = false;

      root.RunCheckRules();

      root.RuleLog.Should().Equal(
        "Ping",                // 1
        "Pong",                // cascade: Ping is an input of Pong -> 2
        "Pong",                // unchanged
        "Ping");               // cascade: Pong is an input of Ping -> 3
      root.Ping.Should().Be(3);
      root.Pong.Should().Be(2);
    }

    [TestMethod]
    public void CheckRules_ShortCircuitsPerProperty()
    {
      var root = Create<ShortCircuitBaselineRoot>();

      root.RunCheckRules();

      // X:p1 is skipped because X:p0 is broken (ProcessThroughPriority = 0),
      // Y:p1 still runs, and Z:after-stop is skipped because Z:stop stops processing.
      root.RuleLog.Should().Equal("X:p0-broken", "Y:p1", "Z:stop");
      root.BrokenRulesCollection.Select(r => r.Property).Should().Equal("X");
    }

    [TestMethod]
    public void CheckRules_ShortCircuit_HonorsProcessThroughPriority()
    {
      var root = Create<ShortCircuitBaselineRoot>();
      root.ProcessThroughPriority = 1;

      root.RunCheckRules();

      root.RuleLog.Should().Equal("X:p0-broken", "X:p1", "Y:p1", "Z:stop");
    }

    [TestMethod]
    public void CheckRules_HonorsDenyCheckRulesAndDenyAsAffectedProperty()
    {
      var root = Create<RunModeBaselineRoot>();

      root.RunCheckRules();

      root.RuleLog.Should().Equal(
        "P:normal",            // P:deny-check-rules never runs in CheckRules()
        "Q:normal",            // cascade: Q is affected by P:normal; Q:deny-as-affected is skipped
        "Q:deny-as-affected",
        "Q:normal");
    }

    [TestMethod]
    public void CheckRules_InputDependencies_CascadeIfDirtySuppressesCascade()
    {
      var root = Create<InputDependencyRoot>();

      root.RunCheckRules();

      root.RuleLog.Should().Equal(
        "K:rule",
        "M2:input-K",          // cascade: K is an input; M:cascade-if-dirty is skipped because M isn't dirty
        "M:cascade-if-dirty",
        "M2:input-K");
    }

    [TestMethod]
    public void CheckRules_ObjectRulesRunFirstWithoutCascade()
    {
      var root = Create<ObjectRuleBaselineRoot>();

      var affected = root.RunCheckRules();

      root.RuleLog.Should().Equal(
        "Object:sets-P",       // object rules first; P becomes dirty but nothing cascades
        "Q:input-P",           // properties in order of first rule registration: Q, then P
        "P:rule",
        "Q:input-P");          // cascade: P is an input of Q's rule
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
  }
}
