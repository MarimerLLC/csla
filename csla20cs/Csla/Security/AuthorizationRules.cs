using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace Csla.Security
{

  /// <summary>
  /// Maintains a list of allowed and denied user roles
  /// for each property.
  /// </summary>
  /// <remarks></remarks>
  [Serializable()]
  public class AuthorizationRules
  {
    private Dictionary<string, RolesForProperty> _rules;

    private Dictionary<string, RolesForProperty> Rules
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
        case AccessType.ReadAllowed :
          return currentRoles.ReadAllowed.ToArray();
        case AccessType.ReadDenied :
          return currentRoles.ReadDenied.ToArray();
        case AccessType.WriteAllowed :
          return currentRoles.WriteAllowed.ToArray();
        case AccessType.WriteDenied :
          return currentRoles.WriteDenied.ToArray();
      }
      return null;
    }

    private RolesForProperty GetRolesForProperty(string propertyName)
    {
      RolesForProperty currentRoles = null;
      if (!Rules.ContainsKey(propertyName))
      {
        currentRoles = new RolesForProperty();
        Rules.Add(propertyName, currentRoles);
      }
      else
        currentRoles = Rules[propertyName];
      return currentRoles;
    }

    #endregion

    #region Add Roles

    public void AllowRead(string propertyName, params string[] roles)
    {
      RolesForProperty currentRoles = GetRolesForProperty(propertyName);
      foreach (string item in roles)
      {
        currentRoles.ReadAllowed.Add(item);
      }
    }

    public void DenyRead(string propertyName, params string[] roles)
    {
      RolesForProperty currentRoles = GetRolesForProperty(propertyName);
      foreach (string item in roles)
      {
        currentRoles.ReadDenied.Add(item);
      }
    }

    public void AllowWrite(string propertyName, params string[] roles)
    {
      RolesForProperty currentRoles = GetRolesForProperty(propertyName);
      foreach (string item in roles)
      {
        currentRoles.WriteAllowed.Add(item);
      }
    }

    public void DenyWrite(string propertyName, params string[] roles)
    {
      RolesForProperty currentRoles = GetRolesForProperty(propertyName);
      foreach (string item in roles)
      {
        currentRoles.WriteDenied.Add(item);
      }
    }

    #endregion

    #region Check Roles

    public bool HasReadAllowedRoles(string propertyName)
    {
      return (GetRolesForProperty(propertyName).ReadAllowed.Count > 0);
    }

    public bool IsReadAllowed(string propertyName)
    {
      return GetRolesForProperty(
        propertyName).IsReadAllowed(Thread.CurrentPrincipal);
    }

    public bool HasReadDeniedRoles(string propertyName)
    {
      return (GetRolesForProperty(propertyName).ReadDenied.Count > 0);
    }

    public bool IsReadDenied(string propertyName)
    {
      return GetRolesForProperty(propertyName).IsReadDenied(Thread.CurrentPrincipal);
    }

    public bool HasWriteAllowedRoles(string propertyName)
    {
      return (GetRolesForProperty(propertyName).WriteAllowed.Count > 0);
    }

    public bool IsWriteAllowed(string propertyName)
    {
      return GetRolesForProperty(propertyName).IsWriteAllowed(Thread.CurrentPrincipal);
    }

    public bool HasWriteDeniedRoles(string propertyName)
    {
      return (GetRolesForProperty(propertyName).WriteDenied.Count > 0);
    }

    public bool IsWriteDenied(string propertyName)
    {
      return GetRolesForProperty(propertyName).IsWriteDenied(Thread.CurrentPrincipal);
    }

    #endregion
  }
}