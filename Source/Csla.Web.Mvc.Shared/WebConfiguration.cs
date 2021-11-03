#if NETSTANDARD2_0 || NETCOREAPP3_1 || NET5_0_OR_GREATER
//-----------------------------------------------------------------------
// <copyright file="WebConfiguration.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implement extension methods for .NET Core configuration</summary>
//-----------------------------------------------------------------------
using System;
using Microsoft.AspNetCore.Builder;

namespace Csla.Configuration
{
  /// <summary>
  /// Implement extension methods for .NET Core configuration
  /// </summary>
  public static class WebConfigurationExtensions
  {
    /// <summary>
    /// Configures the application to use CSLA .NET
    /// </summary>
    /// <param name="app">ApplicationBuilder object</param>
    public static IApplicationBuilder UseCsla(this IApplicationBuilder app)
    {
      return app;
    }

#if NET6_0_OR_GREATER
    /// <summary>
    /// Configures the application to use CSLA .NET
    /// </summary>
    /// <param name="app">ApplicationBuilder object</param>
    public static WebApplicationBuilder UseCsla(this WebApplicationBuilder app)
    {
      return app;
    }
#endif
  }
}
#endif