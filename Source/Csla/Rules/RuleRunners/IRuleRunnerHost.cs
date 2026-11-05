//-----------------------------------------------------------------------
// <copyright file="IRuleRunnerHost.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Runs individual business rules on behalf of a rule runner</summary>
//-----------------------------------------------------------------------

using Csla.Core;

namespace Csla.Rules
{
  /// <summary>
  /// Runs individual business rules for a business object and records
  /// their results. A rule runner decides which rules run and in what
  /// order; the host owns the mechanics of running one rule.
  /// </summary>
  internal interface IRuleRunnerHost
  {
    /// <summary>
    /// Gets the application context for the business object.
    /// </summary>
    ApplicationContext ApplicationContext { get; }

    /// <summary>
    /// Gets the business rules for the business object's type and rule set,
    /// in registration order.
    /// </summary>
    IReadOnlyList<IBusinessRuleBase> Rules { get; }

    /// <summary>
    /// Gets a value indicating whether rules should cascade when a rule
    /// changes a property value through its output values.
    /// </summary>
    bool CascadeOnDirtyProperties { get; }

    /// <summary>
    /// Gets the priority through which all rules for a property run,
    /// even after a rule for that property is broken.
    /// </summary>
    int ProcessThroughPriority { get; }

    /// <summary>
    /// Removes the broken rules that originated from the rules for a property.
    /// </summary>
    /// <param name="property">The property, or null for object-level rules.</param>
    void ClearBrokenRules(IPropertyInfo? property);

    /// <summary>
    /// Runs one rule: supplies its input values, executes it, applies its
    /// output values, records its broken rules and tracks busy state.
    /// </summary>
    /// <param name="rule">The rule to run.</param>
    /// <param name="executionContext">The context the rule runs in.</param>
    /// <param name="onAsyncRuleCompleted">
    /// For an async rule, called when the rule completes, after its output
    /// values and broken rules are applied. Returns the names of any further
    /// properties whose rules it ran. Null to do nothing on completion.
    /// </param>
    /// <returns>
    /// The outcome of a sync rule. For an async rule, an empty outcome,
    /// because the rule is still running.
    /// </returns>
    RuleRunOutcome RunRule(IBusinessRuleBase rule, RuleContextModes executionContext, AsyncRuleCompletedHandler? onAsyncRuleCompleted);
  }

  /// <summary>
  /// Called when an async rule completes.
  /// </summary>
  /// <param name="context">The completed rule's context.</param>
  /// <returns>The names of any further properties whose rules were run.</returns>
  internal delegate IEnumerable<string> AsyncRuleCompletedHandler(IRuleContext context);

  /// <summary>
  /// The outcome of running one rule.
  /// </summary>
  internal sealed class RuleRunOutcome
  {
    /// <summary>
    /// The outcome of an async rule that is still running.
    /// </summary>
    public static RuleRunOutcome Pending { get; } = new([], [], false, false);

    public RuleRunOutcome(List<string> affectedProperties, List<string> dirtyProperties, bool hasBrokenError, bool stopProcessing)
    {
      AffectedProperties = affectedProperties;
      DirtyProperties = dirtyProperties;
      HasBrokenError = hasBrokenError;
      StopProcessing = stopProcessing;
    }

    /// <summary>
    /// Names of the rule's affected properties and of the properties it set
    /// through output values.
    /// </summary>
    public IReadOnlyList<string> AffectedProperties { get; }

    /// <summary>
    /// Names of the properties whose values the rule changed through output values.
    /// </summary>
    public IReadOnlyList<string> DirtyProperties { get; }

    /// <summary>
    /// Gets a value indicating whether the rule returned a broken result
    /// with error severity.
    /// </summary>
    public bool HasBrokenError { get; }

    /// <summary>
    /// Gets a value indicating whether the rule asked for no further rules
    /// for its property to run.
    /// </summary>
    public bool StopProcessing { get; }
  }
}
