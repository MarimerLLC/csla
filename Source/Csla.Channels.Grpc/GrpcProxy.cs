//-----------------------------------------------------------------------
// <copyright file="GrpcProxy.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implements a data portal proxy to relay data portal</summary>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Csla.Configuration;
using Csla.DataPortalClient;
using Google.Protobuf;
using Grpc.Net.Client;

namespace Csla.Channels.Grpc
{
  /// <summary>
  /// Implements a data portal proxy to relay data portal
  /// calls to a remote application server by using gRPC.
  /// </summary>
  public class GrpcProxy : DataPortalProxy
  {
    /// <summary>
    /// Creates an instance of the object, initializing
    /// it to use the supplied GrpcChannel object and URL.
    /// </summary>
    /// <param name="applicationContext"></param>
    /// <param name="channel">GrpcChannel instance</param>
    /// <param name="options">Proxy options</param>
    /// <param name="dataPortalOptions">Data portal options</param>
    /// <exception cref="ArgumentNullException"><paramref name="applicationContext"/>, <paramref name="dataPortalOptions"/> or <paramref name="options"/> is <see langword="null"/>.</exception>
    public GrpcProxy(ApplicationContext applicationContext, GrpcChannel channel, GrpcProxyOptions options, DataPortalOptions dataPortalOptions)
      : base(applicationContext)
    {
      if (options is null)
        throw new ArgumentNullException(nameof(options));
      if (dataPortalOptions is null)
        throw new ArgumentNullException(nameof(dataPortalOptions));

      _channel = channel ?? throw new ArgumentNullException(nameof(channel));
      DataPortalUrl = options.DataPortalUrl;
      _versionRoutingTag = dataPortalOptions.VersionRoutingTag;
    }

    private GrpcChannel? _channel;
    private static GrpcChannel? _defaultChannel;
    private string? _versionRoutingTag;

    /// <summary>
    /// Gets the GrpcChannel used by the gRPC client.
    /// </summary>
    protected virtual GrpcChannel GetChannel()
    {
      if (_channel == null)
      {
        _defaultChannel ??= GrpcChannel.ForAddress(DataPortalUrl);
        _channel = _defaultChannel;
      }
      return _channel;
    }

    /// <summary>
    /// Sets the GrpcChannel used by gRPC clients.
    /// </summary>
    /// <param name="channel">GrpcChannel instance</param>
    /// <exception cref="ArgumentNullException"><paramref name="channel"/> is <see langword="null"/>.</exception>
#if NET8_0_OR_GREATER
    [MemberNotNull(nameof(_defaultChannel))]
#endif
    protected static void SetChannel(GrpcChannel channel)
    {
      _defaultChannel = channel ?? throw new ArgumentNullException(nameof(channel));
    }

    private GrpcService.GrpcServiceClient? _grpcClient;

    /// <summary>
    /// Get gRPC client object used by data portal.
    /// </summary>
    protected virtual GrpcService.GrpcServiceClient GetGrpcClient()
    {
      return _grpcClient ??= new GrpcService.GrpcServiceClient(GetChannel());
    }

    /// <summary>
    /// Create message and send to Grpc server.
    /// Return Response from server
    /// </summary>
    /// <param name="serialized">Serialized request</param>
    /// <param name="operation">DataPortal operation</param>
    /// <param name="routingToken">Routing Tag for server</param>
    /// <param name="isSync">True if the client-side proxy should synchronously invoke the server.</param>
    /// <returns>Serialized response from server</returns>
    protected override async Task<byte[]> CallDataPortalServer(byte[] serialized, string operation, string routingToken, bool isSync)
    {
      ByteString outbound = ByteString.CopyFrom(serialized);
      var request = new RequestMessage
      {
        Body = outbound,
        Operation = CreateOperationTag(operation, _versionRoutingTag, routingToken)
      };
      ResponseMessage response;
      if (isSync)
        response = GetGrpcClient().Invoke(request);
      else
        response = await GetGrpcClient().InvokeAsync(request);
      return response.Body.ToByteArray();
    }

    internal async Task<ResponseMessage> RouteMessage(RequestMessage request)
    {
      return await GetGrpcClient().InvokeAsync(request);
    }

    private string CreateOperationTag(string operation, string? versionToken, string routingToken)
    {
      if (!string.IsNullOrWhiteSpace(versionToken) || !string.IsNullOrWhiteSpace(routingToken))
        return $"{operation}/{routingToken}-{versionToken}";
      return operation;
    }
  }
}