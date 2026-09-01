//-----------------------------------------------------------------------
// <copyright file="RuleCacheTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Tests for RuleCache</summary>
//-----------------------------------------------------------------------

using Csla.Rules;
using Csla.Testing.Rules;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Testing.Tests.Rules
{
  /// <summary>
  /// Tests for <see cref="RuleCache"/>.
  /// </summary>
  /// <remarks>
  /// Every test measures a DELTA in registration counts rather than an absolute, and clears
  /// before measuring. The caches and the counters are process-wide static state, so a test
  /// asserting "the count is 1" would pass or fail depending on what ran before it.
  /// </remarks>
  [TestClass]
  public class RuleCacheTests : RuleTesterTestBase
  {
    [TestMethod]
    public void ClearRequiresAType()
    {
      var act = () => RuleCache.Clear(null!);

      act.Should().Throw<ArgumentNullException>();
    }

    [TestMethod]
    public void ClearBusinessRulesRequiresAType()
    {
      var act = () => RuleCache.ClearBusinessRules(null!);

      act.Should().Throw<ArgumentNullException>();
    }

    [TestMethod]
    public void ClearAuthorizationRulesRequiresAType()
    {
      var act = () => RuleCache.ClearAuthorizationRules(null!);

      act.Should().Throw<ArgumentNullException>();
    }

    [TestMethod]
    public void BusinessRulesAreRegisteredOnlyOnceWhileCached()
    {
      RuleCache.Clear();
      RuleCacheCountingObject.Create(TestApplicationContext);

      int after = RuleCacheCountingObject.BusinessRuleRegistrations;
      RuleCacheCountingObject.Create(TestApplicationContext);

      RuleCacheCountingObject.BusinessRuleRegistrations.Should().Be(after,
        "a cached registration must not run again");
    }

    [TestMethod]
    public void ClearingBusinessRulesCausesReregistration()
    {
      RuleCacheCountingObject.Create(TestApplicationContext);
      int before = RuleCacheCountingObject.BusinessRuleRegistrations;

      RuleCache.ClearBusinessRules();
      RuleCacheCountingObject.Create(TestApplicationContext);

      RuleCacheCountingObject.BusinessRuleRegistrations.Should().Be(before + 1,
        "clearing the cache must make the next use register again");
    }

    [TestMethod]
    public void ClearingOneTypeLeavesOtherTypesCached()
    {
      RuleCacheCountingObject.Create(TestApplicationContext);
      RuleCacheOtherCountingObject.Create(TestApplicationContext);
      int cleared = RuleCacheCountingObject.BusinessRuleRegistrations;
      int untouched = RuleCacheOtherCountingObject.BusinessRuleRegistrations;

      RuleCache.ClearBusinessRules(typeof(RuleCacheCountingObject));
      RuleCacheCountingObject.Create(TestApplicationContext);
      RuleCacheOtherCountingObject.Create(TestApplicationContext);

      RuleCacheCountingObject.BusinessRuleRegistrations.Should().Be(cleared + 1,
        "the cleared type must register again");
      RuleCacheOtherCountingObject.BusinessRuleRegistrations.Should().Be(untouched,
        "a type that was not cleared must stay cached");
    }

    /// <summary>
    /// Triggers object-level authorization registration.
    /// </summary>
    /// <remarks>
    /// Constructing the object is NOT enough: AddObjectAuthorizationRules runs on the first
    /// permission check for the type, not on construction. Measured while writing these tests --
    /// creating the object left the counter at zero.
    /// </remarks>
    private static void CheckPermission()
      => BusinessRules.HasPermission(TestApplicationContext, AuthorizationActions.GetObject,
                                     typeof(RuleCacheCountingObject));

    [TestMethod]
    public void ClearingAuthorizationRulesCausesReregistration()
    {
      CheckPermission();
      int before = RuleCacheCountingObject.AuthorizationRuleRegistrations;

      RuleCache.ClearAuthorizationRules();
      CheckPermission();

      RuleCacheCountingObject.AuthorizationRuleRegistrations.Should().Be(before + 1,
        "clearing the authorization cache must make the next check register again");
    }

    [TestMethod]
    public void AuthorizationRulesAreRegisteredOnlyOnceWhileCached()
    {
      RuleCache.ClearAuthorizationRules();
      CheckPermission();
      int after = RuleCacheCountingObject.AuthorizationRuleRegistrations;

      CheckPermission();

      RuleCacheCountingObject.AuthorizationRuleRegistrations.Should().Be(after,
        "a cached registration must not run again");
    }

    [TestMethod]
    public void ClearEmptiesBothCaches()
    {
      RuleCacheCountingObject.Create(TestApplicationContext);
      CheckPermission();
      int business = RuleCacheCountingObject.BusinessRuleRegistrations;
      int authorization = RuleCacheCountingObject.AuthorizationRuleRegistrations;

      RuleCache.Clear();
      RuleCacheCountingObject.Create(TestApplicationContext);
      CheckPermission();

      RuleCacheCountingObject.BusinessRuleRegistrations.Should().Be(business + 1);
      RuleCacheCountingObject.AuthorizationRuleRegistrations.Should().Be(authorization + 1);
    }

    [TestMethod]
    public void ClearingIsSafeWhenNothingHasBeenCached()
    {
      RuleCache.Clear();

      var act = () => RuleCache.Clear();

      act.Should().NotThrow("clearing an already-empty cache is a no-op");
    }
  }
}
