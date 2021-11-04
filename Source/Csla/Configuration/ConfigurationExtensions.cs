#if NETSTANDARD2_0 || NET5_0 || NET6_0
//-----------------------------------------------------------------------
// <copyright file="ConfigurationExtensions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implement extension methods for base .NET configuration</summary>
//-----------------------------------------------------------------------
using System;
using Csla.DataPortalClient;
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
    /// <param name="options">Options for configuring CSLA .NET</param>
    public static ICslaBuilder AddCsla(this IServiceCollection services, Action<CslaConfiguration> options)
    {
      var builder = new CslaBuilder(services);

      // Configuration
      var cslaOptions = new CslaConfiguration(services);
      options?.Invoke(cslaOptions);

      // ApplicationContext
      services.TryAddScoped<ApplicationContext>();
      services.TryAddScoped(typeof(Core.IContextManager), typeof(Core.ApplicationContextManager));

      // Data portal services
      if (cslaOptions.DataPortal().UseDataPortalServer)
      {
        services.TryAddTransient(typeof(Server.IDataPortalServer), typeof(Csla.Server.DataPortal));
        services.TryAddTransient<Server.DataPortalSelector>();
        services.TryAddTransient<Server.SimpleDataPortal>();
        services.TryAddTransient<Server.FactoryDataPortal>();
        services.TryAddTransient<Server.DataPortalBroker>();
        services.TryAddSingleton(typeof(Server.Dashboard.IDashboard), typeof(Csla.Server.Dashboard.NullDashboard));
      }

      // Data portal API
      services.TryAddTransient(typeof(IDataPortal<>), typeof(DataPortal<>));
      services.TryAddTransient(typeof(IChildDataPortal<>), typeof(DataPortal<>));

      return builder;
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