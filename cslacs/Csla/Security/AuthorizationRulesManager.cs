using System;
using System.Collections.Generic;
using System.ComponentModel;

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

    internal RolesForProperty GetRolesForProperty(string propertyName)
    {
      RolesForProperty currentRoles = null;
      if (!RulesList.ContainsKey(propertyName))
      {
        currentRoles = new RolesForProperty();
        RulesList.Add(propertyName, currentRoles);
      }
      else
        currentRoles = RulesList[propertyName];
      return currentRoles;
    }

    #endregion
  }
}
