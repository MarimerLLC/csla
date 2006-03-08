using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

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

    /// <summary>
    /// Specify the roles allowed to read a given
    /// property.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="roles">List of roles granted read access.</param>
    /// <remarks>
    /// This method may be called multiple times, with the roles in
    /// each call being added to the end of the list of allowed roles.
    /// In other words, each call is cumulative, adding more roles
    /// to the list.
    /// </remarks>
    public void AllowRead(string propertyName, params string[] roles)
    {
      RolesForProperty currentRoles = GetRolesForProperty(propertyName);
      foreach (string item in roles)
      {
        currentRoles.ReadAllowed.Add(item);
      }
    }

    /// <summary>
    /// Specify the roles denied read access to 
    /// a given property.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="roles">List of roles denied read access.</param>
    /// <remarks>
    /// This method may be called multiple times, with the roles in
    /// each call being added to the end of the list of denied roles.
    /// In other words, each call is cumulative, adding more roles
    /// to the list.
    /// </remarks>
    public void DenyRead(string propertyName, params string[] roles)
    {
      RolesForProperty currentRoles = GetRolesForProperty(propertyName);
      foreach (string item in roles)
      {
        currentRoles.ReadDenied.Add(item);
      }
    }

    /// <summary>
    /// Specify the roles allowed to write a given
    /// property.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="roles">List of roles granted write access.</param>
    /// <remarks>
    /// This method may be called multiple times, with the roles in
    /// each call being added to the end of the list of allowed roles.
    /// In other words, each call is cumulative, adding more roles
    /// to the list.
    /// </remarks>
    public void AllowWrite(string propertyName, params string[] roles)
    {
      RolesForProperty currentRoles = GetRolesForProperty(propertyName);
      foreach (string item in roles)
      {
        currentRoles.WriteAllowed.Add(item);
      }
    }

    /// <summary>
    /// Specify the roles denied write access to 
    /// a given property.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="roles">List of roles denied write access.</param>
    /// <remarks>
    /// This method may be called multiple times, with the roles in
    /// each call being added to the end of the list of denied roles.
    /// In other words, each call is cumulative, adding more roles
    /// to the list.
    /// </remarks>
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

    /// <summary>
    /// Indicates whether the property has a list
    /// of roles granted read access.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    public bool HasReadAllowedRoles(string propertyName)
    {
      return (GetRolesForProperty(propertyName).ReadAllowed.Count > 0);
    }

    /// <summary>
    /// Indicates whether the current user as defined by
    /// <see cref="Csla.ApplicationContext.User" />
    /// is explicitly allowed to read the property.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    public bool IsReadAllowed(string propertyName)
    {
      return GetRolesForProperty(
        propertyName).IsReadAllowed(ApplicationContext.User);
    }

    /// <summary>
    /// Indicates whether the property has a list
    /// of roles denied read access.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    public bool HasReadDeniedRoles(string propertyName)
    {
      return (GetRolesForProperty(propertyName).ReadDenied.Count > 0);
    }

    /// <summary>
    /// Indicates whether the current user as defined by
    /// <see cref="Csla.ApplicationContext.User" />
    /// is explicitly denied read access to the property.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    public bool IsReadDenied(string propertyName)
    {
      return GetRolesForProperty(propertyName).IsReadDenied(ApplicationContext.User);
    }

    /// <summary>
    /// Indicates whether the property has a list
    /// of roles granted write access.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    public bool HasWriteAllowedRoles(string propertyName)
    {
      return (GetRolesForProperty(propertyName).WriteAllowed.Count > 0);
    }

    /// <summary>
    /// Indicates whether the current user as defined by
    /// <see cref="Csla.ApplicationContext.User" />
    /// is explicitly allowed to set the property.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    public bool IsWriteAllowed(string propertyName)
    {
      return GetRolesForProperty(propertyName).IsWriteAllowed(ApplicationContext.User);
    }

    /// <summary>
    /// Indicates whether the property has a list
    /// of roles denied write access.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    public bool HasWriteDeniedRoles(string propertyName)
    {
      return (GetRolesForProperty(propertyName).WriteDenied.Count > 0);
    }

    /// <summary>
    /// Indicates whether the current user as defined by
    /// <see cref="Csla.ApplicationContext.User" />
    /// is explicitly denied write access to the property.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    public bool IsWriteDenied(string propertyName)
    {
      return GetRolesForProperty(propertyName).IsWriteDenied(ApplicationContext.User);
    }

    #endregion
  }
}