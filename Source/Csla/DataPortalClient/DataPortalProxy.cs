﻿//-----------------------------------------------------------------------
// <copyright file="DataPortalProxy.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Abstract data portal proxy with common data portal proxy behaviour. Implements IDataPortalProxy</summary>
//-----------------------------------------------------------------------

using Csla.Serialization.Mobile;
using Csla.Serialization;
using Csla.Server;
using Csla.Server.Hosts.DataPortalChannel;
using Csla.Configuration;

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
    public DataPortalProxy(ApplicationContext applicationContext)
    {
      ApplicationContext = applicationContext;
    }

    /// <summary>
    /// Gets or sets the current ApplicationContext object.
    /// </summary>
    protected ApplicationContext ApplicationContext { get; set; }

    /// <summary>
    /// Gets a value indicating whether the data portal
    /// is hosted on a remote server.
    /// </summary>
    public virtual bool IsServerRemote => true;

    /// <summary>
    /// Gets or sets the Client timeout
    /// in milliseconds (0 uses default timeout).
    /// </summary>
    public virtual int Timeout { get; set; }

    /// <summary>
    /// Gets the URL address for the data portal server
    /// used by this proxy instance.
    /// </summary>
    public string DataPortalUrl { get; protected set; }

    /// <summary>
    /// Called by <see cref="DataPortal" /> to create a
    /// new business object.
    /// </summary>
    /// <param name="objectType">Type of business object to create.</param>
    /// <param name="criteria">Criteria object describing business object.</param>
    /// <param name="context">
    /// <see cref="Server.DataPortalContext" /> object passed to the server.
    /// </param>
    /// <param name="isSync">True if the client-side proxy should synchronously invoke the server.</param>
    public async virtual Task<DataPortalResult> Create(Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      DataPortalResult result;
      try
      {
        var request = GetBaseCriteriaRequest();
        request.TypeName = AssemblyNameTranslator.GetAssemblyQualifiedName(objectType.AssemblyQualifiedName);
        if (criteria is not IMobileObject)
        {
          criteria = new PrimitiveCriteria(criteria);
        }
        request.CriteriaData = SerializationFormatterFactory.GetFormatter(ApplicationContext).Serialize(criteria);
        request = ConvertRequest(request);
        var serialized = SerializationFormatterFactory.GetFormatter(ApplicationContext).Serialize(request);
        serialized = await CallDataPortalServer(serialized, "create", GetRoutingToken(objectType), isSync).ConfigureAwait(false);
        var response = (DataPortalResponse)SerializationFormatterFactory.GetFormatter(ApplicationContext).Deserialize(serialized);
        response = ConvertResponse(response);

        if (response.ErrorData == null)
        {
          var obj = SerializationFormatterFactory.GetFormatter(ApplicationContext).Deserialize(response.ObjectData);
          result = new DataPortalResult(ApplicationContext, obj, null);
        }
        else if (response.ErrorData != null)
        {
          var ex = new DataPortalException(response.ErrorData);
          result = new DataPortalResult(ApplicationContext, null, ex);
        }
        else
        {
          throw new DataPortalException("null response", null);
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

    /// <summary>
    /// Called by <see cref="DataPortal" /> to load an
    /// existing business object.
    /// </summary>
    /// <param name="objectType">Type of business object to create.</param>
    /// <param name="criteria">Criteria object describing business object.</param>
    /// <param name="context">
    /// <see cref="Server.DataPortalContext" /> object passed to the server.
    /// </param>
    /// <param name="isSync">True if the client-side proxy should synchronously invoke the server.</param>
    public async virtual Task<DataPortalResult> Fetch(Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      DataPortalResult result;
      try
      {
        var request = GetBaseCriteriaRequest();
        request.TypeName = AssemblyNameTranslator.GetAssemblyQualifiedName(objectType.AssemblyQualifiedName);
        if (criteria is not IMobileObject)
        {
          criteria = new PrimitiveCriteria(criteria);
        }
        request.CriteriaData = SerializationFormatterFactory.GetFormatter(ApplicationContext).Serialize(criteria);
        request = ConvertRequest(request);

        var serialized = SerializationFormatterFactory.GetFormatter(ApplicationContext).Serialize(request);

        serialized = await CallDataPortalServer(serialized, "fetch", GetRoutingToken(objectType), isSync).ConfigureAwait(false);

        var response = (DataPortalResponse)SerializationFormatterFactory.GetFormatter(ApplicationContext).Deserialize(serialized);
        response = ConvertResponse(response);
        if (response.ErrorData == null)
        {
          var obj = SerializationFormatterFactory.GetFormatter(ApplicationContext).Deserialize(response.ObjectData);
          result = new DataPortalResult(ApplicationContext, obj, null);
        }
        else if (response.ErrorData != null)
        {
          var ex = new DataPortalException(response.ErrorData);
          result = new DataPortalResult(ApplicationContext, null, ex);
        }
        else
        {
          throw new DataPortalException("null response", null);
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

    /// <summary>
    /// Called by <see cref="DataPortal" /> to update a
    /// business object.
    /// </summary>
    /// <param name="obj">The business object to update.</param>
    /// <param name="context">
    /// <see cref="Server.DataPortalContext" /> object passed to the server.
    /// </param>
    /// <param name="isSync">True if the client-side proxy should synchronously invoke the server.</param>
    public async virtual Task<DataPortalResult> Update(object obj, DataPortalContext context, bool isSync)
    {
      DataPortalResult result;
      try
      {
        var request = GetBaseUpdateCriteriaRequest();
        request.ObjectData = SerializationFormatterFactory.GetFormatter(ApplicationContext).Serialize(obj);
        request = ConvertRequest(request);

        var serialized = SerializationFormatterFactory.GetFormatter(ApplicationContext).Serialize(request);

        serialized = await CallDataPortalServer(serialized, "update", GetRoutingToken(obj.GetType()), isSync).ConfigureAwait(false);

        var response = (DataPortalResponse)SerializationFormatterFactory.GetFormatter(ApplicationContext).Deserialize(serialized);
        response = ConvertResponse(response);
        if (response.ErrorData == null)
        {
          var newobj = SerializationFormatterFactory.GetFormatter(ApplicationContext).Deserialize(response.ObjectData);
          result = new DataPortalResult(ApplicationContext, newobj, null);
        }
        else if (response.ErrorData != null)
        {
          var ex = new DataPortalException(response.ErrorData);
          result = new DataPortalResult(ApplicationContext, null, ex);
        }
        else
        {
          throw new DataPortalException("null response", null);
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

    /// <summary>
    /// Called by <see cref="DataPortal" /> to delete a
    /// business object.
    /// </summary>
    /// <param name="objectType">Type of business object to create.</param>
    /// <param name="criteria">Criteria object describing business object.</param>
    /// <param name="context">
    /// <see cref="Server.DataPortalContext" /> object passed to the server.
    /// </param>
    /// <param name="isSync">True if the client-side proxy should synchronously invoke the server.</param>
    public async virtual Task<DataPortalResult> Delete(Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      DataPortalResult result;
      try
      {
        var request = GetBaseCriteriaRequest();
        request.TypeName = AssemblyNameTranslator.GetAssemblyQualifiedName(objectType.AssemblyQualifiedName);
        if (criteria is not IMobileObject)
        {
          criteria = new PrimitiveCriteria(criteria);
        }
        request.CriteriaData = SerializationFormatterFactory.GetFormatter(ApplicationContext).Serialize(criteria);
        request = ConvertRequest(request);

        var serialized = SerializationFormatterFactory.GetFormatter(ApplicationContext).Serialize(request);

        serialized = await CallDataPortalServer(serialized, "delete", GetRoutingToken(objectType), isSync).ConfigureAwait(false);

        var response = (DataPortalResponse)SerializationFormatterFactory.GetFormatter(ApplicationContext).Deserialize(serialized);
        response = ConvertResponse(response);
        if (response.ErrorData == null)
        {
          result = new DataPortalResult(ApplicationContext, null, null);
        }
        else if (response.ErrorData != null)
        {
          var ex = new DataPortalException(response.ErrorData);
          result = new DataPortalResult(ApplicationContext, null, ex);
        }
        else
        {
          throw new DataPortalException("null response", null);
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
    protected abstract Task<byte[]> CallDataPortalServer(byte[] serialized, string operation, string routingToken, bool isSync);

    private string GetRoutingToken(Type objectType)
    {
      string result = null;
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

    internal bool ExecutionIsNotOnLogicalOrPhysicalServer
    {
      get
      {
        return ApplicationContext.LogicalExecutionLocation != ApplicationContext.LogicalExecutionLocations.Server
          && ApplicationContext.ExecutionLocation != ApplicationContext.ExecutionLocations.Server;
      }
    }

    #region Criteria

    private CriteriaRequest GetBaseCriteriaRequest()
    {
      var result = ApplicationContext.CreateInstanceDI<CriteriaRequest>();
      var securityOptions = ApplicationContext.GetRequiredService<SecurityOptions>();
      result.CriteriaData = null;
      result.ClientContext = SerializationFormatterFactory.GetFormatter(ApplicationContext).Serialize(ApplicationContext.ClientContext);
      result.Principal = SerializationFormatterFactory.GetFormatter(ApplicationContext)
          .Serialize(securityOptions.FlowSecurityPrincipalFromClient ? ApplicationContext.User : null);
      result.ClientCulture = System.Globalization.CultureInfo.CurrentCulture.Name;
      result.ClientUICulture = System.Globalization.CultureInfo.CurrentUICulture.Name;
      return result;
    }

    private UpdateRequest GetBaseUpdateCriteriaRequest()
    {
      var result = ApplicationContext.CreateInstanceDI<UpdateRequest>();
      var securityOptions = ApplicationContext.GetRequiredService<SecurityOptions>();
      result.ObjectData = null;
      result.ClientContext = SerializationFormatterFactory.GetFormatter(ApplicationContext).Serialize(ApplicationContext.ClientContext);
      result.Principal = SerializationFormatterFactory.GetFormatter(ApplicationContext)
          .Serialize(securityOptions.FlowSecurityPrincipalFromClient ? ApplicationContext.User : null);
      result.ClientCulture = Thread.CurrentThread.CurrentCulture.Name;
      result.ClientUICulture = Thread.CurrentThread.CurrentUICulture.Name;
      return result;
    }

    #endregion Criteria
  }
}