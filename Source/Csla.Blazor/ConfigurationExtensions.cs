//-----------------------------------------------------------------------
// <copyright file="BlazorConfigurationExtensions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implement extension methods for .NET Core configuration</summary>
//-----------------------------------------------------------------------
using Csla.Blazor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
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
    /// <param name="options">Options object</param>
    /// <returns></returns>
    public static ICslaBuilder WithBlazorServerSupport(this ICslaBuilder builder, BlazorServerConfigurationOptions options = null)
    {
      if (options == null)
        options = new BlazorServerConfigurationOptions();
      builder.Services.TryAddTransient(typeof(ViewModel<>), typeof(ViewModel<>));
      if (options.UseCslaPermissionsPolicy)
      {
        builder.Services.AddSingleton<IAuthorizationPolicyProvider, CslaPermissionsPolicyProvider>();
        builder.Services.AddSingleton<IAuthorizationHandler, CslaPermissionsHandler>();
      }
      return builder;
    }
  }

  /// <summary>
  /// Options that can be provided to the WithBlazorServerSupport
  /// method.
  /// </summary>
  public class BlazorServerConfigurationOptions
  {
    /// <summary>
    /// Indicates whether the app should be configured to
    /// use CSLA permissions policies (default = true).
    /// </summary>
    public bool UseCslaPermissionsPolicy { get; set; } = true;
  }
}