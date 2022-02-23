//-----------------------------------------------------------------------
// <copyright file="AspNetCoreConfigurationExtensions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implement extension methods for AspNet configuration</summary>
//-----------------------------------------------------------------------
using System;
#if NET5_0_OR_GREATER
using Csla.AspNetCore.Blazor;
using Microsoft.AspNetCore.Components.Server.Circuits;
#endif
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Csla.Configuration
{
  /// <summary>
  /// Implement extension methods for AspNet configuration
  /// </summary>
  public static class AspNetCoreConfigurationExtensions
  {
    /// <summary>
    /// Configures services to provide CSLA AspNetCore support
    /// </summary>
    /// <param name="config">CslaOptions instance</param>
    /// <returns></returns>
    public static CslaOptions AddAspNetCore(this CslaOptions config)
    {
      return AddAspNetCore(config, null);
    }

    /// <summary>
    /// Configures services to provide CSLA AspNetCore support
    /// </summary>
    /// <param name="config">CslaOptions instance</param>
    /// <param name="options">Options object</param>
    /// <returns></returns>
    public static CslaOptions AddAspNetCore(this CslaOptions config, Action<AspNetCoreConfigurationOptions> options)
    {
      var localOptions = new AspNetCoreConfigurationOptions();
      options?.Invoke(localOptions);
#if NET5_0_OR_GREATER
      config.Services.AddScoped<ActiveCircuitState>();
      config.Services.AddScoped(typeof(CircuitHandler), typeof(ActiveCircuitHandler));
#endif
      return config;
    }
  }

  /// <summary>
  /// Options that can be provided to the AddAspNetCore
  /// method.
  /// </summary>
  public class AspNetCoreConfigurationOptions
  {
    /// <summary>
    /// Indicates whether the app should be configured to
    /// use CSLA permissions policies (default = true).
    /// </summary>
    public bool UseCslaPermissionsPolicy { get; set; } = true;
  }
}
