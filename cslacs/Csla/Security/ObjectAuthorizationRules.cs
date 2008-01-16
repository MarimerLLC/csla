using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        bool createdManager = false;
        lock (_managers)
        {
          if (!_managers.TryGetValue(objectType, out result))
          {
            result = new RolesForType();
            _managers.Add(objectType, result);
            createdManager = true;
          }
        }
        // if necessary, create instance to trigger
        // static constructor
        if (createdManager)
        {
          lock (objectType)
          {
            var flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
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
