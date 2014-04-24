using Csla.Serialization.Mobile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Csla.Server.Hosts.HttpChannel;
using Csla.Core;
using System.Security.Principal;

namespace Csla.Server.Hosts
{
  /// <summary>
  /// Exposes server-side DataPortal functionality
  /// through HTTP request/response.
  /// </summary>
  public class HttpPortal : ApiController
  {
    /// <summary>
    /// Entry point for all data portal operations.
    /// </summary>
    /// <param name="operation">Name of the data portal operation to perform.</param>
    /// <returns>Results from the server-side data portal.</returns>
    public async Task<HttpResponseMessage> Post(string operation)
    {
      var result = new HttpResponse();
      HttpErrorInfo errorData = null;
      try
      {
        var data = await Request.Content.ReadAsByteArrayAsync();
        var buffer = new MemoryStream(data);
        buffer.Position = 0;
        var request = MobileFormatter.Deserialize(buffer.ToArray());
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
      }
      catch (Exception ex)
      {
        errorData = new HttpErrorInfo(ex);
      }
      var restResult = new HttpResponse { ErrorData = errorData, GlobalContext = result.GlobalContext, ObjectData = result.ObjectData };

      var response = Request.CreateResponse();
      var bytes = MobileFormatter.Serialize(restResult);
      response.Content = new ByteArrayContent(bytes);
      return response;
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

        var processor = new Csla.Server.Hosts.Mobile.MobileRequestProcessor();
        var createRequest = new Csla.Server.Hosts.Mobile.MobileCriteriaRequest(
          request.TypeName,
          criteria,
          (IPrincipal)MobileFormatter.Deserialize(request.Principal),
          (ContextDictionary)MobileFormatter.Deserialize(request.GlobalContext),
          (ContextDictionary)MobileFormatter.Deserialize(request.ClientContext),
          request.ClientCulture,
          request.ClientUICulture);

        var createResponse = await processor.Create(createRequest).ConfigureAwait(false);
        if (createResponse.Error != null)
        {
          result.ErrorData = new HttpErrorInfo(createResponse.Error);
        }
        result.GlobalContext = MobileFormatter.Serialize(createResponse.GlobalContext);
        result.ObjectData = MobileFormatter.Serialize(createResponse.Object);
      }
      catch (Exception ex)
      {
        result.ErrorData = new HttpErrorInfo(ex);
      }
      finally
      {
        Csla.Server.Hosts.Mobile.MobileRequestProcessor.ClearContext();
      }
      return ConvertResponse(result);
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
        var processor = new Csla.Server.Hosts.Mobile.MobileRequestProcessor();
        var fetchRequest = new Csla.Server.Hosts.Mobile.MobileCriteriaRequest(
          request.TypeName,
          criteria,
          (IPrincipal)MobileFormatter.Deserialize(request.Principal),
          (ContextDictionary)MobileFormatter.Deserialize(request.GlobalContext),
          (ContextDictionary)MobileFormatter.Deserialize(request.ClientContext),
          request.ClientCulture,
          request.ClientUICulture);

#if NET40
        var fetchResponse = processor.Fetch(fetchRequest);
#else
        var fetchResponse = await processor.Fetch(fetchRequest).ConfigureAwait(false);
#endif
        if (fetchResponse.Error != null)
        {
          result.ErrorData = new HttpErrorInfo(fetchResponse.Error);
        }
        result.GlobalContext = MobileFormatter.Serialize(fetchResponse.GlobalContext);
        result.ObjectData = MobileFormatter.Serialize(fetchResponse.Object);
      }
      catch (Exception ex)
      {
        result.ErrorData = new HttpErrorInfo(ex);
      }
      finally
      {
        Csla.Server.Hosts.Mobile.MobileRequestProcessor.ClearContext();
      }
      return ConvertResponse(result);
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

        var processor = new Csla.Server.Hosts.Mobile.MobileRequestProcessor();
        var updateRequest = new Csla.Server.Hosts.Mobile.MobileUpdateRequest(
          obj,
          (IPrincipal)MobileFormatter.Deserialize(request.Principal),
          (ContextDictionary)MobileFormatter.Deserialize(request.GlobalContext),
          (ContextDictionary)MobileFormatter.Deserialize(request.ClientContext),
          request.ClientCulture,
          request.ClientUICulture);

#if NET40
        var updateResponse = processor.Update(updateRequest);
#else
        var updateResponse = await processor.Update(updateRequest).ConfigureAwait(false);
#endif
        if (updateResponse.Error != null)
        {
          result.ErrorData = new HttpErrorInfo(updateResponse.Error);
        }
        result.GlobalContext = MobileFormatter.Serialize(updateResponse.GlobalContext);
        result.ObjectData = MobileFormatter.Serialize(updateResponse.Object);
      }
      catch (Exception ex)
      {
        result.ErrorData = new HttpErrorInfo(ex);
      }
      finally
      {
        Csla.Server.Hosts.Mobile.MobileRequestProcessor.ClearContext();
      }
      return ConvertResponse(result);
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

        var processor = new Csla.Server.Hosts.Mobile.MobileRequestProcessor();
        var deleteRequest = new Csla.Server.Hosts.Mobile.MobileCriteriaRequest(
          request.TypeName,
          criteria,
          (IPrincipal)MobileFormatter.Deserialize(request.Principal),
          (ContextDictionary)MobileFormatter.Deserialize(request.GlobalContext),
          (ContextDictionary)MobileFormatter.Deserialize(request.ClientContext),
          request.ClientCulture,
          request.ClientUICulture);

#if NET40
        var deleteResponse = processor.Delete(deleteRequest);
#else
        var deleteResponse = await processor.Delete(deleteRequest).ConfigureAwait(false);
#endif
        if (deleteResponse.Error != null)
        {
          result.ErrorData = new HttpErrorInfo(deleteResponse.Error);
        }
        result.GlobalContext = MobileFormatter.Serialize(deleteResponse.GlobalContext);
        result.ObjectData = MobileFormatter.Serialize(deleteResponse.Object);
      }
      catch (Exception ex)
      {
        result.ErrorData = new HttpErrorInfo(ex);
      }
      finally
      {
        Csla.Server.Hosts.Mobile.MobileRequestProcessor.ClearContext();
      }
      return ConvertResponse(result);
    }

    #region Criteria

    private static object GetCriteria(byte[] criteriaData)
    {
      object criteria = null;
      if (criteriaData != null)
        criteria = MobileFormatter.Deserialize(criteriaData);
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
