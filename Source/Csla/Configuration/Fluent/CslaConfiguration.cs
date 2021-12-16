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
  public class CslaConfiguration : ICslaConfiguration
  {
    /// <summary>
    /// Gets or sets the current service collection.
    /// </summary>
    public IServiceCollection Services { get; set; }

    private readonly CslaDataPortalConfiguration _dataPortalConfiguration;
    /// <summary>
    /// Expose the data portal configuration information
    /// </summary>
    public CslaDataPortalConfiguration DataPortalConfiguration { get => _dataPortalConfiguration; }

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="services">Service collection</param>
    public CslaConfiguration(IServiceCollection services)
    {
      Services = services;
      _dataPortalConfiguration= new CslaDataPortalConfiguration(this);
    }

    /// <summary>
    /// Sets a value indicating whether CSLA
    /// should fallback to using reflection instead of
    /// System.Linq.Expressions (true, default).
    /// </summary>
    /// <param name="value">Value</param>
    public CslaConfiguration UseReflectionFallback(bool value)
    {
      ApplicationContext.UseReflectionFallback = value;
      return this;
    }

    /// <summary>
    /// Sets a value specifying how CSLA .NET should
    /// raise PropertyChanged events.
    /// </summary>
    /// <param name="mode">Property changed mode</param>
    public CslaConfiguration PropertyChangedMode(ApplicationContext.PropertyChangedModes mode)
    {
      ConfigurationManager.AppSettings["CslaPropertyChangedMode"] = mode.ToString();
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
    public CslaConfiguration VersionRoutingTag(string version)
    {
      if (!string.IsNullOrWhiteSpace(version))
        if (version.Contains("-") || version.Contains("/"))
          throw new ArgumentException("VersionRoutingTag");
      ConfigurationManager.AppSettings["CslaVersionRoutingTag"] = version;
      return this;
    }

    /// <summary>
    /// Sets the factory type that creates PropertyInfo objects.
    /// </summary>
    /// <param name="typeName">Factory type name</param>
    public CslaConfiguration PropertyInfoFactory(string typeName)
    {
      ConfigurationManager.AppSettings["CslaPropertyInfoFactory"] = typeName;
      return this;
    }
  }
}