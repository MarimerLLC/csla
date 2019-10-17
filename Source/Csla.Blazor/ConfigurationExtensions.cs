//-----------------------------------------------------------------------
// <copyright file="WebConfiguration.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implement extension methods for .NET Core configuration</summary>
//-----------------------------------------------------------------------
using System;
using Microsoft.AspNetCore.Components.Builder;

namespace Csla.Configuration
{
  /// <summary>
  /// Implement extension methods for .NET Core configuration
  /// </summary>
  public static class BlazorConfigurationExtensions
  {
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