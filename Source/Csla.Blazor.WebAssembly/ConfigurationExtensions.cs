//-----------------------------------------------------------------------
// <copyright file="BlazorWasmConfigurationExtensions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implement extension methods for .NET Core configuration</summary>
//-----------------------------------------------------------------------
using System;
using Csla.Blazor;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Csla.Configuration
{
  /// <summary>
  /// Implement extension methods for .NET Core configuration
  /// </summary>
  public static class BlazorWasmConfigurationExtensions
  {
    /// <summary>
    /// Configures the application to use CSLA .NET
    /// </summary>
    /// <param name="builder">IWebAssemblyHostBuilder object</param>
    public static WebAssemblyHostBuilder UseCsla(this WebAssemblyHostBuilder builder)
    {
      return UseCsla(builder, null);
    }

    /// <summary>
    /// Configures the application to use CSLA .NET
    /// in a Blazor WebAssembly runtime
    /// </summary>
    /// <param name="builder">IWebAssemblyHostBuilder object</param>
    /// <param name="config">Implement to configure CSLA .NET</param>
    public static WebAssemblyHostBuilder UseCsla(
      this WebAssemblyHostBuilder builder, Action<CslaConfiguration> config)
    {
      builder.Services.TryAddTransient(typeof(ViewModel<>), typeof(ViewModel<>));
      builder.Services.TryAddSingleton<IAuthorizationPolicyProvider, CslaPermissionsPolicyProvider>();
      builder.Services.TryAddSingleton<IAuthorizationHandler, CslaPermissionsHandler>();
      builder.Services.TryAddSingleton(typeof(Csla.Core.IContextManager), typeof(Csla.Core.ApplicationContextManagerStatic));

      Csla.Channels.Http.HttpProxy.UseTextSerialization = true;

      config?.Invoke(CslaConfiguration.Configure());

      return builder;
    }
  }
}