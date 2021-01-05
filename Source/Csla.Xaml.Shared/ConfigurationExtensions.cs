//-----------------------------------------------------------------------
// <copyright file="WebConfiguration.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implement extension methods for .NET Core configuration</summary>
//-----------------------------------------------------------------------
using System;
using Microsoft.Extensions.Hosting;

namespace Csla.Configuration
{
  /// <summary>
  /// Implement extension methods for .NET Core configuration
  /// </summary>
  public static class XamlConfigurationExtensions
  {
    /// <summary>
    /// Configures the application to use CSLA .NET
    /// </summary>
    /// <param name="builder">HostBuilder instance</param>
    public static HostBuilder UseCsla(this HostBuilder builder)
    {
      UseCsla(builder, null);
      return builder;
    }

    /// <summary>
    /// Configures the application to use CSLA .NET
    /// </summary>
    /// <param name="builder">HostBuilder instance</param>
    /// <param name="config">Implement to configure CSLA .NET</param>
    public static HostBuilder UseCsla(
      this HostBuilder builder, Action<CslaConfiguration> config)
    {
      CslaConfiguration.Configure().
        ContextManager(typeof(Csla.Xaml.ApplicationContextManager));
      config?.Invoke(CslaConfiguration.Configure());
      return builder;
    }
  }
}
