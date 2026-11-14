//-----------------------------------------------------------------------
// <copyright file="BusinessRuleAsync.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Base class used to create business and validation</summary>
//-----------------------------------------------------------------------

namespace Csla.Rules
{
  /// <summary>
  /// Base class used to create async 
  /// business and validation rules.
  /// </summary>
  public abstract class BusinessRuleAsync : BusinessRuleBase, IBusinessRuleAsync
  {
    /// <summary>
    /// Gets a value indicating whether the rule will run
    /// on a background thread.
    /// </summary>
    public override bool IsAsync
    {
      get => true;
      protected set { }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the rule runs in priority
    /// order with the other rules. When true, the rules after it (for
    /// every property) wait until it completes, and its result
    /// short-circuits them like a sync rule's result
    /// (<see cref="BusinessRules.ProcessThroughPriority"/>,
    /// <see cref="RuleResult.StopProcessing"/>). Defaults to false: the
    /// rule starts in priority order and then runs in the background.
    /// </summary>
    /// <remarks>
    /// Use this for a rule that is async only because it does I/O, such
    /// as a database lookup, and whose result decides whether later rules
    /// run. While the rule runs, the properties whose rules wait for it
    /// are busy. Only the batch CheckRules runner (the default) honors
    /// this value; the legacy runner ignores it.
    /// </remarks>
    /// <exception cref="ArgumentException">The value is set after the rule has run.</exception>
    public new bool IsSequential
    {
      get => base.IsSequential;
      set => SetIsSequential(value);
    }

    /// <summary>
    /// Creates an instance of the rule that applies
    /// to a business object as a whole.
    /// </summary>
    protected BusinessRuleAsync()
      : this(null)
    { }

    /// <summary>
    /// Creates an instance of the rule that applies
    /// to a specfic property.
    /// </summary>
    /// <param name="primaryProperty">Primary property for this rule.</param>
    protected BusinessRuleAsync(Core.IPropertyInfo? primaryProperty)
      : base(primaryProperty)
    { }

    /// <summary>
    /// Business or validation rule implementation.
    /// </summary>
    /// <param name="context">Rule context object.</param>
    protected virtual Task ExecuteAsync(IRuleContext context)
    {
      return Task.CompletedTask;
    }

    Task IBusinessRuleAsync.ExecuteAsync(IRuleContext context)
    {
      PropertiesLocked = true;
      return ExecuteAsync(context);
    }
  }
}
