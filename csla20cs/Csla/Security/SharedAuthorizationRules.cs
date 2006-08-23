using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Csla.Security
{

  /// <summary>
  /// Maintains a list of all the per-type
  /// <see cref="AuthorizationRulesManager"/> objects
  /// loaded in memory.
  /// </summary>
  internal static class SharedAuthorizationRules
  {
    private static Dictionary<Type, AuthorizationRulesManager> _managers =
      new Dictionary<Type, AuthorizationRulesManager>();

    /// <summary>
    /// Gets the <see cref="AuthorizationRulesManager"/> for the 
    /// specified object type, optionally creating a new instance 
    /// of the object if necessary.
    /// </summary>
    /// <param name="objectType">
    /// Type of business object for which the rules apply.
    /// </param>
    /// <param name="create">Indicates whether to create
    /// a new instance of the object if one doesn't exist.</param>
    internal static AuthorizationRulesManager GetManager(Type objectType, bool create)
    {
      AuthorizationRulesManager result = null;
      if (_managers.ContainsKey(objectType))
        result = _managers[objectType];
      else if (create)
      {
        result = new AuthorizationRulesManager();
        _managers.Add(objectType, result);
      }
      return result;
    }

    /// <summary>
    /// Gets a value indicating whether a set of rules
    /// have been created for a given <see cref="Type" />.
    /// </summary>
    /// <param name="objectType">
    /// Type of business object for which the rules apply.
    /// </param>
    /// <returns><see langword="true" /> if rules exist for the type.</returns>
    public static bool RulesExistFor(Type objectType)
    {
      return _managers.ContainsKey(objectType);
    }

    #region Get Roles

    /// <summary>
    /// Returns a list of roles for the property,
    /// object type and requested access type.
    /// </summary>
    /// <param name="propertyName">
    /// Name of the object property.</param>
    /// <param name="objectType">
    /// Type of the business object.
    /// </param>
    /// <param name="access">Desired access type.</param>
    /// <returns>An string array of roles.</returns>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static string[] GetRolesForProperty(string propertyName,
      Type objectType, AccessType access)
    {
      RolesForProperty currentRoles = GetRolesForProperty(propertyName, objectType);
      if (currentRoles != null)
      {
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
      }
      return null;
    }

    private static RolesForProperty GetRolesForProperty(string propertyName, Type objectType)
    {
      AuthorizationRulesManager manager = GetManager(objectType, true);
      RolesForProperty currentRoles = null;
      if (!manager.RulesList.ContainsKey(propertyName))
      {
        currentRoles = new RolesForProperty();
        manager.RulesList.Add(propertyName, currentRoles);
      }
      else
        currentRoles = manager.RulesList[propertyName];
      return currentRoles;
    }

    #endregion

    #region Add Roles

    /// <summary>
    /// Specify the roles allowed to read a given
    /// property.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="objectType">Type of the business object.</param>
    /// <param name="roles">List of roles granted read access.</param>
    /// <remarks>
    /// This method may be called multiple times, with the roles in
    /// each call being added to the end of the list of allowed roles.
    /// In other words, each call is cumulative, adding more roles
    /// to the list.
    /// </remarks>
    public static void AllowRead(string propertyName, Type objectType, params string[] roles)
    {
      RolesForProperty currentRoles = GetRolesForProperty(propertyName, objectType);
      foreach (string item in roles)
        currentRoles.ReadAllowed.Add(item);
    }

    /// <summary>
    /// Specify the roles denied read access to 
    /// a given property.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="objectType">Type of the business object.</param>
    /// <param name="roles">List of roles denied read access.</param>
    /// <remarks>
    /// This method may be called multiple times, with the roles in
    /// each call being added to the end of the list of denied roles.
    /// In other words, each call is cumulative, adding more roles
    /// to the list.
    /// </remarks>
    public static void DenyRead(string propertyName, Type objectType, params string[] roles)
    {
      RolesForProperty currentRoles = GetRolesForProperty(propertyName, objectType);
      foreach (string item in roles)
        currentRoles.ReadDenied.Add(item);
    }

    /// <summary>
    /// Specify the roles allowed to write a given
    /// property.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="objectType">Type of the business object.</param>
    /// <param name="roles">List of roles granted write access.</param>
    /// <remarks>
    /// This method may be called multiple times, with the roles in
    /// each call being added to the end of the list of allowed roles.
    /// In other words, each call is cumulative, adding more roles
    /// to the list.
    /// </remarks>
    public static void AllowWrite(string propertyName, Type objectType, params string[] roles)
    {
      RolesForProperty currentRoles = GetRolesForProperty(propertyName, objectType);
      foreach (string item in roles)
        currentRoles.WriteAllowed.Add(item);
    }

    /// <summary>
    /// Specify the roles denied write access to 
    /// a given property.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="objectType">Type of the business object.</param>
    /// <param name="roles">List of roles denied write access.</param>
    /// <remarks>
    /// This method may be called multiple times, with the roles in
    /// each call being added to the end of the list of denied roles.
    /// In other words, each call is cumulative, adding more roles
    /// to the list.
    /// </remarks>
    public static void DenyWrite(string propertyName, Type objectType, params string[] roles)
    {
      RolesForProperty currentRoles = GetRolesForProperty(propertyName, objectType);
      foreach (string item in roles)
        currentRoles.WriteDenied.Add(item);
    }

    #endregion

  }
}