//-----------------------------------------------------------------------
// <copyright file="HttpPortal.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Exposes server-side DataPortal functionality</summary>
//-----------------------------------------------------------------------
using Csla.Serialization.Mobile;
using Csla.Core;
using System.Security.Principal;
using Csla.Serialization;
using Csla.Server.Hosts.DataPortalChannel;
using Csla.Properties;

namespace Csla.Server.Hosts
{
  /// <summary>
  /// Exposes server-side DataPortal functionality
  /// through HTTP request/response.
  /// </summary>
  public class HttpPortal
  {
    private IDataPortalServer dataPortalServer;
    private ApplicationContext _applicationContext;

    /// <summary>
    /// Creates an instance of the type
    /// </summary>
    /// <param name="applicationContext"></param>
    /// <param name="dataPortal">Data portal server service</param>
    public HttpPortal(ApplicationContext applicationContext, IDataPortalServer dataPortal)
    {
      dataPortalServer = dataPortal;
      _applicationContext = applicationContext;
    }

    /// <summary>
    /// Create and initialize an existing business object.
    /// </summary>
    /// <param name="request">The request parameter object.</param>
    public async Task<DataPortalResponse> Create(CriteriaRequest request)
    {
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

        var objectType = Reflection.MethodCaller.GetType(AssemblyNameTranslator.GetAssemblyQualifiedName(request.TypeName));
        var context = new DataPortalContext(
          _applicationContext, (IPrincipal)_applicationContext.GetRequiredService<ISerializationFormatter>().Deserialize(request.Principal),
          true,
          request.ClientCulture,
          request.ClientUICulture,
          (IContextDictionary)_applicationContext.GetRequiredService<ISerializationFormatter>().Deserialize(request.ClientContext));

        var dpr = await dataPortalServer.Create(objectType, criteria, context, true);

        if (dpr.Error != null)
          result.ErrorData = _applicationContext.CreateInstance<DataPortalErrorInfo>(_applicationContext, dpr.Error);
        result.ObjectData = _applicationContext.GetRequiredService<ISerializationFormatter>().Serialize(dpr.ReturnObject);
      }
      catch (Exception ex)
      {
        result.ErrorData = _applicationContext.CreateInstance<DataPortalErrorInfo>(_applicationContext, ex);
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

        var objectType = Reflection.MethodCaller.GetType(AssemblyNameTranslator.GetAssemblyQualifiedName(request.TypeName));
        var context = new DataPortalContext(
          _applicationContext, (IPrincipal)_applicationContext.GetRequiredService<ISerializationFormatter>().Deserialize(request.Principal),
          true,
          request.ClientCulture,
          request.ClientUICulture,
          (IContextDictionary)_applicationContext.GetRequiredService<ISerializationFormatter>().Deserialize(request.ClientContext));

        var dpr = await dataPortalServer.Fetch(objectType, criteria, context, true);

        if (dpr.Error != null)
          result.ErrorData = _applicationContext.CreateInstance<DataPortalErrorInfo>(_applicationContext, dpr.Error);
        result.ObjectData = _applicationContext.GetRequiredService<ISerializationFormatter>().Serialize(dpr.ReturnObject);
      }
      catch (Exception ex)
      {
        result.ErrorData = _applicationContext.CreateInstance<DataPortalErrorInfo>(_applicationContext, ex);
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
      var result = _applicationContext.CreateInstanceDI<DataPortalResponse>();
      try
      {
        request = ConvertRequest(request);
        // unpack object
        object obj = GetCriteria(_applicationContext, request.ObjectData) ?? throw new InvalidOperationException(Resources.ObjectToBeUpdatedCouldNotBeDeserialized);

        var context = new DataPortalContext(
          _applicationContext, (IPrincipal)_applicationContext.GetRequiredService<ISerializationFormatter>().Deserialize(request.Principal),
          true,
          request.ClientCulture,
          request.ClientUICulture,
          (IContextDictionary)_applicationContext.GetRequiredService<ISerializationFormatter>().Deserialize(request.ClientContext));

        var dpr = await dataPortalServer.Update(obj, context, true);

        if (dpr.Error != null)
          result.ErrorData = _applicationContext.CreateInstance<DataPortalErrorInfo>(_applicationContext, dpr.Error);

        result.ObjectData = _applicationContext.GetRequiredService<ISerializationFormatter>().Serialize(dpr.ReturnObject);
      }
      catch (Exception ex)
      {
        result.ErrorData = _applicationContext.CreateInstance<DataPortalErrorInfo>(_applicationContext, ex);
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

        var objectType = Reflection.MethodCaller.GetType(AssemblyNameTranslator.GetAssemblyQualifiedName(request.TypeName));
        var context = new DataPortalContext(
          _applicationContext, (IPrincipal)_applicationContext.GetRequiredService<ISerializationFormatter>().Deserialize(request.Principal),
          true,
          request.ClientCulture,
          request.ClientUICulture,
          (IContextDictionary)_applicationContext.GetRequiredService<ISerializationFormatter>().Deserialize(request.ClientContext));

        var dpr = await dataPortalServer.Delete(objectType, criteria, context, true);

        if (dpr.Error != null)
          result.ErrorData = _applicationContext.CreateInstance<DataPortalErrorInfo>(_applicationContext, dpr.Error);
        result.ObjectData = _applicationContext.GetRequiredService<ISerializationFormatter>().Serialize(dpr.ReturnObject);
      }
      catch (Exception ex)
      {
        result.ErrorData = _applicationContext.CreateInstance<DataPortalErrorInfo>(_applicationContext, ex);
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
        criteria = applicationContext.GetRequiredService<ISerializationFormatter>().Deserialize(criteriaData);
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