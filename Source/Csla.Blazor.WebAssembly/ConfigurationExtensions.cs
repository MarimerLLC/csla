//-----------------------------------------------------------------------
// <copyright file="BlazorWasmConfigurationExtensions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implement extension methods for .NET Core configuration</summary>
//-----------------------------------------------------------------------
using Csla.State;
using Csla.Blazor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using Csla.Blazor.WebAssembly.State;

namespace Csla.Configuration
{
  /// <summary>
  /// Implement extension methods for Blazor WebAssembly
  /// </summary>
  public static class BlazorWasmConfigurationExtensions
  {
    /// <summary>
    /// Registers services necessary for Blazor WebAssembly.
    /// </summary>
    /// <param name="config">CslaConfiguration object</param>
    /// <param name="options">Options object</param>
    /// <returns></returns>
    public static CslaOptions AddBlazorWebAssembly(this CslaOptions config, Action<BlazorWebAssemblyConfigurationOptions> options)
    {
      var blazorOptions = new BlazorWebAssemblyConfigurationOptions();
      options?.Invoke(blazorOptions);

      config.Services.TryAddTransient(typeof(ViewModel<>), typeof(ViewModel<>));
      config.Services.TryAddScoped<IAuthorizationPolicyProvider, CslaPermissionsPolicyProvider>();
      config.Services.TryAddScoped<IAuthorizationHandler, CslaPermissionsHandler>();
      config.Services.TryAddScoped(typeof(Csla.Core.IContextManager), typeof(Csla.Blazor.WebAssembly.ApplicationContextManager));
      config.Services.TryAddScoped(typeof(AuthenticationStateProvider), typeof(Csla.Blazor.Authentication.CslaAuthenticationStateProvider));
#if NET8_0_OR_GREATER
      // use Blazor state management
      config.Services.AddSingleton((p) 
        => (ISessionManager)ActivatorUtilities.CreateInstance(p, blazorOptions.SessionManagerType));
#endif
      return config;
    }
  }

  /// <summary>
  /// Options for Blazor wasm-interactive.
  /// </summary>
  public class BlazorWebAssemblyConfigurationOptions
  {
    /// <summary>
    /// Gets or sets the type of the ISessionManager service.
    /// </summary>
    public Type SessionManagerType { get; set; } = typeof(SessionManager);
  }
}