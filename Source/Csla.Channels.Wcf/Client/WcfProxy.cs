//-----------------------------------------------------------------------
// <copyright file="WcfProxy.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Provides consistent context information between the client</summary>
//-----------------------------------------------------------------------

using System.ServiceModel;
using Csla.Configuration;
using Csla.DataPortalClient;

namespace Csla.Channels.Wcf.Client
{
  /// <summary>
  /// Represents a <see cref="DataPortalProxy"/> that communicates with a remote data portal using WCF.
  /// </summary>
  /// <param name="applicationContext">
  /// The client side context for the data portal.
  /// </param>
  /// <param name="wcfProxyOptions">
  /// The options that are used to configure the data portal proxy.
  /// </param>
  /// <param name="dataPortalOptions">
  /// The options that are used to configure the client side data portal.
  /// </param>
  /// <exception cref="ArgumentNullException">
  /// <paramref name="wcfProxyOptions"/> is <see langword="null"/>.
  /// </exception>
  public class WcfProxy(ApplicationContext applicationContext, WcfProxyOptions wcfProxyOptions, DataPortalOptions dataPortalOptions) : DataPortalProxy(applicationContext)
  {
    protected WcfProxyOptions _options = wcfProxyOptions ?? throw new ArgumentNullException(nameof(wcfProxyOptions));
    protected string? _versionRoutingTag = dataPortalOptions.VersionRoutingTag;

    /// <summary>
    /// Gets the URL address for the data portal server used by this proxy instance.
    /// </summary>
    public override string DataPortalUrl => _options.DataPortalUrl;

    protected override async Task<byte[]> CallDataPortalServer(byte[] serialized, string operation, string? routingToken, bool isSync)
    {
      var client = new WcfPortalClient(_options.Binding, new EndpointAddress(_options.DataPortalUrl));

      var wcfRequest = new WcfRequest
      {
        Operation = CreateOperationTag(operation, _versionRoutingTag, routingToken),
        Body = serialized
      };

      try
      {
        var response = isSync ? client.Invoke(wcfRequest) : await client.InvokeAsync(wcfRequest);

        client.Close();

        return response.Body;
      }
      catch (Exception)
      {
        client.Abort();
        throw;
      }
    }

    internal async Task<WcfResponse> RouteMessage(WcfRequest request)
    {
      var client = new WcfPortalClient(_options.Binding, new EndpointAddress(_options.DataPortalUrl));

      try
      {
        var response = await client.InvokeAsync(request);

        client.Close();

        return response;
      }
      catch (Exception)
      {
        client.Abort();
        throw;
      }
    }

    private string CreateOperationTag(string operation, string? versionToken, string? routingToken)
    {
      if (!string.IsNullOrWhiteSpace(versionToken) || !string.IsNullOrWhiteSpace(routingToken))
        return $"{operation}/{routingToken}-{versionToken}";
      return operation;
    }
  }
}
