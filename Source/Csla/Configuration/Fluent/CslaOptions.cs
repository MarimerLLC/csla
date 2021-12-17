//-----------------------------------------------------------------------
// <copyright file="CslaConfiguration.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Use this type to configure the settings for CSLA .NET</summary>
//-----------------------------------------------------------------------
using Csla.Core;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Csla.Configuration
{
  /// <summary>
  /// Use this type to configure the settings for CSLA .NET.
  /// </summary>
  public class CslaOptions
  {
    /// <summary>
    /// Gets the current service collection.
    /// </summary>
    public IServiceCollection Services { get; private set; }

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="services">Service collection</param>
    public CslaOptions(IServiceCollection services)
    {
      Services = services;
      DataPortalClientOptions = new DataPortalClientOptions(this);
    }

    /// <summary>
    /// Sets a value indicating whether CSLA
    /// should fallback to using reflection instead of
    /// System.Linq.Expressions (true, default).
    /// </summary>
    /// <param name="value">Value</param>
    public CslaOptions UseReflectionFallback(bool value)
    {
      ApplicationContext.UseReflectionFallback = value;
      return this;
    }

    /// <summary>
    /// Sets a value specifying how CSLA .NET should
    /// raise PropertyChanged events.
    /// </summary>
    /// <param name="mode">Property changed mode</param>
    public CslaOptions PropertyChangedMode(ApplicationContext.PropertyChangedModes mode)
    {
      ApplicationContext.PropertyChangedMode = mode;
      return this;
    }

    /// <summary>
    /// Sets a value representing the application version
    /// for use in server-side data portal routing.
    /// </summary>
    /// <param name="version">
    /// Application version used to create data portal
    /// routing tag (can not contain '-').
    /// </param>
    /// <remarks>
    /// If this value is set then you must use the
    /// .NET Core server-side Http data portal endpoint
    /// as a router so the request can be routed to
    /// another app server that is running the correct
    /// version of the application's assemblies.
    /// </remarks>
    public CslaOptions VersionRoutingTag(string version)
    {
      if (!string.IsNullOrWhiteSpace(version))
        if (version.Contains("-") || version.Contains("/"))
          throw new ArgumentException("VersionRoutingTag");
      ApplicationContext.VersionRoutingTag = version;
      return this;
    }

    /// <summary>
    /// Sets the factory type that creates PropertyInfo objects.
    /// </summary>
    public CslaOptions RegisterPropertyInfoFactory<T>() where T : IPropertyInfoFactory
    {
      Core.FieldManager.PropertyInfoFactory.FactoryType = typeof(T);
      return this;
    }

    /// <summary>
    /// Gets the SecurityOptions instance.
    /// </summary>
    internal SecurityOptions SecurityOptions { get; set; } = new SecurityOptions();
    /// <summary>
    /// Gets the SerializationOptions instance.
    /// </summary>
    internal SerializationOptions SerializationOptions { get; set; } = new SerializationOptions();
    /// <summary>
    /// Gets the DataPortalClientOptions instance.
    /// </summary>
    internal DataPortalClientOptions DataPortalClientOptions { get; private set; }
    /// <summary>
    /// Gets the DataPortalServerOptions instance.
    /// </summary>
    internal DataPortalServerOptions DataPortalServerOptions { get; private set; } = new DataPortalServerOptions();
    /// <summary>
    /// Gets the DataOptions instance.
    /// </summary>
    internal DataOptions DataOptions { get; set; } = new DataOptions();
  }
}