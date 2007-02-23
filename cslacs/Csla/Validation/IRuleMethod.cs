using System;

namespace Csla.Validation
{
  /// <summary>
  /// Tracks all information for a rule.
  /// </summary>
  internal interface IRuleMethod
  {
    /// <summary>
    /// Gets the priority of the rule method.
    /// </summary>
    /// <value>The priority value.</value>
    /// <remarks>
    /// Priorities are processed in descending
    /// order, so priority 0 is processed
    /// before priority 1, etc.</remarks>
    int Priority { get;}

    /// <summary>
    /// Gets the name of the rule.
    /// </summary>
    /// <remarks>
    /// The rule's name must be unique and is used
    /// to identify a broken rule in the BrokenRules
    /// collection.
    /// </remarks>
    string RuleName { get;}

    /// <summary>
    /// Returns the name of the field, property or column
    /// to which the rule applies.
    /// </summary>
    RuleArgs RuleArgs { get;}

    /// <summary>
    /// Invokes the rule to validate the data.
    /// </summary>
    /// <returns>
    /// <see langword="true" /> if the data is valid, 
    /// <see langword="false" /> if the data is invalid.
    /// </returns>
    bool Invoke(object target);
  }
}
