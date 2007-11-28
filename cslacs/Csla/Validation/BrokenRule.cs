using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Validation
{
  /// <summary>
  /// Stores details about a specific broken business rule.
  /// </summary>
  [Serializable()]
  public class BrokenRule
  {
    private string _ruleName;
    private string _description;
    private string _property;
    private RuleSeverity _severity;

    internal BrokenRule(IRuleMethod rule)
    {
      _ruleName = rule.RuleName;
      _description = rule.RuleArgs.Description;
      _property = rule.RuleArgs.PropertyName;
      _severity = rule.RuleArgs.Severity;
    }

    internal BrokenRule(string source, BrokenRule rule)
    {
      _ruleName = string.Format("rule://{0}.{1}", source, rule.RuleName.Replace("rule://", string.Empty));
      _description = rule.Description;
      _property = rule.Property;
      _severity = rule.Severity;
    }

    /// <summary>
    /// Provides access to the name of the broken rule.
    /// </summary>
    /// <value>The name of the rule.</value>
    public string RuleName
    {
      get { return _ruleName; }
    }

    /// <summary>
    /// Provides access to the description of the broken rule.
    /// </summary>
    /// <value>The description of the rule.</value>
    public string Description
    {
      get { return _description; }
    }

    /// <summary>
    /// Provides access to the property affected by the broken rule.
    /// </summary>
    /// <value>The property affected by the rule.</value>
    public string Property
    {
      get { return _property; }
    }

    /// <summary>
    /// Gets the severity of the broken rule.
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
    public RuleSeverity Severity
    {
      get { return _severity; }
    }
  }
}