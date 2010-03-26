using System;
using System.Collections.Generic;
using System.Security.Principal;

namespace Csla.Security
{

  /// <summary>
  /// Maintains authorization roles for a business object
  /// or business object type.
  /// </summary>
  public class AuthorizationRulesManager
  {
    private Dictionary<string, RolesForProperty> _rules;

    internal Dictionary<string, RolesForProperty> RulesList
    {
      get
      {
        if (_rules == null)
          _rules = new Dictionary<string, RolesForProperty>();
        return _rules;
      }
    }

    #region Get Roles

    private static object _rolesLock = new object();

    internal RolesForProperty GetRolesForProperty(string propertyName)
    {
      RolesForProperty currentRoles = null;
      if (!RulesList.TryGetValue(propertyName, out currentRoles))
      {
        lock (_rolesLock)
        {
          if (!RulesList.TryGetValue(propertyName, out currentRoles))
          {
            currentRoles = new RolesForProperty();
            RulesList.Add(propertyName, currentRoles);
          }
        }
      }
      return currentRoles;
    }

    #endregion

    #region IsInRole

    internal static bool PrincipalRoleInList(IPrincipal principal, List<string> roleList)
    {
      bool result = false;
      foreach (string role in roleList)
      {
        if (IsInRole(principal, role))
        {
          result = true;
          break;
        }
      }
      return result;
    }

    private static IsInRoleProvider mIsInRoleProvider;

    private static bool IsInRole(IPrincipal principal, string role)
    {
      if (mIsInRoleProvider == null)
      {
        string provider = ApplicationContext.IsInRoleProvider;
        if (string.IsNullOrEmpty(provider))
          mIsInRoleProvider = IsInRoleDefault;
        else
        {
          string[] items = provider.Split(',');
          Type containingType = Csla.Reflection.MethodCaller.GetType(items[0] + "," + items[1]);
          mIsInRoleProvider = (IsInRoleProvider)(Delegate.CreateDelegate(typeof(IsInRoleProvider), containingType, items[2]));
        }
      }
      return mIsInRoleProvider(principal, role);
    }

    private static bool IsInRoleDefault(IPrincipal principal, string role)
    {
      return principal.IsInRole(role);
    }

    #endregion
  }

  /// <summary>
  /// Delegate for the method called when the a role
  /// needs to be checked for the current user.
  /// </summary>
  /// <param name="principal">
  /// The current security principal object.
  /// </param>
  /// <param name="role">
  /// The role to be checked.
  /// </param>
  /// <returns>
  /// True if the current user is in the specified role.
  /// </returns>
  public delegate bool IsInRoleProvider(IPrincipal principal, string role);
}
