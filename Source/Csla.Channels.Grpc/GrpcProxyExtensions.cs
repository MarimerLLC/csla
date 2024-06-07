//-----------------------------------------------------------------------
// <copyright file="GrpcProxyExtensions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implement extension methods data portal channel</summary>
//-----------------------------------------------------------------------

using Csla.DataPortalClient;
using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;

namespace Csla.Configuration
{
  /// <summary>
  /// Implement extension methods data portal channel
  /// </summary>
  public static class GrpcProxyExtensions
  {
    /// <summary>
    /// Configure data portal client to use GrpcProxy.
    /// </summary>
    /// <param name="config">CslaDataPortalConfiguration object</param>
    /// <param name="options">Data portal proxy options</param>
    /// <exception cref="ArgumentNullException"><paramref name="config"/> is <see langword="null"/>.</exception>
    public static DataPortalClientOptions UseGrpcProxy(this DataPortalClientOptions config, Action<Csla.Channels.Grpc.GrpcProxyOptions>? options)
    {
      if (config is null)
        throw new ArgumentNullException(nameof(config));

      var proxyOptions = new Csla.Channels.Grpc.GrpcProxyOptions();
      options?.Invoke(proxyOptions);
      config.Services.AddTransient(typeof(IDataPortalProxy),
        sp =>
        {
          var applicationContext = sp.GetRequiredService<ApplicationContext>();
          var dataPortalOptions = sp.GetRequiredService<DataPortalOptions>();
          var channel = GrpcChannel.ForAddress(proxyOptions.DataPortalUrl);
          return new Csla.Channels.Grpc.GrpcProxy(applicationContext, channel, proxyOptions, dataPortalOptions);
        });
      return config;
    }
  }
}
