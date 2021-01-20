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
using Csla.Server.Hosts.HttpChannel;
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
        var url = $"{route}?operation={operation}";
        var proxy = new GrpcProxy(url);
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
      var result = new HttpResponse();
      HttpErrorInfo errorData = null;
      try
      {
        var request = SerializationFormatterFactory.GetFormatter().Deserialize(requestData.ToByteArray());
        result = await CallPortal(operation, request);
      }
      catch (Exception ex)
      {
        errorData = new HttpErrorInfo(ex);
      }
      var portalResult = new HttpResponse { ErrorData = errorData, GlobalContext = result.GlobalContext, ObjectData = result.ObjectData };
      var buffer = SerializationFormatterFactory.GetFormatter().Serialize(portalResult);
      return new ResponseMessage { Body = ByteString.CopyFrom(buffer) };
    }

    private async Task<HttpResponse> CallPortal(string operation, object request)
    {
      HttpResponse result;
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
    public async Task<HttpResponse> Create(CriteriaRequest request)
    {
      var result = new HttpResponse();
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
          (ContextDictionary)SerializationFormatterFactory.GetFormatter().Deserialize(request.ClientContext),
          (ContextDictionary)SerializationFormatterFactory.GetFormatter().Deserialize(request.GlobalContext));

        var prtl = new Csla.Server.DataPortal();
        var dpr = await prtl.Create(objectType, criteria, context, true);

        if (dpr.Error != null)
          result.ErrorData = new HttpErrorInfo(dpr.Error);
        result.GlobalContext = SerializationFormatterFactory.GetFormatter().Serialize(dpr.GlobalContext);
        result.ObjectData = SerializationFormatterFactory.GetFormatter().Serialize(dpr.ReturnObject);
      }
      catch (Exception ex)
      {
        result.ErrorData = new HttpErrorInfo(ex);
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
    public async Task<HttpResponse> Fetch(CriteriaRequest request)
    {
      var result = new HttpResponse();
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
          (ContextDictionary)SerializationFormatterFactory.GetFormatter().Deserialize(request.ClientContext),
          (ContextDictionary)SerializationFormatterFactory.GetFormatter().Deserialize(request.GlobalContext));

        var prtl = new Csla.Server.DataPortal();
        var dpr = await prtl.Fetch(objectType, criteria, context, true);

        if (dpr.Error != null)
          result.ErrorData = new HttpErrorInfo(dpr.Error);
        result.GlobalContext = SerializationFormatterFactory.GetFormatter().Serialize(dpr.GlobalContext);
        result.ObjectData = SerializationFormatterFactory.GetFormatter().Serialize(dpr.ReturnObject);
      }
      catch (Exception ex)
      {
        result.ErrorData = new HttpErrorInfo(ex);
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
    public async Task<HttpResponse> Update(UpdateRequest request)
    {
      var result = new HttpResponse();
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
          (ContextDictionary)SerializationFormatterFactory.GetFormatter().Deserialize(request.ClientContext),
          (ContextDictionary)SerializationFormatterFactory.GetFormatter().Deserialize(request.GlobalContext));

        var prtl = new Csla.Server.DataPortal();
        var dpr = await prtl.Update(obj, context, true);

        if (dpr.Error != null)
          result.ErrorData = new HttpErrorInfo(dpr.Error);

        result.GlobalContext = SerializationFormatterFactory.GetFormatter().Serialize(dpr.GlobalContext);
        result.ObjectData = SerializationFormatterFactory.GetFormatter().Serialize(dpr.ReturnObject);
      }
      catch (Exception ex)
      {
        result.ErrorData = new HttpErrorInfo(ex);
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
    public async Task<HttpResponse> Delete(CriteriaRequest request)
    {
      var result = new HttpResponse();
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
          (ContextDictionary)SerializationFormatterFactory.GetFormatter().Deserialize(request.ClientContext),
          (ContextDictionary)SerializationFormatterFactory.GetFormatter().Deserialize(request.GlobalContext));

        var prtl = new Csla.Server.DataPortal();
        var dpr = await prtl.Delete(objectType, criteria, context, true);

        if (dpr.Error != null)
          result.ErrorData = new HttpErrorInfo(dpr.Error);
        result.GlobalContext = SerializationFormatterFactory.GetFormatter().Serialize(dpr.GlobalContext);
        result.ObjectData = SerializationFormatterFactory.GetFormatter().Serialize(dpr.ReturnObject);
      }
      catch (Exception ex)
      {
        result.ErrorData = new HttpErrorInfo(ex);
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

    #endregion

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
    protected virtual HttpResponse ConvertResponse(HttpResponse response)
    {
      return response;
    }

    #endregion
  }
}
