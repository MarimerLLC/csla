//-----------------------------------------------------------------------
// <copyright file="BlazorConfigurationExtensions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implement extension methods for .NET Core configuration</summary>
//-----------------------------------------------------------------------
using System;
using Csla.Blazor;
using Csla.Core;
using Csla.Runtime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
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
    /// <param name="config">CslaOptions instance</param>
    /// <returns></returns>
    public static CslaOptions AddServerSideBlazor(this CslaOptions config)
    {
      return AddServerSideBlazor(config, null);
    }

    /// <summary>
    /// Configures services to provide CSLA Blazor server support
    /// </summary>
    /// <param name="config">CslaOptions instance</param>
    /// <param name="options">Options object</param>
    /// <returns></returns>
    public static CslaOptions AddServerSideBlazor(this CslaOptions config, Action<BlazorServerConfigurationOptions> options)
    {
      var blazorOptions = new BlazorServerConfigurationOptions();
      options?.Invoke(blazorOptions);

      // minimize PropertyChanged events
      config.BindingOptions.PropertyChangedMode = ApplicationContext.PropertyChangedModes.Windows;

#if NET5_0_OR_GREATER
      var managerType = Type.GetType("Csla.AspNetCore.Blazor.ApplicationContextManagerBlazor,Csla.AspNetCore");
      if (managerType is null)
        throw new TypeLoadException("Csla.AspNetCore.Blazor.ApplicationContextManagerBlazor,Csla.AspNetCore");
      config.Services.AddScoped(typeof(IContextManager), managerType);
#endif

      // use Blazor viewmodel
      config.Services.TryAddTransient(typeof(ViewModel<>), typeof(ViewModel<>));
      if (blazorOptions.UseCslaPermissionsPolicy)
      {
        config.Services.AddTransient<IAuthorizationPolicyProvider, CslaPermissionsPolicyProvider>();
        config.Services.AddTransient<IAuthorizationHandler, CslaPermissionsHandler>();
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
