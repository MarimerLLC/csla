//-----------------------------------------------------------------------
// <copyright file="CslaDataPortalConfiguration.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Use this type to configure the settings for CSLA .NET</summary>
//-----------------------------------------------------------------------
using System;
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
      var services = builder.Services;
      services.TryAddTransient(typeof(Server.IDataPortalServer), typeof(Csla.Server.DataPortal));
      services.TryAddTransient<Server.DataPortalSelector>();
      services.TryAddTransient<Server.SimpleDataPortal>();
      services.TryAddTransient<Server.FactoryDataPortal>();
      services.TryAddTransient<Server.DataPortalBroker>();
      services.TryAddSingleton(typeof(Server.Dashboard.IDashboard), typeof(Csla.Server.Dashboard.NullDashboard));
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

    /// <summary>
    /// Sets a value indicating whether objects should be
    /// automatically cloned by the data portal Update()
    /// method when using a local data portal configuration.
    /// </summary>
    /// <param name="value">Value (defaults to true)</param>
    public CslaDataPortalConfiguration AutoCloneOnUpdate(bool value)
    {
      ConfigurationManager.AppSettings["CslaAutoCloneOnUpdate"] = value.ToString();
      return this;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the
    /// server-side business object should be returned to
    /// the client as part of the DataPortalException.
    /// </summary>
    /// <param name="value">Value (default is false)</param>
    public CslaDataPortalConfiguration DataPortalReturnObjectOnException(bool value)
    {
      ConfigurationManager.AppSettings["CslaDataPortalReturnObjectOnException"] = value.ToString();
      return this;
    }
  }
}
