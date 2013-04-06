using System;
using System.Collections.Generic;
using System.Reflection;

namespace Csla.Security
{
  /// <summary>
  /// Maintains a list of all object level
  /// authorization roles.
  /// </summary>
  internal class ObjectAuthorizationRules
  {
    private static Dictionary<Type, RolesForType> _managers =
      new Dictionary<Type, RolesForType>();

    internal static RolesForType GetRoles(Type objectType)
    {
      RolesForType result = null;
      if (!_managers.TryGetValue(objectType, out result))
      {
        lock (_managers)
        {
          if (!_managers.TryGetValue(objectType, out result))
          {
            result = new RolesForType();
            _managers.Add(objectType, result);
            // invoke method to add auth roles
            var flags = 
              BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
            MethodInfo method = objectType.GetMethod(
              "AddObjectAuthorizationRules", flags);
            if (method != null)
              method.Invoke(null, null);
          }
        }
      }
      return result;
    }

    public static bool RulesExistFor(Type objectType)
    {
      return _managers.ContainsKey(objectType);
    }
  }
}
