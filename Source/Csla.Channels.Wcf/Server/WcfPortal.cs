//-----------------------------------------------------------------------
// <copyright file="WcfPortal.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Provides consistent context information between the client</summary>
//-----------------------------------------------------------------------

using Csla.Channels.Wcf.Client;
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
    /// Gets a dictionary containing the URLs for each
    /// data portal route, where each key is the
    /// routing tag identifying the route URL.
    /// </summary>
    protected static Dictionary<string, string> RoutingTagUrls = [];

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
    /// Routes a message using tag based data portal operations.
    /// </summary>
    /// <param name="operation">
    /// Name of the data portal operation to perform
    /// </param>
    /// <param name="routingTag">
    /// Routing tag from caller
    /// </param>
    /// <param name="request">
    /// Request message
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="routingTag"/> or <paramref name="request"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="routingTag"/> is <see langword="null"/>, empty or only consists of white spaces.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// <see cref="WcfPortalOptions.RouterBinding"/> is <see langword="null"/>.
    /// </exception>
    protected virtual async Task<WcfResponse> RouteMessage(string operation, string routingTag, WcfRequest request)
    {
      if (string.IsNullOrWhiteSpace(operation))
        throw new ArgumentException(string.Format(Resources.StringNotNullOrWhiteSpaceException, nameof(operation)), nameof(operation));
      if (routingTag is null)
        throw new ArgumentNullException(nameof(routingTag));
      if (request is null)
        throw new ArgumentNullException(nameof(request));

      if (RoutingTagUrls.TryGetValue(routingTag, out string? route) && route != "localhost")
      {
        var portalOptions = _applicationContext.GetRequiredService<WcfPortalOptions>();

        var routerBinding = portalOptions.RouterBinding ?? throw new InvalidOperationException($"The {nameof(WcfPortalOptions)}.{nameof(WcfPortalOptions.RouterBinding)} property must not be null in order to use data portal routing.");

        var proxyOptions = new WcfProxyOptions
        {
          Binding = routerBinding,
          DataPortalUrl = $"{route}?operation={operation}",
        };

        var dataPortalOptions = _applicationContext.GetRequiredService<Configuration.DataPortalOptions>();
        var proxy = new WcfProxy(_applicationContext, proxyOptions, dataPortalOptions);
        var clientRequest = new WcfRequest
        {
          Body = request.Body,
          Operation = operation
        };

        return await proxy.RouteMessage(clientRequest);
      }
      else
      {
        return await InvokePortal(operation, request.Body).ConfigureAwait(false);
      }
    }

    private async Task<WcfResponse> InvokePortal(string operation, byte[] requestData)
    {
      var request = DeserializeRequired<object>(requestData);
      var result = await CallPortal(operation, request);
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
