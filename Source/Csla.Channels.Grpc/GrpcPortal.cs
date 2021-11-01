//-----------------------------------------------------------------------
// <copyright file="GrpcPortal.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Exposes server-side DataPortal functionality</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
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
  public class GrpcPortal : Csla.Channels.Grpc.GrpcService.GrpcServiceBase
  {
    private IDataPortalServer dataPortalServer;
    private ApplicationContext ApplicationContext { get; set; }

    /// <summary>
    /// Creates an instance of the type
    /// </summary>
    /// <param name="dataPortal">Data portal server service</param>
    /// <param name="applicationContext"></param>
    public GrpcPortal(IDataPortalServer dataPortal, ApplicationContext applicationContext)
    {
      dataPortalServer = dataPortal;
      ApplicationContext = applicationContext;
    }

    /// <summary>
    /// Handle inbound message.
    /// </summary>
    /// <param name="request">Request message</param>
    /// <param name="context">Server call context</param>
    /// <returns></returns>
    public override async Task<ResponseMessage> Invoke(RequestMessage request, ServerCallContext context)
    {
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
    protected static Dictionary<string, string> RoutingTagUrls = new Dictionary<string, string>();

    /// <summary>
    /// Entry point for routing tag based data portal operations.
    /// </summary>
    /// <param name="operation">Name of the data portal operation to perform</param>
    /// <param name="routingTag">Routing tag from caller</param>
    /// <param name="request">Request message</param>
    protected virtual async Task<ResponseMessage> RouteMessage(string operation, string routingTag, RequestMessage request)
    {
      if (RoutingTagUrls.TryGetValue(routingTag, out string route) && route != "localhost")
      {
        var options = new GrpcProxyOptions { DataPortalUrl = $"{route}?operation={operation}" };
        var channel = ApplicationContext.CreateInstance<global::Grpc.Net.Client.GrpcChannel>();
        var proxy = new GrpcProxy(ApplicationContext, channel, options);
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
      var result = new DataPortalResponse();
      DataPortalErrorInfo errorData = null;
      try
      {
        var request = SerializationFormatterFactory.GetFormatter().Deserialize(requestData.ToByteArray());
        result = await CallPortal(operation, request);
      }
      catch (Exception ex)
      {
        errorData = new DataPortalErrorInfo(ex);
      }
      var portalResult = new DataPortalResponse { ErrorData = errorData, ObjectData = result.ObjectData };
      var buffer = SerializationFormatterFactory.GetFormatter().Serialize(portalResult);
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
    public async Task<DataPortalResponse> Create(CriteriaRequest request)
    {
      var result = new DataPortalResponse();
      try
      {
        request = ConvertRequest(request);

        // unpack criteria data into object
        object criteria = GetCriteria(request.CriteriaData);
        if (criteria is Csla.DataPortalClient.PrimitiveCriteria)
        {
          criteria = ((Csla.DataPortalClient.PrimitiveCriteria)criteria).Value;
        }

        var objectType = Csla.Reflection.MethodCaller.GetType(AssemblyNameTranslator.GetAssemblyQualifiedName(request.TypeName), true);
        var context = new DataPortalContext(
          (IPrincipal)SerializationFormatterFactory.GetFormatter().Deserialize(request.Principal),
          true,
          request.ClientCulture,
          request.ClientUICulture,
          (ContextDictionary)SerializationFormatterFactory.GetFormatter().Deserialize(request.ClientContext));

        var dpr = await dataPortalServer.Create(objectType, criteria, context, true);

        if (dpr.Error != null)
          result.ErrorData = new DataPortalErrorInfo(dpr.Error);
        result.ObjectData = SerializationFormatterFactory.GetFormatter().Serialize(dpr.ReturnObject);
      }
      catch (Exception ex)
      {
        result.ErrorData = new DataPortalErrorInfo(ex);
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
    public async Task<DataPortalResponse> Fetch(CriteriaRequest request)
    {
      var result = new DataPortalResponse();
      try
      {
        request = ConvertRequest(request);

        // unpack criteria data into object
        object criteria = GetCriteria(request.CriteriaData);
        if (criteria is Csla.DataPortalClient.PrimitiveCriteria)
        {
          criteria = ((Csla.DataPortalClient.PrimitiveCriteria)criteria).Value;
        }

        var objectType = Csla.Reflection.MethodCaller.GetType(AssemblyNameTranslator.GetAssemblyQualifiedName(request.TypeName), true);
        var context = new DataPortalContext(
          (IPrincipal)SerializationFormatterFactory.GetFormatter().Deserialize(request.Principal),
          true,
          request.ClientCulture,
          request.ClientUICulture,
          (ContextDictionary)SerializationFormatterFactory.GetFormatter().Deserialize(request.ClientContext));

        var dpr = await dataPortalServer.Fetch(objectType, criteria, context, true);

        if (dpr.Error != null)
          result.ErrorData = new DataPortalErrorInfo(dpr.Error);
        result.ObjectData = SerializationFormatterFactory.GetFormatter().Serialize(dpr.ReturnObject);
      }
      catch (Exception ex)
      {
        result.ErrorData = new DataPortalErrorInfo(ex);
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
    public async Task<DataPortalResponse> Update(UpdateRequest request)
    {
      var result = new DataPortalResponse();
      try
      {
        request = ConvertRequest(request);
        // unpack object
        object obj = GetCriteria(request.ObjectData);

        var context = new DataPortalContext(
          (IPrincipal)SerializationFormatterFactory.GetFormatter().Deserialize(request.Principal),
          true,
          request.ClientCulture,
          request.ClientUICulture,
          (ContextDictionary)SerializationFormatterFactory.GetFormatter().Deserialize(request.ClientContext));

        var dpr = await dataPortalServer.Update(obj, context, true);

        if (dpr.Error != null)
          result.ErrorData = new DataPortalErrorInfo(dpr.Error);

        result.ObjectData = SerializationFormatterFactory.GetFormatter().Serialize(dpr.ReturnObject);
      }
      catch (Exception ex)
      {
        result.ErrorData = new DataPortalErrorInfo(ex);
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
    public async Task<DataPortalResponse> Delete(CriteriaRequest request)
    {
      var result = new DataPortalResponse();
      try
      {
        request = ConvertRequest(request);

        // unpack criteria data into object
        object criteria = GetCriteria(request.CriteriaData);
        if (criteria is Csla.DataPortalClient.PrimitiveCriteria)
        {
          criteria = ((Csla.DataPortalClient.PrimitiveCriteria)criteria).Value;
        }

        var objectType = Csla.Reflection.MethodCaller.GetType(AssemblyNameTranslator.GetAssemblyQualifiedName(request.TypeName), true);
        var context = new DataPortalContext(
          (IPrincipal)SerializationFormatterFactory.GetFormatter().Deserialize(request.Principal),
          true,
          request.ClientCulture,
          request.ClientUICulture,
          (ContextDictionary)SerializationFormatterFactory.GetFormatter().Deserialize(request.ClientContext));

        var dpr = await dataPortalServer.Delete(objectType, criteria, context, true);

        if (dpr.Error != null)
          result.ErrorData = new DataPortalErrorInfo(dpr.Error);
        result.ObjectData = SerializationFormatterFactory.GetFormatter().Serialize(dpr.ReturnObject);
      }
      catch (Exception ex)
      {
        result.ErrorData = new DataPortalErrorInfo(ex);
        throw;
      }
      finally
      {
        result = ConvertResponse(result);
      }
      return result;
    }

    #region Criteria

    private static object GetCriteria(byte[] criteriaData)
    {
      object criteria = null;
      if (criteriaData != null)
        criteria = SerializationFormatterFactory.GetFormatter().Deserialize(criteriaData);
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