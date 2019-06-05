#if NETSTANDARD2_0
//-----------------------------------------------------------------------
// <copyright file="WebConfiguration.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implement extension methods for .NET Core configuration</summary>
//-----------------------------------------------------------------------
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;

namespace Csla.Configuration
{
  /// <summary>
  /// Implement extension methods for .NET Core configuration
  /// </summary>
  public static class WebConfigurationExtensions
  {
    /// <summary>
    /// Adds CSLA to the application
    /// </summary>
    /// <param name="config">ApplicationBuilder object</param>
    public static IApplicationBuilder UseCsla(this IApplicationBuilder app)
    {
      Csla.ApplicationContext.DefaultServiceProvider = app.ApplicationServices;
      return app;
    }
  }
}
#endif