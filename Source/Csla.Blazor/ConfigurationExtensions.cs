//-----------------------------------------------------------------------
// <copyright file="BlazorConfigurationExtensions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implement extension methods for .NET Core configuration</summary>
//-----------------------------------------------------------------------
using Csla.Blazor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Csla.Configuration
{
  /// <summary>
  /// Implement extension methods for .NET Core configuration
  /// </summary>
  public static class BlazorConfigurationExtensions
  {
    /// <summary>
    /// Configures services to provide CSLA Blazor server support
    /// </summary>
    /// <param name="builder">ICslaBuilder instance</param>
    /// <returns></returns>
    public static ICslaBuilder WithBlazorServerSupport(this ICslaBuilder builder)
    {
      builder.Services.TryAddTransient(typeof(ViewModel<>), typeof(ViewModel<>));
      builder.Services.TryAddSingleton<IAuthorizationPolicyProvider, CslaPermissionsPolicyProvider>();
      builder.Services.TryAddSingleton<IAuthorizationHandler, CslaPermissionsHandler>();
      return builder;
    }
  }
}