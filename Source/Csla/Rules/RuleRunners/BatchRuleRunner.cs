//-----------------------------------------------------------------------
// <copyright file="BatchRuleRunner.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Runs all business rules as a batch</summary>
//-----------------------------------------------------------------------

using Csla.Core;

namespace Csla.Rules
{
  /// <summary>
  /// Runs BusinessRules.CheckRules() as a batch: every property rule runs
  /// once, then only the rules invalidated by changed values rerun.
  /// CheckRules(property) and CheckObjectRules() behave as in
  /// <see cref="LegacyRuleRunner"/>.
  /// </summary>
  /// <remarks>
  /// <list type="number">
  /// <item>Object rules run first, with no cascade.</item>
  /// <item>Pass 1: every property rule runs once, with no cascade.</item>
  /// <item>
  /// Pass 2: for each property whose value a rule changed through output
  /// values (a dirty property), the rules for that property and the rules
  /// that use it as an input rerun. Rules rerun per property group, in full,
  /// in mode CheckRules | AsAffectedProperty.
  /// </item>
  /// <item>
  /// If CascadeOnDirtyProperties is true, pass 2 repeats until no values
  /// change, up to the configured number of passes.
  /// </item>
  /// </list>
  /// Short-circuiting (ProcessThroughPriority and StopProcessing) applies
  /// per property, as it does in <see cref="LegacyRuleRunner"/>.
  /// </remarks>
  internal sealed class BatchRuleRunner : LegacyRuleRunner
  {
    private readonly int _maxCheckRulesPasses;

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="maxCheckRulesPasses">Maximum number of pass-2 rounds when cascading.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="maxCheckRulesPasses"/> is less than 1.</exception>
    public BatchRuleRunner(int maxCheckRulesPasses)
    {
      if (maxCheckRulesPasses < 1)
        throw new ArgumentOutOfRangeException(nameof(maxCheckRulesPasses));
      _maxCheckRulesPasses = maxCheckRulesPasses;
    }

    /// <inheritdoc />
    public override List<string> CheckRules(IRuleRunnerHost host)
    {
      var objectResult = RunObjectRules(host, RuleContextModes.CheckRules, false);
      var affectedProperties = objectResult.AffectedProperties;
      var dirtyProperties = new HashSet<string>(objectResult.DirtyProperties);
      var plan = new BatchPlan(host.Rules);

      // pass 1: every property rule once
      RunGroups(host, plan, plan.Properties, RuleContextModes.CheckRules, affectedProperties, dirtyProperties);

      // pass 2: rerun what changed values invalidate
      var passes = 0;
      while (dirtyProperties.Count > 0)
      {
        var groups = plan.GroupsInvalidatedBy(dirtyProperties);
        if (groups.Count == 0)
          break;
        if (passes == _maxCheckRulesPasses)
          throw new InvalidOperationException(string.Format(
            Properties.Resources.CheckRulesDidNotSettle, _maxCheckRulesPasses, string.Join(", ", dirtyProperties)));
        passes++;

        var newlyDirty = new HashSet<string>();
        RunGroups(host, plan, groups, RuleContextModes.CheckRules | RuleContextModes.AsAffectedProperty, affectedProperties, newlyDirty);
        if (!host.CascadeOnDirtyProperties)
          break;
        dirtyProperties = newlyDirty;
      }

      return affectedProperties;
    }

    /// <summary>
    /// Runs the rules of the given property groups, interleaved in plan order.
    /// </summary>
    private void RunGroups(
      IRuleRunnerHost host,
      BatchPlan plan,
      IReadOnlyCollection<IPropertyInfo> groups,
      RuleContextModes executionContext,
      List<string> affectedProperties,
      HashSet<string> dirtyProperties)
    {
      var states = new Dictionary<IPropertyInfo, GroupState>(groups.Count);
      foreach (var property in groups)
      {
        host.ClearBrokenRules(property);
        states[property] = new GroupState();
      }

      var onAsyncRuleCompleted = AsyncCascadeHandler(host);
      foreach (var rule in plan.Rules)
      {
        if (!states.TryGetValue(rule.PrimaryProperty!, out var group) || group.Stopped)
          continue;
        if (!BusinessRules.CanRunRule(host.ApplicationContext, rule, executionContext))
          continue;
        // implicit short-circuiting
        if (group.AnyRuleBroken && rule.Priority > host.ProcessThroughPriority)
        {
          group.Stopped = true;
          continue;
        }

        var outcome = host.RunRule(rule, executionContext, onAsyncRuleCompleted);
        affectedProperties.AddRange(outcome.AffectedProperties);
        dirtyProperties.UnionWith(outcome.DirtyProperties);
        if (outcome.HasBrokenError)
          group.AnyRuleBroken = true;
        // explicit short-circuiting
        if (outcome.StopProcessing)
          group.Stopped = true;
      }

      foreach (var property in groups)
        affectedProperties.Add(property.Name);
    }

    /// <summary>
    /// Short-circuit state of one property's rules.
    /// </summary>
    private sealed class GroupState
    {
      public bool AnyRuleBroken;
      public bool Stopped;
    }

    /// <summary>
    /// The property rules of a type in run order, and the dependencies
    /// between properties.
    /// </summary>
    private sealed class BatchPlan
    {
      private readonly Dictionary<string, IPropertyInfo> _propertiesByName = [];
      private readonly Dictionary<string, List<IPropertyInfo>> _dependentsByInputName = [];

      public BatchPlan(IReadOnlyList<IBusinessRuleBase> rules)
      {
        var firstAppearance = new Dictionary<IPropertyInfo, int>();
        var keyed = new List<(IBusinessRuleBase Rule, int Property, int Registration)>();
        for (var i = 0; i < rules.Count; i++)
        {
          var rule = rules[i];
          var primary = rule.PrimaryProperty;
          if (primary == null)
            continue;

          if (!firstAppearance.TryGetValue(primary, out var propertyIndex))
          {
            propertyIndex = firstAppearance.Count;
            firstAppearance.Add(primary, propertyIndex);
            Properties.Add(primary);
            if (!_propertiesByName.ContainsKey(primary.Name))
              _propertiesByName.Add(primary.Name, primary);
          }
          keyed.Add((rule, propertyIndex, i));

          if (rule.InputProperties != null)
            foreach (var input in rule.InputProperties)
            {
              if (!_dependentsByInputName.TryGetValue(input.Name, out var dependents))
                _dependentsByInputName.Add(input.Name, dependents = []);
              if (!dependents.Contains(primary))
                dependents.Add(primary);
            }
        }

        Rules = keyed
          .OrderBy(k => k.Property)
          .ThenBy(k => k.Rule.Priority)
          .ThenBy(k => k.Registration)
          .Select(k => k.Rule)
          .ToList();
      }

      /// <summary>
      /// Property rules in run order.
      /// </summary>
      public List<IBusinessRuleBase> Rules { get; }

      /// <summary>
      /// Properties that have rules, in order of their first rule's registration.
      /// </summary>
      public List<IPropertyInfo> Properties { get; } = [];

      /// <summary>
      /// Gets the property groups whose rules must rerun because these
      /// properties changed: the properties themselves, if they have rules,
      /// and the properties of rules that use them as inputs.
      /// </summary>
      public HashSet<IPropertyInfo> GroupsInvalidatedBy(IEnumerable<string> dirtyProperties)
      {
        var result = new HashSet<IPropertyInfo>();
        foreach (var name in dirtyProperties)
        {
          if (_propertiesByName.TryGetValue(name, out var property))
            result.Add(property);
          if (_dependentsByInputName.TryGetValue(name, out var dependents))
            result.UnionWith(dependents);
        }
        return result;
      }
    }
  }
}
