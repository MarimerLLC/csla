//-----------------------------------------------------------------------
// <copyright file="CslaDataPortalConfiguration.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Use this type to configure the settings for CSLA .NET</summary>
//-----------------------------------------------------------------------
using System;
using Csla.DataPortalClient;
using Csla.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Csla.Configuration
{
  /// <summary>
  /// Extension method for CslaDataPortalConfiguration
  /// </summary>
  public static class DataPortalConfigurationExtensions
  {
    /// <summary>
    /// Extension method for CslaDataPortalConfiguration
    /// </summary>
    public static CslaOptions DataPortal(this CslaOptions config)
    {
      return DataPortal(config, null);
    }
    
    /// <summary>
    /// Extension method for CslaDataPortalConfiguration
    /// </summary>
    /// <param name="config"></param>
    /// <param name="options"></param>
    public static CslaOptions DataPortal(this CslaOptions config, Action<DataPortalClientOptions> options)
    {
      options?.Invoke(config.DataPortalClientOptions);
      return config;
    }

    /// <summary>
    /// Add services required to host the server-side
    /// data portal.
    /// </summary>
    /// <param name="config">The configuration that is to be affected</param>
    public static DataPortalClientOptions AddServerSideDataPortal(this DataPortalClientOptions config)
    {
      return AddServerSideDataPortal(config, null);
    }

    /// <summary>
    /// Add services required to host the server-side
    /// data portal.
    /// </summary>
    /// <param name="config">The configuration that is to be affected</param>
    /// <param name="options">The action that is to be used to influence the configuration options</param>
    public static DataPortalClientOptions AddServerSideDataPortal(this DataPortalClientOptions config, Action<DataPortalServerOptions> options)
    {
      options?.Invoke(config.ServerOptions);
      return config;
    }

    /// <summary>
    /// Set up the data portal by applying the configuration that has been built
    /// </summary>
    /// <param name="config">The configuration to use in setting up the data portal</param>
    internal static void AddRequiredDataPortalServices(this CslaOptions config)
    {
      var services = config.Services;

      // LocalProxy must always be available to support RunLocal
      services.TryAddTransient((p) => new Channels.Local.LocalProxyOptions());
      services.AddTransient<Channels.Local.LocalProxy, Channels.Local.LocalProxy>();

      services.TryAddTransient<IDataPortalFactory, DataPortalFactory>();
      services.TryAddTransient<IChildDataPortalFactory, ChildDataPortalFactory>();

      services.TryAddScoped(typeof(IAuthorizeDataPortal), config.DataPortalServerOptions.AuthorizerProviderType);
      foreach (Type interceptorType in config.DataPortalServerOptions.InterceptorProviders)
      {
        services.AddScoped(typeof(IInterceptDataPortal), interceptorType);
      }
      services.TryAddScoped(typeof(IObjectFactoryLoader), config.DataPortalServerOptions.ObjectFactoryLoaderType);
      services.TryAddScoped(typeof(IDataPortalActivator), config.DataPortalServerOptions.ActivatorType);
      services.TryAddScoped(typeof(IDataPortalExceptionInspector), config.DataPortalServerOptions.ExceptionInspectorType);

      services.TryAddScoped<DataPortalExceptionHandler>();
      services.TryAddTransient(typeof(IDataPortalServer), typeof(DataPortal));
      services.TryAddScoped<InterceptorManager>();
      services.TryAddTransient<DataPortalSelector>();
      services.TryAddTransient<SimpleDataPortal>();
      services.TryAddTransient<FactoryDataPortal>();
      services.TryAddTransient<DataPortalBroker>();
      services.TryAddSingleton(typeof(Server.Dashboard.IDashboard), typeof(Server.Dashboard.NullDashboard));
    }
  }
}
