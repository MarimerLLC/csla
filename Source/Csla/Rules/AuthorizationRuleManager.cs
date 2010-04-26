using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Csla.Rules
{
  /// <summary>
  /// Manages the list of authorization 
  /// rules for a business type.
  /// </summary>
  public class AuthorizationRuleManager
  {
#if !SILVERLIGHT
    private static Lazy<System.Collections.Concurrent.ConcurrentDictionary<RuleSetKey, AuthorizationRuleManager>> _perTypeRules =
      new Lazy<System.Collections.Concurrent.ConcurrentDictionary<RuleSetKey, AuthorizationRuleManager>>();

    internal static AuthorizationRuleManager GetRulesForType(Type type, string ruleSet)
    {
      var key = new RuleSetKey { Type = type, RuleSet = ruleSet };
      var result = _perTypeRules.Value.GetOrAdd(key, (t) => { return new AuthorizationRuleManager(); });
      InitializePerTypeRules(result, type);
      return result;
    }
#else
    private static Dictionary<RuleSetKey, AuthorizationRuleManager> _perTypeRules = new Dictionary<RuleSetKey, AuthorizationRuleManager>();

    internal static AuthorizationRuleManager GetRulesForType(Type type, string ruleSet)
    {
      AuthorizationRuleManager result = null;
      var key = new RuleSetKey { Type = type, RuleSet = ruleSet };
      if (!_perTypeRules.TryGetValue(key, out result))
      {
        lock (_perTypeRules)
        {
          if (!_perTypeRules.TryGetValue(key, out result))
          {
            result = new AuthorizationRuleManager();
            _perTypeRules.Add(key, result);
          }
        }
      }
      InitializePerTypeRules(result, type);
      return result;
    }
#endif

    internal static AuthorizationRuleManager GetRulesForType(Type type)
    {
      return GetRulesForType(type, null);
    }

    private static void InitializePerTypeRules(AuthorizationRuleManager mgr, Type type)
    {
      if (!mgr.InitializedPerType)
        lock (mgr)
          if (!mgr.InitializedPerType)
          {
            mgr.InitializedPerType = true;
            // invoke method to add auth roles
            var flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
            System.Reflection.MethodInfo method = type.GetMethod("AddObjectAuthorizationRules", flags);
            if (method != null)
              method.Invoke(null, null);
          }
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
    public List<IAuthorizationRule> Rules { get; private set; }

    /// <summary>
    /// Gets or sets a value indicating whether the rules have been
    /// initialized.
    /// </summary>
    public bool Initialized { get; internal set; }
    /// <summary>
    /// Gets or sets a value indicating whether the rules have been
    /// initialized.
    /// </summary>
    public bool InitializedPerType { get; internal set; }

    private AuthorizationRuleManager()
    {
      Rules = new List<IAuthorizationRule>();
    }
  }
}
