//-----------------------------------------------------------------------
// <copyright file="AuthorizationRules.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Provides consistent context information between the client</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Csla.Rules;
using Csla.Rules.CommonRules;

namespace Csla.Security
{
  /// <summary>
  /// Add object level rules 
  /// </summary>
  public class AuthorizationRules
  {
    #region Object Level Roles
    /// <summary>
    /// Specify the roles allowed to get (fetch)
    /// a given type of business object.
    /// </summary>
    /// <param name="objectType">Type of business object.</param>
    /// <param name="roles">List of roles.</param>
    public static void AllowGet(Type objectType, params string[] roles)
    {
      BusinessRules.AddRule(objectType, new IsInRole(AuthorizationActions.GetObject, roles));
    }

    /// <summary>
    /// Specify the roles not allowed to get (fetch)
    /// a given type of business object.
    /// </summary>
    /// <param name="objectType">Type of business object.</param>
    /// <param name="roles">List of roles.</param>
    public static void DenyGet(Type objectType, params string[] roles)
    {
      BusinessRules.AddRule(objectType, new IsNotInRole(AuthorizationActions.GetObject, roles));
    }

    /// <summary>
    /// Specify the roles allowed to edit (save)
    /// a given type of business object.
    /// </summary>
    /// <param name="objectType">Type of business object.</param>
    /// <param name="roles">List of roles.</param>
    public static void AllowEdit(Type objectType, params string[] roles)
    {
      BusinessRules.AddRule(objectType, new IsInRole(AuthorizationActions.EditObject, roles));
    }

    /// <summary>
    /// Specify the roles not allowed to edit (save)
    /// a given type of business object.
    /// </summary>
    /// <param name="objectType">Type of business object.</param>
    /// <param name="roles">List of roles.</param>
    public static void DenyEdit(Type objectType, params string[] roles)
    {
      BusinessRules.AddRule(objectType, new IsNotInRole(AuthorizationActions.EditObject, roles));
    }

    /// <summary>
    /// Specify the roles allowed to create
    /// a given type of business object.
    /// </summary>
    /// <param name="objectType">Type of business object.</param>
    /// <param name="roles">List of roles.</param>
    public static void AllowCreate(Type objectType, params string[] roles)
    {
      BusinessRules.AddRule(objectType, new IsInRole(AuthorizationActions.CreateObject, roles));
    }

    /// <summary>
    /// Specify the roles not allowed to create
    /// a given type of business object.
    /// </summary>
    /// <param name="objectType">Type of business object.</param>
    /// <param name="roles">List of roles.</param>
    public static void DenyCreate(Type objectType, params string[] roles)
    {
      BusinessRules.AddRule(objectType, new IsNotInRole(AuthorizationActions.CreateObject, roles));
    }

    /// <summary>
    /// Specify the roles allowed to delete
    /// a given type of business object.
    /// </summary>
    /// <param name="objectType">Type of business object.</param>
    /// <param name="roles">List of roles.</param>
    public static void AllowDelete(Type objectType, params string[] roles)
    {
      BusinessRules.AddRule(objectType, new IsInRole(AuthorizationActions.DeleteObject, roles));
    }

    /// <summary>
    /// Specify the roles not allowed to delete
    /// a given type of business object.
    /// </summary>
    /// <param name="objectType">Type of business object.</param>
    /// <param name="roles">List of roles.</param>
    public static void DenyDelete(Type objectType, params string[] roles)
    {
      BusinessRules.AddRule(objectType, new IsNotInRole(AuthorizationActions.DeleteObject, roles));
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
      return BusinessRules.HasPermission(AuthorizationActions.CreateObject, objectType);
    }

    /// <summary>
    /// Gets a value indicating whether the current user
    /// is allowed to get (fetch) an instance of the business
    /// object.
    /// </summary>
    /// <param name="objectType">Type of business object.</param>
    public static bool CanGetObject(Type objectType)
    {
      return BusinessRules.HasPermission(AuthorizationActions.GetObject, objectType);
    }

    /// <summary>
    /// Gets a value indicating whether the current user
    /// is allowed to edit (save) an instance of the business
    /// object.
    /// </summary>
    /// <param name="objectType">Type of business object.</param>
    public static bool CanEditObject(Type objectType)
    {
      return BusinessRules.HasPermission(AuthorizationActions.EditObject, objectType);
    }

    /// <summary>
    /// Gets a value indicating whether the current user
    /// is allowed to delete an instance of the business
    /// object.
    /// </summary>
    /// <param name="objectType">Type of business object.</param>
    public static bool CanDeleteObject(Type objectType)
    {
      return BusinessRules.HasPermission(AuthorizationActions.DeleteObject, objectType);
    }

    #endregion
  }
}
