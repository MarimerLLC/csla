//-----------------------------------------------------------------------
// <copyright file="IAuthorizeDataPortal.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Interface to be implemented by a custom</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    void Authorize(AuthorizeRequest clientRequest);
  }
}