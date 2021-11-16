//-----------------------------------------------------------------------
// <copyright file="GrpcProxyExtensions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implement extension methods data portal channel</summary>
//-----------------------------------------------------------------------
using System;
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
    public static CslaDataPortalConfiguration UseHttpProxy(
      this CslaDataPortalConfiguration config, Action<Csla.Channels.Grpc.GrpcProxyOptions> options)
    {
      var proxyOptions = new Csla.Channels.Grpc.GrpcProxyOptions();
      options?.Invoke(proxyOptions);
      config.Services.AddTransient(typeof(IDataPortalProxy),
        sp =>
        {
          var applicationContext = sp.GetRequiredService<ApplicationContext>();
          var channel = GrpcChannel.ForAddress(proxyOptions.DataPortalUrl);
          return new Csla.Channels.Grpc.GrpcProxy(applicationContext, channel, proxyOptions);
        });
      return config;
    }
  }
}
