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
    private static Dictionary<Type, ValidationRulesManager> _managers = 
      new Dictionary<Type, ValidationRulesManager>();

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
      if (!_managers.TryGetValue(objectType, out result) && create)
      {
        lock (_managers)
        {
          if (!_managers.TryGetValue(objectType, out result))
          {
            result = new ValidationRulesManager();
            _managers.Add(objectType, result);
          }
        }
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

    internal static void RemoveManager(Type type)
    {
      if (_managers.ContainsKey(type))
        _managers.Remove(type);
    }
  }
}
