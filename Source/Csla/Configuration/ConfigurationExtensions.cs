#if NETSTANDARD2_0 || NET5_0 || NET6_0
//-----------------------------------------------------------------------
// <copyright file="ConfigurationExtensions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implement extension methods for base .NET configuration</summary>
//-----------------------------------------------------------------------
using System;
using System.Linq;
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
    public static IServiceCollection AddCsla(this IServiceCollection services)
    {
      return AddCsla(services, null);
    }

    /// <summary>
    /// Add CSLA .NET services for use by the application.
    /// </summary>
    /// <param name="services">ServiceCollection object</param>
    /// <param name="options">Options for configuring CSLA .NET</param>
    public static IServiceCollection AddCsla(this IServiceCollection services, Action<CslaConfiguration> options)
    {
      // Custom configuration
      var cslaOptions = new CslaConfiguration(services);
      options?.Invoke(cslaOptions);

      // ApplicationContext defaults
      services.AddScoped<ApplicationContext>();
      RegisterContextManager(services);

      // Data portal API defaults
      services.TryAddTransient(typeof(IDataPortal<>), typeof(DataPortal<>));
      services.TryAddTransient(typeof(IChildDataPortal<>), typeof(DataPortal<>));

      // LocalProxy is always necessary to support RunLocal
      services.TryAddTransient((p) => new Channels.Local.LocalProxyOptions());
      services.AddTransient<Channels.Local.LocalProxy, Channels.Local.LocalProxy>();

      // Default to using LocalProxy and local data portal
      var proxyInit = services.Where(i => i.ServiceType.Equals(typeof(IDataPortalProxy))).Any();
      if (!proxyInit)
      {
        cslaOptions.DataPortal().UseLocalProxy();
        cslaOptions.DataPortal().AddServerSideDataPortal();
      }

      return services;
    }

    private static void RegisterContextManager(IServiceCollection services)
    {
      var contextManagerType = typeof(Core.IContextManager);

      var managerInit = services.Where(i => i.ServiceType.Equals(contextManagerType)).Any();
      if (managerInit) return;

      if (LoadContextManager(services, "Csla.AspNetCore.ApplicationContextManager, Csla.AspNetCore")) return;
      if (LoadContextManager(services, "Csla.Xaml.ApplicationContextManager, Csla.Xaml")) return;
      if (LoadContextManager(services, "Csla.Web.Mvc.ApplicationContextManager, Csla.Web.Mvc")) return;
      if (LoadContextManager(services, "Csla.Web.ApplicationContextManager, Csla.Web")) return;
      if (LoadContextManager(services, "Csla.Windows.Forms.ApplicationContextManager, Csla.Windows.Forms")) return;

      // default to AsyncLocal context manager
      services.AddScoped(contextManagerType, typeof(Core.ApplicationContextManager));
    }

    private static bool LoadContextManager(IServiceCollection services, string managerTypeName)
    {
      var managerType = Type.GetType(managerTypeName, false);
      if (managerType != null)
      {
        services.AddScoped(typeof(Core.IContextManager), managerType);
        return true;
      }
      return false;
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