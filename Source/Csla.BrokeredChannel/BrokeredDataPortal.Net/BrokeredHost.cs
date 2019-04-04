//-----------------------------------------------------------------------
// <copyright file="BrokeredDataPortal.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implements a data portal host</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;

namespace Csla.BrokeredDataPortalHost
{
  /// <summary>
  /// Brokered assembly entry point for the
  /// data portal. For use by side-loaded WinRT client apps.
  /// </summary>
  public sealed class BrokeredHost
  {
    /// <summary>
    /// Create and initialize a business object.
    /// </summary>
    /// <param name="objectTypeName">Type of business object to create.</param>
    /// <param name="criteriaData">Criteria object describing business object.</param>
    /// <param name="contextData">
    /// Server.DataPortalContext object passed to the server.
    /// </param>
    public IAsyncOperation<IList<byte>> Create(string objectTypeName, [ReadOnlyArray] byte[] criteriaData, [ReadOnlyArray] byte[] contextData)
    {
      return Task<IList<byte>>.Run(async () =>
      {
        var portal = GetPortal();
        var method = portal.GetType().GetMethod("Create");
        var result = await (Task<byte[]>)method.Invoke(portal, new object[] { objectTypeName, criteriaData, contextData });
        return (IList<byte>)result;
      }).AsAsyncOperation();
    }

    /// <summary>
    /// Get an existing business object.
    /// </summary>
    /// <param name="objectTypeName">Type of business object to retrieve.</param>
    /// <param name="criteriaData">Criteria object describing business object.</param>
    /// <param name="contextData">
    /// Server.DataPortalContext object passed to the server.
    /// </param>
    public IAsyncOperation<IList<byte>> Fetch(string objectTypeName, [ReadOnlyArray] byte[] criteriaData, [ReadOnlyArray] byte[] contextData)
    {
      return Task<IList<byte>>.Run(async () =>
      {
        var portal = GetPortal();
        var method = portal.GetType().GetMethod("Fetch");
        var result = await (Task<byte[]>)method.Invoke(portal, new object[] { objectTypeName, criteriaData, contextData });
        return (IList<byte>)result;
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
        var portal = GetPortal();
        var method = portal.GetType().GetMethod("Update");
        var result = await (Task<byte[]>)method.Invoke(portal, new object[] { objectData, contextData });
        return (IList<byte>)result;
      }).AsAsyncOperation();
    }

    /// <summary>
    /// Delete a business object.
    /// </summary>
    /// <param name="objectTypeName">Type of business object to create.</param>
    /// <param name="criteriaData">Criteria object describing business object.</param>
    /// <param name="contextData">
    /// Server.DataPortalContext object passed to the server.
    /// </param>
    public IAsyncOperation<IList<byte>> Delete(string objectTypeName, [ReadOnlyArray] byte[] criteriaData, [ReadOnlyArray] byte[] contextData)
    {
      return Task<IList<byte>>.Run(async () =>
      {
        var portal = GetPortal();
        var method = portal.GetType().GetMethod("Delete");
        var result = await (Task<byte[]>)method.Invoke(portal, new object[] { objectTypeName, criteriaData, contextData });
        return (IList<byte>)result;
      }).AsAsyncOperation();
    }

    private object GetPortal()
    {
      var typeName = "Csla.Server.Hosts.BrokeredPortal, Csla.BrokeredDataPortal";
      Type type = null;
      try
      {
        type = Type.GetType(typeName);
        if (type == null)
          throw new TypeLoadException("BrokeredHost: " + typeName);
      }
      catch (Exception ex)
      {
        throw new TypeLoadException("BrokeredHost: " + typeName, ex);
      }
      return Activator.CreateInstance(type);
    }
  }
}
