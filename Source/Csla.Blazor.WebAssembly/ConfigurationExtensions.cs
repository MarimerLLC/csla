//-----------------------------------------------------------------------
// <copyright file="BlazorWasmConfigurationExtensions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implement extension methods for .NET Core configuration</summary>
//-----------------------------------------------------------------------
using Csla.Blazor;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Csla.Configuration
{
  /// <summary>
  /// Implement extension methods for Blazor WebAssembly
  /// </summary>
  public static class BlazorWasmConfigurationExtensions
  {
    /// <summary>
    /// Registers services necessary for Windows Forms
    /// </summary>
    /// <param name="config">CslaConfiguration object</param>
    /// <returns></returns>
    public static ICslaConfiguration WithBlazorWebAssembly(this ICslaConfiguration config)
    {
      config.Services.TryAddTransient(typeof(ViewModel<>), typeof(ViewModel<>));
      config.Services.TryAddSingleton<IAuthorizationPolicyProvider, CslaPermissionsPolicyProvider>();
      config.Services.TryAddSingleton<IAuthorizationHandler, CslaPermissionsHandler>();
      config.Services.TryAddSingleton(typeof(Csla.Core.IContextManager), typeof(Csla.Core.ApplicationContextManagerStatic));
      return config;
    }

    /// <summary>
    /// Configures the application to use CSLA .NET
    /// </summary>
    /// <param name="builder">IWebAssemblyHostBuilder object</param>
    public static WebAssemblyHostBuilder UseCsla(this WebAssemblyHostBuilder builder)
    {
      Csla.Channels.Http.HttpProxy.UseTextSerialization = true;
      return builder;
    }
  }
}