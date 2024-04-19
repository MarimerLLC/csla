﻿//-----------------------------------------------------------------------
// <copyright file="CslaPermissionsHandler.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Authorization handler for CSLA permissions</summary>
//-----------------------------------------------------------------------

using Microsoft.AspNetCore.Authorization;

namespace Csla.Blazor
{
  /// <summary>
  /// Authorization handler for CSLA permissions.
  /// </summary>
  public class CslaPermissionsHandler : AuthorizationHandler<CslaPermissionRequirement>
  {
    private ApplicationContext _applicationContext;

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="applicationContext"></param>
    public CslaPermissionsHandler(ApplicationContext applicationContext)
    {
      _applicationContext = applicationContext;
    }

    /// <summary>
    /// Handle requirements
    /// </summary>
    /// <param name="context"></param>
    /// <param name="requirement"></param>
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CslaPermissionRequirement requirement)
    {
      if (Rules.BusinessRules.HasPermission(_applicationContext, requirement.Action, requirement.ObjectType))
        context.Succeed(requirement);
      else
        context.Fail();
      return Task.CompletedTask;
    }
  }
}
