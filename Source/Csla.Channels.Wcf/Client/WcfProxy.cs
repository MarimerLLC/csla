//-----------------------------------------------------------------------
// <copyright file="WcfProxy.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Provides consistent context information between the client</summary>
//-----------------------------------------------------------------------

using System.ServiceModel;
using Csla.DataPortalClient;

namespace Csla.Channels.Wcf.Client
{
  /// <summary>
  /// Represents a <see cref="DataPortalProxy"/> that communicates with a remote data portal using WCF.
  /// </summary>
  /// <param name="applicationContext">
  /// The client side context for the data portal.
  /// </param>
  /// <param name="options">
  /// The options that are used to configure the data portal proxy.
  /// </param>
  /// <exception cref="ArgumentNullException">
  /// <paramref name="options"/> is <see langword="null"/>.
  /// </exception>
  public class WcfProxy(ApplicationContext applicationContext, WcfProxyOptions options) : DataPortalProxy(applicationContext)
  {
    protected WcfProxyOptions _options = options ?? throw new ArgumentNullException(nameof(options));

    /// <summary>
    /// Gets the URL address for the data portal server used by this proxy instance.
    /// </summary>
    public override string DataPortalUrl => _options.DataPortalUrl;

    protected override async Task<byte[]> CallDataPortalServer(byte[] serialized, string operation, string? routingToken, bool isSync)
    {
      var channelFactory = new ChannelFactory<IWcfPortal>(_options.Binding);

      var client = channelFactory.CreateChannel(new EndpointAddress(_options.DataPortalUrl));

      var wcfRequest = new WcfRequest
      {
        Operation = operation,
        Body = serialized
      };

      //Note: I'm not sure if we need to support routing here.

      // This implementation is following the pattern used in the gRPC channel. I'm not sure it is necessary for the WCF portal.
      if (isSync)
      {
        return client.Invoke(wcfRequest).Body;
      }

      var response = await client.InvokeAsync(wcfRequest);

      return response.Body;
    }
  }
}
