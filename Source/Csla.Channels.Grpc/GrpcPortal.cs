//-----------------------------------------------------------------------
// <copyright file="GrpcPortal.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Exposes server-side DataPortal functionality</summary>
//-----------------------------------------------------------------------

using System.Security.Principal;
using Csla.Core;
using Csla.Serialization;
using Csla.Serialization.Mobile;
using Csla.Server;
using Csla.Server.Hosts.DataPortalChannel;
using Google.Protobuf;
using Grpc.Core;

namespace Csla.Channels.Grpc
{
  /// <summary>
  /// Exposes server-side DataPortal functionality
  /// through gRPC.
  /// </summary>
  public class GrpcPortal : GrpcService.GrpcServiceBase
  {
    private IDataPortalServer dataPortalServer;
    private ApplicationContext _applicationContext;

    /// <summary>
    /// Creates an instance of the type
    /// </summary>
    /// <param name="dataPortal">Data portal server service</param>
    /// <param name="applicationContext"></param>
    /// <exception cref="ArgumentNullException"><paramref name="applicationContext"/> or <paramref name="dataPortal"/> is <see langword="null"/>.</exception>
    public GrpcPortal(IDataPortalServer dataPortal, ApplicationContext applicationContext)
    {
      dataPortalServer = dataPortal ?? throw new ArgumentNullException(nameof(dataPortal));
      _applicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
    }

    /// <summary>
    /// Handle inbound message.
    /// </summary>
    /// <param name="request">Request message</param>
    /// <param name="context">Server call context</param>
    /// <exception cref="ArgumentNullException"><paramref name="context"/> or <paramref name="request"/> is <see langword="null"/>.</exception>
    public override async Task<ResponseMessage> Invoke(RequestMessage request, ServerCallContext context)
    {
      if (request is null)
        throw new ArgumentNullException(nameof(request));
      if (context is null)
        throw new ArgumentNullException(nameof(context));

      var operation = request.Operation;
      if (operation.Contains("/"))
      {
        var temp = operation.Split('/');
        return await RouteMessage(temp[0], temp[1], request);
      }
      else
      {
        return await InvokePortal(operation, request.Body).ConfigureAwait(false);
      }
    }

    /// <summary>
    /// Gets a dictionary containing the URLs for each
    /// data portal route, where each key is the
    /// routing tag identifying the route URL.
    /// </summary>
    protected static Dictionary<string, string> RoutingTagUrls = [];

    /// <summary>
    /// Entry point for routing tag based data portal operations.
    /// </summary>
    /// <param name="operation">Name of the data portal operation to perform</param>
    /// <param name="routingTag">Routing tag from caller</param>
    /// <param name="request">Request message</param>
    /// <exception cref="ArgumentNullException"><paramref name="routingTag"/> or <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="routingTag"/> is null, empty or only consists of white spaces.</exception>
    protected virtual async Task<ResponseMessage> RouteMessage(string operation, string routingTag, RequestMessage request)
    {
      if (string.IsNullOrWhiteSpace(operation))
        throw new ArgumentException(string.Format(Properties.Resources.StringNotNullOrWhiteSpaceException, nameof(operation)), nameof(operation));
      if (routingTag is null)
        throw new ArgumentNullException(nameof(routingTag));
      if (request is null)
        throw new ArgumentNullException(nameof(request));

      if (RoutingTagUrls.TryGetValue(routingTag, out string? route) && route != "localhost")
      {
        var options = new GrpcProxyOptions { DataPortalUrl = $"{route}?operation={operation}" };
        var channel = _applicationContext.CreateInstanceDI<global::Grpc.Net.Client.GrpcChannel>();
        var dataPortalOptions = _applicationContext.GetRequiredService<Configuration.DataPortalOptions>();
        var proxy = new GrpcProxy(_applicationContext, channel, options, dataPortalOptions);
        var clientRequest = new RequestMessage
        {
          Body = request.Body,
          Operation = operation
        };
        var clientResponse = await proxy.RouteMessage(clientRequest);
        return new ResponseMessage { Body = clientResponse.Body };
      }
      else
      {
        return await InvokePortal(operation, request.Body).ConfigureAwait(false);
      }
    }

    private async Task<ResponseMessage> InvokePortal(string operation, ByteString requestData)
    {
      var result = _applicationContext.CreateInstanceDI<DataPortalResponse>();
      DataPortalErrorInfo? errorData = null;
      try
      {
        var request = SerializationFormatterFactory.GetFormatter(_applicationContext).Deserialize(requestData.ToByteArray());
        result = await CallPortal(operation, request);
      }
      catch (Exception ex)
      {
        errorData = _applicationContext.CreateInstanceDI<DataPortalErrorInfo>(ex);
      }
      var portalResult = _applicationContext.CreateInstanceDI<DataPortalResponse>();
      portalResult.ErrorData = errorData;
      portalResult.ObjectData = result.ObjectData;
      var buffer = SerializationFormatterFactory.GetFormatter(_applicationContext).Serialize(portalResult);
      return new ResponseMessage { Body = ByteString.CopyFrom(buffer) };
    }

    private async Task<DataPortalResponse> CallPortal(string operation, object request)
    {
      DataPortalResponse result;
      switch (operation)
      {
        case "create":
          result = await Create((CriteriaRequest)request).ConfigureAwait(false);
          break;

        case "fetch":
          result = await Fetch((CriteriaRequest)request).ConfigureAwait(false);
          break;

        case "update":
          result = await Update((UpdateRequest)request).ConfigureAwait(false);
          break;

        case "delete":
          result = await Delete((CriteriaRequest)request).ConfigureAwait(false);
          break;

        default:
          throw new InvalidOperationException(operation);
      }
      return result;
    }

    /// <summary>
    /// Create and initialize an existing business object.
    /// </summary>
    /// <param name="request">The request parameter object.</param>
    /// <exception cref="ArgumentNullException"><paramref name="request"/> is <see langword="null"/>.</exception>
    public async Task<DataPortalResponse> Create(CriteriaRequest request)
    {
      if (request is null)
        throw new ArgumentNullException(nameof(request));

      var result = _applicationContext.CreateInstanceDI<DataPortalResponse>();
      try
      {
        request = ConvertRequest(request);

        // unpack criteria data into object
        object? criteria = GetCriteria(_applicationContext, request.CriteriaData);
        if (criteria is DataPortalClient.PrimitiveCriteria primitiveCriteria)
        {
          criteria = primitiveCriteria.Value;
        }

        var objectType = Reflection.MethodCaller.GetType(AssemblyNameTranslator.GetAssemblyQualifiedName(request.TypeName), true);
        var context = new DataPortalContext(
          _applicationContext, (IPrincipal)SerializationFormatterFactory.GetFormatter(_applicationContext).Deserialize(request.Principal),
          true,
          request.ClientCulture,
          request.ClientUICulture,
          (IClientContext)SerializationFormatterFactory.GetFormatter(_applicationContext).Deserialize(request.ClientContext));

        var dpr = await dataPortalServer.Create(objectType, criteria, context, true);

        if (dpr.Error != null)
          result.ErrorData = _applicationContext.CreateInstanceDI<DataPortalErrorInfo>(dpr.Error);
        result.ObjectData = SerializationFormatterFactory.GetFormatter(_applicationContext).Serialize(dpr.ReturnObject);
      }
      catch (Exception ex)
      {
        result.ErrorData = _applicationContext.CreateInstanceDI<DataPortalErrorInfo>(ex);
        throw;
      }
      finally
      {
        result = ConvertResponse(result);
      }
      return result;
    }

    /// <summary>
    /// Get an existing business object.
    /// </summary>
    /// <param name="request">The request parameter object.</param>
    /// <exception cref="ArgumentNullException"><paramref name="request"/> is <see langword="null"/>.</exception>
    public async Task<DataPortalResponse> Fetch(CriteriaRequest request)
    {
      if (request is null)
        throw new ArgumentNullException(nameof(request));

      var result = _applicationContext.CreateInstanceDI<DataPortalResponse>();
      try
      {
        request = ConvertRequest(request);

        // unpack criteria data into object
        object? criteria = GetCriteria(_applicationContext, request.CriteriaData);
        if (criteria is DataPortalClient.PrimitiveCriteria primitiveCriteria)
        {
          criteria = primitiveCriteria.Value;
        }

        var objectType = Reflection.MethodCaller.GetType(AssemblyNameTranslator.GetAssemblyQualifiedName(request.TypeName), true);
        var context = new DataPortalContext(
          _applicationContext, (IPrincipal)SerializationFormatterFactory.GetFormatter(_applicationContext).Deserialize(request.Principal),
          true,
          request.ClientCulture,
          request.ClientUICulture,
          (IClientContext)SerializationFormatterFactory.GetFormatter(_applicationContext).Deserialize(request.ClientContext));

        var dpr = await dataPortalServer.Fetch(objectType, criteria, context, true);

        if (dpr.Error != null)
          result.ErrorData = _applicationContext.CreateInstanceDI<DataPortalErrorInfo>(dpr.Error);
        result.ObjectData = SerializationFormatterFactory.GetFormatter(_applicationContext).Serialize(dpr.ReturnObject);
      }
      catch (Exception ex)
      {
        result.ErrorData = _applicationContext.CreateInstanceDI<DataPortalErrorInfo>(ex);
        throw;
      }
      finally
      {
        result = ConvertResponse(result);
      }
      return result;
    }

    /// <summary>
    /// Update a business object.
    /// </summary>
    /// <param name="request">The request parameter object.</param>
    /// <exception cref="ArgumentNullException"><paramref name="request"/> is <see langword="null"/>.</exception>
    public async Task<DataPortalResponse> Update(UpdateRequest request)
    {
      if (request is null)
        throw new ArgumentNullException(nameof(request));

      var result = _applicationContext.CreateInstanceDI<DataPortalResponse>();
      try
      {
        request = ConvertRequest(request);
        // unpack object
        object? obj = GetCriteria(_applicationContext, request.ObjectData);

        var context = new DataPortalContext(
          _applicationContext, (IPrincipal)SerializationFormatterFactory.GetFormatter(_applicationContext).Deserialize(request.Principal),
          true,
          request.ClientCulture,
          request.ClientUICulture,
          (IClientContext)SerializationFormatterFactory.GetFormatter(_applicationContext).Deserialize(request.ClientContext));

        var dpr = await dataPortalServer.Update(obj, context, true);

        if (dpr.Error != null)
          result.ErrorData = _applicationContext.CreateInstanceDI<DataPortalErrorInfo>(dpr.Error);

        result.ObjectData = SerializationFormatterFactory.GetFormatter(_applicationContext).Serialize(dpr.ReturnObject);
      }
      catch (Exception ex)
      {
        result.ErrorData = _applicationContext.CreateInstanceDI<DataPortalErrorInfo>(ex);
        throw;
      }
      finally
      {
        result = ConvertResponse(result);
      }
      return result;
    }

    /// <summary>
    /// Delete a business object.
    /// </summary>
    /// <param name="request">The request parameter object.</param>
    /// <exception cref="ArgumentNullException"><paramref name="request"/> is <see langword="null"/>.</exception>
    public async Task<DataPortalResponse> Delete(CriteriaRequest request)
    {
      if (request is null)
        throw new ArgumentNullException(nameof(request));

      var result = _applicationContext.CreateInstanceDI<DataPortalResponse>();
      try
      {
        request = ConvertRequest(request);

        // unpack criteria data into object
        object? criteria = GetCriteria(_applicationContext, request.CriteriaData);
        if (criteria is DataPortalClient.PrimitiveCriteria primitiveCriteria)
        {
          criteria = primitiveCriteria.Value;
        }

        var objectType = Reflection.MethodCaller.GetType(AssemblyNameTranslator.GetAssemblyQualifiedName(request.TypeName), true);
        var context = new DataPortalContext(
          _applicationContext, (IPrincipal)SerializationFormatterFactory.GetFormatter(_applicationContext).Deserialize(request.Principal),
          true,
          request.ClientCulture,
          request.ClientUICulture,
          (IClientContext)SerializationFormatterFactory.GetFormatter(_applicationContext).Deserialize(request.ClientContext));

        var dpr = await dataPortalServer.Delete(objectType, criteria, context, true);

        if (dpr.Error != null)
          result.ErrorData = _applicationContext.CreateInstanceDI<DataPortalErrorInfo>(dpr.Error);
        result.ObjectData = SerializationFormatterFactory.GetFormatter(_applicationContext).Serialize(dpr.ReturnObject);
      }
      catch (Exception ex)
      {
        result.ErrorData = _applicationContext.CreateInstanceDI<DataPortalErrorInfo>(ex);
        throw;
      }
      finally
      {
        result = ConvertResponse(result);
      }
      return result;
    }

    #region Criteria

    private static object? GetCriteria(ApplicationContext applicationContext, byte[]? criteriaData)
    {
      object? criteria = null;
      if (criteriaData != null)
        criteria = SerializationFormatterFactory.GetFormatter(applicationContext).Deserialize(criteriaData);
      return criteria;
    }

    #endregion Criteria

    #region Extention Method for Requests

    /// <summary>
    /// Override to convert the request data before it
    /// is transferred over the network.
    /// </summary>
    /// <param name="request">Request object.</param>
    protected virtual UpdateRequest ConvertRequest(UpdateRequest request)
    {
      return request;
    }

    /// <summary>
    /// Override to convert the request data before it
    /// is transferred over the network.
    /// </summary>
    /// <param name="request">Request object.</param>
    protected virtual CriteriaRequest ConvertRequest(CriteriaRequest request)
    {
      return request;
    }

    /// <summary>
    /// Override to convert the response data after it
    /// comes back from the network.
    /// </summary>
    /// <param name="response">Response object.</param>
    protected virtual DataPortalResponse ConvertResponse(DataPortalResponse response)
    {
      return response;
    }

    #endregion Extention Method for Requests
  }
}