//-----------------------------------------------------------------------
// <copyright file="BusinessRuleTesterTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Tests for BusinessRuleTester</summary>
//-----------------------------------------------------------------------

using System.Security.Claims;
using Csla.Configuration;
using Csla.Rules;
using Csla.Testing.Rules;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Testing.Tests.Rules
{
  [TestClass]
  public class BusinessRuleTesterTests : RuleTesterTestBase
  {
    [TestMethod]
    public void ForRequiresARule()
    {
      var act = () => BusinessRuleTester.For(null!);

      act.Should().Throw<ArgumentNullException>();
    }

    [TestMethod]
    public void WithInputRequiresAProperty()
    {
      var tester = BusinessRuleTester.For(new SyncRequiredRule(TestBusinessObject.NameProperty));

      var act = () => tester.WithInput(null!, "x");

      act.Should().Throw<ArgumentNullException>();
    }

    [TestMethod]
    public async Task SyncRuleRunsThroughExecuteAsync()
    {
      var result = await BusinessRuleTester
        .For(new SyncRequiredRule(TestBusinessObject.NameProperty))
        .WithInput(TestBusinessObject.NameProperty, "")
        .ExecuteAsync();

      result.HasErrors.Should().BeTrue();
      result.IsSuccess.Should().BeFalse();
      result.ErrorMessages.Should().ContainSingle().Which.Should().Be(SyncRequiredRule.ErrorMessage);
    }

    [TestMethod]
    public async Task AsyncRuleRunsThroughTheSameExecuteAsync()
    {
      var result = await BusinessRuleTester
        .For(new AsyncRequiredRule(TestBusinessObject.NameProperty))
        .WithInput(TestBusinessObject.NameProperty, "")
        .ExecuteAsync();

      result.HasErrors.Should().BeTrue();
      result.ErrorMessages.Should().ContainSingle().Which.Should().Be(AsyncRequiredRule.ErrorMessage);
    }

    [TestMethod]
    public void SyncRuleRunsThroughExecute()
    {
      var result = BusinessRuleTester
        .For(new SyncRequiredRule(TestBusinessObject.NameProperty))
        .WithInput(TestBusinessObject.NameProperty, "Rocky")
        .Execute();

      result.HasErrors.Should().BeFalse();
      result.IsSuccess.Should().BeTrue();
    }

    [TestMethod]
    public void ExecuteThrowsForAnAsyncRule()
    {
      var tester = BusinessRuleTester
        .For(new AsyncRequiredRule(TestBusinessObject.NameProperty))
        .WithInput(TestBusinessObject.NameProperty, "");

      var act = () => tester.Execute();

      act.Should().Throw<InvalidOperationException>();
    }

    [TestMethod]
    public async Task RuleImplementingNeitherInterfaceThrows()
    {
      var tester = BusinessRuleTester.For(new NotARunnableRule(TestBusinessObject.NameProperty));

      var act = async () => await tester.ExecuteAsync();

      await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
    }

    [TestMethod]
    public async Task CompleteAddsASuccessResultWhenTheRuleAddsNothing()
    {
      var result = await BusinessRuleTester
        .For(new SyncRequiredRule(TestBusinessObject.NameProperty))
        .WithInput(TestBusinessObject.NameProperty, "Rocky")
        .ExecuteAsync();

      result.Results.Should().ContainSingle();
      result.Results[0].Success.Should().BeTrue();
      result.IsSuccess.Should().BeTrue();
      result.HasErrors.Should().BeFalse();
      result.HasWarnings.Should().BeFalse();
      result.HasInformation.Should().BeFalse();
    }

    [DataTestMethod]
    [DataRow(RuleSeverity.Error)]
    [DataRow(RuleSeverity.Warning)]
    [DataRow(RuleSeverity.Information)]
    public async Task SeverityIsReportedByTheResult(RuleSeverity severity)
    {
      var result = await BusinessRuleTester
        .For(new SeverityRule(TestBusinessObject.NameProperty, severity))
        .ExecuteAsync();

      result.IsSuccess.Should().BeFalse();
      result.HasErrors.Should().Be(severity == RuleSeverity.Error);
      result.HasWarnings.Should().Be(severity == RuleSeverity.Warning);
      result.HasInformation.Should().Be(severity == RuleSeverity.Information);

      var messages = severity switch
      {
        RuleSeverity.Error => result.ErrorMessages,
        RuleSeverity.Warning => result.WarningMessages,
        _ => result.InformationMessages
      };
      messages.Should().ContainSingle().Which.Should().Be(SeverityRule.Message);
    }

    [TestMethod]
    public async Task OutValuesAndDirtyPropertiesAreReported()
    {
      var rule = new OutValueRule(TestBusinessObject.NameProperty, TestBusinessObject.DisplayNameProperty);

      var result = await BusinessRuleTester
        .For(rule)
        .WithInput(TestBusinessObject.NameProperty, "rocky")
        .ExecuteAsync();

      result.OutputPropertyValues.Should().ContainKey(TestBusinessObject.DisplayNameProperty);
      result.GetOutValue<string>(TestBusinessObject.DisplayNameProperty).Should().Be("ROCKY");
      result.TryGetOutValue<string>(TestBusinessObject.DisplayNameProperty, out var value).Should().BeTrue();
      value.Should().Be("ROCKY");
      result.DirtyProperties.Should().ContainSingle().Which.Should().BeSameAs(TestBusinessObject.DisplayNameProperty);
    }

    [TestMethod]
    public async Task TryGetOutValueReturnsFalseWhenNoOutValueWasSet()
    {
      var result = await BusinessRuleTester
        .For(new SyncRequiredRule(TestBusinessObject.NameProperty))
        .WithInput(TestBusinessObject.NameProperty, "Rocky")
        .ExecuteAsync();

      result.TryGetOutValue<string>(TestBusinessObject.NameProperty, out var value).Should().BeFalse();
      value.Should().BeNull();
      var act = () => result.GetOutValue<string>(TestBusinessObject.NameProperty);
      act.Should().Throw<KeyNotFoundException>();
    }

    [TestMethod]
    public async Task InputsArePopulatedFromTheTarget()
    {
      var rule = new InputProbeRule(TestBusinessObject.NameProperty, TestBusinessObject.NameProperty, TestBusinessObject.AgeProperty);
      var target = TestBusinessObject.Create(TestApplicationContext, "Rocky", 42);

      await BusinessRuleTester.For(rule).OnTarget(target).ExecuteAsync();

      rule.SawInputs.Should().NotBeNull();
      rule.SawInputs![TestBusinessObject.NameProperty].Should().Be("Rocky");
      rule.SawInputs[TestBusinessObject.AgeProperty].Should().Be(42);
    }

    [TestMethod]
    public async Task ExplicitInputsWinOverTheTarget()
    {
      var rule = new InputProbeRule(TestBusinessObject.NameProperty, TestBusinessObject.NameProperty, TestBusinessObject.AgeProperty);
      var target = TestBusinessObject.Create(TestApplicationContext, "Rocky", 42);

      await BusinessRuleTester
        .For(rule)
        .OnTarget(target)
        .WithInput(TestBusinessObject.NameProperty, "Overridden")
        .ExecuteAsync();

      rule.SawInputs![TestBusinessObject.NameProperty].Should().Be("Overridden");
      rule.SawInputs[TestBusinessObject.AgeProperty].Should().Be(42);
    }

    [TestMethod]
    public async Task LazyLoadPropertyWithNoFieldDataIsNotSuppliedAsInput()
    {
      var rule = new InputProbeRule(TestBusinessObject.NameProperty, TestBusinessObject.LazyProperty);
      var target = TestBusinessObject.Create(TestApplicationContext, "Rocky", 42);

      await BusinessRuleTester.For(rule).OnTarget(target).ExecuteAsync();

      rule.SawInputs.Should().NotContainKey(TestBusinessObject.LazyProperty);
    }

    [TestMethod]
    public async Task LazyLoadPropertyWithFieldDataIsSuppliedAsInput()
    {
      var rule = new InputProbeRule(TestBusinessObject.NameProperty, TestBusinessObject.LazyProperty);
      var target = TestBusinessObject.Create(TestApplicationContext, "Rocky", 42);
      target.LoadLazyValue("loaded");

      await BusinessRuleTester.For(rule).OnTarget(target).ExecuteAsync();

      rule.SawInputs![TestBusinessObject.LazyProperty].Should().Be("loaded");
    }

    [TestMethod]
    public async Task SyncRuleIsGivenTheTarget()
    {
      var rule = new InputProbeRule(TestBusinessObject.NameProperty);
      var target = TestBusinessObject.Create(TestApplicationContext, "Rocky", 42);

      var result = await BusinessRuleTester.For(rule).OnTarget(target).ExecuteAsync();

      result.Context.Target.Should().BeSameAs(target);
    }

    [TestMethod]
    public async Task AsyncRuleIsNotGivenTheTargetByDefault()
    {
      var rule = new AsyncTargetProbeRule(TestBusinessObject.NameProperty, provideTarget: false);
      var target = TestBusinessObject.Create(TestApplicationContext, "Rocky", 42);

      await BusinessRuleTester.For(rule).OnTarget(target).ExecuteAsync();

      rule.SawTarget.Should().BeNull();
    }

    [TestMethod]
    public async Task AsyncRuleIsGivenTheTargetWhenItOptsIn()
    {
      var rule = new AsyncTargetProbeRule(TestBusinessObject.NameProperty, provideTarget: true);
      var target = TestBusinessObject.Create(TestApplicationContext, "Rocky", 42);

      await BusinessRuleTester.For(rule).OnTarget(target).ExecuteAsync();

      rule.SawTarget.Should().BeSameAs(target);
    }

    [TestMethod]
    public async Task DefaultModeIsPropertyChanged()
    {
      var rule = new ContextModeProbeRule(TestBusinessObject.NameProperty);

      await BusinessRuleTester.For(rule).ExecuteAsync();

      rule.SawMode.Should().Be(RuleContextModes.PropertyChanged);
      rule.SawCheckRulesContext.Should().BeFalse();
    }

    [TestMethod]
    public async Task InModeIsReflectedInTheContext()
    {
      var rule = new ContextModeProbeRule(TestBusinessObject.NameProperty);

      await BusinessRuleTester.For(rule).InMode(RuleContextModes.CheckRules).ExecuteAsync();

      rule.SawMode.Should().Be(RuleContextModes.CheckRules);
      rule.SawCheckRulesContext.Should().BeTrue();
    }

    [TestMethod]
    public async Task RuleCanResolveAnInjectedService()
    {
      var result = await BusinessRuleTester
        .For(new RuleUsingInjectedService(TestBusinessObject.NameProperty))
        .WithInput(TestBusinessObject.NameProperty, "Rocky")
        .ConfigureServices(services => services.AddScoped<IGreetingService, GreetingService>())
        .ExecuteAsync();

      result.InformationMessages.Should().ContainSingle().Which.Should().Be("Hello, Rocky");
    }

    [TestMethod]
    public async Task ConfigureCslaIsApplied()
    {
      using var result = await BusinessRuleTester
        .For(new SyncRequiredRule(TestBusinessObject.NameProperty))
        .WithInput(TestBusinessObject.NameProperty, "Rocky")
        .ConfigureCsla(options => options.DefaultWaitForIdleTimeoutInSeconds = 17)
        .ExecuteAsync();

      var options = result.Context.ApplicationContext.GetRequiredService<Csla.Configuration.CslaOptions>();
      options.DefaultWaitForIdleTimeoutInSeconds.Should().Be(17);
    }

    [TestMethod]
    public async Task UsingApplicationContextSkipsTheBuiltInBootstrap()
    {
      var services = new ServiceCollection();
      services.AddCsla();
      services.AddCslaTesting();
      using var provider = services.BuildServiceProvider();
      var applicationContext = provider.GetRequiredService<ApplicationContext>();

      var result = await BusinessRuleTester
        .For(new SyncRequiredRule(TestBusinessObject.NameProperty))
        .WithInput(TestBusinessObject.NameProperty, "Rocky")
        .UsingApplicationContext(applicationContext)
        .ExecuteAsync();

      result.Context.ApplicationContext.Should().BeSameAs(applicationContext);
      result.IsSuccess.Should().BeTrue();
    }

    [TestMethod]
    public async Task AsUserIsVisibleToTheRuleWhileItRuns()
    {
      var rule = new PrincipalProbeRule(TestBusinessObject.NameProperty);

      await BusinessRuleTester
        .For(rule)
        .AsUser("Rocky", "Admin")
        .ExecuteAsync();

      rule.SawName.Should().Be("Rocky");
      rule.SawIsInAdminRole.Should().BeTrue();
      rule.SawAuthenticated.Should().BeTrue();
    }

    [TestMethod]
    public async Task AsUnauthenticatedIsVisibleToTheRuleWhileItRuns()
    {
      var rule = new PrincipalProbeRule(TestBusinessObject.NameProperty);

      await BusinessRuleTester
        .For(rule)
        .AsUnauthenticated()
        .ExecuteAsync();

      rule.SawAuthenticated.Should().BeFalse();
      rule.SawIsInAdminRole.Should().BeFalse();
    }

    [TestMethod]
    public async Task AsPrincipalIsVisibleToTheRuleWhileItRuns()
    {
      var rule = new PrincipalProbeRule(TestBusinessObject.NameProperty);
      var identity = new ClaimsIdentity("test", ClaimTypes.Name, ClaimTypes.Role);
      identity.AddClaim(new Claim(ClaimTypes.Name, "Custom"));

      await BusinessRuleTester
        .For(rule)
        .AsPrincipal(new ClaimsPrincipal(identity))
        .ExecuteAsync();

      rule.SawName.Should().Be("Custom");
    }
  }
}
