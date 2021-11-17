//-----------------------------------------------------------------------
// <copyright file="XamlConfigurationExtensions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implement extension methods for .NET Core configuration</summary>
//-----------------------------------------------------------------------
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Csla.Configuration
{
  /// <summary>
  /// Implement extension methods for Xaml
  /// </summary>
  public static class XamlConfigurationExtensions
  {
    /// <summary>
    /// Registers services necessary for Xaml-based
    /// environments.
    /// </summary>
    /// <param name="config">CslaConfiguration object</param>
    /// <returns></returns>
    public static ICslaConfiguration WithXaml(this ICslaConfiguration config)
    {
      config.Services.TryAddScoped(typeof(Csla.Core.IContextManager), typeof(Csla.Xaml.ApplicationContextManager));
      return config;
    }

    /// <summary>
    /// Configures the application to use CSLA .NET
    /// </summary>
    /// <param name="builder">HostBuilder instance</param>
    public static HostBuilder UseCsla(this HostBuilder builder)
    {
      return builder;
    }
  }
}
