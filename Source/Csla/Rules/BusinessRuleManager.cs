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

    /// <summary>
    /// Gets the list of rule objects for the business type.
    /// </summary>
    public List<IBusinessRule> RuleMethods { get; private set; }

    private BusinessRuleManager()
    {
      RuleMethods = new List<IBusinessRule>();
    }
  }
}
