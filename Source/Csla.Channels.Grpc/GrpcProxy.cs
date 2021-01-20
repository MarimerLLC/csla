//-----------------------------------------------------------------------
// <copyright file="GrpcProxy.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implements a data portal proxy to relay data portal</summary>
//-----------------------------------------------------------------------
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Csla.Core;
using Csla.Serialization.Mobile;
using Csla.Server;
using Google.Protobuf;
using Grpc.Net.Client;
using Csla.DataPortalClient;
using Csla.Serialization;

namespace Csla.Channels.Grpc
{
  /// <summary>
  /// Implements a data portal proxy to relay data portal
  /// calls to a remote application server by using gRPC.
  /// </summary>
  public class GrpcProxy : IDataPortalProxy
  {
    /// <summary>
    /// Gets or sets the HttpClient timeout
    /// in milliseconds (0 uses default HttpClient timeout).
    /// </summary>
    public int Timeout { get; set; }

    /// <summary>
    /// Creates an instance of the object, initializing
    /// it to use the DefaultUrl 
    /// values.
    /// </summary>
    public GrpcProxy()
      : this(null, ApplicationContext.DataPortalUrlString)
    { }

    /// <summary>
    /// Creates an instance of the object, initializing
    /// it to use the supplied URL.
    /// </summary>
    /// <param name="dataPortalUrl">Server endpoint URL</param>
    public GrpcProxy(string dataPortalUrl)
      : this(null, dataPortalUrl)
    { }

    /// <summary>
    /// Creates an instance of the object, initializing
    /// it to use the supplied GrpcChannel object.
    /// </summary>
    /// <param name="channel">GrpcChannel instance</param>
    public GrpcProxy(GrpcChannel channel)
      : this(channel, ApplicationContext.DataPortalUrlString)
    { }

    /// <summary>
    /// Creates an instance of the object, initializing
    /// it to use the supplied GrpcChannel object and URL.
    /// </summary>
    /// <param name="channel">GrpcChannel instance</param>
    /// <param name="dataPortalUrl">Server endpoint URL</param>
    public GrpcProxy(GrpcChannel channel, string dataPortalUrl)
    {
      _channel = channel;
      DataPortalUrl = dataPortalUrl;
    }

    /// <summary>
    /// Gets the URL address for the data portal server
    /// used by this proxy instance.
    /// </summary>
    public string DataPortalUrl { get; protected set; }

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
        if (_defaultChannel == null)
          _defaultChannel = GrpcChannel.ForAddress(DataPortalUrl);
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
      if (_grpcClient == null)
        _grpcClient = new GrpcService.GrpcServiceClient(GetChannel());
      return _grpcClient;
    }

    /// <summary>
    /// Gets a value indicating whether the data portal
    /// is hosted on a remote server.
    /// </summary>
    public bool IsServerRemote => true;

    /// <summary>
    /// Called by <see cref="DataPortal" /> to create a
    /// new business object.
    /// </summary>
    /// <param name="objectType">Type of business object to create.</param>
    /// <param name="criteria">Criteria object describing business object.</param>
    /// <param name="context">DataPortalContext object passed to the server.</param>
    /// <param name="isSync">True if the client-side proxy should synchronously invoke the server.</param>
    public async Task<DataPortalResult> Create(Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      DataPortalResult result;
      try
      {
        var request = GetBaseCriteriaRequest();
        request.TypeName = AssemblyNameTranslator.GetAssemblyQualifiedName(objectType.AssemblyQualifiedName);
        if (!(criteria is IMobileObject))
        {
          criteria = new PrimitiveCriteria(criteria);
        }
        request.CriteriaData = SerializationFormatterFactory.GetFormatter().Serialize(criteria);
        request = ConvertRequest(request);

        var serialized = SerializationFormatterFactory.GetFormatter().Serialize(request);

        serialized = await CallDataPortalServer(serialized, "create", GetRoutingToken(objectType), isSync);

        var response = (Csla.Server.Hosts.HttpChannel.HttpResponse)SerializationFormatterFactory.GetFormatter().Deserialize(serialized);
        response = ConvertResponse(response);
        var globalContext = (ContextDictionary)SerializationFormatterFactory.GetFormatter().Deserialize(response.GlobalContext);
        if (response != null && response.ErrorData == null)
        {
          var obj = SerializationFormatterFactory.GetFormatter().Deserialize(response.ObjectData);
          result = new DataPortalResult(obj, null, globalContext);
        }
        else if (response != null && response.ErrorData != null)
        {
          var ex = new DataPortalException(response.ErrorData);
          result = new DataPortalResult(null, ex, globalContext);
        }
        else
        {
          throw new DataPortalException("null response", null);
        }
      }
      catch (Exception ex)
      {
        result = new DataPortalResult(null, ex, null);
      }
      if (result.Error != null)
        throw result.Error;
      return result;
    }

    /// <summary>
    /// Called by <see cref="DataPortal" /> to load an
    /// existing business object.
    /// </summary>
    /// <param name="objectType">Type of business object to create.</param>
    /// <param name="criteria">Criteria object describing business object.</param>
    /// <param name="context">
    /// <see cref="Server.DataPortalContext" /> object passed to the server.
    /// </param>
    /// <param name="isSync">True if the client-side proxy should synchronously invoke the server.</param>
    public async Task<DataPortalResult> Fetch(Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      DataPortalResult result;
      try
      {
        var request = GetBaseCriteriaRequest();
        request.TypeName = AssemblyNameTranslator.GetAssemblyQualifiedName(objectType.AssemblyQualifiedName);
        if (!(criteria is IMobileObject))
        {
          criteria = new PrimitiveCriteria(criteria);
        }
        request.CriteriaData = SerializationFormatterFactory.GetFormatter().Serialize(criteria);
        request = ConvertRequest(request);

        var serialized = SerializationFormatterFactory.GetFormatter().Serialize(request);

        serialized = await CallDataPortalServer(serialized, "fetch", GetRoutingToken(objectType), isSync);

        var response = (Csla.Server.Hosts.HttpChannel.HttpResponse)SerializationFormatterFactory.GetFormatter().Deserialize(serialized);
        response = ConvertResponse(response);
        var globalContext = (ContextDictionary)SerializationFormatterFactory.GetFormatter().Deserialize(response.GlobalContext);
        if (response != null && response.ErrorData == null)
        {
          var obj = SerializationFormatterFactory.GetFormatter().Deserialize(response.ObjectData);
          result = new DataPortalResult(obj, null, globalContext);
        }
        else if (response != null && response.ErrorData != null)
        {
          var ex = new DataPortalException(response.ErrorData);
          result = new DataPortalResult(null, ex, globalContext);
        }
        else
        {
          throw new DataPortalException("null response", null);
        }
      }
      catch (Exception ex)
      {
        result = new DataPortalResult(null, ex, null);
      }
      if (result.Error != null)
        throw result.Error;
      return result;
    }

    /// <summary>
    /// Called by <see cref="DataPortal" /> to update a
    /// business object.
    /// </summary>
    /// <param name="obj">The business object to update.</param>
    /// <param name="context">
    /// <see cref="Server.DataPortalContext" /> object passed to the server.
    /// </param>
    /// <param name="isSync">True if the client-side proxy should synchronously invoke the server.</param>
    public async Task<DataPortalResult> Update(object obj, DataPortalContext context, bool isSync)
    {
      DataPortalResult result;
      try
      {
        var request = GetBaseUpdateCriteriaRequest();
        request.ObjectData = SerializationFormatterFactory.GetFormatter().Serialize(obj);
        request = ConvertRequest(request);

        var serialized = SerializationFormatterFactory.GetFormatter().Serialize(request);

        serialized = await CallDataPortalServer(serialized, "update", GetRoutingToken(obj.GetType()), isSync);

        var response = (Csla.Server.Hosts.HttpChannel.HttpResponse)SerializationFormatterFactory.GetFormatter().Deserialize(serialized);
        response = ConvertResponse(response);
        var globalContext = (ContextDictionary)SerializationFormatterFactory.GetFormatter().Deserialize(response.GlobalContext);
        if (response != null && response.ErrorData == null)
        {
          var newobj = SerializationFormatterFactory.GetFormatter().Deserialize(response.ObjectData);
          result = new DataPortalResult(newobj, null, globalContext);
        }
        else if (response != null && response.ErrorData != null)
        {
          var ex = new DataPortalException(response.ErrorData);
          result = new DataPortalResult(null, ex, globalContext);
        }
        else
        {
          throw new DataPortalException("null response", null);
        }
      }
      catch (Exception ex)
      {
        result = new DataPortalResult(null, ex, null);
      }
      if (result.Error != null)
        throw result.Error;
      return result;
    }

    /// <summary>
    /// Called by <see cref="DataPortal" /> to delete a
    /// business object.
    /// </summary>
    /// <param name="objectType">Type of business object to create.</param>
    /// <param name="criteria">Criteria object describing business object.</param>
    /// <param name="context">
    /// <see cref="Server.DataPortalContext" /> object passed to the server.
    /// </param>
    /// <param name="isSync">True if the client-side proxy should synchronously invoke the server.</param>
    public async Task<DataPortalResult> Delete(Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      DataPortalResult result;
      try
      {

        var request = GetBaseCriteriaRequest();
        request.TypeName = AssemblyNameTranslator.GetAssemblyQualifiedName(objectType.AssemblyQualifiedName);
        if (!(criteria is IMobileObject))
        {
          criteria = new PrimitiveCriteria(criteria);
        }
        request.CriteriaData = SerializationFormatterFactory.GetFormatter().Serialize(criteria);
        request = ConvertRequest(request);

        var serialized = SerializationFormatterFactory.GetFormatter().Serialize(request);

        serialized = await CallDataPortalServer(serialized, "delete", GetRoutingToken(objectType), isSync);

        var response = (Csla.Server.Hosts.HttpChannel.HttpResponse)SerializationFormatterFactory.GetFormatter().Deserialize(serialized);
        response = ConvertResponse(response);
        var globalContext = (ContextDictionary)SerializationFormatterFactory.GetFormatter().Deserialize(response.GlobalContext);
        if (response != null && response.ErrorData == null)
        {
          result = new DataPortalResult(null, null, globalContext);
        }
        else if (response != null && response.ErrorData != null)
        {
          var ex = new DataPortalException(response.ErrorData);
          result = new DataPortalResult(null, ex, globalContext);
        }
        else
        {
          throw new DataPortalException("null response", null);
        }
      }
      catch (Exception ex)
      {
        result = new DataPortalResult(null, ex, null);
      }
      if (result.Error != null)
        throw result.Error;
      return result;
    }

    private async Task<byte[]> CallDataPortalServer(byte[] serialized, string operation, string routingToken, bool isSync)
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
      else
        return operatation;
    }

    private string GetRoutingToken(Type objectType)
    {
      string result = null;
      var list = objectType.GetCustomAttributes(typeof(DataPortalServerRoutingTagAttribute), false);
      if (list.Count() > 0)
        result = ((DataPortalServerRoutingTagAttribute)list[0]).RoutingTag;
      return result;
    }

    /// <summary>
    /// Override this method to manipulate the message
    /// request data sent to the server.
    /// </summary>
    /// <param name="request">Update request data.</param>
    protected virtual Csla.Server.Hosts.HttpChannel.UpdateRequest ConvertRequest(Csla.Server.Hosts.HttpChannel.UpdateRequest request)
    {
      return request;
    }

    /// <summary>
    /// Override this method to manipulate the message
    /// request data sent to the server.
    /// </summary>
    /// <param name="request">Criteria request data.</param>
    protected virtual Csla.Server.Hosts.HttpChannel.CriteriaRequest ConvertRequest(Csla.Server.Hosts.HttpChannel.CriteriaRequest request)
    {
      return request;
    }

    /// <summary>
    /// Override this method to manipulate the message
    /// request data returned from the server.
    /// </summary>
    /// <param name="response">Response data.</param>
    protected virtual Csla.Server.Hosts.HttpChannel.HttpResponse ConvertResponse(Csla.Server.Hosts.HttpChannel.HttpResponse response)
    {
      return response;
    }

    #region Criteria

    private Csla.Server.Hosts.HttpChannel.CriteriaRequest GetBaseCriteriaRequest()
    {
      var request = new Csla.Server.Hosts.HttpChannel.CriteriaRequest();
      request.CriteriaData = null;
      request.ClientContext = SerializationFormatterFactory.GetFormatter().Serialize(ApplicationContext.ClientContext);
#pragma warning disable CS0618 // Type or member is obsolete
      request.GlobalContext = SerializationFormatterFactory.GetFormatter().Serialize(ApplicationContext.GlobalContext);
#pragma warning restore CS0618 // Type or member is obsolete
      if (ApplicationContext.AuthenticationType == "Windows")
      {
        request.Principal = SerializationFormatterFactory.GetFormatter().Serialize(null);
      }
      else
      {
        request.Principal = SerializationFormatterFactory.GetFormatter().Serialize(ApplicationContext.User);
      }
      request.ClientCulture = System.Globalization.CultureInfo.CurrentCulture.Name;
      request.ClientUICulture = System.Globalization.CultureInfo.CurrentUICulture.Name;
      return request;
    }

    private Csla.Server.Hosts.HttpChannel.UpdateRequest GetBaseUpdateCriteriaRequest()
    {
      var request = new Csla.Server.Hosts.HttpChannel.UpdateRequest();
      request.ObjectData = null;
      request.ClientContext = SerializationFormatterFactory.GetFormatter().Serialize(ApplicationContext.ClientContext);
#pragma warning disable CS0618 // Type or member is obsolete
      request.GlobalContext = SerializationFormatterFactory.GetFormatter().Serialize(ApplicationContext.GlobalContext);
#pragma warning restore CS0618 // Type or member is obsolete
      if (ApplicationContext.AuthenticationType == "Windows")
      {
        request.Principal = SerializationFormatterFactory.GetFormatter().Serialize(null);
      }
      else
      {
        request.Principal = SerializationFormatterFactory.GetFormatter().Serialize(ApplicationContext.User);
      }
      request.ClientCulture = Thread.CurrentThread.CurrentCulture.Name;
      request.ClientUICulture = Thread.CurrentThread.CurrentUICulture.Name;
      return request;
    }

    #endregion
  }
}
