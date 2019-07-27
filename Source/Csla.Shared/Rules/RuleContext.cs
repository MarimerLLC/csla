//-----------------------------------------------------------------------
// <copyright file="RuleContext.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Context information provided to a business rule</summary>
//-----------------------------------------------------------------------
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Core;
using Csla.Properties;
using Csla.Threading;

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
    AsAffectedPoperty = 8,
  }

  /// <summary>
  /// Context information provided to a business rule
  /// when it is invoked.
  /// </summary>
  public class RuleContext : IRuleContext
  {
    /// <summary>
    /// Gets the rule object.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public IBusinessRuleBase Rule { get; internal set; }

    /// <summary>
    /// Gets a reference to the target business object.
    /// </summary>
    public object Target { get; internal set; }

    /// <summary>
    /// Gets a dictionary containing copies of property values from
    /// the target business object.
    /// </summary>
    public Dictionary<Csla.Core.IPropertyInfo, object> InputPropertyValues { get; internal set; }

    private LazySingleton<List<IPropertyInfo>> _dirtyProperties;
    /// <summary>
    /// Gets a list of dirty properties (value was updated).
    /// </summary>
    /// <value>
    /// The dirty properties.
    /// </value>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public List<IPropertyInfo> DirtyProperties
    {
      get
      {
        if (!_dirtyProperties.IsValueCreated)
          return null;
        return _dirtyProperties.Value;
      }
    }

    private readonly LazySingleton<Dictionary<IPropertyInfo, object>> _outputPropertyValues;
    /// <summary>
    /// Gets a dictionary containing copies of property values that
    /// should be updated in the target object.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Dictionary<Csla.Core.IPropertyInfo, object> OutputPropertyValues
    {
      get
      {
        if (!_outputPropertyValues.IsValueCreated)
          return null;
        return _outputPropertyValues.Value;
      }
    }

    /// <summary>
    /// Gets a reference to the list of results being returned
    /// by this rule.
    /// </summary>
    private List<RuleResult> _results;
    /// <summary>
    /// Gets a list of RuleResult objects containing the
    /// results of the rule.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public List<RuleResult> Results
    {
      get
      {
        if (_results == null)
          _results = new List<RuleResult>();
        return _results;
      }
      private set
      {
        _results = value;
      }
    }

    private readonly Action<IRuleContext> _completeHandler;

    /// <summary>
    /// Gets or sets the name of the origin property.
    /// </summary>
    /// <value>The name of the origin property.</value>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public string OriginPropertyName { get; internal set; }

    /// <summary>
    /// Gets the execution context.
    /// </summary>
    /// <value>The execution context.</value>
    public RuleContextModes ExecuteContext { get; internal set; }


    /// <summary>
    /// Executes the inner rule from the outer rules context. 
    /// Creates a chained context and if CanRunRule will execute the inner rule.  
    /// </summary>
    /// <param name="innerRule">The inner rule.</param>
    public void ExecuteRule(IBusinessRuleBase innerRule)
    {
      var chainedContext = GetChainedContext(innerRule);
      if (BusinessRules.CanRunRule(innerRule, chainedContext.ExecuteContext))
      {
        if (innerRule is IBusinessRule syncRule)
          syncRule.Execute(chainedContext);
        else if (innerRule is IBusinessRuleAsync asyncRule)
          asyncRule.ExecuteAsync(chainedContext).ContinueWith((t) => { chainedContext.Complete(); });
        else
          throw new ArgumentOutOfRangeException(innerRule.GetType().FullName);
      }
    }

    /// <summary>
    /// Gets a value indicating whether this instance is cascade context as a result of AffectedProperties.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is cascade context; otherwise, <c>false</c>.
    /// </value>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool IsCascadeContext
    {
      get { return (ExecuteContext & RuleContextModes.AsAffectedPoperty) > 0; }
    }

    /// <summary>
    /// Gets a value indicating whether this instance is property changed context.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is property changed context; otherwise, <c>false</c>.
    /// </value>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool IsPropertyChangedContext
    {
      get { return (ExecuteContext & RuleContextModes.PropertyChanged) > 0; }
    }

    /// <summary>
    /// Gets a value indicating whether this instance is check rules context.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is check rules context; otherwise, <c>false</c>.
    /// </value>
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

    internal RuleContext(Action<IRuleContext> completeHandler)
    {
      _completeHandler = completeHandler;
      _outputPropertyValues = new LazySingleton<Dictionary<IPropertyInfo, object>>();
      _dirtyProperties = new LazySingleton<List<IPropertyInfo>>();
    }

    internal RuleContext(Action<IRuleContext> completeHandler, RuleContextModes executeContext) : this(completeHandler)
    {
      ExecuteContext = executeContext;
    }

    internal RuleContext(Action<IRuleContext> completeHandler, LazySingleton<Dictionary<IPropertyInfo, object>> outputPropertyValues, LazySingleton<List<IPropertyInfo>> dirtyProperties, RuleContextModes executeContext)
    {
      _completeHandler = completeHandler;
      _outputPropertyValues = outputPropertyValues;
      _dirtyProperties = dirtyProperties;
      ExecuteContext = executeContext;
    }

    /// <summary>
    /// Creates a RuleContext instance for testing.
    /// </summary>
    /// <param name="completeHandler">Callback for async rule.</param>
    /// <param name="rule">Reference to the rule object.</param>
    /// <param name="target">Target business object.</param>
    /// <param name="inputPropertyValues">Input property values used by the rule.</param>
    public RuleContext(Action<IRuleContext> completeHandler, IBusinessRuleBase rule, object target, Dictionary<Csla.Core.IPropertyInfo, object> inputPropertyValues)
      : this(completeHandler)
    {
      Rule = rule;
      if (rule.PrimaryProperty != null)
        OriginPropertyName = rule.PrimaryProperty.Name;
      Target = target;
      InputPropertyValues = inputPropertyValues;
      ExecuteContext = RuleContextModes.PropertyChanged;
    }

    /// <summary>
    /// Gets a new RuleContext object for a chained rule.
    /// </summary>
    /// <param name="rule">Chained rule that will use
    /// this new context.</param>
    /// <remarks>
    /// The properties from the existing RuleContext will be
    /// used to create the new context, with the exception
    /// of the Rule property which is set using the supplied
    /// IBusinessRule value.
    /// </remarks>
    public IRuleContext GetChainedContext(IBusinessRuleBase rule)
    {
      var result = new RuleContext(_completeHandler, _outputPropertyValues, _dirtyProperties, ExecuteContext);
      result.Rule = rule;
      result.OriginPropertyName = OriginPropertyName;
      result.InputPropertyValues = InputPropertyValues;
      result.Results = Results;

      if (!rule.IsAsync || rule.ProvideTargetWhenAsync)
        result.Target = Target;
      return result;
    }

    /// <summary>
    /// Add a Error severity result to the Results list.
    /// </summary>
    /// <param name="description">Human-readable description of
    /// why the rule failed.</param>
    public void AddErrorResult(string description)
    {
      Results.Add(new RuleResult(Rule.RuleName, Rule.PrimaryProperty, description));
    }

    /// <summary>
    /// Add a Error severity result to the Results list.
    /// </summary>
    /// <param name="description">Human-readable description of
    /// why the rule failed.</param>
    /// <param name="stopProcessing">True if no further rules should be processed
    /// for the current property.</param>
    public void AddErrorResult(string description, bool stopProcessing)
    {
      Results.Add(new RuleResult(Rule.RuleName, Rule.PrimaryProperty, description) { StopProcessing = stopProcessing });
    }

    /// <summary>
    /// Add a Error severity result to the Results list.
    /// This method is only allowed on "object" level rules to allow an object level rule to set warning result on a field. 
    /// </summary>
    /// <param name="property">Property to which the result applies.</param>
    /// <param name="description">Human-readable description of
    /// why the rule failed.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">When property is not defined in AffectedProperties list.</exception>   
    public void AddErrorResult(Csla.Core.IPropertyInfo property, string description)
    {
      if (!Rule.AffectedProperties.Contains(property))
        throw new ArgumentOutOfRangeException(property.Name, string.Format(Resources.PropertyNotInAffectedPropertiesException, property.Name));
      Results.Add(new RuleResult(Rule.RuleName, property, description));
    }

    /// <summary>
    /// Add a Warning severity result to the Results list.
    /// </summary>
    /// <param name="description">Human-readable description of
    /// why the rule failed.</param>
    public void AddWarningResult(string description)
    {
      Results.Add(new RuleResult(Rule.RuleName, Rule.PrimaryProperty, description) { Severity = RuleSeverity.Warning });
    }

    /// <summary>
    /// Add a Warning severity result to the Results list.
    /// </summary>
    /// <param name="description">Human-readable description of
    /// why the rule failed.</param>
    /// <param name="stopProcessing">True if no further rules should be processed
    /// for the current property.</param>
    public void AddWarningResult(string description, bool stopProcessing)
    {
      Results.Add(new RuleResult(Rule.RuleName, Rule.PrimaryProperty, description) { Severity = RuleSeverity.Warning, StopProcessing = stopProcessing });
    }


    /// <summary>
    /// Add a Warning severity result to the Results list.
    /// This method is only allowed on "object" level rules to allow an object level rule to set warning result on a field. 
    /// </summary>
    /// <param name="property">Property to which the result applies.</param>
    /// <param name="description">Human-readable description of  why the rule failed.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">When property is not defined in AffectedProperties list.</exception>    
    public void AddWarningResult(Csla.Core.IPropertyInfo property, string description)
    {
      if (!Rule.AffectedProperties.Contains(property))
        throw new ArgumentOutOfRangeException(property.Name, string.Format(Resources.PropertyNotInAffectedPropertiesException, property.Name));
      Results.Add(new RuleResult(Rule.RuleName, property, description) { Severity = RuleSeverity.Warning });
    }

    /// <summary>
    /// Add an Information severity result to the Results list.
    /// </summary>
    /// <param name="description">Human-readable description of
    /// why the rule failed.</param>
    public void AddInformationResult(string description)
    {
      Results.Add(new RuleResult(Rule.RuleName, Rule.PrimaryProperty, description) { Severity = RuleSeverity.Information });
    }

    /// <summary>
    /// Add an Information severity result to the Results list.
    /// </summary>
    /// <param name="description">Human-readable description of why the rule failed.</param>
    /// <param name="stopProcessing">True if no further rules should be processed for the current property.</param>
    public void AddInformationResult(string description, bool stopProcessing)
    {
      Results.Add(new RuleResult(Rule.RuleName, Rule.PrimaryProperty, description) { Severity = RuleSeverity.Information, StopProcessing = stopProcessing });
    }

    /// <summary>
    /// Add an Information severity result to the Results list.
    /// This method is only allowed on "object" level rules to allow an object level rule to set warning result on a field. 
    /// </summary>
    /// <param name="property">Property to which the result applies.</param>
    /// <param name="description">Human-readable description of why the rule failed.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">When property is not defined in AffectedProperties list.</exception>   
    public void AddInformationResult(Csla.Core.IPropertyInfo property, string description)
    {
      if (!Rule.AffectedProperties.Contains(property))
        throw new ArgumentOutOfRangeException(property.Name, string.Format(Resources.PropertyNotInAffectedPropertiesException, property.Name));
      Results.Add(new RuleResult(Rule.RuleName, property, description) { Severity = RuleSeverity.Information });
    }

    /// <summary>
    /// Add a Success severity result to the Results list.
    /// </summary>
    /// <param name="stopProcessing">True if no further rules should be processed for the current property.</param>
    public void AddSuccessResult(bool stopProcessing)
    {
      Results.Add(new RuleResult(Rule.RuleName, Rule.PrimaryProperty) { Severity = RuleSeverity.Success, StopProcessing = stopProcessing });
    }

    /// <summary>
    /// Add an outbound value to update the rule's primary 
    /// property on the business object once the rule is complete.
    /// </summary>
    /// <param name="value">New property value.</param>
    public void AddOutValue(object value)
    {
      _outputPropertyValues.Value.Add(Rule.PrimaryProperty, value);
    }

    /// <summary>
    /// Add an outbound value to update a property on the business
    /// object once the rule is complete.
    /// </summary>
    /// <param name="property">Property to update.</param>
    /// <param name="value">New property value.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">When property is not defined in AffectedProperties list.</exception>   
    public void AddOutValue(Csla.Core.IPropertyInfo property, object value)
    {
      if (!Rule.AffectedProperties.Contains(property))
        throw new ArgumentOutOfRangeException(property.Name, string.Format(Resources.PropertyNotInAffectedPropertiesException, property.Name));
      _outputPropertyValues.Value.Add(property, value);
    }


    /// <summary>
    /// Adds a property name as a dirty field (changed value).
    /// </summary>
    /// <param name="property">The property.</param>
    /// <exception cref="System.ArgumentOutOfRangeException"></exception>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void AddDirtyProperty(Csla.Core.IPropertyInfo property)
    {
      if (!Rule.AffectedProperties.Contains(property))
        throw new ArgumentOutOfRangeException(property.Name, string.Format(Resources.PropertyNotInAffectedPropertiesException, property.Name));
      _dirtyProperties.Value.Add(property);
    }

    /// <summary>
    /// Indicates that the rule processing is complete, so
    /// CSLA .NET will process the Results list. This method
    /// must be invoked on the UI thread.
    /// </summary>
    public void Complete()
    {
      if (Results.Count == 0)
        Results.Add(new RuleResult(Rule.RuleName, Rule.PrimaryProperty));
      _completeHandler?.Invoke(this);
    }

    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="propertyInfo">The property info.</param>
    /// <returns></returns>
    public  T GetInputValue<T>(PropertyInfo<T> propertyInfo)
    {
      return (T)InputPropertyValues[propertyInfo];
    }

    /// <summary>
    /// Gets the value with explicit cast
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="propertyInfo">The generic property info.</param>
    /// <returns></returns>
    public T GetInputValue<T>(IPropertyInfo propertyInfo)
    {
      return (T)InputPropertyValues[propertyInfo];
    }

    /// <summary>
    /// Tries to get the value. Use this method on LazyLoaded properties to test if value has been provided or not.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="propertyInfo">The generic property info.</param>
    /// <param name="value">The value.</param>
    /// <returns>true if value exists else false</returns>
    public bool TryGetInputValue<T>(PropertyInfo<T> propertyInfo, ref T value)
    {
      if (!InputPropertyValues.ContainsKey(propertyInfo))
      {
        value = default(T);
        return false;
      }

      value = (T)InputPropertyValues[propertyInfo];
      return true;
    }

    /// <summary>
    /// Tries to get the value with explicit cast. Use this method on LazyLoaded properties to test if value has been provided or not.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="propertyInfo">The generic property info.</param>
    /// <param name="value">The value.</param>
    /// <returns>true if value exists else false</returns>
    public bool TryGetInputValue<T>(IPropertyInfo propertyInfo, ref T value)
    {
      if (!InputPropertyValues.ContainsKey(propertyInfo))
      {
        value = default(T);
        return false;
      }

      value = (T)InputPropertyValues[propertyInfo];
      return true;
    }
  }
}