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
#if !NETFX_CORE
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
      var found = false;
      try
      {
        found = _perTypeRules.TryGetValue(key, out result);
      }
      catch
      { /* failure will drop into !found block */ }
      if (!found)
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


    /// <summary>
    /// Remove/delete all the rules for the given type.
    /// </summary>
    /// <param name="type">The type.</param>
    internal static void CleanupRulesForType(Type type)
    {
      lock (_perTypeRules)
      {

        // the first RuleSet is already added to list when this check is executed so so if count > 1 then we have already initialized type rules.
#if !NETFX_CORE
        var typeRules = _perTypeRules.Value.Where(value => value.Key.Type == type);
        foreach (var key in typeRules)
        {
          BusinessRuleManager manager;
          _perTypeRules.Value.TryRemove(key.Key, out manager);
        }
#else
        var typeRules = _perTypeRules.Where(value => value.Key.Type == type).ToArray();
        foreach (var key in typeRules)
        {
          _perTypeRules.Remove(key.Key);
        }
#endif
      }
    }

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