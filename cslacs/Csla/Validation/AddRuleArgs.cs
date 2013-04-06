using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Validation
{
  /// <summary>
  /// Contains information about a
  /// data annotation business rule.
  /// </summary>
  public class AddRuleArgs : EventArgs
  {
    /// <summary>
    /// Reference to the business object.
    /// </summary>
    public object BusinessObject { get; set; }
    /// <summary>
    /// Data annotation validation attribute.
    /// </summary>
    public object Attribute { get; set; }
    /// <summary>
    /// Property to which the rule is attached.
    /// </summary>
    public System.Reflection.PropertyInfo PropertyInfo { get; set; }
    /// <summary>
    /// Flag indicating whether the rule has been added.
    /// </summary>
    public bool RuleAdded { get; set; }
  }
}
