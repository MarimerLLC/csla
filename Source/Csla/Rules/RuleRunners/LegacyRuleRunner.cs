//-----------------------------------------------------------------------
// <copyright file="LegacyRuleRunner.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Runs business rules the way CSLA 10 and earlier did</summary>
//-----------------------------------------------------------------------

using Csla.Core;

namespace Csla.Rules
{
  /// <summary>
  /// Runs business rules the way CSLA 10 and earlier did: each property's
  /// rules run in turn, recursively cascading to affected properties and
  /// to rules that use the property as an input.
  /// </summary>
  internal class LegacyRuleRunner : IRuleRunner
  {
    /// <inheritdoc />
    public virtual List<string> CheckRules(IRuleRunnerHost host)
    {
      var affectedProperties = RunObjectRules(host, RuleContextModes.CheckRules, false).AffectedProperties;
      var properties = host.Rules.Where(p => p.PrimaryProperty != null)
                                 .Select(p => p.PrimaryProperty!)
                                 .Distinct();
      foreach (var property in properties)
        affectedProperties.AddRange(CheckRulesForProperty(host, property, true, RuleContextModes.CheckRules));
      return affectedProperties;
    }

    /// <inheritdoc />
    public List<string> CheckRules(IRuleRunnerHost host, IPropertyInfo property)
    {
      return CheckRulesForProperty(host, property, true, RuleContextModes.PropertyChanged);
    }

    /// <inheritdoc />
    public List<string> CheckObjectRules(IRuleRunnerHost host)
    {
      return RunObjectRules(host, RuleContextModes.CheckObjectRules, true).AffectedProperties;
    }

    /// <summary>
    /// Runs the object-level rules.
    /// </summary>
    /// <param name="host">The rule runner host.</param>
    /// <param name="executionContext">The execution context.</param>
    /// <param name="cascade">True to rerun the rules of the object rules' affected properties.</param>
    protected RunRulesResult RunObjectRules(IRuleRunnerHost host, RuleContextModes executionContext, bool cascade)
    {
      var rules = from r in host.Rules
                  where r.PrimaryProperty == null
                    && BusinessRules.CanRunRule(host.ApplicationContext, r, executionContext)
                  orderby r.Priority
                  select r;
      host.ClearBrokenRules(null);
      // Changed to cascade propertyRule to make async ObjectLevel rules rerun PropertyLevel rules.
      var firstResult = RunRules(host, rules, false, executionContext);

      // rerun property level rules for affected properties
      if (cascade)
      {
        var propertiesToRun = new List<IPropertyInfo>();
        foreach (var item in rules)
          if (!item.IsAsync)
          {
            foreach (var p in item.AffectedProperties)
              propertiesToRun.Add(p);
          }
        // run rules for affected properties
        foreach (var item in propertiesToRun.Distinct())
        {
          var doCascade = false;
          if (host.CascadeOnDirtyProperties)
            doCascade = firstResult.DirtyProperties.Any(p => p == item.Name);
          firstResult.AffectedProperties.AddRange(CheckRulesForProperty(host, item, doCascade,
                                                            executionContext | RuleContextModes.AsAffectedProperty));
        }
      }

      return firstResult;
    }

    /// <summary>
    /// Invokes all rules for a specific property.
    /// </summary>
    /// <param name="host">The rule runner host.</param>
    /// <param name="property">The property.</param>
    /// <param name="cascade">if set to <c>true</c> [cascade].</param>
    /// <param name="executionMode">The execute mode.</param>
    protected List<string> CheckRulesForProperty(IRuleRunnerHost host, IPropertyInfo property, bool cascade, RuleContextModes executionMode)
    {
      // checking rules for the primary property
      var primaryRules = from r in host.Rules
                         where ReferenceEquals(r.PrimaryProperty, property)
                           && BusinessRules.CanRunRule(host.ApplicationContext, r, executionMode)
                         orderby r.Priority
                         select r;

      host.ClearBrokenRules(property);
      var primaryResult = RunRules(host, primaryRules, cascade, executionMode);
      if (host.CascadeOnDirtyProperties)
        cascade = cascade || primaryResult.DirtyProperties.Any();
      if (cascade)
      {
        // get properties affected by all rules
        var propertiesToRun = new List<IPropertyInfo>();
        foreach (var item in primaryRules)
          if (!item.IsAsync)
          {
            foreach (var p in item.AffectedProperties)
              if (!ReferenceEquals(property, p))
                propertiesToRun.Add(p);
          }

        // gets a list rules of of "affected" properties by adding
        // PrimaryProperty where property is in InputProperties
        var inputRules = from r in host.Rules
                         where !ReferenceEquals(r.PrimaryProperty, property)
                               && r.PrimaryProperty != null
                               && r.InputProperties != null
                               && r.InputProperties.Contains(property)
                         select r;

        var dirtyProperties = primaryResult.DirtyProperties;
        var inputProperties = from r in inputRules
                              where !r.CascadeIfDirty || dirtyProperties.Contains(r.PrimaryProperty!.Name)
                              select r.PrimaryProperty;

        foreach (var p in inputProperties)
        {
          if (!ReferenceEquals(property, p))
            propertiesToRun.Add(p);
        }
        // run rules for affected properties
        foreach (var item in propertiesToRun.Distinct())
        {
          var doCascade = false;
          if (host.CascadeOnDirtyProperties)
            doCascade = primaryResult.DirtyProperties.Any(p => p == item.Name);
          primaryResult.AffectedProperties.AddRange(CheckRulesForProperty(host, item, doCascade,
                                                               executionMode | RuleContextModes.AsAffectedProperty));
        }
      }

      // always make sure to add PrimaryProperty
      primaryResult.AffectedProperties.Add(property.Name);
      return primaryResult.AffectedProperties.Distinct().ToList();
    }

    /// <summary>
    /// Runs the enumerable list of rules.
    /// </summary>
    /// <param name="host">The rule runner host.</param>
    /// <param name="rules">The rules.</param>
    /// <param name="cascade">if set to <c>true</c> cascade.</param>
    /// <param name="executionContext">The execution context.</param>
    protected RunRulesResult RunRules(IRuleRunnerHost host, IEnumerable<IBusinessRuleBase> rules, bool cascade, RuleContextModes executionContext)
    {
      var affectedProperties = new List<string>();
      var dirtyProperties = new List<string>();
      bool anyRuleBroken = false;
      var onAsyncRuleCompleted = cascade ? AsyncCascadeHandler(host) : null;
      foreach (var rule in rules)
      {
        // implicit short-circuiting
        if (anyRuleBroken && rule.Priority > host.ProcessThroughPriority)
          break;

        var outcome = host.RunRule(rule, executionContext, onAsyncRuleCompleted);
        affectedProperties.AddRange(outcome.AffectedProperties);
        dirtyProperties.AddRange(outcome.DirtyProperties);
        if (outcome.HasBrokenError)
          anyRuleBroken = true;
        // explicit short-circuiting
        if (outcome.StopProcessing)
          break;
      }
      // return any synchronous results
      return new RunRulesResult(affectedProperties, dirtyProperties);
    }

    /// <summary>
    /// Gets a handler that, when an async rule completes, runs the rules
    /// for that rule's affected properties.
    /// </summary>
    /// <param name="host">The rule runner host.</param>
    protected AsyncRuleCompletedHandler AsyncCascadeHandler(IRuleRunnerHost host)
    {
      return r =>
      {
        var affected = new List<string>();
        foreach (var item in r.Rule.AffectedProperties.Distinct())
          if (!ReferenceEquals(r.Rule.PrimaryProperty, item))
          {
            var doCascade = false;
            if (host.CascadeOnDirtyProperties && (r.DirtyProperties != null))
              doCascade = r.DirtyProperties.Any(p => p.Name == item.Name);
            affected.AddRange(CheckRulesForProperty(host, item, doCascade, r.ExecuteContext | RuleContextModes.AsAffectedProperty));
          }
        return affected;
      };
    }
  }
}
