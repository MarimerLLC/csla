//-----------------------------------------------------------------------
// <copyright file="RuleResult.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Contains information about the result of a rule.</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Properties;

namespace Csla.Rules
{
  /// <summary>
  /// Contains information about the result of a rule.
  /// </summary>
  public class RuleResult
  {
    /// <summary>
    /// Gets the unique name of the rule that created
    /// this result.
    /// </summary>
    public string RuleName { get; private set; }
    /// <summary>
    /// Gets a value indicating whether the 
    /// rule was successful.
    /// </summary>
    public bool Success { get; private set; }
    /// <summary>
    /// Gets a human-readable description of 
    /// why the rule failed.
    /// </summary>
    public string Description { get; private set; }
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
    /// Gets the primary property for this result.
    /// </summary>
    public Csla.Core.IPropertyInfo PrimaryProperty { get; private set; }
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
    /// <param name="ruleName">Unique name of the rule creating
    /// this result.</param>
    /// <param name="property">Property to which this result should
    /// be attached.</param>
    public RuleResult(string ruleName, Core.IPropertyInfo property)
    {
      RuleName = ruleName;
      PrimaryProperty = property;
      Success = true;
      Severity = RuleSeverity.Success;
    }

    /// <summary>
    /// Creates a failure result.
    /// </summary>
    /// <param name="ruleName">Unique name of the rule creating
    /// this result.</param>
    /// <param name="property">Property to which this result should
    /// be attached.</param>
    /// <param name="description">Human-readable description of
    /// why the rule failed.</param>
    public RuleResult(string ruleName, Core.IPropertyInfo property, string description)
    {

      if (string.IsNullOrEmpty(description))
        throw new ArgumentException(string.Format(Resources.RuleMessageRequired, ruleName), "description");

      RuleName = ruleName;
      PrimaryProperty = property;
      Description = description;
      Success = string.IsNullOrEmpty(description);
      Severity = RuleSeverity.Error;
    }
  }
}