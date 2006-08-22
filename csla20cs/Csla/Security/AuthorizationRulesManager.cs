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

    /// <summary>
    /// Returns a list of roles for the property
    /// and requested access type.
    /// </summary>
    /// <param name="propertyName">
    /// Name of the object property.</param>
    /// <param name="access">Desired access type.</param>
    /// <returns>An string array of roles.</returns>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public string[] GetRolesForProperty(string propertyName, AccessType access)
    {
      RolesForProperty currentRoles = GetRolesForProperty(propertyName);
      switch (access)
      {
        case AccessType.ReadAllowed:
          return currentRoles.ReadAllowed.ToArray();
        case AccessType.ReadDenied:
          return currentRoles.ReadDenied.ToArray();
        case AccessType.WriteAllowed:
          return currentRoles.WriteAllowed.ToArray();
        case AccessType.WriteDenied:
          return currentRoles.WriteDenied.ToArray();
      }
      return null;
    }

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
