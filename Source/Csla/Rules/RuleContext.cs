//-----------------------------------------------------------------------
// <copyright file="RuleContext.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Context information provided to a business rule</summary>
//-----------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Xml.Linq;
using Csla.Core;
using Csla.Properties;
using Microsoft.Extensions.DependencyInjection;

namespace Csla.Rules
{
  /// <summary>
  /// RuleContext mode flags
  /// </summary>
  [Flags]
  public enum RuleContextModes
  {
    /// <summary>
    /// Default value, rule can run in any context
    /// </summary>
    Any = 247,
    /// <summary>
    /// Called from CheckRules
    /// </summary>
    CheckRules = 1,
    /// <summary>
    /// Called from CheckObjectRules
    /// </summary>
    CheckObjectRules = 2,
    /// <summary>
    /// Called from PropertyHasChanged event on BO but not including cascade calls by AffectedProperties
    /// </summary>
    PropertyChanged = 4,
    /// <summary>
    /// Include cascaded calls by AffectedProperties
    /// </summary>
    AsAffectedProperty = 8,
  }

  /// <summary>
  /// Context information provided to a business rule
  /// when it is invoked.
  /// </summary>
  public class RuleContext : IRuleContext
  {
    /// <inheritdoc />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public IBusinessRuleBase Rule { get; internal set; }

    /// <inheritdoc />
    public object? Target { get; internal set; }

    /// <inheritdoc />
    public Dictionary<IPropertyInfo, object?> InputPropertyValues { get; }

    /// <inheritdoc />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public List<IPropertyInfo> DirtyProperties
    {
      get;
    }

    /// <inheritdoc />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Dictionary<IPropertyInfo, object?> OutputPropertyValues
    {
      get;
    }

    /// <inheritdoc />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public List<RuleResult> Results
    {
      get;
    }

    private readonly Action<IRuleContext> _completeHandler;

    /// <inheritdoc />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public string OriginPropertyName { get; private set; }

    /// <inheritdoc />
    public RuleContextModes ExecuteContext { get; internal set; }


    /// <inheritdoc />
    public void ExecuteRule(IBusinessRuleBase innerRule)
    {
      if (innerRule is null)
        throw new ArgumentNullException(nameof(innerRule));

      var chainedContext = GetChainedContext(innerRule);
      if (BusinessRules.CanRunRule(ApplicationContext, innerRule, chainedContext.ExecuteContext))
      {
        if (innerRule is IBusinessRule syncRule)
          syncRule.Execute(chainedContext);
        else if (innerRule is IBusinessRuleAsync asyncRule)
          asyncRule.ExecuteAsync(chainedContext).ContinueWith(_ => { chainedContext.Complete(); });
        else
          throw new ArgumentOutOfRangeException(innerRule.GetType().FullName);
      }
    }

    /// <inheritdoc />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool IsCascadeContext
    {
      get { return (ExecuteContext & RuleContextModes.AsAffectedProperty) > 0; }
    }

    /// <inheritdoc />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool IsPropertyChangedContext
    {
      get { return (ExecuteContext & RuleContextModes.PropertyChanged) > 0; }
    }

    /// <inheritdoc />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool IsCheckRulesContext
    {
      get { return (ExecuteContext & RuleContextModes.CheckRules) > 0; }
    }

    /// <summary>
    /// Gets a value indicating whether this instance is check object rules context.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is check object rules context; otherwise, <c>false</c>.
    /// </value>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool IsCheckObjectRulesContext
    {
      get { return (ExecuteContext & RuleContextModes.CheckObjectRules) > 0; }
    }

    internal RuleContext(ApplicationContext applicationContext, IBusinessRuleBase rule, RuleContextModes executeContext, object? target, Action<IRuleContext> completeHandler)
      : this(applicationContext, rule, executeContext, completeHandler, [], target)
    {
    }

    internal RuleContext(ApplicationContext applicationContext, IBusinessRuleBase rule, RuleContextModes executeContext, Action<IRuleContext> completeHandler, Dictionary<IPropertyInfo, object?> inputPropertyValues, object? target)
      : this(applicationContext, rule, executeContext, completeHandler, inputPropertyValues, target, [], [], [])
    {
    }

    internal RuleContext(ApplicationContext applicationContext, IBusinessRuleBase rule, RuleContextModes executeContext, Action<IRuleContext> completeHandler, Dictionary<IPropertyInfo, object?> inputPropertyValues, object? target, Dictionary<IPropertyInfo, object?> outputPropertyValues, List<IPropertyInfo> dirtyProperties, List<RuleResult> results)
    {
      ApplicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
      Rule = rule ?? throw new ArgumentNullException(nameof(rule));
      ExecuteContext = executeContext;
      _completeHandler = completeHandler ?? throw new ArgumentNullException(nameof(completeHandler));
      InputPropertyValues = inputPropertyValues ?? throw new ArgumentNullException(nameof(inputPropertyValues));
      Target = target;
      OutputPropertyValues = outputPropertyValues ?? throw new ArgumentNullException(nameof(outputPropertyValues));
      DirtyProperties = dirtyProperties ?? throw new ArgumentNullException(nameof(dirtyProperties));
      Results = results ?? throw new ArgumentNullException(nameof(results));
      string originPropertyName = "";
      if (rule.PrimaryProperty != null)
        originPropertyName = rule.PrimaryProperty.Name;
      OriginPropertyName = originPropertyName;
    }

    /// <summary>
    /// Creates a RuleContext instance for unit tests.
    /// </summary>
    /// <param name="applicationContext">Current ApplicationContext</param>
    /// <param name="completeHandler">Callback for async rule.</param>
    /// <param name="rule">Reference to the rule object.</param>
    /// <param name="target">Target business object.</param>
    /// <param name="inputPropertyValues">Input property values used by the rule.</param>
    /// <exception cref="ArgumentNullException"><paramref name="applicationContext"/>, <paramref name="completeHandler"/>, <paramref name="rule"/> or <paramref name="inputPropertyValues"/> is <see langword="null"/>.</exception>
    public RuleContext(ApplicationContext applicationContext, Action<IRuleContext> completeHandler, IBusinessRuleBase rule, object? target, Dictionary<IPropertyInfo, object?> inputPropertyValues)
      : this(applicationContext, rule, RuleContextModes.PropertyChanged, completeHandler, inputPropertyValues, target)
    {
    }

    /// <inheritdoc />
    public IRuleContext GetChainedContext(IBusinessRuleBase rule)
    {
      if (rule is null)
        throw new ArgumentNullException(nameof(rule));

      object? ruleTarget = null;
      if (!rule.IsAsync || rule.ProvideTargetWhenAsync)
        ruleTarget = Target;

      return new RuleContext(ApplicationContext, rule, ExecuteContext, _completeHandler, InputPropertyValues, ruleTarget, OutputPropertyValues, DirtyProperties, Results)
      {
        OriginPropertyName = OriginPropertyName
      };
    }

    /// <inheritdoc />
    public void AddErrorResult(string description)
    {
      if (string.IsNullOrWhiteSpace(description))
        throw new ArgumentException(string.Format(Resources.StringNotNullOrWhiteSpaceException, nameof(description)), nameof(description));

      Results.Add(new RuleResult(Rule.RuleName, Rule.PrimaryProperty, description, Rule.DisplayIndex));
    }

    /// <inheritdoc />
    public void AddErrorResult(string description, bool stopProcessing)
    {
      if (string.IsNullOrWhiteSpace(description))
        throw new ArgumentException(string.Format(Resources.StringNotNullOrWhiteSpaceException, nameof(description)), nameof(description));

      Results.Add(new RuleResult(Rule.RuleName, Rule.PrimaryProperty, description, Rule.DisplayIndex) { StopProcessing = stopProcessing });
    }

    /// <inheritdoc />
    public void AddErrorResult(IPropertyInfo property, string description)
    {
      if (property is null)
        throw new ArgumentNullException(nameof(property));
      if (string.IsNullOrWhiteSpace(description))
        throw new ArgumentException(string.Format(Resources.StringNotNullOrWhiteSpaceException, nameof(description)), nameof(description));
      if (!Rule.AffectedProperties.Contains(property))
        throw new ArgumentOutOfRangeException(property.Name, string.Format(Resources.PropertyNotInAffectedPropertiesException, property.Name));
      Results.Add(new RuleResult(Rule.RuleName, property, description, Rule.DisplayIndex));
    }

    /// <inheritdoc />
    public void AddWarningResult(string description)
    {
      if (string.IsNullOrWhiteSpace(description))
        throw new ArgumentException(string.Format(Resources.StringNotNullOrWhiteSpaceException, nameof(description)), nameof(description));

      Results.Add(new RuleResult(Rule.RuleName, Rule.PrimaryProperty, description, Rule.DisplayIndex) { Severity = RuleSeverity.Warning });
    }

    /// <inheritdoc />
    public void AddWarningResult(string description, bool stopProcessing)
    {
      if (string.IsNullOrWhiteSpace(description))
        throw new ArgumentException(string.Format(Resources.StringNotNullOrWhiteSpaceException, nameof(description)), nameof(description));

      Results.Add(new RuleResult(Rule.RuleName, Rule.PrimaryProperty, description, Rule.DisplayIndex) { Severity = RuleSeverity.Warning, StopProcessing = stopProcessing });
    }


    /// <inheritdoc />
    public void AddWarningResult(IPropertyInfo property, string description)
    {
      if (property is null)
        throw new ArgumentNullException(nameof(property));
      if (string.IsNullOrWhiteSpace(description))
        throw new ArgumentException(string.Format(Resources.StringNotNullOrWhiteSpaceException, nameof(description)), nameof(description));
      if (!Rule.AffectedProperties.Contains(property))
        throw new ArgumentOutOfRangeException(property.Name, string.Format(Resources.PropertyNotInAffectedPropertiesException, property.Name));
      Results.Add(new RuleResult(Rule.RuleName, property, description, Rule.DisplayIndex) { Severity = RuleSeverity.Warning });
    }

    /// <inheritdoc />
    public void AddInformationResult(string description)
    {
      if (string.IsNullOrWhiteSpace(description))
        throw new ArgumentException(string.Format(Resources.StringNotNullOrWhiteSpaceException, nameof(description)), nameof(description));

      Results.Add(new RuleResult(Rule.RuleName, Rule.PrimaryProperty, description, Rule.DisplayIndex) { Severity = RuleSeverity.Information });
    }

    /// <inheritdoc />
    public void AddInformationResult(string description, bool stopProcessing)
    {
      if (string.IsNullOrWhiteSpace(description))
        throw new ArgumentException(string.Format(Resources.StringNotNullOrWhiteSpaceException, nameof(description)), nameof(description));

      Results.Add(new RuleResult(Rule.RuleName, Rule.PrimaryProperty, description, Rule.DisplayIndex) { Severity = RuleSeverity.Information, StopProcessing = stopProcessing });
    }

    /// <inheritdoc />
    public void AddInformationResult(IPropertyInfo property, string description)
    {
      if (property is null)
        throw new ArgumentNullException(nameof(property));
      if (string.IsNullOrWhiteSpace(description))
        throw new ArgumentException(string.Format(Resources.StringNotNullOrWhiteSpaceException, nameof(description)), nameof(description));
      if (!Rule.AffectedProperties.Contains(property))
        throw new ArgumentOutOfRangeException(property.Name, string.Format(Resources.PropertyNotInAffectedPropertiesException, property.Name));
      Results.Add(new RuleResult(Rule.RuleName, property, description, Rule.DisplayIndex) { Severity = RuleSeverity.Information });
    }

    /// <inheritdoc />
    public void AddSuccessResult(bool stopProcessing)
    {
      Results.Add(new RuleResult(Rule.RuleName, Rule.PrimaryProperty, Rule.DisplayIndex) { Severity = RuleSeverity.Success, StopProcessing = stopProcessing });
    }

    /// <inheritdoc />
    public void AddOutValue(object? value)
    {
      if (Rule.PrimaryProperty is null)
        throw new InvalidOperationException($"{nameof(Rule)}.{nameof(Rule.PrimaryProperty)} == null");
      
      OutputPropertyValues.Add(Rule.PrimaryProperty, value);
    }

    /// <inheritdoc />
    public void AddOutValue(IPropertyInfo property, object? value)
    {
      if (property is null)
        throw new ArgumentNullException(nameof(property));

      OutputPropertyValues[property] = (value is null) ? property.DefaultValue : Utilities.CoerceValue(property.Type, value.GetType(), null, value);
    }


    /// <inheritdoc />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void AddDirtyProperty(IPropertyInfo property)
    {
      if (property is null)
        throw new ArgumentNullException(nameof(property));

      if (!Rule.AffectedProperties.Contains(property))
        throw new ArgumentOutOfRangeException(property.Name, string.Format(Resources.PropertyNotInAffectedPropertiesException, property.Name));
      DirtyProperties.Add(property);
    }

    /// <inheritdoc />
    public void Complete()
    {
      if (Results.Count == 0)
        Results.Add(new RuleResult(Rule.RuleName, Rule.PrimaryProperty, Rule.DisplayIndex));
      _completeHandler?.Invoke(this);
    }

    /// <inheritdoc />
    public  T? GetInputValue<T>(PropertyInfo<T> propertyInfo)
    {
      return GetInputValue<T>((IPropertyInfo)propertyInfo);
    }

    /// <inheritdoc />
    public T? GetInputValue<T>(IPropertyInfo propertyInfo)
    {
      if (propertyInfo is null)
        throw new ArgumentNullException(nameof(propertyInfo));

      return (T?)InputPropertyValues[propertyInfo];
    }

    /// <inheritdoc />
    public bool TryGetInputValue<T>(PropertyInfo<T> propertyInfo, ref T? value)
    {
      return TryGetInputValue<T>((IPropertyInfo)propertyInfo, ref value);
    }

    /// <inheritdoc />
    public bool TryGetInputValue<T>(IPropertyInfo propertyInfo, ref T? value)
    {
      if (propertyInfo is null)
        throw new ArgumentNullException(nameof(propertyInfo));

      if (!InputPropertyValues.TryGetValue(propertyInfo, out var propertyValue))
      {
        value = default(T);
        return false;
      }

      value = (T?)propertyValue;
      return true;
    }

    /// <inheritdoc />
    public ApplicationContext ApplicationContext { get; }

    /// <inheritdoc />
    public IDataPortalFactory DataPortalFactory => ApplicationContext.CurrentServiceProvider.GetRequiredService<IDataPortalFactory>();
  }
}
