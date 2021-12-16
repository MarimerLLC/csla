//-----------------------------------------------------------------------
// <copyright file="CslaDataPortalConfiguration.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Use this type to configure the settings for CSLA .NET</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
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
      if (opt.ObjectFactoryLoaderType != null)
        services.AddScoped(typeof(IObjectFactoryLoader), opt.ObjectFactoryLoaderType);
      if (opt.ActivatorType != null)
        services.AddTransient(typeof(IDataPortalActivator), opt.ActivatorType);
      if (opt.ExceptionInspectorType != null)
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

  /// <summary>
  /// Server-side data portal options.
  /// </summary>
  public class DataPortalServerOptions
  {
    /// <summary>
    /// Gets or sets a value indicating whether objects should be
    /// automatically cloned by the data portal Update()
    /// method when using a local data portal configuration.
    /// </summary>
    public bool AutoCloneOnUpdate
    {
      get
      {
        bool result = true;
        string setting = ConfigurationManager.AppSettings["CslaAutoCloneOnUpdate"];
        if (!string.IsNullOrEmpty(setting))
          result = bool.Parse(setting);
        return result;
      }
      set 
      {
        ConfigurationManager.AppSettings["CslaAutoCloneOnUpdate"] = value.ToString();
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the
    /// server-side business object should be returned to
    /// the client as part of the DataPortalException.
    /// </summary>
    /// <param name="value">Value (default is false)</param>
    public bool DataPortalReturnObjectOnException
    {
      get
      {
        bool result = false;
        string setting = ConfigurationManager.AppSettings["CslaDataPortalReturnObjectOnException"];
        if (!string.IsNullOrEmpty(setting))
          result = bool.Parse(setting);
        return result;
      }
      set
      {
        ConfigurationManager.AppSettings["CslaDataPortalReturnObjectOnException"] = value.ToString();
      }
    }

    /// <summary>
    /// Gets or sets a value containing the type of the
    /// IDataPortalAuthorizer to be used by the data portal.
    /// An instance of this type is created using dependency
    /// injection.
    /// </summary>
    public Type AuthorizerProviderType { get; set; } = typeof(ActiveAuthorizer);

    /// <summary>
    /// Gets a list of the IInterceptDataPortal instances
    /// that should be executed by the server-side data portal.
    /// injection.
    /// </summary>
    public List<IInterceptDataPortal> InterceptorProviders { get; } = new List<IInterceptDataPortal>();

    /// <summary>
    /// Gets or sets the type of the ExceptionInspector.
    /// </summary>
    public Type ExceptionInspectorType { get; set; }

    /// <summary>
    /// Gets or sets the type of the Activator.
    /// </summary>
    public Type ActivatorType { get; set; }

    /// <summary>
    /// Gets or sets the type name of the factor loader used to create
    /// server-side instances of business object factories when using
    /// the FactoryDataPortal model. Type must implement
    /// <see cref="IObjectFactoryLoader"/>.
    /// </summary>
    public Type ObjectFactoryLoaderType { get; set; }
  }
}
