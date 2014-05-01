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
using Windows.Foundation;

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
    public IAsyncOperation<IList<byte>> Create(Type objectType, [ReadOnlyArray] byte[] criteriaData, [ReadOnlyArray] byte[] contextData)
    {
      return Task<IList<byte>>.Run(async () =>
        {
          var criteria = Csla.Serialization.Mobile.MobileFormatter.Deserialize(criteriaData);
          var context = (DataPortalContext)Csla.Serialization.Mobile.MobileFormatter.Deserialize(contextData);
          var portal = new DataPortal();
          var result = await portal.Create(objectType, criteria, context, false);
          return (IList<byte>)Csla.Serialization.Mobile.MobileFormatter.Serialize(result).ToList();
        }).AsAsyncOperation();
    }

    /// <summary>
    /// Get an existing business object.
    /// </summary>
    /// <param name="objectType">Type of business object to retrieve.</param>
    /// <param name="criteriaData">Criteria object describing business object.</param>
    /// <param name="contextData">
    /// Server.DataPortalContext object passed to the server.
    /// </param>
    public IAsyncOperation<IList<byte>> Fetch(Type objectType, [ReadOnlyArray] byte[] criteriaData, [ReadOnlyArray] byte[] contextData)
    {
      return Task<IList<byte>>.Run(async () =>
        {
          var criteria = Csla.Serialization.Mobile.MobileFormatter.Deserialize(criteriaData);
          var context = (DataPortalContext)Csla.Serialization.Mobile.MobileFormatter.Deserialize(contextData);
          var portal = new DataPortal();
          var result = await portal.Fetch(objectType, criteria, context, false);
          return (IList<byte>)Csla.Serialization.Mobile.MobileFormatter.Serialize(result).ToList();
        }).AsAsyncOperation();
    }

    /// <summary>
    /// Update a business object.
    /// </summary>
    /// <param name="objectData">Business object to update.</param>
    /// <param name="contextData">
    /// Server.DataPortalContext object passed to the server.
    /// </param>
    public IAsyncOperation<IList<byte>> Update([ReadOnlyArray] byte[] objectData, [ReadOnlyArray] byte[] contextData)
    {
      return Task<IList<byte>>.Run(async () =>
        {
          var obj = Csla.Serialization.Mobile.MobileFormatter.Deserialize(objectData);
          var context = (DataPortalContext)Csla.Serialization.Mobile.MobileFormatter.Deserialize(contextData);
          var portal = new DataPortal();
          var result = await portal.Update(obj, context, false);
          return (IList<byte>)Csla.Serialization.Mobile.MobileFormatter.Serialize(result).ToList();
        }).AsAsyncOperation();
    }

    /// <summary>
    /// Delete a business object.
    /// </summary>
    /// <param name="objectType">Type of business object to create.</param>
    /// <param name="criteriaData">Criteria object describing business object.</param>
    /// <param name="contextData">
    /// Server.DataPortalContext object passed to the server.
    /// </param>
    public IAsyncOperation<IList<byte>> Delete(Type objectType, [ReadOnlyArray] byte[] criteriaData, [ReadOnlyArray] byte[] contextData)
    {
      return Task<IList<byte>>.Run(async () =>
        {
          var criteria = Csla.Serialization.Mobile.MobileFormatter.Deserialize(criteriaData);
          var context = (DataPortalContext)Csla.Serialization.Mobile.MobileFormatter.Deserialize(contextData);
          var portal = new DataPortal();
          var result = await portal.Delete(objectType, criteria, context, false);
          return (IList<byte>)Csla.Serialization.Mobile.MobileFormatter.Serialize(result).ToList();
        }).AsAsyncOperation();
    }
  }
}
