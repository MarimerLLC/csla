//-----------------------------------------------------------------------
// <copyright file="DataPortalProxy.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Abstract data portal proxy with common data portal proxy behaviour. Implements IDataPortalProxy</summary>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Csla.Configuration;
using Csla.Serialization;
using Csla.Serialization.Mobile;
using Csla.Server;
using Csla.Server.Hosts.DataPortalChannel;

namespace Csla.DataPortalClient
{
  /// <summary>
  /// Implements a data portal proxy to relay data portal
  /// calls to a remote application server.
  /// </summary>
  public abstract class DataPortalProxy : IDataPortalProxy
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="applicationContext"></param>
    /// <exception cref="ArgumentNullException"><paramref name="applicationContext"/> is <see langword="null"/>.</exception>
    protected DataPortalProxy(ApplicationContext applicationContext)
    {
      ApplicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
    }

    /// <summary>
    /// Gets or sets the current ApplicationContext object.
    /// </summary>
    protected ApplicationContext ApplicationContext { get; }

    /// <summary>
    /// Gets a value indicating whether the data portal
    /// is hosted on a remote server.
    /// </summary>
    public virtual bool IsServerRemote => true;


    /// <summary>
    /// Gets the URL address for the data portal server
    /// used by this proxy instance.
    /// </summary>
    public abstract string DataPortalUrl { get; }

    /// <inheritdoc />
    public async virtual Task<DataPortalResult> Create([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      if (objectType is null)
        throw new ArgumentNullException(nameof(objectType));
      if (criteria is null)
        throw new ArgumentNullException(nameof(criteria));
      if (context is null)
        throw new ArgumentNullException(nameof(context));

      DataPortalResult result;
      try
      {
        var request = GetBaseCriteriaRequest(criteria);
        request.TypeName = AssemblyNameTranslator.GetAssemblyQualifiedName(objectType.AssemblyQualifiedName!);
        
        request = ConvertRequest(request);
        var serialized = ApplicationContext.GetRequiredService<ISerializationFormatter>().Serialize(request);
        serialized = await CallDataPortalServer(serialized, "create", GetRoutingToken(objectType), isSync).ConfigureAwait(false);
        var response = (DataPortalResponse)ApplicationContext.GetRequiredService<ISerializationFormatter>().Deserialize(serialized)!;
        response = ConvertResponse(response);

        if (!response.HasError)
        {
          var obj = ApplicationContext.GetRequiredService<ISerializationFormatter>().Deserialize(response.ObjectData);
          result = new DataPortalResult(ApplicationContext, obj, null);
        }
        else
        {
          var ex = new DataPortalException(response.ErrorData);
          result = new DataPortalResult(ApplicationContext, null, ex);
        }
      }
      catch (Exception ex)
      {
        result = new DataPortalResult(ApplicationContext, null, ex);
      }

      var operation = DataPortalOperations.Create;
      OnServerComplete(result, objectType, operation);

      if (ExecutionIsNotOnLogicalOrPhysicalServer)
        OnServerCompleteClient(result, objectType, operation);

      if (result.Error != null)
        throw result.Error;
      return result;
    }

    /// <inheritdoc />
    public async virtual Task<DataPortalResult> Fetch([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      if (objectType is null)
        throw new ArgumentNullException(nameof(objectType));
      if (criteria is null)
        throw new ArgumentNullException(nameof(criteria));
      if (context is null)
        throw new ArgumentNullException(nameof(context));

      DataPortalResult result;
      try
      {
        var request = GetBaseCriteriaRequest(criteria);
        request.TypeName = AssemblyNameTranslator.GetAssemblyQualifiedName(objectType.AssemblyQualifiedName!);
        request = ConvertRequest(request);

        var serialized = ApplicationContext.GetRequiredService<ISerializationFormatter>().Serialize(request);

        serialized = await CallDataPortalServer(serialized, "fetch", GetRoutingToken(objectType), isSync).ConfigureAwait(false);

        var response = (DataPortalResponse)ApplicationContext.GetRequiredService<ISerializationFormatter>().Deserialize(serialized)!;
        response = ConvertResponse(response);
        if (!response.HasError)
        {
          var obj = ApplicationContext.GetRequiredService<ISerializationFormatter>().Deserialize(response.ObjectData);
          result = new DataPortalResult(ApplicationContext, obj, null);
        }
        else
        {
          var ex = new DataPortalException(response.ErrorData);
          result = new DataPortalResult(ApplicationContext, null, ex);
        }
      }
      catch (Exception ex)
      {
        result = new DataPortalResult(ApplicationContext, null, ex);
      }

      var operation = DataPortalOperations.Fetch;
      OnServerComplete(result, objectType, operation);

      if (ExecutionIsNotOnLogicalOrPhysicalServer)
        OnServerCompleteClient(result, objectType, operation);

      if (result.Error != null)
        throw result.Error;
      return result;
    }

    /// <inheritdoc />
    public async virtual Task<DataPortalResult> Update(object obj, DataPortalContext context, bool isSync)
    {
      if (obj is null)
        throw new ArgumentNullException(nameof(obj));
      if (context is null)
        throw new ArgumentNullException(nameof(context));

      DataPortalResult result;
      try
      {
        var request = GetBaseUpdateCriteriaRequest(obj);
        
        request = ConvertRequest(request);

        var serialized = ApplicationContext.GetRequiredService<ISerializationFormatter>().Serialize(request);

        serialized = await CallDataPortalServer(serialized, "update", GetRoutingToken(obj.GetType()), isSync).ConfigureAwait(false);

        var response = (DataPortalResponse)ApplicationContext.GetRequiredService<ISerializationFormatter>().Deserialize(serialized)!;
        response = ConvertResponse(response);
        if (!response.HasError)
        {
          var newobj = ApplicationContext.GetRequiredService<ISerializationFormatter>().Deserialize(response.ObjectData);
          result = new DataPortalResult(ApplicationContext, newobj, null);
        }
        else
        {
          var ex = new DataPortalException(response.ErrorData);
          result = new DataPortalResult(ApplicationContext, null, ex);
        }
      }
      catch (Exception ex)
      {
        result = new DataPortalResult(ApplicationContext, null, ex);
      }

      var operation = DataPortalOperations.Update;
      OnServerComplete(result, obj.GetType(), operation);

      if (ExecutionIsNotOnLogicalOrPhysicalServer)
        OnServerCompleteClient(result, obj.GetType(), operation);

      if (result.Error != null)
        throw result.Error;
      return result;
    }

    /// <inheritdoc />
    public async virtual Task<DataPortalResult> Delete([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      if (objectType is null)
        throw new ArgumentNullException(nameof(objectType));
      if (criteria is null)
        throw new ArgumentNullException(nameof(criteria));
      if (context is null)
        throw new ArgumentNullException(nameof(context));

      DataPortalResult result;
      try
      {
        var request = GetBaseCriteriaRequest(criteria);
        request.TypeName = AssemblyNameTranslator.GetAssemblyQualifiedName(objectType.AssemblyQualifiedName!);
        request = ConvertRequest(request);

        var serialized = ApplicationContext.GetRequiredService<ISerializationFormatter>().Serialize(request);

        serialized = await CallDataPortalServer(serialized, "delete", GetRoutingToken(objectType), isSync).ConfigureAwait(false);

        var response = (DataPortalResponse)ApplicationContext.GetRequiredService<ISerializationFormatter>().Deserialize(serialized)!;
        response = ConvertResponse(response);
        if (response.ErrorData == null)
        {
          result = new DataPortalResult(ApplicationContext, null, null);
        }
        else
        {
          var ex = new DataPortalException(response.ErrorData);
          result = new DataPortalResult(ApplicationContext, null, ex);
        }
      }
      catch (Exception ex)
      {
        result = new DataPortalResult(ApplicationContext, null, ex);
      }

      var operation = DataPortalOperations.Delete;
      OnServerComplete(result, objectType, operation);

      if (ExecutionIsNotOnLogicalOrPhysicalServer)
        OnServerCompleteClient(result, objectType, operation);

      if (result.Error != null)
        throw result.Error;
      return result;
    }

    /// <summary>
    /// Override this method to manipulate the message
    /// request data sent to the server.
    /// </summary>
    /// <param name="request">Criteria request data.</param>
    protected virtual CriteriaRequest ConvertRequest(CriteriaRequest request)
    {
      return request;
    }

    /// <summary>
    /// Override this method to manipulate the message
    /// request data sent to the server.
    /// </summary>
    /// <param name="request">Update request data.</param>
    protected virtual UpdateRequest ConvertRequest(UpdateRequest request)
    {
      return request;
    }

    /// <summary>
    /// Override this method to manipulate the message
    /// request data returned from the server.
    /// </summary>
    /// <param name="response">Response data.</param>
    protected virtual DataPortalResponse ConvertResponse(DataPortalResponse response)
    {
      return response;
    }

    /// <summary>
    /// Override this method with implementation of sending and receiving of data to the server
    /// Returns Serialized response from server
    /// </summary>
    /// <param name="serialized">Serialized request</param>
    /// <param name="operation">DataPortal operation</param>
    /// <param name="routingToken">Routing Tag for server</param>
    /// <param name="isSync">True if the client-side proxy should synchronously invoke the server.</param>
    /// <returns>Serialized response from server</returns>
    protected abstract Task<byte[]> CallDataPortalServer(byte[] serialized, string operation, string? routingToken, bool isSync);

   private static string? GetRoutingToken(Type objectType)
   {
      string? result = null;
      var list = objectType.GetCustomAttributes(typeof(DataPortalServerRoutingTagAttribute), false);
      if (list.Length > 0)
        result = ((DataPortalServerRoutingTagAttribute)list[0]).RoutingTag;
      return result;
    }

    /// <summary>
    /// Called after completion of DataPortal operation regardless if operation was originated from the client or from chained calls on the server side.
    /// </summary>
    /// <param name="result">Result from DataPortal operation.</param>
    /// <param name="objectType">Type of business object.</param>
    /// <param name="operationType">The requested data portal operation type</param>
    protected virtual void OnServerComplete(DataPortalResult result, Type objectType, DataPortalOperations operationType)
    {

    }

    /// <summary>
    /// Called after completion of a DataPortal operation which was initiated from the <see cref="ApplicationContext.ExecutionLocations.Client"/> 
    /// This is NOT called on completion of chained DataPortal operations initiated on the server side.
    /// </summary>
    /// <param name="result">Result from DataPortal operation.</param>
    /// <param name="objectType">Type of business object.</param>
    /// <param name="operationType">The requested data portal operation type</param>
    protected virtual void OnServerCompleteClient(DataPortalResult result, Type objectType, DataPortalOperations operationType)
    {

    }

    internal bool ExecutionIsNotOnLogicalOrPhysicalServer =>
      ApplicationContext.LogicalExecutionLocation != ApplicationContext.LogicalExecutionLocations.Server
      && ApplicationContext.ExecutionLocation != ApplicationContext.ExecutionLocations.Server;

    #region Criteria

    private CriteriaRequest GetBaseCriteriaRequest(object criteria)
    {
      if (criteria is not IMobileObject)
        criteria = new PrimitiveCriteria(criteria);
      
      var criteriaData = ApplicationContext.GetRequiredService<ISerializationFormatter>().Serialize(criteria);

      return CreateRequest<CriteriaRequest>(criteriaData); 
    }

    private UpdateRequest GetBaseUpdateCriteriaRequest(object obj)
    {
      return CreateRequest<UpdateRequest>(ApplicationContext.GetRequiredService<ISerializationFormatter>().Serialize(obj));
    }

    private T CreateRequest<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>(object payload)
    {
      var securityOptions = ApplicationContext.GetRequiredService<SecurityOptions>();

      var serializer = ApplicationContext.GetRequiredService<ISerializationFormatter>();
      var clientContext = serializer.Serialize(ApplicationContext.ClientContext);
      var principal = serializer.Serialize(securityOptions.FlowSecurityPrincipalFromClient ? ApplicationContext.User : null);
      var clientCulture = System.Globalization.CultureInfo.CurrentCulture.Name;
      var clientUICulture = System.Globalization.CultureInfo.CurrentUICulture.Name;

      return ApplicationContext.CreateInstanceDI<T>(principal, clientContext, clientCulture, clientUICulture, payload);
    }

    #endregion Criteria
  }
}