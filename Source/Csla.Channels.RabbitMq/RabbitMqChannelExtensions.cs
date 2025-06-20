//-----------------------------------------------------------------------
// <copyright file="RabbitMqProxyExtensions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implement extension methods data portal channel</summary>
//-----------------------------------------------------------------------

using Csla.Channels.RabbitMq;
using Csla.DataPortalClient;
using Microsoft.Extensions.DependencyInjection;

namespace Csla.Configuration
{
  /// <summary>
  /// Implement extension methods data portal channel
  /// </summary>
  public static class RabbitMqChannelExtensions
  {
    /// <summary>
    /// Configure data portal client to use RabbitMqProxy.
    /// </summary>
    /// <param name="config">CslaDataPortalConfiguration object</param>
    /// <param name="options">Data portal proxy options</param>
    /// <exception cref="ArgumentNullException"><paramref name="config"/> or <paramref name="options"/> is <see langword="null"/>.</exception>
    public static DataPortalClientOptions UseRabbitMqProxy(this DataPortalClientOptions config, Action<RabbitMqProxyOptions> options)
    {
      if (config is null)
        throw new ArgumentNullException(nameof(config));
      if (options is null)
        throw new ArgumentNullException(nameof(options));

      var proxyOptions = new RabbitMqProxyOptions();
      options.Invoke(proxyOptions);
      config.Services.AddTransient(typeof(IDataPortalProxy),
        sp =>
        {
          var applicationContext = sp.GetRequiredService<ApplicationContext>();
          return new RabbitMqProxy(applicationContext, proxyOptions);
        });
      return config;
    }

    /// <summary>
    /// Configure the data portal server to use the RabbitMq channel.
    /// </summary>
    /// <param name="config"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static DataPortalServerOptions UseRabbitMqPortal(this DataPortalServerOptions config, Action<RabbitMqPortalOptions>? options)
    {
      if (config is null)
        throw new ArgumentNullException(nameof(config));

      var portalOptions = new RabbitMqPortalOptions();
      options?.Invoke(portalOptions);

      config.Services.AddScoped(_ => portalOptions);
      config.Services.AddTransient(typeof(IRabbitMqPortalFactory), portalOptions.PortalFactoryType);
      return config;
    }
  }
}
