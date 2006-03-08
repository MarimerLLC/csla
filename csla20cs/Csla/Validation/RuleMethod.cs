using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Validation
{
  /// <summary>
  /// Tracks all information for a rule.
  /// </summary>
  internal class RuleMethod
  {
    private object _target;
    private RuleHandler _handler;
    private string _ruleName = String.Empty;
    private RuleArgs _args;

    /// <summary>
    /// Returns the name of the method implementing the rule
    /// and the property, field or column name to which the
    /// rule applies.
    /// </summary>
    public override string ToString()
    {
      return _ruleName;
    }

    /// <summary>
    /// Gets the name of the rule.
    /// </summary>
    /// <remarks>
    /// The rule's name must be unique and is used
    /// to identify a broken rule in the BrokenRules
    /// collection.
    /// </remarks>
    public string RuleName
    {
      get { return _ruleName; }
    }

    /// <summary>
    /// Returns the name of the field, property or column
    /// to which the rule applies.
    /// </summary>
    public RuleArgs RuleArgs
    {
      get { return _args; }
    }

    /// <summary>
    /// Creates and initializes the rule.
    /// </summary>
    /// <param name="target">Reference to the object containing the data to validate.</param>
    /// <param name="handler">The address of the method implementing the rule.</param>
    /// <param name="propertyName">The field, property or column to which the rule applies.</param>
    public RuleMethod(object target, RuleHandler handler, string propertyName)
    {
      _target = target;
      _handler = handler;
      _args = new RuleArgs(propertyName);
      _ruleName = _handler.Method.Name + "!" + _args.ToString();
    }

    /// <summary>
    /// Creates and initializes the rule.
    /// </summary>
    /// <param name="target">Reference to the object containing the data to validate.</param>
    /// <param name="handler">The address of the method implementing the rule.</param>
    /// <param name="args">A RuleArgs object.</param>
    public RuleMethod(object target, RuleHandler handler, RuleArgs args)
    {
      _target = target;
      _handler = handler;
      _args = args;
      _ruleName = _handler.Method.Name + "!" + _args.ToString();
    }

    /// <summary>
    /// Invokes the rule to validate the data.
    /// </summary>
    /// <returns>True if the data is valid, False if the data is invalid.</returns>
    public bool Invoke()
    {
      return _handler.Invoke(_target, _args);
    }
  }
}
