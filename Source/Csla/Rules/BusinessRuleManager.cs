//-----------------------------------------------------------------------
// <copyright file="BusinessRuleManager.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Manages the list of rules for a business type.</summary>
//-----------------------------------------------------------------------
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
#if !SILVERLIGHT
    private static Lazy<System.Collections.Concurrent.ConcurrentDictionary<RuleSetKey, BusinessRuleManager>> _perTypeRules =
      new Lazy<System.Collections.Concurrent.ConcurrentDictionary<RuleSetKey, BusinessRuleManager>>();

    internal static BusinessRuleManager GetRulesForType(Type type, string ruleSet)
    {
      if (ruleSet == ApplicationContext.DefaultRuleSet) ruleSet = null;

      var key = new RuleSetKey { Type = type, RuleSet = ruleSet };
      return _perTypeRules.Value.GetOrAdd(key, (t) => { return new BusinessRuleManager(); });
    }
#else
    private static Dictionary<RuleSetKey, BusinessRuleManager> _perTypeRules = new Dictionary<RuleSetKey, BusinessRuleManager>();

    internal static BusinessRuleManager GetRulesForType(Type type, string ruleSet)
    {
      if (ruleSet == ApplicationContext.DefaultRuleSet) ruleSet = null;

      BusinessRuleManager result = null;
      var key = new RuleSetKey { Type = type, RuleSet = ruleSet };
      if (!_perTypeRules.TryGetValue(key, out result))
      {
        lock (_perTypeRules)
        {
          if (!_perTypeRules.TryGetValue(key, out result))
          {
            result = new BusinessRuleManager();
            _perTypeRules.Add(key, result);
          }
        }
      }
      return result;
    }

#endif

    internal static BusinessRuleManager GetRulesForType(Type type)
    {
      return GetRulesForType(type, null);

    }

    private class RuleSetKey
    {
      public Type Type { get; set; }
      public string RuleSet { get; set; }

      public override bool Equals(object obj)
      {
        var other = obj as RuleSetKey;
        if (other == null)
          return false;
        else
          return this.Type.Equals(other.Type) && RuleSet == other.RuleSet;
      }

      public override int GetHashCode()
      {
        return (this.Type.FullName + RuleSet).GetHashCode();
      }
    }

    /// <summary>
    /// Gets the list of rule objects for the business type.
    /// </summary>
    public List<IBusinessRule> Rules { get; private set; }

    /// <summary>
    /// Gets or sets a value indicating whether the rules have been
    /// initialized.
    /// </summary>
    public bool Initialized { get; set; }

    private BusinessRuleManager()
    {
      Rules = new List<IBusinessRule>();
    }
  }
}