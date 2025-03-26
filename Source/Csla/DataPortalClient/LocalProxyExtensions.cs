﻿//-----------------------------------------------------------------------
// <copyright file="LocalProxyExtensions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implement extension methods data portal channel</summary>
//-----------------------------------------------------------------------

using Csla.Channels.Local;
using Csla.DataPortalClient;
using Microsoft.Extensions.DependencyInjection;

namespace Csla.Configuration
{
  /// <summary>
  /// Implement extension methods data portal channel
  /// </summary>
  public static class LocalProxyExtensions
  {
    /// <summary>
    /// Configure data portal client to use LocalProxy.
    /// </summary>
    /// <param name="config">CslaDataPortalConfiguration object</param>
    /// <exception cref="ArgumentNullException"><paramref name="config"/> is <see langword="null"/>.</exception>
    public static DataPortalClientOptions UseLocalProxy(this DataPortalClientOptions config)
    {
      return UseLocalProxy(config, null);
    }

    /// <summary>
    /// Configure data portal client to use LocalProxy.
    /// </summary>
    /// <param name="config">CslaDataPortalConfiguration object</param>
    /// <param name="options">Data portal proxy options</param>
    /// <exception cref="ArgumentNullException"><paramref name="config"/> is <see langword="null"/>.</exception>
    public static DataPortalClientOptions UseLocalProxy(this DataPortalClientOptions config, Action<LocalProxyOptions>? options)
    {
      if (config is null)
        throw new ArgumentNullException(nameof(config));

      var proxyOptions = new LocalProxyOptions();
      options?.Invoke(proxyOptions);
      config.Services.AddTransient<IDataPortalProxy, LocalProxy>();
      config.Services.AddTransient(_ => proxyOptions);
      return config;
    }
  }
}
