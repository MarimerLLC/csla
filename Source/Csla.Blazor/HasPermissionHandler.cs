//-----------------------------------------------------------------------
// <copyright file="CslaPermissionsHandler.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Authorization handler for CSLA permissions</summary>
//-----------------------------------------------------------------------
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Csla.Blazor
{
  /// <summary>
  /// Authorization handler for CSLA permissions.
  /// </summary>
  public class CslaPermissionsHandler : AuthorizationHandler<CslaPermissionRequirement>
  {
    /// <summary>
    /// Handle requirements
    /// </summary>
    /// <param name="context"></param>
    /// <param name="requirement"></param>
    /// <returns></returns>
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CslaPermissionRequirement requirement)
    {
      if (Rules.BusinessRules.HasPermission(requirement.Action, requirement.ObjectType))
        context.Succeed(requirement);
      else
        context.Fail();
      return Task.CompletedTask;
    }
  }
}
