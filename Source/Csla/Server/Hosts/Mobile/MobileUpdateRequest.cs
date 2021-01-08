//-----------------------------------------------------------------------
// <copyright file="MobileUpdateRequest.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Class that will be used to execute an Update request from a client.</summary>
//-----------------------------------------------------------------------
using System;
using System.Security.Principal;
using Csla.Core;

namespace Csla.Server.Hosts.Mobile
{
  /// <summary>
  /// Class that will be used to execute an Update request from a client.
  /// </summary>
  public class MobileUpdateRequest : IMobileRequest
  {
    /// <summary>
    /// Business object that will be updated.
    /// </summary>
    public object ObjectToUpdate { get; set; }
    /// <summary>
    /// Principal that will be set for the request
    /// </summary>
    public IPrincipal Principal { get; set; }
    /// <summary>
    /// Global context object.
    /// </summary>
    public ContextDictionary GlobalContext { get; set; }
    /// <summary>
    /// Client context object.
    /// </summary>
    public ContextDictionary ClientContext { get; set; }
    /// <summary>
    /// The client culture.
    /// </summary>
    public string ClientCulture { get; set; }
    /// <summary>
    /// The client UI culture.
    /// </summary>
    public string ClientUICulture { get; set; }

    /// <summary>
    /// New instance of criteria object
    /// </summary>
    /// <param name="objectToUpdate">Business object that will be updated.</param>
    /// <param name="principal">Principal that will be set for the request</param>
    /// <param name="globalContext">Global context object.</param>
    /// <param name="clientContext">Client context object.</param>
    /// <param name="clientCulture">The client culture.</param>
    /// <param name="clientUICulture">The client UI culture.</param>
    public MobileUpdateRequest(
      object objectToUpdate,
      IPrincipal principal,
      ContextDictionary globalContext,
      ContextDictionary clientContext,
      string clientCulture,
      string clientUICulture)
    {
      ObjectToUpdate = objectToUpdate;
      Principal = principal;
      GlobalContext = globalContext;
      ClientContext = clientContext;
      ClientCulture = clientCulture;
      ClientUICulture = clientUICulture;
    }
  }
}