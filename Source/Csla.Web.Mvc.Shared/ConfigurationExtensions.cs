//-----------------------------------------------------------------------
// <copyright file="AspNetCoreConfigurationExtensions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implement extension methods for AspNet configuration</summary>
//-----------------------------------------------------------------------
#if NETSTANDARD2_0 || NET5_0_OR_GREATER || NETCOREAPP3_1
using System;
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
    /// <param name="config">ICslaConfiguration instance</param>
    /// <returns></returns>
    public static ICslaConfiguration AddAspNetCore(this ICslaConfiguration config)
    {
      return AddAspNetCore(config, null);
    }

    /// <summary>
    /// Configures services to provide CSLA AspNetCore support
    /// </summary>
    /// <param name="config">ICslaConfiguration instance</param>
    /// <param name="options">Options object</param>
    /// <returns></returns>
    public static ICslaConfiguration AddAspNetCore(this ICslaConfiguration config, Action<AspNetCoreConfigurationOptions> options)
    {
      var localOptions = new AspNetCoreConfigurationOptions();
      options?.Invoke(localOptions);
      config.Services.TryAddTransient((p) => new Channels.Local.LocalProxyOptions { CreateScopePerCall = false });
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
#endif