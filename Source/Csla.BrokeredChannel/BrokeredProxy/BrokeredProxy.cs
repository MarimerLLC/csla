//-----------------------------------------------------------------------
// <copyright file="BrokeredProxy.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Implements a data portal proxy to relay data portal</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Csla.Server;

namespace BrokeredProxy
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
      if (isSync)
        throw new NotSupportedException("isSync == true");
      var contextData = Csla.Serialization.Mobile.MobileFormatter.Serialize(context);
      var criteriaData = Csla.Serialization.Mobile.MobileFormatter.Serialize(criteria);
      var portal = new BrokeredDataPortal.BrokeredPortal();
      var resultData = await portal.Create(objectType, criteriaData, contextData);
      var result = (DataPortalResult)Csla.Serialization.Mobile.MobileFormatter.Deserialize(resultData.ToArray());
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
      if (isSync)
        throw new NotSupportedException("isSync == true");
      var contextData = Csla.Serialization.Mobile.MobileFormatter.Serialize(context);
      var criteriaData = Csla.Serialization.Mobile.MobileFormatter.Serialize(criteria);
      var portal = new BrokeredDataPortal.BrokeredPortal();
      var resultData = await portal.Fetch(objectType, criteriaData, contextData);
      var result = (DataPortalResult)Csla.Serialization.Mobile.MobileFormatter.Deserialize(resultData.ToArray());
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
      if (isSync)
        throw new NotSupportedException("isSync == true");
      var contextData = Csla.Serialization.Mobile.MobileFormatter.Serialize(context);
      var objectData = Csla.Serialization.Mobile.MobileFormatter.Serialize(obj);
      var portal = new BrokeredDataPortal.BrokeredPortal();
      var resultData = await portal.Update(objectData, contextData);
      var result = (DataPortalResult)Csla.Serialization.Mobile.MobileFormatter.Deserialize(resultData.ToArray());
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
      if (isSync)
        throw new NotSupportedException("isSync == true");
      var contextData = Csla.Serialization.Mobile.MobileFormatter.Serialize(context);
      var criteriaData = Csla.Serialization.Mobile.MobileFormatter.Serialize(criteria);
      var portal = new BrokeredDataPortal.BrokeredPortal();
      var resultData = await portal.Delete(objectType, criteriaData, contextData);
      var result = (DataPortalResult)Csla.Serialization.Mobile.MobileFormatter.Deserialize(resultData.ToArray());
      return result;
    }
  }
}
