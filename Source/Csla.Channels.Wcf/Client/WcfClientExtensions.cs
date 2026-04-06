//-----------------------------------------------------------------------
// <copyright file="WcfClientExtensions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Provides consistent context information between the client</summary>
//-----------------------------------------------------------------------

using Csla.Configuration;
using Csla.DataPortalClient;
using Microsoft.Extensions.DependencyInjection;

namespace Csla.Channels.Wcf.Client
{
  /// <summary>
  /// Provides extension methods that can be used to configure the client side data portal to use the WCF channel.
  /// </summary>
  public static class WcfClientExtensions
  {
    /// <summary>
    /// Configures the client side data portal to use the WCF channel.
    /// </summary>
    /// <param name="config">
    /// The client side data portal options that the WCF configuration will be applied to.
    /// </param>
    /// <param name="setOptions">
    /// An optional action that will be invoked to set options for the <see cref="WcfProxy"/>.
    /// </param>
    /// <returns>
    /// The <paramref name="config"/> with the WCF configuration applied.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="config"/> is <see langword="null"/>.
    /// </exception>
    public static DataPortalClientOptions UseWcfProxy(this DataPortalClientOptions config, Action<WcfProxyOptions>? setOptions)
    {
      if (config is null)
        throw new ArgumentNullException(nameof(config));

      var proxyOptions = new WcfProxyOptions();

      setOptions?.Invoke(proxyOptions);

      config.Services.AddSingleton(_ => proxyOptions);
      config.Services.AddTransient<IDataPortalProxy>(provider =>
      {
        var applicationContext = provider.GetRequiredService<ApplicationContext>();
        var options = provider.GetRequiredService<WcfProxyOptions>();
        var dataPortalOptions = provider.GetRequiredService<DataPortalOptions>();
        var wcfProxy = new WcfProxy(applicationContext, options, dataPortalOptions);
        return wcfProxy;
      });

      return config;
    }

    /// <summary>
    /// Configures the client side data portal to use the WCF channel.
    /// </summary>
    /// <param name="config">
    /// The client side data portal options that the WCF configuration will be applied to.
    /// </param>
    /// <returns>
    /// The <paramref name="config"/> with the WCF configuration applied.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="config"/> is <see langword="null"/>.
    /// </exception>
    public static DataPortalClientOptions UseWcfProxy(this DataPortalClientOptions config) => config.UseWcfProxy(null);
  }
}
