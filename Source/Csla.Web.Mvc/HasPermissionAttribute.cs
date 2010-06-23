//-----------------------------------------------------------------------
// <copyright file="HasPermissionAttribute.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Restricts callers to an action method.</summary>
//-----------------------------------------------------------------------
using System;
using System.Web;
using System.Web.Mvc;
using Csla.Rules;

namespace Csla.Web.Mvc
{
  /// <summary>
  /// Restricts callers to an action method.
  /// </summary>
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
  public class HasPermissionAttribute : AuthorizeAttribute
  {
    private AuthorizationActions _action;
    private Type _objectType;

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="action">AuthorizationActions for which the authorization is required.</param>
    /// <param name="objectType">CSLA object type for which the action is applied.</param>
    public HasPermissionAttribute(AuthorizationActions action, Type objectType)
    {
      _action = action;
      _objectType = objectType;
    }

    /// <summary>
    /// Determines whether access to the core framework is authorized.
    /// </summary>
    /// <param name="httpContext">The HTTP context.</param>
    /// <returns>True if access is authorized; otherwise, false.</returns>
    protected override bool AuthorizeCore(HttpContextBase httpContext)
    {
      return BusinessRules.HasPermission(_action, _objectType);
    }
  }
}
