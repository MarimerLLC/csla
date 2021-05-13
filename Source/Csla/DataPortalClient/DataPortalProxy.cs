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
using Csla.Core;
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
        if (!(criteria is IMobileObject))
        {
          criteria = new PrimitiveCriteria(criteria);
        }
        request.CriteriaData = SerializationFormatterFactory.GetFormatter().Serialize(criteria);
        request = ConvertRequest(request);
        var serialized = SerializationFormatterFactory.GetFormatter().Serialize(request);
        serialized = await CallDataPortalServer(serialized, "create", GetRoutingToken(objectType), isSync);
        var response = (DataPortalResponse)SerializationFormatterFactory.GetFormatter().Deserialize(serialized);
        response = ConvertResponse(response);

        var globalContext = (ContextDictionary)SerializationFormatterFactory.GetFormatter().Deserialize(response.GlobalContext);
        if (response.ErrorData == null)
        {
          var obj = SerializationFormatterFactory.GetFormatter().Deserialize(response.ObjectData);
          result = new DataPortalResult(obj, null, globalContext);
        }
        else if (response.ErrorData != null)
        {
          var ex = new DataPortalException(response.ErrorData);
          result = new DataPortalResult(null, ex, globalContext);
        }
        else
        {
          throw new DataPortalException("null response", null);
        }
      }
      catch (Exception ex)
      {
        result = new DataPortalResult(null, ex, null);
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
        if (!(criteria is IMobileObject))
        {
          criteria = new PrimitiveCriteria(criteria);
        }
        request.CriteriaData = SerializationFormatterFactory.GetFormatter().Serialize(criteria);
        request = ConvertRequest(request);

        var serialized = SerializationFormatterFactory.GetFormatter().Serialize(request);

        serialized = await CallDataPortalServer(serialized, "fetch", GetRoutingToken(objectType), isSync);

        var response = (DataPortalResponse)SerializationFormatterFactory.GetFormatter().Deserialize(serialized);
        response = ConvertResponse(response);
        var globalContext = (ContextDictionary)SerializationFormatterFactory.GetFormatter().Deserialize(response.GlobalContext);
        if (response.ErrorData == null)
        {
          var obj = SerializationFormatterFactory.GetFormatter().Deserialize(response.ObjectData);
          result = new DataPortalResult(obj, null, globalContext);
        }
        else if (response.ErrorData != null)
        {
          var ex = new DataPortalException(response.ErrorData);
          result = new DataPortalResult(null, ex, globalContext);
        }
        else
        {
          throw new DataPortalException("null response", null);
        }
      }
      catch (Exception ex)
      {
        result = new DataPortalResult(null, ex, null);
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
        var globalContext = (ContextDictionary)SerializationFormatterFactory.GetFormatter().Deserialize(response.GlobalContext);
        if (response.ErrorData == null)
        {
          var newobj = SerializationFormatterFactory.GetFormatter().Deserialize(response.ObjectData);
          result = new DataPortalResult(newobj, null, globalContext);
        }
        else if (response.ErrorData != null)
        {
          var ex = new DataPortalException(response.ErrorData);
          result = new DataPortalResult(null, ex, globalContext);
        }
        else
        {
          throw new DataPortalException("null response", null);
        }
      }
      catch (Exception ex)
      {
        result = new DataPortalResult(null, ex, null);
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
        if (!(criteria is IMobileObject))
        {
          criteria = new PrimitiveCriteria(criteria);
        }
        request.CriteriaData = SerializationFormatterFactory.GetFormatter().Serialize(criteria);
        request = ConvertRequest(request);

        var serialized = SerializationFormatterFactory.GetFormatter().Serialize(request);

        serialized = await CallDataPortalServer(serialized, "delete", GetRoutingToken(objectType), isSync);

        var response = (DataPortalResponse)SerializationFormatterFactory.GetFormatter().Deserialize(serialized);
        response = ConvertResponse(response);
        var globalContext = (ContextDictionary)SerializationFormatterFactory.GetFormatter().Deserialize(response.GlobalContext);
        if (response.ErrorData == null)
        {
          result = new DataPortalResult(null, null, globalContext);
        }
        else if (response.ErrorData != null)
        {
          var ex = new DataPortalException(response.ErrorData);
          result = new DataPortalResult(null, ex, globalContext);
        }
        else
        {
          throw new DataPortalException("null response", null);
        }
      }
      catch (Exception ex)
      {
        result = new DataPortalResult(null, ex, null);
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

    protected abstract Task<byte[]> CallDataPortalServer(byte[] serialized, string operation, string routingToken, bool isSync);

    protected virtual string GetRoutingToken(Type objectType)
    {
      string result = null;
      var list = objectType.GetCustomAttributes(typeof(DataPortalServerRoutingTagAttribute), false);
      if (list.Length > 0)
        result = ((DataPortalServerRoutingTagAttribute)list[0]).RoutingTag;
      return result;
    }

    #region Criteria

    private static CriteriaRequest GetBaseCriteriaRequest()
    {
      return new CriteriaRequest
      {
        CriteriaData = null,
        ClientContext = SerializationFormatterFactory.GetFormatter().Serialize(ApplicationContext.ClientContext),
#pragma warning disable CS0618 // Type or member is obsolete
        GlobalContext = SerializationFormatterFactory.GetFormatter().Serialize(ApplicationContext.GlobalContext),
#pragma warning restore CS0618 // Type or member is obsolete
        Principal = SerializationFormatterFactory.GetFormatter()
          .Serialize(ApplicationContext.AuthenticationType == "Windows" ? null : ApplicationContext.User),
        ClientCulture = System.Globalization.CultureInfo.CurrentCulture.Name,
        ClientUICulture = System.Globalization.CultureInfo.CurrentUICulture.Name
      };
    }

    private static UpdateRequest GetBaseUpdateCriteriaRequest()
    {
      return new UpdateRequest
      {
        ObjectData = null,
        ClientContext = SerializationFormatterFactory.GetFormatter().Serialize(ApplicationContext.ClientContext),
#pragma warning disable CS0618 // Type or member is obsolete
        GlobalContext = SerializationFormatterFactory.GetFormatter().Serialize(ApplicationContext.GlobalContext),
#pragma warning restore CS0618 // Type or member is obsolete
        Principal = SerializationFormatterFactory.GetFormatter()
          .Serialize(ApplicationContext.AuthenticationType == "Windows" ? null : ApplicationContext.User),
        ClientCulture = Thread.CurrentThread.CurrentCulture.Name,
        ClientUICulture = Thread.CurrentThread.CurrentUICulture.Name
      };
    }

    #endregion Criteria
  }
}