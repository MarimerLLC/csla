//-----------------------------------------------------------------------
// <copyright file="CslaDataPortalConfiguration.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Use this type to configure the settings for CSLA .NET</summary>
//-----------------------------------------------------------------------
using System;
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
    public static DataPortalClientOptions DataPortal(this CslaOptions config)
    {
      return DataPortal(config, null);
    }
    
    /// <summary>
    /// Extension method for CslaDataPortalConfiguration
    /// </summary>
    /// <param name="config"></param>
    /// <param name="options"></param>
    public static DataPortalClientOptions DataPortal(this CslaOptions config, Action<DataPortalClientOptions> options)
    {
      options?.Invoke(config.DataPortalClientOptions);
      return config.DataPortalClientOptions;
    }

    /// <summary>
    /// Add services required to host the server-side
    /// data portal.
    /// </summary>
    /// <param name="builder"></param>
    public static DataPortalClientOptions AddServerSideDataPortal(this DataPortalClientOptions builder)
    {
      return AddServerSideDataPortal(builder, null);
    }

    /// <summary>
    /// Add services required to host the server-side
    /// data portal.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="options"></param>
    public static DataPortalClientOptions AddServerSideDataPortal(this DataPortalClientOptions builder, Action<DataPortalServerOptions> options)
    {
      var opt = builder.DataPortalServerOptions;
      options?.Invoke(opt);
      var services = builder.Services;
      services.TryAddScoped(typeof(IAuthorizeDataPortal), opt.AuthorizerProviderType);
      services.TryAddScoped((p) => opt.InterceptorProviders);
      services.TryAddScoped(typeof(IObjectFactoryLoader), opt.ObjectFactoryLoaderType);
      services.TryAddScoped(typeof(IDataPortalActivator), opt.ActivatorType);
      services.TryAddScoped(typeof(IDataPortalExceptionInspector), opt.ExceptionInspectorType);

      services.TryAddScoped<DataPortalExceptionHandler>();
      services.TryAddTransient(typeof(IDataPortalServer), typeof(DataPortal));
      services.TryAddScoped<InterceptorManager>();
      services.TryAddTransient<DataPortalSelector>();
      services.TryAddTransient<SimpleDataPortal>();
      services.TryAddTransient<FactoryDataPortal>();
      services.TryAddTransient<DataPortalBroker>();
      services.TryAddSingleton(typeof(Server.Dashboard.IDashboard), typeof(Server.Dashboard.NullDashboard));
      return builder;
    }
  }
}
