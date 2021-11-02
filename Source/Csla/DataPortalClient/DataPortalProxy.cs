//-----------------------------------------------------------------------
// <copyright file="DataPortalProxy.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Abstract data portal proxy with common data portal proxy behaviour. Implements IDataPortalProxy</summary>
//-----------------------------------------------------------------------
using System;
using System.Threading;
using System.Threading.Tasks;
using Csla.Serialization.Mobile;
using Csla.Serialization;
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
    public string DataPortalUrl { get; protected set; } = ApplicationContext.DataPortalUrlString;

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
        request.CriteriaData = SerializationFormatterFactory.GetFormatter().Serialize(criteria);
        request = ConvertRequest(request);
        var serialized = SerializationFormatterFactory.GetFormatter().Serialize(request);
        serialized = await CallDataPortalServer(serialized, "create", GetRoutingToken(objectType), isSync);
        var response = (DataPortalResponse)SerializationFormatterFactory.GetFormatter().Deserialize(serialized);
        response = ConvertResponse(response);

        if (response.ErrorData == null)
        {
          var obj = SerializationFormatterFactory.GetFormatter().Deserialize(response.ObjectData);
          result = new DataPortalResult(obj, null);
        }
        else if (response.ErrorData != null)
        {
          var ex = new DataPortalException(response.ErrorData);
          result = new DataPortalResult(null, ex);
        }
        else
        {
          throw new DataPortalException("null response", null);
        }
      }
      catch (Exception ex)
      {
        result = new DataPortalResult(null, ex);
      }
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
        request.CriteriaData = SerializationFormatterFactory.GetFormatter().Serialize(criteria);
        request = ConvertRequest(request);

        var serialized = SerializationFormatterFactory.GetFormatter().Serialize(request);

        serialized = await CallDataPortalServer(serialized, "fetch", GetRoutingToken(objectType), isSync);

        var response = (DataPortalResponse)SerializationFormatterFactory.GetFormatter().Deserialize(serialized);
        response = ConvertResponse(response);
        if (response.ErrorData == null)
        {
          var obj = SerializationFormatterFactory.GetFormatter().Deserialize(response.ObjectData);
          result = new DataPortalResult(obj, null);
        }
        else if (response.ErrorData != null)
        {
          var ex = new DataPortalException(response.ErrorData);
          result = new DataPortalResult(null, ex);
        }
        else
        {
          throw new DataPortalException("null response", null);
        }
      }
      catch (Exception ex)
      {
        result = new DataPortalResult(null, ex);
      }
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
        request.ObjectData = SerializationFormatterFactory.GetFormatter().Serialize(obj);
        request = ConvertRequest(request);

        var serialized = SerializationFormatterFactory.GetFormatter().Serialize(request);

        serialized = await CallDataPortalServer(serialized, "update", GetRoutingToken(obj.GetType()), isSync);

        var response = (DataPortalResponse)SerializationFormatterFactory.GetFormatter().Deserialize(serialized);
        response = ConvertResponse(response);
        if (response.ErrorData == null)
        {
          var newobj = SerializationFormatterFactory.GetFormatter().Deserialize(response.ObjectData);
          result = new DataPortalResult(newobj, null);
        }
        else if (response.ErrorData != null)
        {
          var ex = new DataPortalException(response.ErrorData);
          result = new DataPortalResult(null, ex);
        }
        else
        {
          throw new DataPortalException("null response", null);
        }
      }
      catch (Exception ex)
      {
        result = new DataPortalResult(null, ex);
      }
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
    public async virtual Task<DataPortalResult> Delete(Type objectType, object criteria, DataPortalContext context,
      bool isSync)
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
        request.CriteriaData = SerializationFormatterFactory.GetFormatter().Serialize(criteria);
        request = ConvertRequest(request);

        var serialized = SerializationFormatterFactory.GetFormatter().Serialize(request);

        serialized = await CallDataPortalServer(serialized, "delete", GetRoutingToken(objectType), isSync);

        var response = (DataPortalResponse)SerializationFormatterFactory.GetFormatter().Deserialize(serialized);
        response = ConvertResponse(response);
        if (response.ErrorData == null)
        {
          result = new DataPortalResult(null, null);
        }
        else if (response.ErrorData != null)
        {
          var ex = new DataPortalException(response.ErrorData);
          result = new DataPortalResult(null, ex);
        }
        else
        {
          throw new DataPortalException("null response", null);
        }
      }
      catch (Exception ex)
      {
        result = new DataPortalResult(null, ex);
      }
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
    /// Returns serialised response from server
    /// </summary>
    /// <param name="serialized">Serialised request</param>
    /// <param name="operation">DataPortal operation</param>
    /// <param name="routingToken">Routing Tag for server</param>
    /// <param name="isSync">True if the client-side proxy should synchronously invoke the server.</param>
    /// <returns>Serialised response from server</returns>
    protected abstract Task<byte[]> CallDataPortalServer(byte[] serialized, string operation, string routingToken, bool isSync);

    private string GetRoutingToken(Type objectType)
    {
      string result = null;
      var list = objectType.GetCustomAttributes(typeof(DataPortalServerRoutingTagAttribute), false);
      if (list.Length > 0)
        result = ((DataPortalServerRoutingTagAttribute)list[0]).RoutingTag;
      return result;
    }

    #region Criteria

    private CriteriaRequest GetBaseCriteriaRequest()
    {
      var result = ApplicationContext.CreateInstance<CriteriaRequest>();
      result.CriteriaData = null;
      result.ClientContext = SerializationFormatterFactory.GetFormatter().Serialize(ApplicationContext.ClientContext);
      result.Principal = SerializationFormatterFactory.GetFormatter()
          .Serialize(ApplicationContext.AuthenticationType == "Windows" ? null : ApplicationContext.User);
      result.ClientCulture = System.Globalization.CultureInfo.CurrentCulture.Name;
      result.ClientUICulture = System.Globalization.CultureInfo.CurrentUICulture.Name;
      return result;
    }

    private UpdateRequest GetBaseUpdateCriteriaRequest()
    {
      var result = ApplicationContext.CreateInstance<UpdateRequest>();
      result.ObjectData = null;
      result.ClientContext = SerializationFormatterFactory.GetFormatter().Serialize(ApplicationContext.ClientContext);
      result.Principal = SerializationFormatterFactory.GetFormatter()
          .Serialize(ApplicationContext.AuthenticationType == "Windows" ? null : ApplicationContext.User);
      result.ClientCulture = Thread.CurrentThread.CurrentCulture.Name;
      result.ClientUICulture = Thread.CurrentThread.CurrentUICulture.Name;
      return result;
    }

    #endregion Criteria
  }
}