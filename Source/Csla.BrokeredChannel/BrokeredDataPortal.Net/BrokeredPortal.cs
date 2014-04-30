//-----------------------------------------------------------------------
// <copyright file="BrokeredDataPortal.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Implements a data portal host</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Csla.Server;
using System.Runtime.InteropServices.WindowsRuntime;

namespace BrokeredDataPortal
{
  /// <summary>
  /// Data portal host implemented as a brokered assembly
  /// for use by side-loaded WinRT client apps.
  /// </summary>
  public sealed class BrokeredPortal
  {
    /// <summary>
    /// Create and initialize a business object.
    /// </summary>
    /// <param name="objectType">Type of business object to create.</param>
    /// <param name="criteriaData">Criteria object describing business object.</param>
    /// <param name="contextData">
    /// Server.DataPortalContext object passed to the server.
    /// </param>
    public byte[] Create(Type objectType, [ReadOnlyArray] byte[] criteriaData, [ReadOnlyArray] byte[] contextData)
    {
      var criteria = Csla.Serialization.Mobile.MobileFormatter.Deserialize(criteriaData);
      var context = (DataPortalContext)Csla.Serialization.Mobile.MobileFormatter.Deserialize(contextData);
      var portal = new DataPortal();
      var result = portal.Create(objectType, criteria, context, true).Result;
      return Csla.Serialization.Mobile.MobileFormatter.Serialize(result);
    }

    /// <summary>
    /// Get an existing business object.
    /// </summary>
    /// <param name="objectType">Type of business object to retrieve.</param>
    /// <param name="criteriaData">Criteria object describing business object.</param>
    /// <param name="contextData">
    /// Server.DataPortalContext object passed to the server.
    /// </param>
    public byte[] Fetch(Type objectType, [ReadOnlyArray] byte[] criteriaData, [ReadOnlyArray] byte[] contextData)
    {
      var criteria = Csla.Serialization.Mobile.MobileFormatter.Deserialize(criteriaData);
      var context = (DataPortalContext)Csla.Serialization.Mobile.MobileFormatter.Deserialize(contextData);
      var portal = new DataPortal();
      var result = portal.Fetch(objectType, criteria, context, true).Result;
      return Csla.Serialization.Mobile.MobileFormatter.Serialize(result);
    }

    /// <summary>
    /// Update a business object.
    /// </summary>
    /// <param name="objectData">Business object to update.</param>
    /// <param name="contextData">
    /// Server.DataPortalContext object passed to the server.
    /// </param>
    public byte[] Update([ReadOnlyArray] byte[] objectData, [ReadOnlyArray] byte[] contextData)
    {
      var obj = Csla.Serialization.Mobile.MobileFormatter.Deserialize(objectData);
      var context = (DataPortalContext)Csla.Serialization.Mobile.MobileFormatter.Deserialize(contextData);
      var portal = new DataPortal();
      var result = portal.Update(obj, context, true).Result;
      return Csla.Serialization.Mobile.MobileFormatter.Serialize(result);
    }

    /// <summary>
    /// Delete a business object.
    /// </summary>
    /// <param name="objectType">Type of business object to create.</param>
    /// <param name="criteriaData">Criteria object describing business object.</param>
    /// <param name="contextData">
    /// Server.DataPortalContext object passed to the server.
    /// </param>
    public byte[] Delete(Type objectType, [ReadOnlyArray] byte[] criteriaData, [ReadOnlyArray] byte[] contextData)
    {
      var criteria = Csla.Serialization.Mobile.MobileFormatter.Deserialize(criteriaData);
      var context = (DataPortalContext)Csla.Serialization.Mobile.MobileFormatter.Deserialize(contextData);
      var portal = new DataPortal();
      var result = portal.Delete(objectType, criteria, context, true).Result;
      return Csla.Serialization.Mobile.MobileFormatter.Serialize(result);
    }
  }
}
