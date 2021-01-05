//-----------------------------------------------------------------------
// <copyright file="HasPermissionAttribute.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Restricts callers to an action method.</summary>
//-----------------------------------------------------------------------
#if !NETSTANDARD2_0 && !NETCORE3_1 && !NET5_0
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
    private const string ERROR_MSG = "Authorization denied.";
    private AuthorizationActions _action;
    private Type _objectType;
    private string _errorMsg = ERROR_MSG;

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
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="action">AuthorizationActions for which the authorization is required.</param>
    /// <param name="objectType">CSLA object type for which the action is applied.</param>
    /// <param name="message">Error message for resutl.</param>
    public HasPermissionAttribute(AuthorizationActions action, Type objectType, string message)
      : this(action, objectType)
    {
      _errorMsg = message;
    }

    /// <summary>
    /// Determines whether access to the core framework is authorized.
    /// </summary>
    /// <param name="httpContext">The HTTP context.</param>
    /// <returns>True if access is authorized; otherwise, false.</returns>
    protected override bool AuthorizeCore(HttpContextBase httpContext)
    {
      if (!Csla.ApplicationContext.User.Identity.IsAuthenticated) return false;

      return BusinessRules.HasPermission(_action, _objectType);
    }

    /// <summary>
    /// Processes HTTP requests that fail authorization and handles AJAX requests
    /// appropriately.
    /// </summary>
    /// <param name="filterContext">The filterContext object contains the controller, HTTP context, 
    /// request context, action result, and route data.</param>
    protected override void HandleUnauthorizedRequest(System.Web.Mvc.AuthorizationContext filterContext)
    {
      if (filterContext.HttpContext.Request.IsAjaxRequest())
      {
        filterContext.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
        filterContext.Result = new JsonResult()
        {
          JsonRequestBehavior = JsonRequestBehavior.AllowGet,
          Data = new
          {
            ErrorType = this.GetType().Name,
            Action = filterContext.ActionDescriptor.ActionName,
            Message = _errorMsg
          }
        };
      }
      else
      {
        base.HandleUnauthorizedRequest(filterContext);
      }
    }
  }
}
#endif