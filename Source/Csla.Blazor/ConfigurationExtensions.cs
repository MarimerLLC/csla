//-----------------------------------------------------------------------
// <copyright file="BlazorConfigurationExtensions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implement extension methods for .NET Core configuration</summary>
//-----------------------------------------------------------------------
using System;
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
    /// <param name="config">ICslaConfiguration instance</param>
    /// <returns></returns>
    public static ICslaConfiguration AddBlazorServerSupport(this ICslaConfiguration config)
    {
      return AddBlazorServerSupport(config, null);
    }

    /// <summary>
    /// Configures services to provide CSLA Blazor server support
    /// </summary>
    /// <param name="config">ICslaConfiguration instance</param>
    /// <param name="options">Options object</param>
    /// <returns></returns>
    public static ICslaConfiguration AddBlazorServerSupport(this ICslaConfiguration config, Action<BlazorServerConfigurationOptions> options)
    {
      var blazorOptions = new BlazorServerConfigurationOptions();
      options?.Invoke(blazorOptions);

      // use Blazor server context manager
      var managerTypeName = "Csla.Blazor.Server.ApplicationContextManager, Csla.AspNetCore";
      var managerType = Type.GetType(managerTypeName, false);
      if (managerType == null)
        throw new TypeLoadException(managerTypeName);
      config.Services.AddScoped(typeof(Csla.Core.IContextManager), managerType);

      // use Blazor viewmodel
      config.Services.TryAddTransient(typeof(ViewModel<>), typeof(ViewModel<>));
      if (blazorOptions.UseCslaPermissionsPolicy)
      {
        config.Services.AddSingleton<IAuthorizationPolicyProvider, CslaPermissionsPolicyProvider>();
        config.Services.AddSingleton<IAuthorizationHandler, CslaPermissionsHandler>();
      }
      return config;
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
