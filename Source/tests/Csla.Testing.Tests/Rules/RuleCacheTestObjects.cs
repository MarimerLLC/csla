//-----------------------------------------------------------------------
// <copyright file="RuleCacheTestObjects.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Business objects that count their own rule registrations</summary>
//-----------------------------------------------------------------------

using Csla.Core;
using Csla.Rules;

namespace Csla.Testing.Tests.Rules
{
  /// <summary>
  /// Counts how many times CSLA has asked it to register rules.
  /// </summary>
  /// <remarks>
  /// Registration is what the rule cache suppresses, so counting it is the only way to show
  /// that clearing the cache actually restores it. Asserting that <c>Clear</c> does not throw
  /// would pass just as well against a method that did nothing at all.
  /// </remarks>
  [Serializable]
  public class RuleCacheCountingObject : BusinessBase<RuleCacheCountingObject>
  {
    /// <summary>Times AddBusinessRules has run for this type.</summary>
    public static int BusinessRuleRegistrations;

    /// <summary>Times AddObjectAuthorizationRules has run for this type.</summary>
    public static int AuthorizationRuleRegistrations;

    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
    public string Name
    {
      get => GetProperty(NameProperty);
      set => SetProperty(NameProperty, value);
    }

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();
      BusinessRuleRegistrations++;
      BusinessRules.AddRule(new TestSyncRequiredRule(NameProperty));
    }

    private static void AddObjectAuthorizationRules()
    {
      AuthorizationRuleRegistrations++;
      BusinessRules.AddRule(typeof(RuleCacheCountingObject),
        new TestIsInRoleRule(AuthorizationActions.GetObject, "Admin"));
    }

    public static RuleCacheCountingObject Create(ApplicationContext applicationContext)
      => applicationContext.CreateInstanceDI<RuleCacheCountingObject>();
  }

  /// <summary>
  /// A second counting type, so a per-type clear can be shown to leave other types alone.
  /// </summary>
  [Serializable]
  public class RuleCacheOtherCountingObject : BusinessBase<RuleCacheOtherCountingObject>
  {
    /// <summary>Times AddBusinessRules has run for this type.</summary>
    public static int BusinessRuleRegistrations;

    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
    public string Name
    {
      get => GetProperty(NameProperty);
      set => SetProperty(NameProperty, value);
    }

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();
      BusinessRuleRegistrations++;
      BusinessRules.AddRule(new TestSyncRequiredRule(NameProperty));
    }

    public static RuleCacheOtherCountingObject Create(ApplicationContext applicationContext)
      => applicationContext.CreateInstanceDI<RuleCacheOtherCountingObject>();
  }

  /// <summary>
  /// Minimal always-fails-when-empty rule, so the counting objects have something to register.
  /// </summary>
  public class TestSyncRequiredRule : BusinessRule
  {
    public TestSyncRequiredRule(IPropertyInfo primaryProperty) : base(primaryProperty)
    {
      InputProperties.Add(primaryProperty);
    }

    protected override void Execute(IRuleContext context)
    {
      var value = (string?)context.InputPropertyValues[PrimaryProperty!];
      if (string.IsNullOrWhiteSpace(value))
        context.AddErrorResult($"{PrimaryProperty!.FriendlyName} is required");
    }
  }
}
