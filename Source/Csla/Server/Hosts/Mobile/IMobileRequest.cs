//-----------------------------------------------------------------------
// <copyright file="IMobileRequest.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Interface for all requests from client</summary>
//-----------------------------------------------------------------------
using System;
using Csla.Core;
using System.Security.Principal;

namespace Csla.Server.Hosts.Mobile
{
  /// <summary>
  /// Interface for all requests from client
  /// </summary>
  public interface IMobileRequest
  {
    /// <summary>
    /// Principal that will be set for the request
    /// </summary>
    IPrincipal Principal { get; set; }
    /// <summary>
    /// Global context object.
    /// </summary>
    ContextDictionary GlobalContext { get; set; }
    /// <summary>
    /// Client context object.
    /// </summary>
    ContextDictionary ClientContext { get; set; }
    /// <summary>
    /// The client culture.
    /// </summary>
    string ClientCulture { get; set; }
    /// <summary>
    /// The client UI culture.
    /// </summary>
    string ClientUICulture { get; set; }
  }
}