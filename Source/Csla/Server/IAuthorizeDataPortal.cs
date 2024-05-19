﻿//-----------------------------------------------------------------------
// <copyright file="IAuthorizeDataPortal.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Interface to be implemented by a custom</summary>
//-----------------------------------------------------------------------

namespace Csla.Server
{
  /// <summary>
  /// Interface to be implemented by a custom
  /// authorization provider.
  /// </summary>
  public interface IAuthorizeDataPortal
  {
    /// <summary>
    /// Implement this method to perform custom
    /// authorization on every data portal call.
    /// </summary>
    /// <param name="clientRequest">
    /// Object containing information about the client request.
    /// </param>
    /// <param name="ct">
    /// The cancellation token.
    /// </param>
    Task AuthorizeAsync(AuthorizeRequest clientRequest, CancellationToken ct);
  }
}