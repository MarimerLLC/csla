//-----------------------------------------------------------------------
// <copyright file="HttpPortal.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Exposes server-side DataPortal functionality</summary>
//-----------------------------------------------------------------------
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
  public class HttpPortal
  {
    /// <summary>
    /// Create and initialize an existing business object.
    /// </summary>
    /// <param name="request">The request parameter object.</param>
#pragma warning disable 1998
    public async Task<HttpResponse> Create(CriteriaRequest request)
#pragma warning restore 1998
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

        var objectType = Csla.Reflection.MethodCaller.GetType(request.TypeName, true);
        var context = new DataPortalContext(
          (IPrincipal)MobileFormatter.Deserialize(request.Principal),
          true,
          request.ClientCulture,
          request.ClientUICulture,
          (ContextDictionary)MobileFormatter.Deserialize(request.ClientContext),
          (ContextDictionary)MobileFormatter.Deserialize(request.GlobalContext));

        var prtl = new Csla.Server.DataPortal();
        var dpr = await prtl.Create(objectType, criteria, context, true);

        if (dpr.Error != null)
          result.ErrorData = new HttpErrorInfo(dpr.Error);
        result.GlobalContext = MobileFormatter.Serialize(dpr.GlobalContext);
        result.ObjectData = MobileFormatter.Serialize(dpr.ReturnObject);
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
#pragma warning disable 1998
    public async Task<HttpResponse> Fetch(CriteriaRequest request)
#pragma warning restore 1998
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

        var objectType = Csla.Reflection.MethodCaller.GetType(request.TypeName, true);
        var context = new DataPortalContext(
          (IPrincipal)MobileFormatter.Deserialize(request.Principal),
          true,
          request.ClientCulture,
          request.ClientUICulture,
          (ContextDictionary)MobileFormatter.Deserialize(request.ClientContext),
          (ContextDictionary)MobileFormatter.Deserialize(request.GlobalContext));

        var prtl = new Csla.Server.DataPortal();
        var dpr = await prtl.Fetch(objectType, criteria, context, true);

        if (dpr.Error != null)
          result.ErrorData = new HttpErrorInfo(dpr.Error);
        result.GlobalContext = MobileFormatter.Serialize(dpr.GlobalContext);
        result.ObjectData = MobileFormatter.Serialize(dpr.ReturnObject);
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
#pragma warning disable 1998
    public async Task<HttpResponse> Update(UpdateRequest request)
#pragma warning restore 1998
    {
      var result = new HttpResponse();
      try
      {
        request = ConvertRequest(request);
        // unpack object
        object obj = GetCriteria(request.ObjectData);

        var context = new DataPortalContext(
          (IPrincipal)MobileFormatter.Deserialize(request.Principal),
          true,
          request.ClientCulture,
          request.ClientUICulture,
          (ContextDictionary)MobileFormatter.Deserialize(request.ClientContext),
          (ContextDictionary)MobileFormatter.Deserialize(request.GlobalContext));

        var prtl = new Csla.Server.DataPortal();
        var dpr = await prtl.Update(obj, context, true);

        if (dpr.Error != null)
          result.ErrorData = new HttpErrorInfo(dpr.Error);

        result.GlobalContext = MobileFormatter.Serialize(dpr.GlobalContext);
        result.ObjectData = MobileFormatter.Serialize(dpr.ReturnObject);
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
#pragma warning disable 1998
    public async Task<HttpResponse> Delete(CriteriaRequest request)
#pragma warning restore 1998
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

        var objectType = Csla.Reflection.MethodCaller.GetType(request.TypeName, true);
        var context = new DataPortalContext(
          (IPrincipal)MobileFormatter.Deserialize(request.Principal),
          true,
          request.ClientCulture,
          request.ClientUICulture,
          (ContextDictionary)MobileFormatter.Deserialize(request.ClientContext),
          (ContextDictionary)MobileFormatter.Deserialize(request.GlobalContext));

        var prtl = new Csla.Server.DataPortal();
        var dpr = await prtl.Delete(objectType, criteria, context, true);

        if (dpr.Error != null)
          result.ErrorData = new HttpErrorInfo(dpr.Error);
        result.GlobalContext = MobileFormatter.Serialize(dpr.GlobalContext);
        result.ObjectData = MobileFormatter.Serialize(dpr.ReturnObject);
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
