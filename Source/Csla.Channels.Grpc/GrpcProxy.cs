//-----------------------------------------------------------------------
// <copyright file="GrpcProxy.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implements a data portal proxy to relay data portal</summary>
//-----------------------------------------------------------------------
using System.Threading.Tasks;
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
    public GrpcProxy(ApplicationContext applicationContext, GrpcChannel channel, GrpcProxyOptions options)
      : base(applicationContext)
    {
      _channel = channel;
      DataPortalUrl = options.DataPortalUrl;
    }

    private GrpcChannel _channel;
    private static GrpcChannel _defaultChannel;

    /// <summary>
    /// Gets the GrpcChannel used by the gRPC client.
    /// </summary>
    /// <returns></returns>
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
    protected static void SetChannel(GrpcChannel channel)
    {
      _defaultChannel = channel;
    }

    private GrpcService.GrpcServiceClient _grpcClient;

    /// <summary>
    /// Get gRPC client object used by data portal.
    /// </summary>
    /// <returns></returns>
    protected virtual GrpcService.GrpcServiceClient GetGrpcClient()
    {
      return _grpcClient ??= new GrpcService.GrpcServiceClient(GetChannel());
    }

    /// <summary>
    /// Create message and send to Grpc server.
    /// Return Response from server
    /// </summary>
    /// <param name="serialized">Serialised request</param>
    /// <param name="operation">DataPortal operation</param>
    /// <param name="routingToken">Routing Tag for server</param>
    /// <param name="isSync">True if the client-side proxy should synchronously invoke the server.</param>
    /// <returns>Serialised response from server</returns>
    protected override async Task<byte[]> CallDataPortalServer(byte[] serialized, string operation, string routingToken, bool isSync)
    {
      ByteString outbound = ByteString.CopyFrom(serialized);
      var request = new RequestMessage
      {
        Body = outbound,
        Operation = CreateOperationTag(operation, ApplicationContext.VersionRoutingTag, routingToken)
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

    private string CreateOperationTag(string operatation, string versionToken, string routingToken)
    {
      if (!string.IsNullOrWhiteSpace(versionToken) || !string.IsNullOrWhiteSpace(routingToken))
        return $"{operatation}/{routingToken}-{versionToken}";
      return operatation;
    }
  }
}