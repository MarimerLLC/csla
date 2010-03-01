using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Rules
{
  /// <summary>
  /// Contains information about the result of a rule.
  /// </summary>
  public class RuleResult
  {
    /// <summary>
    /// Gets or sets a value indicating whether the 
    /// rule was successful.
    /// </summary>
    public bool Success { get; set; }
    /// <summary>
    /// Gets or sets a human-readable description of 
    /// why the rule failed.
    /// </summary>
    public string Description { get; set; }
    /// <summary>
    /// Gets or sets the severity of a failed rule.
    /// </summary>
    public RuleSeverity Severity { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether rule 
    /// processing should immediately stop
    /// (applies to sync rules only).
    /// </summary>
    public bool StopProcessing { get; set; }
    /// <summary>
    /// Gets or sets a list of properties that were affected
    /// by the rule, so appropriate PropertyChanged events
    /// can be raised for UI notification.
    /// </summary>
    public List<Core.IPropertyInfo> Properties { get; set; }
    /// <summary>
    /// Gets or sets a dictionary of new property values used
    /// to update the business object's properties after
    /// the rule is complete.
    /// </summary>
    public Dictionary<Core.IPropertyInfo, object> OutputPropertyValues { get; set; }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <param name="property">Property to which this result should
    /// be attached.</param>
    public RuleResult(Core.IPropertyInfo property)
    {
      Properties = new List<Core.IPropertyInfo>() { property };
      Success = true;
    }

    /// <summary>
    /// Creates a failure result.
    /// </summary>
    /// <param name="property">Property to which this result should
    /// be attached.</param>
    /// <param name="description">Human-readable description of
    /// why the rule failed.</param>
    public RuleResult(Core.IPropertyInfo property, string description)
    {
      Properties = new List<Core.IPropertyInfo>() { property };
      Description = description;
      Success = string.IsNullOrEmpty(description);
      if (!Success)
        Severity = RuleSeverity.Error;
    }
  }
}
