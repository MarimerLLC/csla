//-----------------------------------------------------------------------
// <copyright file="RabbitMqProxyExtensions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implement extension methods data portal channel</summary>
//-----------------------------------------------------------------------

using Csla.DataPortalClient;
using Microsoft.Extensions.DependencyInjection;

namespace Csla.Configuration
{
  /// <summary>
  /// Implement extension methods data portal channel
  /// </summary>
  public static class RabbitMqProxyExtensions
  {
    /// <summary>
    /// Configure data portal client to use RabbitMqProxy.
    /// </summary>
    /// <param name="config">CslaDataPortalConfiguration object</param>
    /// <param name="options">Data portal proxy options</param>
    /// <exception cref="ArgumentNullException"><paramref name="config"/> or <paramref name="options"/> is <see langword="null"/>.</exception>
    public static DataPortalClientOptions UseRabbitMqProxy(this DataPortalClientOptions config, Action<Channels.RabbitMq.RabbitMqProxyOptions> options)
    {
      if (config is null)
        throw new ArgumentNullException(nameof(config));
      if (options is null)
        throw new ArgumentNullException(nameof(options));

      var proxyOptions = new Channels.RabbitMq.RabbitMqProxyOptions();
      options.Invoke(proxyOptions);
      config.Services.AddTransient(typeof(IDataPortalProxy),
        sp =>
        {
          var applicationContext = sp.GetRequiredService<ApplicationContext>();
          return new Channels.RabbitMq.RabbitMqProxy(applicationContext, proxyOptions);
        });
      return config;
    }
  }
}
