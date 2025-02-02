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
using System.Runtime.Serialization;

namespace Csla.Server.Hosts
{
  /// <summary>
  /// Exposes server-side DataPortal functionality
  /// through HTTP request/response.
  /// </summary>
  public class HttpPortal
  {
    private readonly IDataPortalServer dataPortalServer;
    private readonly ApplicationContext _applicationContext;

    /// <summary>
    /// Creates an instance of the type
    /// </summary>
    /// <param name="applicationContext"></param>
    /// <param name="dataPortal">Data portal server service</param>
    public HttpPortal(ApplicationContext applicationContext, IDataPortalServer dataPortal)
    {
      dataPortalServer = dataPortal ?? throw new ArgumentNullException(nameof(dataPortal));
      _applicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
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

      var result = new DataPortalResponse();
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
          _applicationContext, Deserialize<IPrincipal>(request.Principal),
          true,
          request.ClientCulture,
          request.ClientUICulture,
          Deserialize<IContextDictionary>(request.ClientContext));

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
    /// <exception cref="ArgumentNullException"><paramref name="request"/> is <see langword="null"/>.</exception>
    public async Task<DataPortalResponse> Fetch(CriteriaRequest request)
    {
      if (request is null)
        throw new ArgumentNullException(nameof(request));

      var result = new DataPortalResponse();
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
          _applicationContext, Deserialize<IPrincipal>(request.Principal),
          true,
          request.ClientCulture,
          request.ClientUICulture,
          Deserialize<IContextDictionary>(request.ClientContext));

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
    /// <exception cref="ArgumentNullException"><paramref name="request"/> is <see langword="null"/>.</exception>
    public async Task<DataPortalResponse> Update(UpdateRequest request)
    {
      if (request is null)
        throw new ArgumentNullException(nameof(request));

      var result = new DataPortalResponse();
      try
      {
        request = ConvertRequest(request);
        object obj = GetCriteria(_applicationContext, request.ObjectData ?? throw new InvalidOperationException(Resources.ObjectToBeUpdatedCouldNotBeDeserialized));

        var context = new DataPortalContext(
          _applicationContext, Deserialize<IPrincipal>(request.Principal),
          true,
          request.ClientCulture,
          request.ClientUICulture,
          Deserialize<IContextDictionary>(request.ClientContext));

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
    /// <exception cref="ArgumentNullException"><paramref name="request"/> is <see langword="null"/>.</exception>
    public async Task<DataPortalResponse> Delete(CriteriaRequest request)
    {
      if (request is null)
        throw new ArgumentNullException(nameof(request));

      var result = new DataPortalResponse();
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
          _applicationContext, Deserialize<IPrincipal>(request.Principal),
          true,
          request.ClientCulture,
          request.ClientUICulture,
          Deserialize<IContextDictionary>(request.ClientContext));

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

    private static object GetCriteria(ApplicationContext applicationContext, byte[] criteriaData)
    {
      return applicationContext.GetRequiredService<ISerializationFormatter>().Deserialize(criteriaData) ?? throw new SerializationException(Resources.ServerSideDataPortalRequestDeserializationFailed);
    }

    #endregion Criteria

    #region Extension Method for Requests

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

    #endregion Extension Method for Requests

    private T Deserialize<T>(byte[] data)
    {
      var deserializedData = _applicationContext.GetRequiredService<ISerializationFormatter>().Deserialize(data) ?? throw new SerializationException(Resources.ServerSideDataPortalRequestDeserializationFailed);
      if (deserializedData is not T castedData)
        throw new SerializationException(string.Format(Resources.DeserializationFailedDueToWrongData, typeof(T).FullName));

      return castedData;
    }
  }
}