using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Rules
{
  /// <summary>
  /// Establishes a relationship between a business/validation
  /// rule and a specific property of a business class.
  /// </summary>
  public class RuleMethod
  {
    /// <summary>
    /// Gets the rule object.
    /// </summary>
    public IBusinessRule Rule { get; private set; }
    /// <summary>
    /// Gets a unique identifier for the specific instance
    /// of the rule within the context of the business object
    /// where the rule is used.
    /// </summary>
    public string RuleName { get; private set; }
    /// <summary>
    /// Gets the primary property to which this rule is attached.
    /// </summary>
    public Csla.Core.IPropertyInfo PrimaryProperty { get; private set; }
    /// <summary>
    /// Gets the rule priority.
    /// </summary>
    public int Priority { get; private set; }

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="rule">Instance of rule.</param>
    /// <param name="property">Property to which the rule is attached.</param>
    /// <param name="priority">Rule priority.</param>
    internal RuleMethod(IBusinessRule rule, Core.IPropertyInfo property, int priority)
    {
      Rule = rule;
      PrimaryProperty = property;
      Priority = priority;
      RuleName = new Rules.RuleUri(Rule, PrimaryProperty).ToString();
    }
  }
}
