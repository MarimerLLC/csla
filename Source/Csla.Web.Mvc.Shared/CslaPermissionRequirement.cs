//-----------------------------------------------------------------------
// <copyright file="HasPermissionAttribute.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Restricts callers to an action method.</summary>
//-----------------------------------------------------------------------
#if NETSTANDARD2_0 || NET5_0 || NETCORE3_1
using System;
using System.Threading.Tasks;
using Csla.Rules;
using Microsoft.AspNetCore.Authorization;

namespace Csla.Web.Mvc
{
  /// <summary>
  /// Permission requirement for CSLA business domain types.
  /// </summary>
  public class CslaPermissionRequirement : IAuthorizationRequirement
  {
    /// <summary>
    /// Per-type authorization action
    /// </summary>
    public AuthorizationActions Action { get; }
    /// <summary>
    /// Business domain object type
    /// </summary>
    public Type ObjectType { get; }

    /// <summary>
    /// Creates instance of type
    /// </summary>
    /// <param name="action">Per-type authorization action</param>
    /// <param name="objectType">Business domain object type</param>
    public CslaPermissionRequirement(AuthorizationActions action, Type objectType)
    {
      Action = action;
      ObjectType = objectType;
    }
  }

  /// <summary>
  /// Handles CSLA permissions
  /// </summary>
  public class CslaPermissionHandler : AuthorizationHandler<CslaPermissionRequirement>
  {
    /// <summary>
    /// Handles CSLA permissions
    /// </summary>
    /// <param name="context">Authorization handler context</param>
    /// <returns></returns>
    public override Task HandleAsync(AuthorizationHandlerContext context)
    {
      foreach (var item in context.PendingRequirements)
        if (item is CslaPermissionRequirement cr)
          HandleRequirementAsync(context, cr);
      return Task.CompletedTask;
    }

    /// <summary>
    /// Handles CSLA permissions
    /// </summary>
    /// <param name="context">Authorization handler context</param>
    /// <param name="requirement">CSLA permissions requirement</param>
    /// <returns></returns>
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, 
      CslaPermissionRequirement requirement)
    {
      if (context.User == null || !context.User.Identity.IsAuthenticated)
        context.Fail();
      else if (BusinessRules.HasPermission(requirement.Action, requirement.ObjectType))
        context.Succeed(requirement);
      return Task.CompletedTask;
    }
  }
}
#endif