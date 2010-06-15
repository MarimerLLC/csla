//-----------------------------------------------------------------------
// <copyright file="RuleContext.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Context information provided to a business rule</summary>
//-----------------------------------------------------------------------
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Rules
{
  /// <summary>
  /// Context information provided to a business rule
  /// when it is invoked.
  /// </summary>
  public class RuleContext
  {
    /// <summary>
    /// Gets the rule object.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public IBusinessRule Rule { get; internal set; }
    /// <summary>
    /// Gets a reference to the target business object.
    /// </summary>
    public object Target { get; internal set; }
    /// <summary>
    /// Gets a dictionary containing copies of property values from
    /// the target business object.
    /// </summary>
    public Dictionary<Csla.Core.IPropertyInfo, object> InputPropertyValues { get; internal set; }
    /// <summary>
    /// Gets a dictionary containing copies of property values that
    /// should be updated in the target object.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Dictionary<Csla.Core.IPropertyInfo, object> OutputPropertyValues { get; private set; }
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

    private Action<RuleContext> _completeHandler;

    internal RuleContext(Action<RuleContext> completeHandler)
    {
      _completeHandler = completeHandler;
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
    public RuleContext GetChainedContext(IBusinessRule rule)
    {
      var result = new RuleContext(_completeHandler);
      result.Rule = rule;
      result.Target = Target;
      result.InputPropertyValues = InputPropertyValues;
      result.Results = Results;
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
    /// </summary>
    /// <param name="property">Property to which the result applies.</param>
    /// <param name="description">Human-readable description of
    /// why the rule failed.</param>
    public void AddErrorResult(Csla.Core.IPropertyInfo property, string description)
    {
      if (!Rule.AffectedProperties.Contains(property))
        throw new ArgumentOutOfRangeException(property.Name);
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
    /// </summary>
    /// <param name="property">Property to which the result applies.</param>
    /// <param name="description">Human-readable description of
    /// why the rule failed.</param>
    public void AddWarningResult(Csla.Core.IPropertyInfo property, string description)
    {
      if (!Rule.AffectedProperties.Contains(property))
        throw new ArgumentOutOfRangeException(property.Name);
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
    /// <param name="description">Human-readable description of
    /// why the rule failed.</param>
    /// <param name="stopProcessing">True if no further rules should be processed
    /// for the current property.</param>
    public void AddInformationResult(string description, bool stopProcessing)
    {
      Results.Add(new RuleResult(Rule.RuleName, Rule.PrimaryProperty, description) { Severity = RuleSeverity.Information, StopProcessing = stopProcessing });
    }

    /// <summary>
    /// Add an Information severity result to the Results list.
    /// </summary>
    /// <param name="property">Property to which the result applies.</param>
    /// <param name="description">Human-readable description of
    /// why the rule failed.</param>
    public void AddInformationResult(Csla.Core.IPropertyInfo property, string description)
    {
      if (!Rule.AffectedProperties.Contains(property))
        throw new ArgumentOutOfRangeException(property.Name);
      Results.Add(new RuleResult(Rule.RuleName, property, description) { Severity = RuleSeverity.Information });
    }

    /// <summary>
    /// Add a Success severity result to the Results list.
    /// </summary>
    /// <param name="stopProcessing">True if no further rules should be processed
    /// for the current property.</param>
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
      if (OutputPropertyValues == null)
        OutputPropertyValues = new Dictionary<Core.IPropertyInfo, object>();
      OutputPropertyValues.Add(Rule.PrimaryProperty, value);
    }

    /// <summary>
    /// Add an outbound value to update a property on the business
    /// object once the rule is complete.
    /// </summary>
    /// <param name="property">Property to update.</param>
    /// <param name="value">New property value.</param>
    public void AddOutValue(Csla.Core.IPropertyInfo property, object value)
    {
      if (!Rule.AffectedProperties.Contains(property))
        throw new ArgumentOutOfRangeException(property.Name);
      if (OutputPropertyValues == null)
        OutputPropertyValues = new Dictionary<Core.IPropertyInfo, object>();
      OutputPropertyValues.Add(property, value);
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
      _completeHandler(this);
    }
  }
}