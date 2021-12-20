//-----------------------------------------------------------------------
// <copyright file="HttpProxyExtensions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implement extension methods data portal channel</summary>
//-----------------------------------------------------------------------
using System;
using System.Net.Http;
using Csla.DataPortalClient;
using Microsoft.Extensions.DependencyInjection;

namespace Csla.Configuration
{
  /// <summary>
  /// Implement extension methods data portal channel
  /// </summary>
  public static class HttpProxyExtensions
  {
    /// <summary>
    /// Configure data portal client to use HttpProxy.
    /// </summary>
    /// <param name="config">CslaDataPortalConfiguration object</param>
    /// <param name="options">Data portal proxy options</param>
    public static DataPortalClientOptions UseHttpProxy(this DataPortalClientOptions config, Action<Csla.Channels.Http.HttpProxyOptions> options)
    {
      var proxyOptions = new Csla.Channels.Http.HttpProxyOptions();
      options?.Invoke(proxyOptions);
      config.Services.AddTransient(typeof(IDataPortalProxy),
        sp =>
        {
          var applicationContext = sp.GetRequiredService<ApplicationContext>();
          var client = sp.GetRequiredService<HttpClient>();
          return new Csla.Channels.Http.HttpProxy(applicationContext, client, proxyOptions);
        });
      return config;
    }
  }
}
