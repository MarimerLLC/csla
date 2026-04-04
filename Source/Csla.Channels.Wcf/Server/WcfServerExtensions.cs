//-----------------------------------------------------------------------
// <copyright file="WcfServerExtensions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Provides consistent context information between the client</summary>
//-----------------------------------------------------------------------

using Csla.Configuration;
using Csla.Server;
using Microsoft.Extensions.DependencyInjection;

#if !NETFRAMEWORK
using CoreWCF.Configuration;
#endif

namespace Csla.Channels.Wcf.Server
{
  /// <summary>
  /// Provides extension methods that can be used to configure the server side data portal to use the WCF channel.
  /// </summary>
  public static class WcfServerExtensions
  {
    /// <summary>
    /// Configures the server side data portal to use the WCF channel.
    /// </summary>
    /// <param name="config">
    /// The server side data portal options that the WCF configuration will be applied to.
    /// </param>
    /// <param name="setOptions">
    /// An optional action that will be invoked to set options for the <see cref="WcfPortal"/>.
    /// </param>
    /// <returns>
    /// The <paramref name="config"/> with the WCF configuration applied.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="config"/> is <see langword="null"/>.
    /// </exception>
    public static DataPortalServerOptions UseWcfPortal(this DataPortalServerOptions config, Action<WcfPortalOptions>? setOptions)
    {
      if (config is null)
        throw new ArgumentNullException(nameof(config));

      var portalOptions = new WcfPortalOptions();

      setOptions?.Invoke(portalOptions);

      config.Services.AddSingleton(_ => portalOptions);

#if NETFRAMEWORK
      config.Services.AddSingleton(provider =>
      {
        var applicationContext = provider.GetRequiredService<ApplicationContext>();
        var dataPortal = provider.GetRequiredService<IDataPortalServer>();
        var options = provider.GetRequiredService<WcfPortalOptions>();
        var host = new WcfPortalHost(dataPortal, applicationContext, typeof(WcfPortal));
        host.AddServiceEndpoint(typeof(IWcfPortalServer), options.Binding, options.DataPortalUrl);
        return host;
      });
#else
      config.Services.AddTransient<WcfPortal>();
#endif
      return config;
    }

    /// <summary>
    /// Configures the server side data portal to use the WCF channel.
    /// </summary>
    /// <param name="config">
    /// The server side data portal options that the WCF configuration will be applied to.
    /// </param>
    /// <returns>
    /// The <paramref name="config"/> with the WCF configuration applied.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="config"/> is <see langword="null"/>.
    /// </exception>
    public static DataPortalServerOptions UseWcfPortal(this DataPortalServerOptions config) => config.UseWcfPortal(null);
  }
}
