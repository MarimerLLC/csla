//-----------------------------------------------------------------------
// <copyright file="BusinessRulesExtensions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Extension methods to simplify lambda business rules</summary>
//-----------------------------------------------------------------------

using System.Collections.Specialized;
using System.Text;
using Csla.Core;

namespace Csla.Rules
{
  /// <summary>
  /// Rule extensions for creating rules with a fluent coding style.
  /// </summary>
  public static class BusinessRulesExtensions
  {
    /// <summary>
    /// Adds a lambda rule with fluent coding style
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="businessRules">The business rules.</param>
    /// <param name="primaryProperty">The primary property.</param>
    /// <param name="ruleHandler">The rule handler.</param>
    /// <param name="message">The message.</param>
    /// <param name="severity">The rule severity.</param>
    /// <exception cref="ArgumentNullException"><paramref name="businessRules"/>, <paramref name="primaryProperty"/>, <paramref name="ruleHandler"/> or <paramref name="message"/> is <see langword="null"/>.</exception>
    public static void AddRule<T>(this BusinessRules businessRules, IPropertyInfo primaryProperty, Func<T, bool> ruleHandler, string message, RuleSeverity severity) 
      where T: BusinessBase
    {
      Guard.NotNull(businessRules);
      Guard.NotNull(primaryProperty);
      Guard.NotNull(ruleHandler);
      Guard.NotNull(message);

      var rule = new CommonRules.Lambda(primaryProperty, o =>
      {
        var target = (T)o.Target!;
        using (target.BypassPropertyChecks)
        {
          if (!ruleHandler.Invoke(target))
          {
            o.Results.Add(new RuleResult(o.Rule.RuleName, primaryProperty, string.Format(message, o.Rule.PrimaryProperty!.FriendlyName), o.Rule.DisplayIndex) {Severity = severity});
          }
        }
      });
      var methodName = ruleHandler.Method.ToString()!;
      rule.AddQueryParameter("s", Convert.ToBase64String(Encoding.Unicode.GetBytes(methodName)));
     
      businessRules.AddRule(rule);
    }

    /// <summary>
    /// Adds a lambda rule with fluent coding style
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="businessRules">The business rules.</param>
    /// <param name="primaryProperty">The primary property.</param>
    /// <param name="ruleHandler">The rule handler.</param>
    /// <param name="message">The message.</param>
    /// <exception cref="ArgumentNullException"><paramref name="businessRules"/>, <paramref name="primaryProperty"/>, <paramref name="ruleHandler"/> or <paramref name="message"/> is <see langword="null"/>.</exception>
    public static void AddRule<T>(this BusinessRules businessRules, IPropertyInfo primaryProperty, Func<T, bool> ruleHandler, string message) where T : BusinessBase
    {
      AddRule(businessRules, primaryProperty, ruleHandler, message, RuleSeverity.Error);
    }

    /// <summary>
    /// Adds a lambda rule with fluent coding style to the RuleSet
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="businessRules">The business rules.</param>
    /// <param name="ruleSet">The RuleSet.</param>
    /// <param name="primaryProperty">The primary property.</param>
    /// <param name="ruleHandler">The rule handler.</param>
    /// <param name="message">The message.</param>
    /// <param name="severity">The rule severity.</param>
    /// <exception cref="ArgumentNullException"><paramref name="businessRules"/>, <paramref name="primaryProperty"/>, <paramref name="ruleHandler"/> or <paramref name="message"/> is <see langword="null"/>.</exception>
    public static void AddRule<T>(this BusinessRules businessRules, string? ruleSet, IPropertyInfo primaryProperty, Func<T, bool> ruleHandler, string message, RuleSeverity severity)
      where T : BusinessBase
    {
      Guard.NotNull(businessRules);
      Guard.NotNull(primaryProperty);
      Guard.NotNull(ruleHandler);
      Guard.NotNull(message);

      var rule = new CommonRules.Lambda(primaryProperty, o =>
      {
        var target = (T)o.Target!;
        using (target.BypassPropertyChecks)
        {
          if (!ruleHandler.Invoke(target))
          {
            o.Results.Add(new RuleResult(o.Rule.RuleName, primaryProperty, string.Format(message, o.Rule.PrimaryProperty!.FriendlyName), o.Rule.DisplayIndex) { Severity = severity });
          }
        }
      });
      var methodName = ruleHandler.Method.ToString()!;
      rule.AddQueryParameter("s", Convert.ToBase64String(Encoding.Unicode.GetBytes(methodName)));
      businessRules.AddRule(rule, ruleSet);
    }

    /// <summary>
    /// Adds a lambda rule with fluent coding style to the RuleSet
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="businessRules">The business rules.</param>
    /// <param name="ruleSet">The RuleSet.</param>
    /// <param name="primaryProperty">The primary property.</param>
    /// <param name="ruleHandler">The rule handler.</param>
    /// <param name="message">The message.</param>
    /// <exception cref="ArgumentNullException"><paramref name="businessRules"/>, <paramref name="primaryProperty"/>, <paramref name="ruleHandler"/> or <paramref name="message"/> is <see langword="null"/>.</exception>
    public static void AddRule<T>(this BusinessRules businessRules, string? ruleSet, IPropertyInfo primaryProperty, Func<T, bool> ruleHandler, string message) where T : BusinessBase
    {
      AddRule(businessRules, ruleSet, primaryProperty, ruleHandler, message, RuleSeverity.Error);
    }

    /// <summary>
    /// Adds a lambda rule with fluent coding style
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="businessRules">The business rules.</param>
    /// <param name="primaryProperty">The primary property.</param>
    /// <param name="ruleHandler">The rule handler.</param>
    /// <param name="messageDelegate">The message delegate.</param>
    /// <param name="severity">The rule severity.</param>
    public static void AddRule<T>(this BusinessRules businessRules, IPropertyInfo primaryProperty, Func<T, bool> ruleHandler, Func<string> messageDelegate, RuleSeverity severity) 
      where T: BusinessBase
    {
      AddRule(businessRules, ApplicationContext.DefaultRuleSet, primaryProperty, ruleHandler, messageDelegate, severity);
    }


    /// <summary>
    /// Adds a lambda rule with fluent coding style
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="businessRules">The business rules.</param>
    /// <param name="primaryProperty">The primary property.</param>
    /// <param name="ruleHandler">The rule handler.</param>
    /// <param name="messageDelegate">The message delegate.</param>
    /// <exception cref="ArgumentNullException"><paramref name="businessRules"/>, <paramref name="primaryProperty"/>, <paramref name="ruleHandler"/> or <paramref name="messageDelegate"/> is <see langword="null"/>.</exception>
    public static void AddRule<T>(this BusinessRules businessRules, IPropertyInfo primaryProperty, Func<T, bool> ruleHandler, Func<string> messageDelegate) where T : BusinessBase
    {
      AddRule(businessRules, ApplicationContext.DefaultRuleSet, primaryProperty, ruleHandler, messageDelegate, RuleSeverity.Error);
    }

    /// <summary>
    /// Adds a lambda rule with fluent coding style to the RuleSet
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="businessRules">The business rules.</param>
    /// <param name="ruleSet">The RuleSet.</param>
    /// <param name="primaryProperty">The primary property.</param>
    /// <param name="ruleHandler">The rule handler.</param>
    /// <param name="messageDelegate">The message delegate.</param>
    /// <param name="severity">The rule severity.</param>
    /// <exception cref="ArgumentNullException"><paramref name="businessRules"/>, <paramref name="primaryProperty"/>, <paramref name="ruleHandler"/> or <paramref name="messageDelegate"/> is <see langword="null"/>.</exception>
    public static void AddRule<T>(this BusinessRules businessRules, string? ruleSet, IPropertyInfo primaryProperty, Func<T, bool> ruleHandler, Func<string> messageDelegate, RuleSeverity severity)
      where T : BusinessBase
    {
      Guard.NotNull(businessRules);
      Guard.NotNull(primaryProperty);
      Guard.NotNull(ruleHandler);
      Guard.NotNull(messageDelegate);

      var rule = new CommonRules.Lambda(primaryProperty, o =>
      {
        var target = (T)o.Target!;
        using (target.BypassPropertyChecks)
        {
          if (!ruleHandler.Invoke(target))
          {
            o.Results.Add(new RuleResult(o.Rule.RuleName, primaryProperty, messageDelegate.Invoke(), o.Rule.DisplayIndex) { Severity = severity });
          }
        }
      });
      var methodName = ruleHandler.Method.ToString()!;
      rule.AddQueryParameter("s", Convert.ToBase64String(Encoding.Unicode.GetBytes(methodName)));
      businessRules.AddRule(rule, ruleSet);
    }

    /// <summary>
    /// Adds a lambda rule with fluent coding style to the RuleSet
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="businessRules">The business rules.</param>
    /// <param name="ruleSet">The RuleSet.</param>
    /// <param name="primaryProperty">The primary property.</param>
    /// <param name="ruleHandler">The rule handler.</param>
    /// <param name="messageDelegate">The message delegate.</param>
    /// <exception cref="ArgumentNullException"><paramref name="businessRules"/>, <paramref name="primaryProperty"/>, <paramref name="ruleHandler"/> or <paramref name="messageDelegate"/> is <see langword="null"/>.</exception>
    public static void AddRule<T>(this BusinessRules businessRules, string? ruleSet, IPropertyInfo primaryProperty, Func<T, bool> ruleHandler, Func<string> messageDelegate) where T : BusinessBase
    {
      AddRule(businessRules, ruleSet, primaryProperty, ruleHandler, messageDelegate, RuleSeverity.Error);
    }
  }
}
