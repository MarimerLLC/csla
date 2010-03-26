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
    /// Gets the rule definition object that reflects the 
    /// association between the rule and the business object
    /// property.
    /// </summary>
    public RuleMethod RuleDefinition { get; internal set; }
    /// <summary>
    /// Gets a reference to the target business object.
    /// </summary>
    public object Target { get; internal set; }
    /// <summary>
    /// Gets the primary property to which this rule is attached
    /// (null for async rules).
    /// </summary>
    public Csla.Core.IPropertyInfo PrimaryProperty { get; internal set; }
    /// <summary>
    /// Gets a dictionary containing copies of property values from
    /// the target business object (for use in all async rule implementations).
    /// </summary>
    public Dictionary<Csla.Core.IPropertyInfo, object> InputPropertyValues { get; internal set; }
    /// <summary>
    /// Gets a reference to the list of results being returned
    /// by this rule.
    /// </summary>
    public List<RuleResult> Results { get; internal set; }

    private Action<RuleContext> _completeHandler;

    internal RuleContext(Action<RuleContext> completeHandler)
    {
      _completeHandler = completeHandler;
    }

    /// <summary>
    /// Add a Error severity result to the Results list.
    /// </summary>
    /// <param name="description">Human-readable description of
    /// why the rule failed.</param>
    public void AddErrorResult(string description)
    {
      Results.Add(new RuleResult(RuleDefinition.Rule.ToString(), PrimaryProperty, description));
    }

    /// <summary>
    /// Add a Warning severity result to the Results list.
    /// </summary>
    /// <param name="description">Human-readable description of
    /// why the rule failed.</param>
    public void AddWarningResult(string description)
    {
      Results.Add(new RuleResult(RuleDefinition.Rule.ToString(), PrimaryProperty, description) { Severity = RuleSeverity.Warning });
    }

    /// <summary>
    /// Add an Information severity result to the Results list.
    /// </summary>
    /// <param name="description">Human-readable description of
    /// why the rule failed.</param>
    public void AddInformationResult(string description)
    {
      Results.Add(new RuleResult(RuleDefinition.Rule.ToString(), PrimaryProperty, description) { Severity = RuleSeverity.Information });
    }

    /// <summary>
    /// Indicates that the rule processing is complete, so
    /// CSLA .NET knows to process the Results list.
    /// </summary>
    public void Complete()
    {
      _completeHandler(this);
    }
  }
}
