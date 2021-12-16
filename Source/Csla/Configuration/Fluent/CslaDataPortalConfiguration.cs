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
  public static class CslaDataPortalConfigurationExtension
  {
    /// <summary>
    /// Extension method for CslaDataPortalConfiguration
    /// </summary>
    public static CslaDataPortalConfiguration DataPortal(this ICslaConfiguration config)
    {
      return DataPortal(config, null);
    }
    
    /// <summary>
    /// Extension method for CslaDataPortalConfiguration
    /// </summary>
    /// <param name="config"></param>
    /// <param name="options"></param>
    public static CslaDataPortalConfiguration DataPortal(this ICslaConfiguration config, Action<DataPortalClientOptions> options)
    {
      var opt = new DataPortalClientOptions();
      options?.Invoke(opt);
      config.Services.AddScoped((p) => opt);
      return new CslaDataPortalConfiguration(config);
    }

    /// <summary>
    /// Add services required to host the server-side
    /// data portal.
    /// </summary>
    /// <param name="builder"></param>
    public static CslaDataPortalConfiguration AddServerSideDataPortal(this CslaDataPortalConfiguration builder)
    {
      return AddServerSideDataPortal(builder, null);
    }

    /// <summary>
    /// Add services required to host the server-side
    /// data portal.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="options"></param>
    public static CslaDataPortalConfiguration AddServerSideDataPortal(this CslaDataPortalConfiguration builder, Action<DataPortalServerOptions> options)
    {
      var opt = new DataPortalServerOptions();
      options?.Invoke(opt);
      var services = builder.Services;
      services.AddScoped((p) => opt);
      services.AddScoped(typeof(IAuthorizeDataPortal), opt.AuthorizerProviderType);
      services.AddScoped((p) => opt.InterceptorProviders);
      services.AddScoped(typeof(IObjectFactoryLoader), opt.ObjectFactoryLoaderType);
      services.AddTransient(typeof(IDataPortalActivator), opt.ActivatorType);
      services.AddScoped(typeof(IDataPortalExceptionInspector), opt.ExceptionInspectorType);

      services.TryAddTransient(typeof(IDataPortalServer), typeof(DataPortal));
      services.TryAddTransient<DataPortalSelector>();
      services.TryAddTransient<SimpleDataPortal>();
      services.TryAddTransient<FactoryDataPortal>();
      services.TryAddTransient<DataPortalBroker>();
      services.TryAddSingleton(typeof(Server.Dashboard.IDashboard), typeof(Server.Dashboard.NullDashboard));
      return builder;
    }
  }

  /// <summary>
  /// Use this type to configure the settings for the
  /// CSLA .NET data portal.
  /// </summary>
  public class CslaDataPortalConfiguration
  {
    /// <summary>
    /// Gets or sets the current configuration object.
    /// </summary>
    public ICslaConfiguration CslaConfiguration { get; set; }

    /// <summary>
    /// Gets the current service collection.
    /// </summary>
    public IServiceCollection Services { get => CslaConfiguration.Services; }

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="config">Current configuration object</param>
    public CslaDataPortalConfiguration(ICslaConfiguration config)
    {
      CslaConfiguration = config;
    }
  }
}
