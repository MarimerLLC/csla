using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Rules
{
  /// <summary>
  /// Manages the list of rules for a business type.
  /// </summary>
  public class BusinessRuleManager
  {
    #region Per Type Rules
#if !SILVERLIGHT
    private static System.Collections.Concurrent.ConcurrentDictionary<Type, BusinessRuleManager> _perTypeRules = 
      new System.Collections.Concurrent.ConcurrentDictionary<Type, BusinessRuleManager>();

    internal static BusinessRuleManager GetRulesForType(Type type)
    {
      return _perTypeRules.GetOrAdd(type, (t) => { return new BusinessRuleManager(); });
    }
#else
    private static Dictionary<Type, BusinessRuleManager> _perTypeRules = new Dictionary<Type, BusinessRuleManager>();

    internal static BusinessRuleManager GetRulesForType(Type type)
    {
      BusinessRuleManager result = null;
      if (!_perTypeRules.TryGetValue(type, out result))
      {
        lock (_perTypeRules)
        {
          if (!_perTypeRules.TryGetValue(type, out result))
          {
            result = new BusinessRuleManager();
            _perTypeRules.Add(type, result);
          }
        }
      }
      return result;
    }

#endif
    #endregion

    private BusinessRuleManager()
    {
      RuleMethods = new List<RuleMethod>();
    }

    /// <summary>
    /// Gets the list of RuleMethod objects for the business type.
    /// </summary>
    public List<RuleMethod> RuleMethods { get; private set; }

    /// <summary>
    /// Invokes all rules for the business type.
    /// </summary>
    /// <returns>
    /// Returns a list of property names affected by the invoked rules.
    /// The PropertyChanged event should be raised for each affected
    /// property.
    /// </returns>
    public List<string> CheckRules()
    {
      var result = new List<string>();
      return result;
    }

    /// <summary>
    /// Invokes all rules attached at the class level
    /// of the business type.
    /// </summary>
    /// <returns>
    /// Returns a list of property names affected by the invoked rules.
    /// The PropertyChanged event should be raised for each affected
    /// property.
    /// </returns>
    public List<string> CheckObjectRules()
    {
      var result = new List<string>();
      return result;
    }

    /// <summary>
    /// Invokes all rules for a specific property of the business type.
    /// </summary>
    /// <param name="property">Property to check.</param>
    /// <returns>
    /// Returns a list of property names affected by the invoked rules.
    /// The PropertyChanged event should be raised for each affected
    /// property.
    /// </returns>
    public List<string> CheckRules(Csla.Core.IPropertyInfo property)
    {
      var result = new List<string>();
      return result;
    }
  }
}
