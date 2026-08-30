//-----------------------------------------------------------------------
// <copyright file="AuthorizationRuleTesterTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Tests for AuthorizationRuleTester</summary>
//-----------------------------------------------------------------------

using Csla.Rules;
using Csla.Testing.Rules;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Testing.Tests.Rules
{
  [TestClass]
  public class AuthorizationRuleTesterTests : RuleTesterTestBase
  {
    [TestMethod]
    public void ForRequiresARule()
    {
      var act = () => AuthorizationRuleTester.For(null!);

      act.Should().Throw<ArgumentNullException>();
    }

    [TestMethod]
    public async Task ATargetTypeIsRequired()
    {
      var tester = AuthorizationRuleTester.For(new TestIsInRoleRule(AuthorizationActions.EditObject, "Admin"));

      var act = async () => await tester.ExecuteAsync();

      await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [TestMethod]
    public async Task PermissionIsGrantedWhenTheUserIsInTheRole()
    {
      var result = await AuthorizationRuleTester
        .For(new TestIsInRoleRule(AuthorizationActions.EditObject, "Admin"))
        .ForType<TestBusinessObject>()
        .AsUser("Rocky", "Admin")
        .ExecuteAsync();

      result.HasPermission.Should().BeTrue();
    }

    [TestMethod]
    public async Task PermissionIsDeniedWhenTheUserIsNotInTheRole()
    {
      var result = await AuthorizationRuleTester
        .For(new TestIsInRoleRule(AuthorizationActions.EditObject, "Admin"))
        .ForType<TestBusinessObject>()
        .AsUser("Rocky", "User")
        .ExecuteAsync();

      result.HasPermission.Should().BeFalse();
    }

    [TestMethod]
    public async Task PermissionIsDeniedForAnUnauthenticatedUser()
    {
      var result = await AuthorizationRuleTester
        .For(new TestIsInRoleRule(AuthorizationActions.EditObject, "Admin"))
        .ForType<TestBusinessObject>()
        .AsUnauthenticated()
        .ExecuteAsync();

      result.HasPermission.Should().BeFalse();
    }

    [TestMethod]
    public async Task AsyncRuleRunsThroughTheSameExecuteAsync()
    {
      var result = await AuthorizationRuleTester
        .For(new TestIsInRoleAsyncRule(AuthorizationActions.EditObject, "Admin"))
        .ForType<TestBusinessObject>()
        .AsUser("Rocky", "Admin")
        .ExecuteAsync();

      result.HasPermission.Should().BeTrue();
    }

    [TestMethod]
    public async Task CancellationTokenReachesAnAsyncRule()
    {
      var rule = new TestIsInRoleAsyncRule(AuthorizationActions.EditObject, "Admin");
      using var cts = new CancellationTokenSource();

      await AuthorizationRuleTester
        .For(rule)
        .ForType<TestBusinessObject>()
        .AsUser("Rocky", "Admin")
        .ExecuteAsync(cts.Token);

      rule.SawToken.Should().Be(cts.Token);
    }

    [TestMethod]
    public void ExecuteThrowsForAnAsyncRule()
    {
      var tester = AuthorizationRuleTester
        .For(new TestIsInRoleAsyncRule(AuthorizationActions.EditObject, "Admin"))
        .ForType<TestBusinessObject>();

      var act = () => tester.Execute();

      act.Should().Throw<InvalidOperationException>();
    }

    [TestMethod]
    public void SyncRuleRunsThroughExecute()
    {
      var result = AuthorizationRuleTester
        .For(new TestIsInRoleRule(AuthorizationActions.EditObject, "Admin"))
        .ForType<TestBusinessObject>()
        .AsUser("Rocky", "Admin")
        .Execute();

      result.HasPermission.Should().BeTrue();
    }

    [TestMethod]
    public async Task CriteriaReachTheRule()
    {
      var rule = new ContextProbeAuthorizationRule(AuthorizationActions.GetObject);

      await AuthorizationRuleTester
        .For(rule)
        .ForType<TestBusinessObject>()
        .WithCriteria(42, "abc")
        .ExecuteAsync();

      rule.SawCriteria.Should().Equal(42, "abc");
    }

    [TestMethod]
    public async Task TargetTypeIsInferredFromTheTarget()
    {
      var rule = new ContextProbeAuthorizationRule(AuthorizationActions.EditObject);
      var target = TestBusinessObject.Create(TestApplicationContext, "Rocky", 42);

      await AuthorizationRuleTester.For(rule).OnTarget(target).ExecuteAsync();

      rule.SawTarget.Should().BeSameAs(target);
      rule.SawTargetType.Should().Be(typeof(TestBusinessObject));
    }

    [TestMethod]
    public async Task ForTypeOverridesTheInferredTargetType()
    {
      var rule = new ContextProbeAuthorizationRule(AuthorizationActions.EditObject);
      var target = TestBusinessObject.Create(TestApplicationContext, "Rocky", 42);

      await AuthorizationRuleTester
        .For(rule)
        .OnTarget(target)
        .ForType(typeof(object))
        .ExecuteAsync();

      rule.SawTargetType.Should().Be(typeof(object));
    }

    [TestMethod]
    public async Task AsUserProducesAnAuthenticatedIdentity()
    {
      var rule = new ContextProbeAuthorizationRule(AuthorizationActions.EditObject);

      await AuthorizationRuleTester
        .For(rule)
        .ForType<TestBusinessObject>()
        .AsUser("Rocky")
        .ExecuteAsync();

      rule.SawAuthenticated.Should().BeTrue();
    }
  }
}
