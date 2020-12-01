//-----------------------------------------------------------------------
// <copyright file="IDataPortalServer.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Interface implemented by server-side data portal</summary>
//-----------------------------------------------------------------------
using System;
using System.Threading.Tasks;

namespace Csla.Server
{
  /// <summary>
  /// Interface implemented by server-side data portal
  /// components.
  /// </summary>
  public interface IDataPortalServer
  {
    /// <summary>
    /// Create a new business object.
    /// </summary>
    /// <param name="objectType">Type of business object to create.</param>
    /// <param name="criteria">Criteria object describing business object.</param>
    /// <param name="context">
    /// <see cref="Server.DataPortalContext" /> object passed to the server.
    /// </param>
    /// <param name="isSync">True if the client-side proxy should synchronously invoke the server.</param>
    Task<DataPortalResult> Create(Type objectType, object criteria, DataPortalContext context, bool isSync);
    /// <summary>
    /// Get an existing business object.
    /// </summary>
    /// <param name="objectType">Type of business object to retrieve.</param>
    /// <param name="criteria">Criteria object describing business object.</param>
    /// <param name="context">
    /// <see cref="Server.DataPortalContext" /> object passed to the server.
    /// </param>
    /// <param name="isSync">True if the client-side proxy should synchronously invoke the server.</param>
    Task<DataPortalResult> Fetch(Type objectType, object criteria, DataPortalContext context, bool isSync);
    /// <summary>
    /// Update a business object.
    /// </summary>
    /// <param name="obj">Business object to update.</param>
    /// <param name="context">
    /// <see cref="Server.DataPortalContext" /> object passed to the server.
    /// </param>
    /// <param name="isSync">True if the client-side proxy should synchronously invoke the server.</param>
    Task<DataPortalResult> Update(object obj, DataPortalContext context, bool isSync);
    /// <summary>
    /// Delete a business object.
    /// </summary>
    /// <param name="objectType">Type of business object to create.</param>
    /// <param name="criteria">Criteria object describing business object.</param>
    /// <param name="context">
    /// <see cref="Server.DataPortalContext" /> object passed to the server.
    /// </param>
    /// <param name="isSync">True if the client-side proxy should synchronously invoke the server.</param>
    Task<DataPortalResult> Delete(Type objectType, object criteria, DataPortalContext context, bool isSync);
  }
}