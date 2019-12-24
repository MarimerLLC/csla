//-----------------------------------------------------------------------
// <copyright file="WebConfiguration.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implement extension methods for .NET Core configuration</summary>
//-----------------------------------------------------------------------
using System;
using Csla.Blazor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;

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
      builder.Services.AddTransient(typeof(ViewModel<>), typeof(ViewModel<>));
      builder.Services.AddSingleton<IAuthorizationPolicyProvider, CslaPermissionsPolicyProvider>();
      builder.Services.AddSingleton<IAuthorizationHandler, CslaPermissionsHandler>();
      return builder;
    }

    /// <summary>
    /// Configures services to provide CSLA Blazor 
    /// WebAssembly client support
    /// </summary>
    /// <param name="builder">ICslaBuilder instance</param>
    /// <returns></returns>
    public static ICslaBuilder WithBlazorClientSupport(this ICslaBuilder builder)
    {
      builder.Services.AddTransient(typeof(ViewModel<>), typeof(ViewModel<>));
      builder.Services.AddSingleton<IAuthorizationPolicyProvider, CslaPermissionsPolicyProvider>();
      builder.Services.AddSingleton<IAuthorizationHandler, CslaPermissionsHandler>();
      return builder;
    }

    /// <summary>
    /// Configures the application to use CSLA .NET
    /// </summary>
    /// <param name="app">ApplicationBuilder object</param>
    public static IComponentsApplicationBuilder UseCsla(this IComponentsApplicationBuilder app)
    {
      return UseCsla(app, null);
    }

    /// <summary>
    /// Configures the application to use CSLA .NET
    /// </summary>
    /// <param name="app">ApplicationBuilder object</param>
    /// <param name="config">Implement to configure CSLA .NET</param>
    public static IComponentsApplicationBuilder UseCsla(
      this IComponentsApplicationBuilder app, Action<CslaConfiguration> config)
    {
      CslaConfiguration.Configure().
        ContextManager(typeof(Csla.Blazor.ApplicationContextManager));
      config?.Invoke(CslaConfiguration.Configure());
      return app;
    }
  }
}