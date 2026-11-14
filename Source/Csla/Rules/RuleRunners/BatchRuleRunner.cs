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
  /// <see cref="LegacyRuleRunner"/>, except that they also wait for
  /// sequential async rules.
  /// </summary>
  /// <remarks>
  /// <list type="number">
  /// <item>Object rules run first, with no cascade.</item>
  /// <item>
  /// Pass 1: every property rule runs once, with no cascade, in priority
  /// order across all properties.
  /// </item>
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
  /// <para>
  /// Short-circuiting (ProcessThroughPriority and StopProcessing) applies
  /// per property, as it does in <see cref="LegacyRuleRunner"/>.
  /// </para>
  /// <para>
  /// Sequential async rules (<see cref="IBusinessRuleBase.IsSequential"/>,
  /// #5014) run in the rule stream: at such a rule the stream waits until
  /// the async rules it started earlier are complete and folds in their
  /// outcomes, then runs the rule and waits for it, and treats its outcome
  /// like a sync rule's. The run is an iterator that yields at each wait.
  /// When the rules have no sequential async rule, nothing waits and
  /// async rules complete as they always did.
  /// </para>
  /// </remarks>
  internal sealed class BatchRuleRunner : LegacyRuleRunner
  {
    private readonly int _maxRerunPasses;

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="maxRerunPasses">Maximum number of pass-2 rounds when cascading.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="maxRerunPasses"/> is less than 1.</exception>
    public BatchRuleRunner(int maxRerunPasses)
    {
      if (maxRerunPasses < 1)
        throw new ArgumentOutOfRangeException(nameof(maxRerunPasses));
      _maxRerunPasses = maxRerunPasses;
    }

    /// <inheritdoc />
    public override bool CollectsAsyncResults => true;

    /// <inheritdoc />
    public override IEnumerable<RuleStreamPause> Run(IRuleRunnerHost host, RuleStream stream)
    {
      var sequential = HasSequentialAsyncRule(host.Rules);
      switch (stream.Kind)
      {
        case RuleStreamKind.AllRules:
          return RunAllRules(new StreamRun(host, stream, sequential));
        case RuleStreamKind.Property:
          return sequential ? RunPropertyRules(new StreamRun(host, stream, true), stream.Root!) : base.Run(host, stream);
        case RuleStreamKind.Cascade:
          return sequential ? RunCompletionCascade(new StreamRun(host, stream, true), stream.CompletedContext!) : base.Run(host, stream);
        default:
          return sequential ? RunObjectRulesOnly(new StreamRun(host, stream, true)) : base.Run(host, stream);
      }
    }

    private static bool HasSequentialAsyncRule(IReadOnlyList<IBusinessRuleBase> rules)
    {
      for (var i = 0; i < rules.Count; i++)
        if (IsSequentialAsync(rules[i]))
          return true;
      return false;
    }

    private static bool IsSequentialAsync(IBusinessRuleBase rule) => rule.IsAsync && rule.IsSequential;

    #region CheckRules()

    /// <summary>
    /// Runs all rules (BusinessRules.CheckRules()).
    /// </summary>
    private IEnumerable<RuleStreamPause> RunAllRules(StreamRun run)
    {
      var host = run.Host;
      var stream = run.Stream;
      HashSet<string> dirtyProperties;
      if (run.Sequential)
      {
        var objectResult = new RunRulesResult([], []);
        dirtyProperties = [];
        run.CurrentDirty = dirtyProperties;
        foreach (var pause in RunObjectRules(run, RuleContextModes.CheckRules, false, objectResult))
          yield return pause;
        if (stream.IsDiscarded)
          yield break;
        dirtyProperties.UnionWith(objectResult.DirtyProperties);
      }
      else
      {
        var objectResult = RunObjectRules(host, RuleContextModes.CheckRules, false);
        stream.AddAffected(objectResult.AffectedProperties);
        dirtyProperties = new HashSet<string>(objectResult.DirtyProperties);
      }
      var plan = new BatchPlan(host.Rules);

      // pass 1: every property rule once
      foreach (var pause in RunGroups(run, plan, plan.Properties, RuleContextModes.CheckRules, isRerun: false, dirtyProperties))
        yield return pause;
      if (stream.IsDiscarded)
        yield break;

      // pass 2: rerun what changed values invalidate
      var passes = 0;
      while (dirtyProperties.Count > 0)
      {
        var groups = plan.GroupsInvalidatedBy(dirtyProperties);
        if (groups.Count == 0)
          break;
        if (passes == _maxRerunPasses)
          throw new InvalidOperationException(string.Format(
            Properties.Resources.CheckRulesDidNotSettle, _maxRerunPasses, string.Join(", ", dirtyProperties)));
        passes++;

        var newlyDirty = new HashSet<string>();
        foreach (var pause in RunGroups(run, plan, groups, RuleContextModes.CheckRules | RuleContextModes.AsAffectedProperty, isRerun: true, newlyDirty))
          yield return pause;
        if (stream.IsDiscarded)
          yield break;
        if (!host.CascadeOnDirtyProperties)
          break;
        dirtyProperties = newlyDirty;
      }
    }

    /// <summary>
    /// Runs the rules of the given property groups, interleaved in plan order.
    /// </summary>
    /// <param name="run">The stream run.</param>
    /// <param name="plan">The run order and dependencies.</param>
    /// <param name="groups">The properties whose rules run.</param>
    /// <param name="executionContext">The execution context.</param>
    /// <param name="isRerun">
    /// True for a rerun pass. A rerun keeps the broken rules of rules that
    /// can't run in <paramref name="executionContext"/> (such as rules that
    /// deny running as an affected property), because they still describe
    /// the current state. Pass 1 clears them, as the legacy runner does.
    /// </param>
    /// <param name="dirtyProperties">Receives the names of properties changed by output values.</param>
    private IEnumerable<RuleStreamPause> RunGroups(
      StreamRun run,
      BatchPlan plan,
      IReadOnlyCollection<IPropertyInfo> groups,
      RuleContextModes executionContext,
      bool isRerun,
      HashSet<string> dirtyProperties)
    {
      var host = run.Host;
      var stream = run.Stream;
      var states = new Dictionary<IPropertyInfo, GroupState>(groups.Count);
      foreach (var property in groups)
      {
        HashSet<string>? keepRuleNames = null;
        if (isRerun)
          foreach (var rule in plan.RulesFor(property))
            if (!BusinessRules.CanRunRule(host.ApplicationContext, rule, executionContext))
              (keepRuleNames ??= []).Add(rule.RuleName);

        if (keepRuleNames == null)
          host.ClearBrokenRules(property);
        else
          host.ClearBrokenRules(property, keepRuleNames);
        states[property] = new GroupState();
      }

      run.CurrentDirty = dirtyProperties;
      var onAsyncRuleCompleted = AsyncCascadeHandler(host);
      var frame = new Frame(plan.Rules, r => states.TryGetValue(r.PrimaryProperty!, out var state) && !state.Stopped);
      run.Frames.Add(frame);
      for (var i = 0; i < plan.Rules.Count; i++)
      {
        if (stream.IsDiscarded)
          yield break;
        var rule = plan.Rules[i];
        if (!states.TryGetValue(rule.PrimaryProperty!, out var group) || group.Stopped)
          continue;
        if (!BusinessRules.CanRunRule(host.ApplicationContext, rule, executionContext))
          continue;

        var sequential = run.Sequential && IsSequentialAsync(rule);
        if (sequential)
        {
          // the async rules started earlier complete first
          frame.Index = i;
          foreach (var pause in WaitAndFold(run))
            yield return pause;
          if (stream.IsDiscarded)
            yield break;
          if (group.Stopped)
            continue;
        }

        // implicit short-circuiting
        if (group.AnyRuleBroken && rule.Priority > host.ProcessThroughPriority)
        {
          group.Stopped = true;
          continue;
        }

        var outcome = host.RunRule(rule, executionContext, onAsyncRuleCompleted, run.Sequential);
        if (outcome.Run is { IsOwned: true } asyncRun)
        {
          if (!sequential)
          {
            asyncRun.Group = group;
            stream.UnfoldedRuns.Add(asyncRun);
            continue;
          }
          if (!asyncRun.IsCompleted)
          {
            frame.Index = i + 1;
            yield return run.Pause([asyncRun]);
            if (stream.IsDiscarded)
              yield break;
          }
          asyncRun.IsFolded = true;
          outcome = asyncRun.Outcome!;
        }

        stream.AddAffected(outcome.AffectedProperties);
        dirtyProperties.UnionWith(outcome.DirtyProperties);
        if (outcome.HasBrokenError)
          group.AnyRuleBroken = true;
        // explicit short-circuiting
        if (outcome.StopProcessing)
          group.Stopped = true;
      }
      run.Frames.Remove(frame);

      // Async rules that are not followed by a sequential rule are not folded
      // here, even if they already completed: they complete as they always
      // did, whatever the timing.
      foreach (var property in groups)
        stream.AddAffected(property.Name);
    }

    /// <summary>
    /// Applies a completed async rule's outcome to a CheckRules() run.
    /// </summary>
    private static void FoldIntoBatch(StreamRun run, AsyncRuleRun asyncRun)
    {
      var outcome = asyncRun.Outcome!;
      if (asyncRun.Group is GroupState group)
      {
        if (outcome.HasBrokenError)
          group.AnyRuleBroken = true;
        if (outcome.StopProcessing)
          group.Stopped = true;
      }
      run.CurrentDirty?.UnionWith(outcome.DirtyProperties);
    }

    #endregion

    #region CheckRules(property) and CheckObjectRules() with sequential rules

    // Iterator versions of the LegacyRuleRunner cascade. They run only when
    // the rules have a sequential async rule; otherwise the base class runs.

    // Async rules still unfolded when a stream ends complete outside it,
    // through a cascade stream (see BusinessRules.RunCompletionCascade).

    private IEnumerable<RuleStreamPause> RunPropertyRules(StreamRun run, IPropertyInfo property) =>
      CheckRulesForProperty(run, property, true, RuleContextModes.PropertyChanged);

    private IEnumerable<RuleStreamPause> RunObjectRulesOnly(StreamRun run) =>
      RunObjectRules(run, RuleContextModes.CheckObjectRules, true, new RunRulesResult([], []));

    /// <summary>
    /// Runs the rules of the affected properties of an async rule that
    /// completed after its stream ended, as the legacy completion handler
    /// does, but waiting at sequential rules.
    /// </summary>
    private IEnumerable<RuleStreamPause> RunCompletionCascade(StreamRun run, IRuleContext context)
    {
      var frame = new Frame(context.Rule.AffectedProperties);
      run.Frames.Add(frame);
      foreach (var item in context.Rule.AffectedProperties.Distinct().ToList())
        if (!ReferenceEquals(context.Rule.PrimaryProperty, item))
        {
          var doCascade = run.Host.CascadeOnDirtyProperties && context.DirtyProperties != null && context.DirtyProperties.Any(p => p.Name == item.Name);
          foreach (var pause in CheckRulesForProperty(run, item, doCascade, context.ExecuteContext | RuleContextModes.AsAffectedProperty))
            yield return pause;
          if (run.Stream.IsDiscarded)
            yield break;
        }
      run.Frames.Remove(frame);
    }

    /// <summary>
    /// Runs the object-level rules.
    /// </summary>
    private IEnumerable<RuleStreamPause> RunObjectRules(StreamRun run, RuleContextModes executionContext, bool cascade, RunRulesResult result)
    {
      var host = run.Host;
      var rules = (from r in host.Rules
                   where r.PrimaryProperty == null
                     && BusinessRules.CanRunRule(host.ApplicationContext, r, executionContext)
                   orderby r.Priority
                   select r).ToList();
      host.ClearBrokenRules(null);
      IEnumerable<IPropertyInfo> pending = cascade
        ? rules.SelectMany(r => r.AffectedProperties)
        : run.Stream.Kind == RuleStreamKind.AllRules ? host.Rules.Where(r => r.PrimaryProperty != null).SelectMany(r => r.AffectedProperties) : [];
      var frame = new Frame(pending);
      run.Frames.Add(frame);
      foreach (var pause in RunRules(run, rules, false, executionContext, result))
        yield return pause;
      if (run.Stream.IsDiscarded)
        yield break;

      // rerun property level rules for affected properties
      if (cascade)
      {
        var propertiesToRun = new List<IPropertyInfo>();
        foreach (var item in rules)
          if (!item.IsAsync || IsSequentialAsync(item))
            propertiesToRun.AddRange(item.AffectedProperties);
        foreach (var item in propertiesToRun.Distinct())
        {
          var doCascade = host.CascadeOnDirtyProperties && result.DirtyProperties.Any(p => p == item.Name);
          foreach (var pause in CheckRulesForProperty(run, item, doCascade, executionContext | RuleContextModes.AsAffectedProperty))
            yield return pause;
          if (run.Stream.IsDiscarded)
            yield break;
        }
      }
      run.Frames.Remove(frame);
    }

    /// <summary>
    /// Runs the rules for a property and cascades to the properties they affect.
    /// </summary>
    private IEnumerable<RuleStreamPause> CheckRulesForProperty(StreamRun run, IPropertyInfo property, bool cascade, RuleContextModes executionMode)
    {
      var host = run.Host;
      var primaryRules = (from r in host.Rules
                          where ReferenceEquals(r.PrimaryProperty, property)
                            && BusinessRules.CanRunRule(host.ApplicationContext, r, executionMode)
                          orderby r.Priority
                          select r).ToList();
      var inputRules = (from r in host.Rules
                        where !ReferenceEquals(r.PrimaryProperty, property)
                              && r.PrimaryProperty != null
                              && r.InputProperties != null
                              && r.InputProperties.Contains(property)
                        select r).ToList();

      host.ClearBrokenRules(property);
      var frame = new Frame(primaryRules.SelectMany(r => r.AffectedProperties).Concat(inputRules.Select(r => r.PrimaryProperty!)));
      run.Frames.Add(frame);
      var primaryResult = new RunRulesResult([], []);
      foreach (var pause in RunRules(run, primaryRules, cascade, executionMode, primaryResult))
        yield return pause;
      if (run.Stream.IsDiscarded)
        yield break;

      if (host.CascadeOnDirtyProperties)
        cascade = cascade || primaryResult.DirtyProperties.Any();
      if (cascade)
      {
        // a sequential rule's affected properties cascade like a sync rule's
        var propertiesToRun = new List<IPropertyInfo>();
        foreach (var item in primaryRules)
          if (!item.IsAsync || IsSequentialAsync(item))
          {
            foreach (var p in item.AffectedProperties)
              if (!ReferenceEquals(property, p))
                propertiesToRun.Add(p);
          }

        var dirtyProperties = primaryResult.DirtyProperties;
        foreach (var r in inputRules)
          if (!r.CascadeIfDirty || dirtyProperties.Contains(r.PrimaryProperty!.Name))
            propertiesToRun.Add(r.PrimaryProperty!);

        foreach (var item in propertiesToRun.Distinct())
        {
          var doCascade = host.CascadeOnDirtyProperties && primaryResult.DirtyProperties.Any(p => p == item.Name);
          foreach (var pause in CheckRulesForProperty(run, item, doCascade, executionMode | RuleContextModes.AsAffectedProperty))
            yield return pause;
          if (run.Stream.IsDiscarded)
            yield break;
        }
      }
      run.Frames.Remove(frame);

      // always make sure to add PrimaryProperty
      run.Stream.AddAffected(property.Name);
    }

    /// <summary>
    /// Runs a list of rules for one property (or the object), with short-circuiting.
    /// </summary>
    private IEnumerable<RuleStreamPause> RunRules(StreamRun run, List<IBusinessRuleBase> rules, bool cascade, RuleContextModes executionContext, RunRulesResult result)
    {
      var host = run.Host;
      var stream = run.Stream;
      var onAsyncRuleCompleted = cascade ? AsyncCascadeHandler(host) : null;
      var group = new GroupState();
      var frame = new Frame(rules, null);
      run.Frames.Add(frame);
      for (var i = 0; i < rules.Count; i++)
      {
        if (stream.IsDiscarded)
          yield break;
        if (group.Stopped)
          break;
        var rule = rules[i];
        var sequential = IsSequentialAsync(rule);
        if (sequential)
        {
          // the async rules started earlier complete first
          frame.Index = i;
          foreach (var pause in WaitAndFold(run))
            yield return pause;
          if (stream.IsDiscarded)
            yield break;
          if (group.Stopped)
            break;
        }

        // implicit short-circuiting
        if (group.AnyRuleBroken && rule.Priority > host.ProcessThroughPriority)
          break;

        var outcome = host.RunRule(rule, executionContext, onAsyncRuleCompleted, true);
        if (outcome.Run is { IsOwned: true } asyncRun)
        {
          if (!sequential)
          {
            asyncRun.Group = group;
            asyncRun.Cascade = cascade;
            stream.UnfoldedRuns.Add(asyncRun);
            continue;
          }
          if (!asyncRun.IsCompleted)
          {
            frame.Index = i + 1;
            yield return run.Pause([asyncRun]);
            if (stream.IsDiscarded)
              yield break;
          }
          asyncRun.IsFolded = true;
          outcome = asyncRun.Outcome!;
        }

        stream.AddAffected(outcome.AffectedProperties);
        result.DirtyProperties.AddRange(outcome.DirtyProperties);
        if (outcome.HasBrokenError)
          group.AnyRuleBroken = true;
        // explicit short-circuiting
        if (outcome.StopProcessing)
          group.Stopped = true;
      }
      run.Frames.Remove(frame);
    }

    #endregion

    /// <summary>
    /// Waits until every async rule the stream started and has not folded
    /// is complete, then folds in their outcomes.
    /// </summary>
    private IEnumerable<RuleStreamPause> WaitAndFold(StreamRun run)
    {
      var stream = run.Stream;
      while (stream.UnfoldedRuns.Count > 0)
      {
        var runs = stream.UnfoldedRuns.ToList();
        if (runs.Exists(r => !r.IsCompleted))
        {
          yield return run.Pause(runs);
          if (stream.IsDiscarded)
            yield break;
        }
        stream.UnfoldedRuns.RemoveAll(runs.Contains);
        foreach (var asyncRun in runs)
        {
          asyncRun.IsFolded = true;
          if (stream.Kind == RuleStreamKind.AllRules)
          {
            FoldIntoBatch(run, asyncRun);
          }
          else
          {
            foreach (var pause in FoldIntoCascade(run, asyncRun))
              yield return pause;
            if (stream.IsDiscarded)
              yield break;
          }
        }
      }
    }

    /// <summary>
    /// Applies a completed async rule's outcome to a CheckRules(property)
    /// or CheckObjectRules() run: its result short-circuits its property's
    /// rules, and the rules of its affected properties run, as the legacy
    /// completion handler runs them.
    /// </summary>
    private IEnumerable<RuleStreamPause> FoldIntoCascade(StreamRun run, AsyncRuleRun asyncRun)
    {
      var outcome = asyncRun.Outcome!;
      if (asyncRun.Group is GroupState group)
      {
        if (outcome.HasBrokenError)
          group.AnyRuleBroken = true;
        if (outcome.StopProcessing)
          group.Stopped = true;
      }
      if (!asyncRun.Cascade || asyncRun.Context == null)
        yield break;

      var context = asyncRun.Context;
      foreach (var item in context.Rule.AffectedProperties.Distinct())
        if (!ReferenceEquals(context.Rule.PrimaryProperty, item))
        {
          var doCascade = run.Host.CascadeOnDirtyProperties && context.DirtyProperties != null && context.DirtyProperties.Any(p => p.Name == item.Name);
          foreach (var pause in CheckRulesForProperty(run, item, doCascade, context.ExecuteContext | RuleContextModes.AsAffectedProperty))
            yield return pause;
          if (run.Stream.IsDiscarded)
            yield break;
        }
    }

    /// <summary>
    /// The state of one stream's run.
    /// </summary>
    private sealed class StreamRun
    {
      public StreamRun(IRuleRunnerHost host, RuleStream stream, bool sequential)
      {
        Host = host;
        Stream = stream;
        Sequential = sequential;
      }

      public IRuleRunnerHost Host { get; }

      public RuleStream Stream { get; }

      /// <summary>
      /// Gets a value indicating whether the rules have a sequential async
      /// rule. If not, nothing waits and async rules complete as they always did.
      /// </summary>
      public bool Sequential { get; }

      /// <summary>
      /// Gets or sets the dirty set of the current CheckRules() pass.
      /// </summary>
      public HashSet<string>? CurrentDirty { get; set; }

      /// <summary>
      /// Gets the rule lists in progress, to find the properties whose
      /// rules have not run yet at a pause.
      /// </summary>
      public List<Frame> Frames { get; } = [];

      public RuleStreamPause Pause(List<AsyncRuleRun> waitFor)
      {
        var waiting = new List<IPropertyInfo>();
        foreach (var frame in Frames)
          foreach (var property in frame.Pending())
            if (!waiting.Contains(property))
              waiting.Add(property);
        return new RuleStreamPause(waitFor, waiting);
      }
    }

    /// <summary>
    /// A rule list in progress, or the properties a step will cascade to.
    /// </summary>
    private sealed class Frame
    {
      private readonly IReadOnlyList<IBusinessRuleBase>? _rules;
      private readonly Func<IBusinessRuleBase, bool>? _filter;
      private readonly IEnumerable<IPropertyInfo>? _properties;

      public Frame(IReadOnlyList<IBusinessRuleBase> rules, Func<IBusinessRuleBase, bool>? filter)
      {
        _rules = rules;
        _filter = filter;
      }

      public Frame(IEnumerable<IPropertyInfo> properties)
      {
        _properties = properties;
      }

      /// <summary>
      /// Gets or sets the index of the next rule to run.
      /// </summary>
      public int Index { get; set; }

      /// <summary>
      /// Gets the affected properties of the rules that have not run yet.
      /// </summary>
      public IEnumerable<IPropertyInfo> Pending()
      {
        if (_properties != null)
          return _properties;
        return _rules!.Skip(Index).Where(r => _filter == null || _filter(r)).SelectMany(r => r.AffectedProperties);
      }
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
      private readonly Dictionary<IPropertyInfo, List<IBusinessRuleBase>> _rulesByProperty = [];

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
          if (!_rulesByProperty.TryGetValue(primary, out var propertyRules))
            _rulesByProperty.Add(primary, propertyRules = []);
          propertyRules.Add(rule);

          if (rule.InputProperties != null)
            foreach (var input in rule.InputProperties)
            {
              if (!_dependentsByInputName.TryGetValue(input.Name, out var dependents))
                _dependentsByInputName.Add(input.Name, dependents = []);
              if (!dependents.Contains(primary))
                dependents.Add(primary);
            }
        }

        // Priority first, across all properties (#4235). Ties keep the
        // CSLA 10 order: by property (first registration), then registration.
        Rules = keyed
          .OrderBy(k => k.Rule.Priority)
          .ThenBy(k => k.Property)
          .ThenBy(k => k.Registration)
          .Select(k => k.Rule)
          .ToList();
      }

      /// <summary>
      /// Property rules in run order: by priority, then by property, then
      /// by registration.
      /// </summary>
      public List<IBusinessRuleBase> Rules { get; }

      /// <summary>
      /// Properties that have rules, in order of their first rule's registration.
      /// </summary>
      public List<IPropertyInfo> Properties { get; } = [];

      /// <summary>
      /// Gets the rules whose primary property is the given property.
      /// </summary>
      public IReadOnlyList<IBusinessRuleBase> RulesFor(IPropertyInfo property)
      {
        return _rulesByProperty.TryGetValue(property, out var rules) ? rules : [];
      }

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
