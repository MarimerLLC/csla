//-----------------------------------------------------------------------
// <copyright file="HttpProxyExtensions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implement extension methods data portal channel</summary>
//-----------------------------------------------------------------------

using Csla.DataPortalClient;
using Csla.Channels.Http;
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
    /// <exception cref="ArgumentNullException"><paramref name="config"/> is <see langword="null"/>.</exception>
    public static DataPortalClientOptions UseHttpProxy(this DataPortalClientOptions config, Action<HttpProxyOptions>? options)
    {
      Guard.NotNull(config);

      var proxyOptions = new HttpProxyOptions();
      options?.Invoke(proxyOptions);
      config.Services.AddTransient<IDataPortalProxy>(
        sp => 
        {
          var applicationContext = sp.GetRequiredService<ApplicationContext>();
          var dataPortalOptions = sp.GetRequiredService<DataPortalOptions>();
          var client = proxyOptions.HttpClientFactory(sp);
          return new HttpProxy(applicationContext, client, proxyOptions, dataPortalOptions);
        });
      return config;
    }
  }
}
