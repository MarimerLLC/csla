//-----------------------------------------------------------------------
// <copyright file="IDataPortalProxy.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Interface implemented by client-side </summary>
//-----------------------------------------------------------------------
using System;

namespace Csla.DataPortalClient
{
  /// <summary>
  /// Interface implemented by client-side 
  /// data portal proxy objects.
  /// </summary>
  public interface IDataPortalProxy : Server.IDataPortalServer
  {
    /// <summary>
    /// Get a value indicating whether this proxy will invoke
    /// a remote data portal server, or run the "server-side"
    /// data portal in the caller's process and AppDomain.
    /// </summary>
    bool IsServerRemote { get; }
  }
}