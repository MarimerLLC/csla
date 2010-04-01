using System;
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
    public Dictionary<Csla.Core.IPropertyInfo, object> OutputPropertyValues { get; set; }
    /// <summary>
    /// Gets a reference to the list of results being returned
    /// by this rule.
    /// </summary>
    public List<RuleResult> Results { get; private set; }

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
      if (Results == null)
        Results = new List<RuleResult>();
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
      if (Results == null)
        Results = new List<RuleResult>();
      Results.Add(new RuleResult(Rule.RuleName, Rule.PrimaryProperty, description));
    }

    /// <summary>
    /// Add a Error severity result to the Results list.
    /// </summary>
    /// <param name="property">Property to which the result applies.</param>
    /// <param name="description">Human-readable description of
    /// why the rule failed.</param>
    public void AddErrorResult(Csla.Core.IPropertyInfo property, string description)
    {
      if (Results == null)
        Results = new List<RuleResult>();
      Results.Add(new RuleResult(Rule.RuleName, property, description));
    }

    /// <summary>
    /// Add a Warning severity result to the Results list.
    /// </summary>
    /// <param name="description">Human-readable description of
    /// why the rule failed.</param>
    public void AddWarningResult(string description)
    {
      if (Results == null)
        Results = new List<RuleResult>();
      Results.Add(new RuleResult(Rule.RuleName, Rule.PrimaryProperty, description) { Severity = RuleSeverity.Warning });
    }

    /// <summary>
    /// Add a Warning severity result to the Results list.
    /// </summary>
    /// <param name="property">Property to which the result applies.</param>
    /// <param name="description">Human-readable description of
    /// why the rule failed.</param>
    public void AddWarningResult(Csla.Core.IPropertyInfo property, string description)
    {
      if (Results == null)
        Results = new List<RuleResult>();
      Results.Add(new RuleResult(Rule.RuleName, property, description) { Severity = RuleSeverity.Warning });
    }

    /// <summary>
    /// Add an Information severity result to the Results list.
    /// </summary>
    /// <param name="description">Human-readable description of
    /// why the rule failed.</param>
    public void AddInformationResult(string description)
    {
      if (Results == null)
        Results = new List<RuleResult>();
      Results.Add(new RuleResult(Rule.RuleName, Rule.PrimaryProperty, description) { Severity = RuleSeverity.Information });
    }

    /// <summary>
    /// Add an Information severity result to the Results list.
    /// </summary>
    /// <param name="property">Property to which the result applies.</param>
    /// <param name="description">Human-readable description of
    /// why the rule failed.</param>
    public void AddInformationResult(Csla.Core.IPropertyInfo property, string description)
    {
      if (Results == null)
        Results = new List<RuleResult>();
      Results.Add(new RuleResult(Rule.RuleName, property, description) { Severity = RuleSeverity.Information });
    }

    /// <summary>
    /// Indicates that the rule processing is complete, so
    /// CSLA .NET will process the Results list. This method
    /// must be invoked on the UI thread.
    /// </summary>
    public void Complete()
    {
      if (Results == null)
      {
        Results = new List<RuleResult>();
        Results.Add(new RuleResult(Rule.RuleName, Rule.PrimaryProperty));
      }
      _completeHandler(this);
    }
  }
}
