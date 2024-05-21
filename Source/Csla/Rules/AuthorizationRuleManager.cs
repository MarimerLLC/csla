﻿//-----------------------------------------------------------------------
// <copyright file="AuthorizationRuleManager.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Manages the list of authorization </summary>
//-----------------------------------------------------------------------

using System.Reflection;
#if NET5_0_OR_GREATER
using System.Runtime.Loader;

using Csla.Runtime;
#endif

namespace Csla.Rules
{
  /// <summary>
  /// Manages the list of authorization 
  /// rules for a business type.
  /// </summary>
  public class AuthorizationRuleManager
  {
#if NET5_0_OR_GREATER
    private static Lazy<System.Collections.Concurrent.ConcurrentDictionary<RuleSetKey, Tuple<string, AuthorizationRuleManager>>> _perTypeRules =
      new Lazy<System.Collections.Concurrent.ConcurrentDictionary<RuleSetKey, Tuple<string, AuthorizationRuleManager>>>();
#else
    private static Lazy<System.Collections.Concurrent.ConcurrentDictionary<RuleSetKey, AuthorizationRuleManager>> _perTypeRules =
      new Lazy<System.Collections.Concurrent.ConcurrentDictionary<RuleSetKey, AuthorizationRuleManager>>();
#endif

    internal static AuthorizationRuleManager GetRulesForType(ApplicationContext applicationContext, Type type, string ruleSet)
    {
      if (ruleSet == ApplicationContext.DefaultRuleSet)
        ruleSet = null;

      var key = new RuleSetKey { Type = type, RuleSet = ruleSet };

#if NET5_0_OR_GREATER
      var rulesInfo = _perTypeRules.Value
        .GetOrAdd(
          key,
          _ => AssemblyLoadContextManager.CreateCacheInstance(type, new AuthorizationRuleManager(), OnAssemblyLoadContextUnload)
        );

      var result = rulesInfo.Item2;
#else
      var result = _perTypeRules.Value.GetOrAdd(key, _ => { return new AuthorizationRuleManager(); });
#endif

      InitializePerTypeRules(applicationContext, result, type);

      return result;
    }

    private bool InitializingPerType { get; set; }

    private static void InitializePerTypeRules(ApplicationContext applicationContext, AuthorizationRuleManager mgr, Type type)
    {
      if (!mgr.InitializedPerType)
        lock (mgr)
          if (!mgr.InitializedPerType && !mgr.InitializingPerType)
          {
            // Only call AddObjectAuthorizationRules when there are no rules for this type
            if (RulesExistForType(type))
            {
              mgr.InitializedPerType = true;
              return;
            }

            try
            {
              mgr.InitializingPerType = true;

              // invoke method to add auth roles
              System.Reflection.MethodInfo method = FindObjectAuthorizationRulesMethod(type);
              if (method != null)
              {
                if (method.GetParameters().Length == 0)
                  method.Invoke(null, null);
                else if (applicationContext != null)
                  method.Invoke(null, [new AddObjectAuthorizationRulesContext(applicationContext)]);
                else
                  throw new InvalidOperationException(
                    $"{nameof(InitializePerTypeRules)} {nameof(applicationContext)} == null");
              }
              mgr.InitializedPerType = true;
            }
            catch (Exception)
            {
              // remove all loaded rules for this type
              CleanupRulesForType(type);
              throw;  // and rethrow the exception
            }
            finally
            {
              mgr.InitializingPerType = false;
            }
          }
    }

    private static System.Reflection.MethodInfo FindObjectAuthorizationRulesMethod(Type type)
    {
      var method = type.GetMethods().FirstOrDefault(
        m => m.IsStatic &&
             m.CustomAttributes.Any(
               a => a.AttributeType == typeof(ObjectAuthorizationRulesAttribute)));
      if (method == null)
      {
        const BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
        method = type.GetMethod("AddObjectAuthorizationRules", flags);
      }
      return method;
    }

    private static bool RulesExistForType(Type type)
    {
      lock (_perTypeRules)
      {
        // the first RuleSet is already added to list when this check is executed so so if count > 1 then we have already initialized type rules.
        return _perTypeRules.Value.Count(value => value.Key.Type == type) > 1;
      }

    }

    private static void CleanupRulesForType(Type type)
    {
      lock (_perTypeRules)
      {

        // the first RuleSet is already added to list when this check is executed so so if count > 1 then we have already initialized type rules.
        var typeRules = _perTypeRules.Value.Where(value => value.Key.Type == type);
        foreach (var key in typeRules)
        {
          _perTypeRules.Value.TryRemove(key.Key, out _);
        }
      }
    }

    private class RuleSetKey
    {
      public Type Type { get; set; }
      public string RuleSet { get; set; }

      public override bool Equals(object obj)
      {
        if (obj is not RuleSetKey other)
          return false;
        else
          return Type.Equals(other.Type) && RuleSet == other.RuleSet;
      }

      public override int GetHashCode()
      {
        return (Type.FullName + RuleSet).GetHashCode();
      }
    }

    /// <summary>
    /// Gets the list of rule objects for the business type.
    /// </summary>
    public List<IAuthorizationRuleBase> Rules { get; private set; }

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
      Rules = new List<IAuthorizationRuleBase>();
    }
#if NET5_0_OR_GREATER

    private static void OnAssemblyLoadContextUnload(AssemblyLoadContext context)
    {
      lock (_perTypeRules)
        AssemblyLoadContextManager.RemoveFromCache(_perTypeRules.Value, context, true);
    }
#endif
  }
}
