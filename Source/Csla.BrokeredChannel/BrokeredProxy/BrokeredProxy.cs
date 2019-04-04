//-----------------------------------------------------------------------
// <copyright file="BrokeredProxy.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implements a data portal proxy to relay data portal</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Csla.Server;
using Csla.Serialization.Mobile;
using Csla.DataPortalClient;
using Csla.BrokeredDataPortalHost;

namespace Csla.DataPortalClient
{
  /// <summary>
  /// Implements a data portal proxy to relay data portal
  /// calls to an application server hosted locally 
  /// in the client process and AppDomain.
  /// </summary>
  public class BrokeredProxy : Csla.DataPortalClient.IDataPortalProxy
  {
    /// <summary>
    /// Get a value indicating whether this proxy will invoke
    /// a remote data portal server, or run the "server-side"
    /// data portal in the caller's process and AppDomain.
    /// </summary>
    public bool IsServerRemote
    {
      get { return true; }
    }

    /// <summary>
    /// Called by the client-side DataPortal to create a
    /// new business object.
    /// </summary>
    /// <param name="objectType">Type of business object to create.</param>
    /// <param name="criteria">Criteria object describing business object.</param>
    /// <param name="context">Server.DataPortalContext object passed to the server.</param>
    /// <param name="isSync">True if the client-side proxy should synchronously invoke the server.</param>
    public async Task<DataPortalResult> Create(Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      DataPortalResult result = null;
      if (isSync)
        throw new NotSupportedException("isSync == true");
      try
      {
        if (!(criteria is IMobileObject))
          criteria = new PrimitiveCriteria(criteria);
        var contextData = Csla.Serialization.Mobile.MobileFormatter.Serialize(context);
        var criteriaData = Csla.Serialization.Mobile.MobileFormatter.Serialize(criteria);
        var portal = new BrokeredHost();
        var objectTypeName = objectType.AssemblyQualifiedName;
        var resultData = await portal.Create(objectTypeName, criteriaData, contextData);
        var response = (Csla.Server.Hosts.HttpChannel.HttpResponse)Csla.Serialization.Mobile.MobileFormatter.Deserialize(resultData.ToArray());
        var globalContext = (Csla.Core.ContextDictionary)Csla.Serialization.Mobile.MobileFormatter.Deserialize(response.GlobalContext);
        if (response != null && response.ErrorData == null)
        {
          var obj = MobileFormatter.Deserialize(response.ObjectData);
          result = new DataPortalResult(obj, null, globalContext);
        }
        else if (response != null && response.ErrorData != null)
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
    /// Called by the client-side DataPortal to fetch a
    /// business object.
    /// </summary>
    /// <param name="objectType">Type of business object to fetch.</param>
    /// <param name="criteria">Criteria object describing business object.</param>
    /// <param name="context">Server.DataPortalContext object passed to the server.</param>
    /// <param name="isSync">True if the client-side proxy should synchronously invoke the server.</param>
    public async Task<DataPortalResult> Fetch(Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      DataPortalResult result = null;
      if (isSync)
        throw new NotSupportedException("isSync == true");
      try
      {
        if (!(criteria is IMobileObject))
          criteria = new PrimitiveCriteria(criteria);
        var contextData = Csla.Serialization.Mobile.MobileFormatter.Serialize(context);
        var criteriaData = Csla.Serialization.Mobile.MobileFormatter.Serialize(criteria);
        var portal = new BrokeredHost();
        var objectTypeName = objectType.AssemblyQualifiedName;
        var resultData = await portal.Fetch(objectTypeName, criteriaData, contextData);
        var response = (Csla.Server.Hosts.HttpChannel.HttpResponse)Csla.Serialization.Mobile.MobileFormatter.Deserialize(resultData.ToArray());
        var globalContext = (Csla.Core.ContextDictionary)Csla.Serialization.Mobile.MobileFormatter.Deserialize(response.GlobalContext);
        if (response != null && response.ErrorData == null)
        {
          var obj = MobileFormatter.Deserialize(response.ObjectData);
          result = new DataPortalResult(obj, null, globalContext);
        }
        else if (response != null && response.ErrorData != null)
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
    /// Called by the client-side DataPortal to update a
    /// business object.
    /// </summary>
    /// <param name="obj">Business object to update.</param>
    /// <param name="context">Server.DataPortalContext object passed to the server.</param>
    /// <param name="isSync">True if the client-side proxy should synchronously invoke the server.</param>
    public async Task<DataPortalResult> Update(object obj, DataPortalContext context, bool isSync)
    {
      DataPortalResult result = null;
      if (isSync)
        throw new NotSupportedException("isSync == true");
      try
      {
        var contextData = Csla.Serialization.Mobile.MobileFormatter.Serialize(context);
        var objectData = Csla.Serialization.Mobile.MobileFormatter.Serialize(obj);
        var portal = new BrokeredHost();
        var resultData = await portal.Update(objectData, contextData);
        var response = (Csla.Server.Hosts.HttpChannel.HttpResponse)Csla.Serialization.Mobile.MobileFormatter.Deserialize(resultData.ToArray());
        var globalContext = (Csla.Core.ContextDictionary)Csla.Serialization.Mobile.MobileFormatter.Deserialize(response.GlobalContext);
        if (response != null && response.ErrorData == null)
        {
          obj = MobileFormatter.Deserialize(response.ObjectData);
          result = new DataPortalResult(obj, null, globalContext);
        }
        else if (response != null && response.ErrorData != null)
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
    /// Called by the client-side DataPortal to delete a
    /// business object.
    /// </summary>
    /// <param name="objectType">Type of business object to delete.</param>
    /// <param name="criteria">Criteria object describing business object.</param>
    /// <param name="context">Server.DataPortalContext object passed to the server.</param>
    /// <param name="isSync">True if the client-side proxy should synchronously invoke the server.</param>
    public async Task<DataPortalResult> Delete(Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      DataPortalResult result = null;
      if (isSync)
        throw new NotSupportedException("isSync == true");
      try
      {
        if (!(criteria is IMobileObject))
          criteria = new PrimitiveCriteria(criteria);
        var contextData = Csla.Serialization.Mobile.MobileFormatter.Serialize(context);
        var criteriaData = Csla.Serialization.Mobile.MobileFormatter.Serialize(criteria);
        var portal = new BrokeredHost();
        var objectTypeName = objectType.AssemblyQualifiedName;
        var resultData = await portal.Delete(objectTypeName, criteriaData, contextData);
        var response = (Csla.Server.Hosts.HttpChannel.HttpResponse)Csla.Serialization.Mobile.MobileFormatter.Deserialize(resultData.ToArray());
        var globalContext = (Csla.Core.ContextDictionary)Csla.Serialization.Mobile.MobileFormatter.Deserialize(response.GlobalContext);
        if (response != null && response.ErrorData == null)
        {
          var obj = MobileFormatter.Deserialize(response.ObjectData);
          result = new DataPortalResult(obj, null, globalContext);
        }
        else if (response != null && response.ErrorData != null)
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
  }
}
