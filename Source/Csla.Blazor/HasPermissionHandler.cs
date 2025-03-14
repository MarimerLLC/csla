//-----------------------------------------------------------------------
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
    public CslaPermissionsHandler(ApplicationContext applicationContext)
    {
      _applicationContext = applicationContext;
    }

    /// <summary>
    /// Handle requirements
    /// </summary>
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, CslaPermissionRequirement requirement)
    {
      if (await Rules.BusinessRules.HasPermissionAsync(_applicationContext, requirement.Action, requirement.ObjectType, CancellationToken.None))
        context.Succeed(requirement);
      else
        context.Fail();
    }
  }
}
