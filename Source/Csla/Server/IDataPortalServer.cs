//-----------------------------------------------------------------------
// <copyright file="IDataPortalServer.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Interface implemented by server-side data portal</summary>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

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
    /// <exception cref="ArgumentNullException"><paramref name="objectType"/>, <paramref name="criteria"/> or <paramref name="context"/> is <see langword="null"/>.</exception>
    Task<DataPortalResult> Create([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type objectType, object criteria, DataPortalContext context, bool isSync);
    /// <summary>
    /// Get an existing business object.
    /// </summary>
    /// <param name="objectType">Type of business object to retrieve.</param>
    /// <param name="criteria">Criteria object describing business object.</param>
    /// <param name="context">
    /// <see cref="Server.DataPortalContext" /> object passed to the server.
    /// </param>
    /// <param name="isSync">True if the client-side proxy should synchronously invoke the server.</param>
    /// <exception cref="ArgumentNullException"><paramref name="objectType"/>, <paramref name="criteria"/> or <paramref name="context"/> is <see langword="null"/>.</exception>
    Task<DataPortalResult> Fetch([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type objectType, object criteria, DataPortalContext context, bool isSync);
    /// <summary>
    /// Update a business object.
    /// </summary>
    /// <param name="obj">Business object to update.</param>
    /// <param name="context">
    /// <see cref="Server.DataPortalContext" /> object passed to the server.
    /// </param>
    /// <param name="isSync">True if the client-side proxy should synchronously invoke the server.</param>
    /// <exception cref="ArgumentNullException"><paramref name="obj"/> or <paramref name="context"/> is <see langword="null"/>.</exception>
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
    /// <exception cref="ArgumentNullException"><paramref name="objectType"/>, <paramref name="criteria"/> or <paramref name="context"/> is <see langword="null"/>.</exception>
    Task<DataPortalResult> Delete([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type objectType, object criteria, DataPortalContext context, bool isSync);
  }
}