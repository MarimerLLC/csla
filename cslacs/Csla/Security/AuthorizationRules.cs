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

    private Type _businessObjectType;
    private AuthorizationRulesManager _typeRules;
    private AuthorizationRulesManager _instanceRules;

    /// <summary>
    /// Creates an instance of the object, initializing
    /// it with the business object type.
    /// </summary>
    /// <param name="businessObjectType">
    /// Type of the business object to which the rules
    /// apply.
    /// </param>
    public AuthorizationRules(Type businessObjectType)
    {
      _businessObjectType = businessObjectType;
    }

    private AuthorizationRulesManager InstanceRules
    {
      get
      {
        if (_instanceRules == null)
          _instanceRules = new AuthorizationRulesManager();
        return _instanceRules;
      }
    }

    private AuthorizationRulesManager TypeRules
    {
      get
      {
        if (_typeRules == null)
          _typeRules = SharedAuthorizationRules.GetManager(_businessObjectType, true);
        return _typeRules;
      }
    }

    #region Add Per-Instance Roles

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
    public void InstanceAllowRead(string propertyName, params string[] roles)
    {
      RolesForProperty currentRoles = InstanceRules.GetRolesForProperty(propertyName);
      foreach (string item in roles)
        currentRoles.ReadAllowed.Add(item);
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
    public void InstanceDenyRead(string propertyName, params string[] roles)
    {
      RolesForProperty currentRoles = InstanceRules.GetRolesForProperty(propertyName);
      foreach (string item in roles)
        currentRoles.ReadDenied.Add(item);
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
    public void InstanceAllowWrite(string propertyName, params string[] roles)
    {
      RolesForProperty currentRoles = InstanceRules.GetRolesForProperty(propertyName);
      foreach (string item in roles)
        currentRoles.WriteAllowed.Add(item);
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
    public void InstanceDenyWrite(string propertyName, params string[] roles)
    {
      RolesForProperty currentRoles = InstanceRules.GetRolesForProperty(propertyName);
      foreach (string item in roles)
        currentRoles.WriteDenied.Add(item);
    }

    /// <summary>
    /// Specify the roles allowed to execute a given
    /// method.
    /// </summary>
    /// <param name="methodName">Name of the method.</param>
    /// <param name="roles">List of roles granted read access.</param>
    /// <remarks>
    /// This method may be called multiple times, with the roles in
    /// each call being added to the end of the list of allowed roles.
    /// In other words, each call is cumulative, adding more roles
    /// to the list.
    /// </remarks>
    public void InstanceAllowExecute(string methodName, params string[] roles)
    {

      RolesForProperty currentRoles = InstanceRules.GetRolesForProperty(methodName);
      foreach (string item in roles)
      {
        currentRoles.ExecuteAllowed.Add(item);
      }

    }

    /// <summary>
    /// Specify the roles denied the right to execute 
    /// a given method.
    /// </summary>
    /// <param name="methodName">Name of the method.</param>
    /// <param name="roles">List of roles denied read access.</param>
    /// <remarks>
    /// This method may be called multiple times, with the roles in
    /// each call being added to the end of the list of denied roles.
    /// In other words, each call is cumulative, adding more roles
    /// to the list.
    /// </remarks>
    public void InstanceDenyExecute(string methodName, params string[] roles)
    {

      RolesForProperty currentRoles = InstanceRules.GetRolesForProperty(methodName);
      foreach (string item in roles)
      {
        currentRoles.ExecuteDenied.Add(item);
      }

    }

    #endregion

    #region Add Per-Type Roles

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
      RolesForProperty currentRoles = TypeRules.GetRolesForProperty(propertyName);
      foreach (string item in roles)
        currentRoles.ReadAllowed.Add(item);
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
      RolesForProperty currentRoles = TypeRules.GetRolesForProperty(propertyName);
      foreach (string item in roles)
        currentRoles.ReadDenied.Add(item);
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
      RolesForProperty currentRoles = TypeRules.GetRolesForProperty(propertyName);
      foreach (string item in roles)
        currentRoles.WriteAllowed.Add(item);
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
      RolesForProperty currentRoles = TypeRules.GetRolesForProperty(propertyName);
      foreach (string item in roles)
        currentRoles.WriteDenied.Add(item);
    }

    /// <summary>
    /// Specify the roles allowed to execute a given
    /// method.
    /// </summary>
    /// <param name="methodName">Name of the property.</param>
    /// <param name="roles">List of roles granted execute access.</param>
    /// <remarks>
    /// This method may be called multiple times, with the roles in
    /// each call being added to the end of the list of allowed roles.
    /// In other words, each call is cumulative, adding more roles
    /// to the list.
    /// </remarks>
    public void AllowExecute(string methodName, params string[] roles)
    {

      RolesForProperty currentRoles = TypeRules.GetRolesForProperty(methodName);
      foreach (string item in roles)
      {
        currentRoles.ExecuteAllowed.Add(item);
      }

    }

    /// <summary>
    /// Specify the roles denied the right to execute 
    /// a given method.
    /// </summary>
    /// <param name="methodName">Name of the property.</param>
    /// <param name="roles">List of roles denied execute access.</param>
    /// <remarks>
    /// This method may be called multiple times, with the roles in
    /// each call being added to the end of the list of denied roles.
    /// In other words, each call is cumulative, adding more roles
    /// to the list.
    /// </remarks>
    public void DenyExecute(string methodName, params string[] roles)
    {

      RolesForProperty currentRoles = TypeRules.GetRolesForProperty(methodName);
      foreach (string item in roles)
      {
        currentRoles.ExecuteDenied.Add(item);
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
      if (InstanceRules.GetRolesForProperty(propertyName).ReadAllowed.Count > 0)
        return true;
      return TypeRules.GetRolesForProperty(propertyName).ReadAllowed.Count > 0;
    }

    /// <summary>
    /// Indicates whether the current user as defined by
    /// <see cref="Csla.ApplicationContext.User" />
    /// is explicitly allowed to read the property.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    public bool IsReadAllowed(string propertyName)
    {
      System.Security.Principal.IPrincipal user = ApplicationContext.User;
      if (InstanceRules.GetRolesForProperty(propertyName).IsReadAllowed(user))
        return true;
      return TypeRules.GetRolesForProperty(propertyName).IsReadAllowed(user);
    }

    /// <summary>
    /// Indicates whether the property has a list
    /// of roles denied read access.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    public bool HasReadDeniedRoles(string propertyName)
    {
      if (InstanceRules.GetRolesForProperty(propertyName).ReadDenied.Count > 0)
        return true;
      return TypeRules.GetRolesForProperty(propertyName).ReadDenied.Count > 0;
    }

    /// <summary>
    /// Indicates whether the current user as defined by
    /// <see cref="Csla.ApplicationContext.User" />
    /// is explicitly denied read access to the property.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    public bool IsReadDenied(string propertyName)
    {
      System.Security.Principal.IPrincipal user = ApplicationContext.User;
      if (InstanceRules.GetRolesForProperty(propertyName).IsReadDenied(user))
        return true;
      return TypeRules.GetRolesForProperty(propertyName).IsReadDenied(user);
    }

    /// <summary>
    /// Indicates whether the property has a list
    /// of roles granted write access.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    public bool HasWriteAllowedRoles(string propertyName)
    {
      if (InstanceRules.GetRolesForProperty(propertyName).WriteAllowed.Count > 0)
        return true;
      return TypeRules.GetRolesForProperty(propertyName).WriteAllowed.Count > 0;
    }

    /// <summary>
    /// Indicates whether the current user as defined by
    /// <see cref="Csla.ApplicationContext.User" />
    /// is explicitly allowed to set the property.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    public bool IsWriteAllowed(string propertyName)
    {
      System.Security.Principal.IPrincipal user = ApplicationContext.User;
      if (InstanceRules.GetRolesForProperty(propertyName).IsWriteAllowed(user))
        return true;
      return TypeRules.GetRolesForProperty(propertyName).IsWriteAllowed(user);
    }

    /// <summary>
    /// Indicates whether the property has a list
    /// of roles denied write access.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    public bool HasWriteDeniedRoles(string propertyName)
    {
      if (InstanceRules.GetRolesForProperty(propertyName).WriteDenied.Count > 0)
        return true;
      return TypeRules.GetRolesForProperty(propertyName).WriteDenied.Count > 0;
    }

    /// <summary>
    /// Indicates whether the current user as defined by
    /// <see cref="Csla.ApplicationContext.User" />
    /// is explicitly denied write access to the property.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    public bool IsWriteDenied(string propertyName)
    {
      System.Security.Principal.IPrincipal user = ApplicationContext.User;
      if (InstanceRules.GetRolesForProperty(propertyName).IsWriteDenied(user))
        return true;
      return TypeRules.GetRolesForProperty(propertyName).IsWriteDenied(user);
    }

    /// <summary>
    /// Indicates whether the property has a list
    /// of roles granted execute access.
    /// </summary>
    /// <param name="methodName">Name of the method.</param>
    public bool HasExecuteAllowedRoles(string methodName)
    {

      bool result = false;
      if (InstanceRules.GetRolesForProperty(methodName).ExecuteAllowed.Count > 0)
      {
        result = true;

      }
      else
      {
        result = TypeRules.GetRolesForProperty(methodName).ExecuteAllowed.Count > 0;
      }

      return result;

    }

    /// <summary>
    /// Indicates whether the current user as defined by
    /// <see cref="Csla.ApplicationContext.User" />
    /// is explicitly allowed to execute the method.
    /// </summary>
    /// <param name="methodName">Name of the method.</param>
    public bool IsExecuteAllowed(string methodName)
    {

      bool result = false;
      System.Security.Principal.IPrincipal user = ApplicationContext.User;
      if (InstanceRules.GetRolesForProperty(methodName).IsExecuteAllowed(user))
      {
        result = true;

      }
      else
      {
        result = TypeRules.GetRolesForProperty(methodName).IsExecuteAllowed(user);
      }
      return result;

    }

    /// <summary>
    /// Indicates whether the property has a list
    /// of roles denied execute access.
    /// </summary>
    /// <param name="methodName">Name of the method.</param>
    public bool HasExecuteDeniedRoles(string methodName)
    {

      bool result = false;
      if (InstanceRules.GetRolesForProperty(methodName).ExecuteDenied.Count > 0)
      {
        result = true;

      }
      else
      {
        result = TypeRules.GetRolesForProperty(methodName).ExecuteDenied.Count > 0;
      }
      return result;

    }

    /// <summary>
    /// Indicates whether the current user as defined by
    /// <see cref="Csla.ApplicationContext.User" />
    /// is explicitly denied execute access to the method.
    /// </summary>
    /// <param name="methodName">Name of the method.</param>
    public bool IsExecuteDenied(string methodName)
    {

      bool result = false;
      System.Security.Principal.IPrincipal user = ApplicationContext.User;
      if (InstanceRules.GetRolesForProperty(methodName).IsExecuteDenied(user))
      {
        result = true;

      }
      else
      {
        result = TypeRules.GetRolesForProperty(methodName).IsExecuteDenied(user);
      }
      return result;

    }

    #endregion

  }
}