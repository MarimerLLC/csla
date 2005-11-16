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

    internal BrokenRule(ValidationRules.RuleMethod rule)
    {
      _ruleName = rule.RuleName;
      _description = rule.RuleArgs.Description;
      _property = rule.RuleArgs.PropertyName;
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
  }
}