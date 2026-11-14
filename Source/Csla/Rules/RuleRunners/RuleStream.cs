//-----------------------------------------------------------------------
// <copyright file="RuleStream.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>One run of CheckRules that can wait for sequential async rules</summary>
//-----------------------------------------------------------------------

using Csla.Core;

namespace Csla.Rules
{
  /// <summary>
  /// What a rule stream runs.
  /// </summary>
  internal enum RuleStreamKind
  {
    /// <summary>
    /// All rules of the object (BusinessRules.CheckRules()).
    /// </summary>
    AllRules,
    /// <summary>
    /// The rules of a property after its value changed (BusinessRules.CheckRules(property)).
    /// </summary>
    Property,
    /// <summary>
    /// The object-level rules (BusinessRules.CheckObjectRules()).
    /// </summary>
    ObjectRules,
    /// <summary>
    /// The rules of the affected properties of an async rule that
    /// completed after its stream ended (the completion cascade).
    /// </summary>
    Cascade
  }

  /// <summary>
  /// One run of CheckRules(), CheckRules(property) or CheckObjectRules().
  /// </summary>
  /// <remarks>
  /// <para>
  /// The rule runner supplies the run as an iterator that yields a
  /// <see cref="RuleStreamPause"/> where the stream must wait for async
  /// rules to complete (a sequential async rule). The iterator holds the
  /// rest of the run. The host drives it and resumes it when the rules it
  /// waits for are complete.
  /// </para>
  /// <para>
  /// The stream also collects the affected properties of the async rules
  /// it started, so CheckRulesAsync can return them.
  /// </para>
  /// </remarks>
  internal sealed class RuleStream
  {
    private readonly List<string> _runnerAffected = [];
    private readonly List<string> _asyncAffected = [];

    public RuleStream(object host, RuleStreamKind kind, IPropertyInfo? root, bool collectsAsyncResults, IRuleContext? completedContext = null, RuleStream? parent = null)
    {
      Host = host;
      Kind = kind;
      Root = root;
      CollectsAsyncResults = collectsAsyncResults;
      CompletedContext = completedContext;
      Parent = parent;
    }

    /// <summary>
    /// Gets the context of the completed async rule whose affected
    /// properties a cascade stream runs the rules of.
    /// </summary>
    public IRuleContext? CompletedContext { get; }

    /// <summary>
    /// Gets the stream that started the async rule of a cascade stream.
    /// The cascade's affected properties also count for the parent, so
    /// CheckRulesAsync returns them.
    /// </summary>
    public RuleStream? Parent { get; }

    /// <summary>
    /// Gets the BusinessRules instance that runs the stream.
    /// </summary>
    public object Host { get; }

    /// <summary>
    /// Gets what the stream runs.
    /// </summary>
    public RuleStreamKind Kind { get; }

    /// <summary>
    /// Gets the property whose rules the stream runs, for a property stream.
    /// </summary>
    public IPropertyInfo? Root { get; }

    /// <summary>
    /// Gets a value indicating whether the stream collects the affected
    /// properties of the async rules it starts.
    /// </summary>
    public bool CollectsAsyncResults { get; }

    /// <summary>
    /// Gets or sets the rest of the run.
    /// </summary>
    public IEnumerator<RuleStreamPause>? Steps { get; set; }

    /// <summary>
    /// Gets the async rules the stream started and has not folded into
    /// its run yet. Only the rule runner changes the list while the stream
    /// runs; the host reads it once the stream has ended.
    /// </summary>
    public List<AsyncRuleRun> UnfoldedRuns { get; } = [];

    /// <summary>
    /// Gets or sets the pause the stream is waiting at, or null.
    /// </summary>
    public RuleStreamPause? CurrentPause { get; set; }

    /// <summary>
    /// Gets a value indicating whether the stream is waiting at a pause.
    /// </summary>
    public bool IsPaused => CurrentPause != null;

    /// <summary>
    /// Gets or sets a value indicating whether the stream has ended.
    /// </summary>
    public bool IsFinished { get; set; }

    private volatile bool _isDiscarded;

    /// <summary>
    /// Gets or sets a value indicating whether a newer run replaced the
    /// stream. The rest of a discarded stream never runs.
    /// </summary>
    public bool IsDiscarded
    {
      get => _isDiscarded;
      set => _isDiscarded = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the results of the async
    /// rules of a discarded stream still apply, because the run that
    /// replaced it failed and nothing reruns them.
    /// </summary>
    public bool KeepsResults { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether a CheckRulesAsync call
    /// awaits the stream.
    /// </summary>
    public bool HasAwaiter { get; set; }

    /// <summary>
    /// Gets or sets the exception that ended the stream after a pause.
    /// </summary>
    public Exception? Error { get; set; }

    /// <summary>
    /// Gets or sets the busy entries the stream holds for the properties
    /// whose rules wait at the current pause.
    /// </summary>
    public List<IPropertyInfo> WaitEntries { get; set; } = [];

    /// <summary>
    /// Gets or sets the names of the properties whose rules a stream of
    /// this kind and root can run, or null for all properties.
    /// </summary>
    public HashSet<string>? Coverage { get; set; }

    /// <summary>
    /// Adds the names of properties affected by rules the runner ran.
    /// </summary>
    public void AddAffected(IEnumerable<string> names)
    {
      var list = names as IReadOnlyCollection<string> ?? names.ToList();
      lock (_runnerAffected)
        _runnerAffected.AddRange(list);
      Parent?.AddAsyncAffected(list);
    }

    /// <summary>
    /// Adds the name of a property affected by a rule the runner ran.
    /// </summary>
    public void AddAffected(string name)
    {
      lock (_runnerAffected)
        _runnerAffected.Add(name);
      Parent?.AddAsyncAffected([name]);
    }

    /// <summary>
    /// Gets the number of names added by <see cref="AddAffected(IEnumerable{string})"/>.
    /// </summary>
    public int RunnerAffectedCount
    {
      get
      {
        lock (_runnerAffected)
          return _runnerAffected.Count;
      }
    }

    /// <summary>
    /// Gets the names of properties affected by rules the runner ran,
    /// starting at the given index.
    /// </summary>
    public List<string> GetRunnerAffected(int start = 0)
    {
      lock (_runnerAffected)
        return _runnerAffected.Skip(start).ToList();
    }

    /// <summary>
    /// Adds the names of the affected properties of a completed async rule.
    /// </summary>
    public void AddAsyncAffected(IEnumerable<string> names)
    {
      var list = names as IReadOnlyCollection<string> ?? names.ToList();
      lock (_runnerAffected)
        _asyncAffected.AddRange(list);
      Parent?.AddAsyncAffected(list);
    }

    /// <summary>
    /// Gets the names of all affected properties: those of the rules the
    /// runner ran and those of the completed async rules the stream started.
    /// </summary>
    public List<string> GetAllAffected()
    {
      lock (_runnerAffected)
        return _runnerAffected.Concat(_asyncAffected).Distinct().ToList();
    }

    /// <summary>
    /// Gets a value indicating whether running the rules of a property
    /// replaces this stream.
    /// </summary>
    public bool Covers(IPropertyInfo property) =>
      Kind == RuleStreamKind.AllRules || (Coverage != null && Coverage.Contains(property.Name));
  }

  /// <summary>
  /// A point where a rule stream waits for async rules to complete.
  /// </summary>
  internal sealed class RuleStreamPause
  {
    public RuleStreamPause(IReadOnlyList<AsyncRuleRun> waitFor, IReadOnlyCollection<IPropertyInfo> waitingProperties)
    {
      WaitFor = waitFor;
      WaitingProperties = waitingProperties;
    }

    /// <summary>
    /// Gets the async rules that must complete before the stream continues.
    /// </summary>
    public IReadOnlyList<AsyncRuleRun> WaitFor { get; }

    /// <summary>
    /// Gets the properties whose rules have not run yet because the
    /// stream waits. They are reported busy while the stream waits.
    /// </summary>
    public IReadOnlyCollection<IPropertyInfo> WaitingProperties { get; }

    /// <summary>
    /// Gets a value indicating whether every rule the pause waits for is complete.
    /// </summary>
    public bool IsSatisfied
    {
      get
      {
        foreach (var run in WaitFor)
          if (!run.IsCompleted)
            return false;
        return true;
      }
    }
  }

  /// <summary>
  /// One run of an async rule started by a rule stream.
  /// </summary>
  internal sealed class AsyncRuleRun
  {
    private volatile bool _isCompleted;

    public AsyncRuleRun(IBusinessRuleBase rule, RuleStream stream, bool isOwned, AsyncRuleCompletedHandler? onCompleted)
    {
      Rule = rule;
      Stream = stream;
      IsOwned = isOwned;
      OnCompleted = onCompleted;
    }

    /// <summary>
    /// Gets the rule.
    /// </summary>
    public IBusinessRuleBase Rule { get; }

    /// <summary>
    /// Gets the stream that started the rule.
    /// </summary>
    public RuleStream Stream { get; }

    /// <summary>
    /// Gets a value indicating whether the stream folds the rule's
    /// outcome into its run. A rule that is not owned completes the way
    /// an async rule always did: through <see cref="OnCompleted"/>. An
    /// owned rule that completes after its stream ended runs the same
    /// cascade as a stream of its own, so sequential rules in it wait.
    /// The results of an owned rule whose stream was discarded are dropped.
    /// </summary>
    public bool IsOwned { get; }

    /// <summary>
    /// Gets the handler that runs when the rule completes outside its stream.
    /// </summary>
    public AsyncRuleCompletedHandler? OnCompleted { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the rule completed
    /// outside its stream, through <see cref="OnCompleted"/>.
    /// </summary>
    public bool IsDetached { get; set; }

    /// <summary>
    /// Gets or sets the short-circuit state the outcome applies to.
    /// </summary>
    public object? Group { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the stream runs the rules
    /// of the rule's affected properties when it folds the outcome.
    /// </summary>
    public bool Cascade { get; set; }

    /// <summary>
    /// Gets the completed rule's context.
    /// </summary>
    public IRuleContext? Context { get; private set; }

    /// <summary>
    /// Gets the completed rule's outcome.
    /// </summary>
    public RuleRunOutcome? Outcome { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the rule completed.
    /// </summary>
    public bool IsCompleted => _isCompleted;

    /// <summary>
    /// Records the rule's completion.
    /// </summary>
    public void Complete(IRuleContext? context, RuleRunOutcome outcome)
    {
      Context = context;
      Outcome = outcome;
      _isCompleted = true;
    }
  }
}
