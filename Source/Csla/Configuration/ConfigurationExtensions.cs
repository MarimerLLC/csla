#if NETSTANDARD2_0 || NET5_0 || NET6_0
//-----------------------------------------------------------------------
// <copyright file="ConfigurationExtensions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implement extension methods for base .NET configuration</summary>
//-----------------------------------------------------------------------
using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Csla.Configuration
{
  /// <summary>
  /// Implement extension methods for base .NET configuration
  /// </summary>
  public static class ConfigurationExtensions
  {
    /// <summary>
    /// Add CSLA .NET services for use by the application.
    /// </summary>
    /// <param name="services">ServiceCollection object</param>
    public static ICslaBuilder AddCsla(this IServiceCollection services)
    {
      return AddCsla(services, null);
    }

    /// <summary>
    /// Add CSLA .NET services for use by the application.
    /// </summary>
    /// <param name="services">ServiceCollection object</param>
    /// <param name="config">Implement to configure CSLA .NET</param>
    public static ICslaBuilder AddCsla(this IServiceCollection services, Action<CslaConfiguration> config)
    {
      services.TryAddScoped<ApplicationContext>();
      services.TryAddScoped(typeof(Core.IContextManager), typeof(Core.ApplicationContextManager));
      services.TryAddTransient(typeof(IDataPortal<>), typeof(DataPortal<>));
      config?.Invoke(CslaConfiguration.Configure());
      return new CslaBuilder(services);
    }

    /// <summary>
    /// Configure CSLA .NET settings from .NET Core configuration
    /// subsystem.
    /// </summary>
    /// <param name="config">Configuration object</param>
    public static IConfiguration ConfigureCsla(this IConfiguration config)
    {
      config.Bind("csla", new CslaConfigurationOptions());
      return config;
    }
  }
}
#endif