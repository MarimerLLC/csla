//-----------------------------------------------------------------------
// <copyright file="BusinessRuleManager.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Manages the list of rules for a business type.</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;

namespace Csla.Rules
{
  /// <summary>
  /// Manages the list of rules for a business type.
  /// </summary>
  public class BusinessRuleManager
  {
    private static Lazy<System.Collections.Concurrent.ConcurrentDictionary<RuleSetKey, BusinessRuleManager>> _perTypeRules =
      new Lazy<System.Collections.Concurrent.ConcurrentDictionary<RuleSetKey, BusinessRuleManager>>();

    internal static BusinessRuleManager GetRulesForType(Type type, string ruleSet)
    {
      if (ruleSet == ApplicationContext.DefaultRuleSet) ruleSet = null;

      var key = new RuleSetKey { Type = type, RuleSet = ruleSet };
      return _perTypeRules.Value.GetOrAdd(key, (t) => { return new BusinessRuleManager(); });
    }

    /// <summary>
    /// Remove/delete all the rules for the given type.
    /// </summary>
    /// <param name="type">The type.</param>
    internal static void CleanupRulesForType(Type type)
    {
      lock (_perTypeRules)
      {
        // the first RuleSet is already added to list when this check is executed so so if count > 1 then we have already initialized type rules.
        var typeRules = _perTypeRules.Value.Where(value => value.Key.Type == type);
        foreach (var key in typeRules)
          _perTypeRules.Value.TryRemove(key.Key, out BusinessRuleManager manager);
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
        if (!(obj is RuleSetKey other))
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
    public List<IBusinessRuleBase> Rules { get; private set; }

    /// <summary>
    /// Gets or sets a value indicating whether the rules have been
    /// initialized.
    /// </summary>
    public bool Initialized { get; set; }

    private BusinessRuleManager()
    {
      Rules = new List<IBusinessRuleBase>();
    }
  }
}