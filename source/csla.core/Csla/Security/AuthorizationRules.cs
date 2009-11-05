using System;
using System.Collections.Generic;
using Csla.Serialization;

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

    #region  Add Per-Type Roles

    /// <summary>
    /// Specify the roles allowed to read a given
    /// property.
    /// </summary>
    /// <param name="propertyInfo">PropertyInfo for the property.</param>
    /// <param name="roles">List of roles granted read access.</param>
    /// <remarks>
    /// This method may be called multiple times, with the roles in
    /// each call being added to the end of the list of allowed roles.
    /// In other words, each call is cumulative, adding more roles
    /// to the list.
    /// </remarks>
    public void AllowRead(Core.IPropertyInfo propertyInfo, params string[] roles)
    {
      RolesForProperty currentRoles = TypeRules.GetRolesForProperty(propertyInfo.Name);
      foreach (string item in roles)
        currentRoles.ReadAllowed.Add(item);
    }

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
    /// <param name="propertyInfo">PropertyInfo for the property.</param>
    /// <param name="roles">List of roles denied read access.</param>
    /// <remarks>
    /// This method may be called multiple times, with the roles in
    /// each call being added to the end of the list of denied roles.
    /// In other words, each call is cumulative, adding more roles
    /// to the list.
    /// </remarks>
    public void DenyRead(Core.IPropertyInfo propertyInfo, params string[] roles)
    {
      RolesForProperty currentRoles = TypeRules.GetRolesForProperty(propertyInfo.Name);
      foreach (string item in roles)
        currentRoles.ReadDenied.Add(item);
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
    /// <param name="propertyInfo">PropertyInfo for the property.</param>
    /// <param name="roles">List of roles granted write access.</param>
    /// <remarks>
    /// This method may be called multiple times, with the roles in
    /// each call being added to the end of the list of allowed roles.
    /// In other words, each call is cumulative, adding more roles
    /// to the list.
    /// </remarks>
    public void AllowWrite(Core.IPropertyInfo propertyInfo, params string[] roles)
    {
      RolesForProperty currentRoles = TypeRules.GetRolesForProperty(propertyInfo.Name);
      foreach (string item in roles)
        currentRoles.WriteAllowed.Add(item);
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
    /// <param name="propertyInfo">PropertyInfo for the property.</param>
    /// <param name="roles">List of roles denied write access.</param>
    /// <remarks>
    /// This method may be called multiple times, with the roles in
    /// each call being added to the end of the list of denied roles.
    /// In other words, each call is cumulative, adding more roles
    /// to the list.
    /// </remarks>
    public void DenyWrite(Core.IPropertyInfo propertyInfo, params string[] roles)
    {
      RolesForProperty currentRoles = TypeRules.GetRolesForProperty(propertyInfo.Name);
      foreach (string item in roles)
        currentRoles.WriteDenied.Add(item);
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
    /// <param name="propertyInfo">PropertyInfo for the method.</param>
    /// <param name="roles">List of roles granted execute access.</param>
    /// <remarks>
    /// This method may be called multiple times, with the roles in
    /// each call being added to the end of the list of allowed roles.
    /// In other words, each call is cumulative, adding more roles
    /// to the list.
    /// </remarks>
    public void AllowExecute(Core.IPropertyInfo propertyInfo, params string[] roles)
    {
      RolesForProperty currentRoles = TypeRules.GetRolesForProperty(propertyInfo.Name);
      foreach (string item in roles)
        currentRoles.ExecuteAllowed.Add(item);
    }

    /// <summary>
    /// Specify the roles allowed to execute a given
    /// method.
    /// </summary>
    /// <param name="methodName">Name of the method.</param>
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
        currentRoles.ExecuteAllowed.Add(item);
    }

    /// <summary>
    /// Specify the roles denied the right to execute 
    /// a given method.
    /// </summary>
    /// <param name="propertyInfo">PropertyInfo for the method.</param>
    /// <param name="roles">List of roles denied execute access.</param>
    /// <remarks>
    /// This method may be called multiple times, with the roles in
    /// each call being added to the end of the list of denied roles.
    /// In other words, each call is cumulative, adding more roles
    /// to the list.
    /// </remarks>
    public void DenyExecute(Core.IPropertyInfo propertyInfo, params string[] roles)
    {
      RolesForProperty currentRoles = TypeRules.GetRolesForProperty(propertyInfo.Name);
      foreach (string item in roles)
        currentRoles.ExecuteDenied.Add(item);
    }

    /// <summary>
    /// Specify the roles denied the right to execute 
    /// a given method.
    /// </summary>
    /// <param name="methodName">Name of the method.</param>
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
        currentRoles.ExecuteDenied.Add(item);
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

    #region Object Level Roles  

    /// <summary>
    /// Specify the roles allowed to get (fetch)
    /// a given type of business object.
    /// </summary>
    /// <param name="objectType">Type of business object.</param>
    /// <param name="roles">List of roles.</param>
    public static void AllowGet(Type objectType, params string[] roles)
    {
      var typeRules = ObjectAuthorizationRules.GetRoles(objectType);
      typeRules.AllowGet(roles);
    }

    /// <summary>
    /// Specify the roles not allowed to get (fetch)
    /// a given type of business object.
    /// </summary>
    /// <param name="objectType">Type of business object.</param>
    /// <param name="roles">List of roles.</param>
    public static void DenyGet(Type objectType, params string[] roles)
    {
      var typeRules = ObjectAuthorizationRules.GetRoles(objectType);
      typeRules.DenyGet(roles);
    }

    /// <summary>
    /// Specify the roles allowed to edit (save)
    /// a given type of business object.
    /// </summary>
    /// <param name="objectType">Type of business object.</param>
    /// <param name="roles">List of roles.</param>
    public static void AllowEdit(Type objectType, params string[] roles)
    {
      var typeRules = ObjectAuthorizationRules.GetRoles(objectType);
      typeRules.AllowEdit(roles);
    }

    /// <summary>
    /// Specify the roles not allowed to edit (save)
    /// a given type of business object.
    /// </summary>
    /// <param name="objectType">Type of business object.</param>
    /// <param name="roles">List of roles.</param>
    public static void DenyEdit(Type objectType, params string[] roles)
    {
      var typeRules = ObjectAuthorizationRules.GetRoles(objectType);
      typeRules.DenyEdit(roles);
    }

    /// <summary>
    /// Specify the roles allowed to create
    /// a given type of business object.
    /// </summary>
    /// <param name="objectType">Type of business object.</param>
    /// <param name="roles">List of roles.</param>
    public static void AllowCreate(Type objectType, params string[] roles)
    {
      var typeRules = ObjectAuthorizationRules.GetRoles(objectType);
      typeRules.AllowCreate(roles);
    }

    /// <summary>
    /// Specify the roles not allowed to create
    /// a given type of business object.
    /// </summary>
    /// <param name="objectType">Type of business object.</param>
    /// <param name="roles">List of roles.</param>
    public static void DenyCreate(Type objectType, params string[] roles)
    {
      var typeRules = ObjectAuthorizationRules.GetRoles(objectType);
      typeRules.DenyCreate(roles);
    }

    /// <summary>
    /// Specify the roles allowed to delete
    /// a given type of business object.
    /// </summary>
    /// <param name="objectType">Type of business object.</param>
    /// <param name="roles">List of roles.</param>
    public static void AllowDelete(Type objectType, params string[] roles)
    {
      var typeRules = ObjectAuthorizationRules.GetRoles(objectType);
      typeRules.AllowDelete(roles);
    }

    /// <summary>
    /// Specify the roles not allowed to delete
    /// a given type of business object.
    /// </summary>
    /// <param name="objectType">Type of business object.</param>
    /// <param name="roles">List of roles.</param>
    public static void DenyDelete(Type objectType, params string[] roles)
    {
      var typeRules = ObjectAuthorizationRules.GetRoles(objectType);
      typeRules.DenyDelete(roles);
    }

    internal static List<string> GetAllowCreateRoles(Type objectType)
    {
      var typeRules = ObjectAuthorizationRules.GetRoles(objectType);
      return typeRules.AllowCreateRoles;
    }

    internal static List<string> GetDenyCreateRoles(Type objectType)
    {
      var typeRules = ObjectAuthorizationRules.GetRoles(objectType);
      return typeRules.DenyCreateRoles;
    }

    internal static List<string> GetAllowGetRoles(Type objectType)
    {
      var typeRules = ObjectAuthorizationRules.GetRoles(objectType);
      return typeRules.AllowGetRoles;
    }

    internal static List<string> GetDenyGetRoles(Type objectType)
    {
      var typeRules = ObjectAuthorizationRules.GetRoles(objectType);
      return typeRules.DenyGetRoles;
    }

    internal static List<string> GetAllowEditRoles(Type objectType)
    {
      var typeRules = ObjectAuthorizationRules.GetRoles(objectType);
      return typeRules.AllowEditRoles;
    }

    internal static List<string> GetDenyEditRoles(Type objectType)
    {
      var typeRules = ObjectAuthorizationRules.GetRoles(objectType);
      return typeRules.DenyEditRoles;
    }

    internal static List<string> GetAllowDeleteRoles(Type objectType)
    {
      var typeRules = ObjectAuthorizationRules.GetRoles(objectType);
      return typeRules.AllowDeleteRoles;
    }

    internal static List<string> GetDenyDeleteRoles(Type objectType)
    {
      var typeRules = ObjectAuthorizationRules.GetRoles(objectType);
      return typeRules.DenyDeleteRoles;
    }


    #endregion

    #region Check Object Level Roles

    /// <summary>
    /// Gets a value indicating whether the current user
    /// is allowed to create an instance of the business
    /// object.
    /// </summary>
    /// <param name="objectType">Type of business object.</param>
    public static bool CanCreateObject(Type objectType)
    {
      bool result = true;
      var principal = ApplicationContext.User;
      var allow = Csla.Security.AuthorizationRules.GetAllowCreateRoles(objectType);
      if (allow != null)
      {
        if (!Csla.Security.AuthorizationRulesManager.PrincipalRoleInList(principal, allow))
          result = false;
      }
      else
      {
        var deny = Csla.Security.AuthorizationRules.GetDenyCreateRoles(objectType);
        if (deny != null)
        {
          if (Csla.Security.AuthorizationRulesManager.PrincipalRoleInList(principal, deny))
            result = false;
        }
      }
      return result;
    }

    /// <summary>
    /// Gets a value indicating whether the current user
    /// is allowed to get (fetch) an instance of the business
    /// object.
    /// </summary>
    /// <param name="objectType">Type of business object.</param>
    public static bool CanGetObject(Type objectType)
    {
      bool result = true;
      var principal = ApplicationContext.User;
      var allow = Csla.Security.AuthorizationRules.GetAllowGetRoles(objectType);
      if (allow != null)
      {
        if (!Csla.Security.AuthorizationRulesManager.PrincipalRoleInList(principal, allow))
          result = false;
      }
      else
      {
        var deny = Csla.Security.AuthorizationRules.GetDenyGetRoles(objectType);
        if (deny != null)
        {
          if (Csla.Security.AuthorizationRulesManager.PrincipalRoleInList(principal, deny))
            result = false;
        }
      }
      return result;
    }

    /// <summary>
    /// Gets a value indicating whether the current user
    /// is allowed to edit (save) an instance of the business
    /// object.
    /// </summary>
    /// <param name="objectType">Type of business object.</param>
    public static bool CanEditObject(Type objectType)
    {
      bool result = true;
      var principal = ApplicationContext.User;
      var allow = Csla.Security.AuthorizationRules.GetAllowEditRoles(objectType);
      if (allow != null)
      {
        if (!Csla.Security.AuthorizationRulesManager.PrincipalRoleInList(principal, allow))
          result = false;
      }
      else
      {
        var deny = Csla.Security.AuthorizationRules.GetDenyEditRoles(objectType);
        if (deny != null)
        {
          if (Csla.Security.AuthorizationRulesManager.PrincipalRoleInList(principal, deny))
            result = false;
        }
      }
      return result;
    }

    /// <summary>
    /// Gets a value indicating whether the current user
    /// is allowed to delete an instance of the business
    /// object.
    /// </summary>
    /// <param name="objectType">Type of business object.</param>
    public static bool CanDeleteObject(Type objectType)
    {
      bool result = true;
      var principal = ApplicationContext.User;
      var allow = Csla.Security.AuthorizationRules.GetAllowDeleteRoles(objectType);
      if (allow != null)
      {
        if (!Csla.Security.AuthorizationRulesManager.PrincipalRoleInList(principal, allow))
          result = false;
      }
      else
      {
        var deny = Csla.Security.AuthorizationRules.GetDenyDeleteRoles(objectType);
        if (deny != null)
        {
          if (Csla.Security.AuthorizationRulesManager.PrincipalRoleInList(principal, deny))
            result = false;
        }
      }
      return result;
    }

    #endregion

  }
}