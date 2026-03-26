//-----------------------------------------------------------------------
// <copyright file="WcfPortal.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Provides consistent context information between the client</summary>
//-----------------------------------------------------------------------

using Csla.Core;
using Csla.Properties;
using Csla.Serialization;
using Csla.Serialization.Mobile;
using Csla.Server;
using Csla.Server.Hosts.DataPortalChannel;
using System.Runtime.Serialization;
using System.Security.Principal;

#if NETFRAMEWORK
using System.ServiceModel;
#else
using CoreWCF;
#endif

namespace Csla.Channels.Wcf.Server
{
  /// <summary>
  /// Represents a server side data portal that is called via WCF.
  /// </summary>
  /// <param name="dataPortal">
  /// The server side data portal that processes the data portal requests.
  /// </param>
  /// <param name="applicationContext">
  /// The server side context for the data portal.
  /// </param>
  /// <exception cref="ArgumentNullException">
  /// <paramref name="dataPortal"/> or <paramref name="applicationContext"/> is <see langword="null"/>.
  /// </exception>
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
  public class WcfPortal(IDataPortalServer dataPortal, ApplicationContext applicationContext) : IWcfPortalServer
  {
    private readonly IDataPortalServer _dataPortalServer = dataPortal ?? throw new ArgumentNullException(nameof(dataPortal));
    private readonly ApplicationContext _applicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));

    /// <summary>
    /// Asynchronously invokes an operation on the remote data portal.
    /// </summary>
    /// <param name="request">
    /// The request that contains the name and parameters necessary to invoke the data portal operation.
    /// </param>
    /// <returns>
    /// As task containing the response from the remote data portal.
    /// </returns>
    public async Task<WcfResponse> InvokeAsync(WcfRequest request)
    {
      if (request is null)
        throw new ArgumentNullException(nameof(request));

      var operation = request.Operation;
      return await InvokePortal(operation, request.Body).ConfigureAwait(false);
    }

    private async Task<WcfResponse> InvokePortal(string operation, byte[] requestData)
    {
      var result = new DataPortalResponse();
      try
      {
        var request = DeserializeRequired<object>(requestData);
        var callResult = await CallPortal(operation, request);
        result.ObjectData = callResult.ObjectData;
      }
      catch (Exception ex)
      {
        result.ErrorData = _applicationContext.CreateInstanceDI<DataPortalErrorInfo>(ex);
      }

      var buffer = _applicationContext.GetRequiredService<ISerializationFormatter>().Serialize(result);
      return new WcfResponse { Body = buffer };
    }

    private async Task<DataPortalResponse> CallPortal(string operation, object request)
    {
      return operation switch
      {
        "create" => await Create((CriteriaRequest)request).ConfigureAwait(false),
        "fetch" => await Fetch((CriteriaRequest)request).ConfigureAwait(false),
        "update" => await Update((UpdateRequest)request).ConfigureAwait(false),
        "delete" => await Delete((CriteriaRequest)request).ConfigureAwait(false),
        _ => throw new InvalidOperationException(operation)
      };
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
        var criteria = GetCriteria(_applicationContext, request.CriteriaData);
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
          DeserializeRequired<IContextDictionary>(request.ClientContext));
        context.OperationName = request.OperationName;

        var dpr = await _dataPortalServer.Create(objectType, criteria, context, true);

        if (dpr.Error != null)
          result.ErrorData = _applicationContext.CreateInstanceDI<DataPortalErrorInfo>(dpr.Error);
        result.ObjectData = _applicationContext.GetRequiredService<ISerializationFormatter>().Serialize(dpr.ReturnObject);
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
        var criteria = GetCriteria(_applicationContext, request.CriteriaData);
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
          DeserializeRequired<IContextDictionary>(request.ClientContext));
        context.OperationName = request.OperationName;

        var dpr = await _dataPortalServer.Fetch(objectType, criteria, context, true);

        if (dpr.Error != null)
          result.ErrorData = _applicationContext.CreateInstanceDI<DataPortalErrorInfo>(dpr.Error);
        result.ObjectData = _applicationContext.GetRequiredService<ISerializationFormatter>().Serialize(dpr.ReturnObject);
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
        // unpack object
        var obj = GetCriteria(_applicationContext, request.ObjectData) ?? throw new InvalidOperationException(Resources.ObjectToBeUpdatedCouldNotBeDeserialized);

        var context = new DataPortalContext(
          _applicationContext, Deserialize<IPrincipal>(request.Principal),
          true,
          request.ClientCulture,
          request.ClientUICulture,
          DeserializeRequired<IContextDictionary>(request.ClientContext));

        var dpr = await _dataPortalServer.Update((ICslaObject)obj, context, true);

        if (dpr.Error != null)
          result.ErrorData = _applicationContext.CreateInstanceDI<DataPortalErrorInfo>(dpr.Error);

        result.ObjectData = _applicationContext.GetRequiredService<ISerializationFormatter>().Serialize(dpr.ReturnObject);
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
        var criteria = GetCriteria(_applicationContext, request.CriteriaData);
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
          DeserializeRequired<IContextDictionary>(request.ClientContext));
        context.OperationName = request.OperationName;

        var dpr = await _dataPortalServer.Delete(objectType, criteria, context, true);

        if (dpr.Error != null)
          result.ErrorData = _applicationContext.CreateInstanceDI<DataPortalErrorInfo>(dpr.Error);
        result.ObjectData = _applicationContext.GetRequiredService<ISerializationFormatter>().Serialize(dpr.ReturnObject);
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

    private T? Deserialize<T>(byte[] data)
    {
      var deserializedData = _applicationContext.GetRequiredService<ISerializationFormatter>().Deserialize(data);
      return (T?)deserializedData;
    }

    private T DeserializeRequired<T>(byte[] data)
    {
      return Deserialize<T>(data) ?? throw new SerializationException(Resources.ServerSideDataPortalRequestDeserializationFailed);
    }
  }
}
