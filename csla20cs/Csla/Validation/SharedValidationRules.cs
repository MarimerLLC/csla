using System;
using System.Collections.Generic;

namespace Csla.Validation
{
  /// <summary>
  /// Maintains a list of all the per-type
  /// <see cref="ValidationRulesManager"/> objects
  /// loaded in memory.
  /// </summary>
  internal static class SharedValidationRules
  {
    private static Dictionary<Type, ValidationRulesManager> _managers = new Dictionary<Type, ValidationRulesManager>();

    /// <summary>
    /// Gets the <see cref="ValidationRulesManager"/> for the 
    /// specified object type, optionally creating a new instance 
    /// of the object if necessary.
    /// </summary>
    /// <param name="objectType">
    /// Type of business object for which the rules apply.
    /// </param>
    /// <param name="create">Indicates whether to create
    /// a new instance of the object if one doesn't exist.</param>
    internal static ValidationRulesManager GetManager(Type objectType, bool create)
    {
      ValidationRulesManager result = null;
      if (_managers.ContainsKey(objectType))
        result = _managers[objectType];
      else if (create)
      {
        result = new ValidationRulesManager();
        _managers.Add(objectType, result);
      }
      return result;
    }

    /// <summary>
    /// Gets a value indicating whether a set of rules
    /// have been created for a given <see cref="Type" />.
    /// </summary>
    /// <param name="objectType">
    /// Type of business object for which the rules apply.
    /// </param>
    /// <returns><see langword="true" /> if rules exist for the type.</returns>
    public static bool RulesExistFor(Type objectType)
    {
      return _managers.ContainsKey(objectType);
    }

    /// <summary>
    /// Adds a rule to the list of rules to be enforced.
    /// </summary>
    /// <remarks>
    /// A rule is implemented by a method which conforms to the 
    /// method signature defined by the RuleHandler delegate.
    /// </remarks>
    /// <param name="handler">The method that implements the rule.</param>
    /// <param name="objectType">
    /// Type of business object for which the rule applies.
    /// </param>
    /// <param name="args">
    /// A RuleArgs object specifying the property name and other arguments
    /// passed to the rule method
    /// </param>
    public static void AddRule(RuleHandler handler, Type objectType, RuleArgs args)
    {
      GetManager(objectType, true).AddRule(handler, args, 0);
    }
  }
}
