﻿//-----------------------------------------------------------------------
// <copyright file="HasPermissionAttribute.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Restricts callers to an action method.</summary>
//-----------------------------------------------------------------------
#if NETSTANDARD2_0 || NET5_0_OR_GREATER || NETCOREAPP3_1
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
    private ApplicationContext _applicationContext;

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="applicationContext">ApplicationContext instance.</param>
    public CslaPermissionHandler(ApplicationContext applicationContext)
    {
      _applicationContext = applicationContext;
    }

    /// <summary>
    /// Handles CSLA permissions
    /// </summary>
    /// <param name="context">Authorization handler context</param>
    public override async Task HandleAsync(AuthorizationHandlerContext context)
    {
      foreach (var item in context.PendingRequirements)
        if (item is CslaPermissionRequirement cr)
          await HandleRequirementAsync(context, cr);
    }

    /// <summary>
    /// Handles CSLA permissions
    /// </summary>
    /// <param name="context">Authorization handler context</param>
    /// <param name="requirement">CSLA permissions requirement</param>
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
      CslaPermissionRequirement requirement)
    {
      if (context.User == null || !context.User.Identity.IsAuthenticated)
        context.Fail();
      else if (await BusinessRules.HasPermissionAsync(_applicationContext, requirement.Action, requirement.ObjectType, CancellationToken.None))
        context.Succeed(requirement);
    }
  }
}
#endif