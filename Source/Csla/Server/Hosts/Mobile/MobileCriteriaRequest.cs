//-----------------------------------------------------------------------
// <copyright file="MobileCriteriaRequest.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Class that will be used to execute a request from a client.</summary>
//-----------------------------------------------------------------------
using System;
using Csla.Core;
using System.Security.Principal;

namespace Csla.Server.Hosts.Mobile
{
  /// <summary>
  /// Class that will be used to execute a request from a client.
  /// This will inlcude Execute, Fetch, Create and Delete requests
  /// </summary>
  public class MobileCriteriaRequest : IMobileRequest
  {
    /// <summary>
    /// Type of object that is the target of the request
    /// </summary>
    public string TypeName { get; set; }
    /// <summary>
    /// Criteria object.
    /// </summary>
    public object Criteria { get; set; }
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
    /// <param name="typeName">Type of object that is the target of the request</param>
    /// <param name="criteria">Criteria object.</param>
    /// <param name="principal">Principal that will be set for the request</param>
    /// <param name="globalContext">Global context object.</param>
    /// <param name="clientContext">Client context object.</param>
    /// <param name="clientCulture">The client culture.</param>
    /// <param name="clientUICulture">The client UI culture.</param>
    public MobileCriteriaRequest(
      string typeName,
      object criteria,
      IPrincipal principal,
      ContextDictionary globalContext,
      ContextDictionary clientContext,
      string clientCulture,
      string clientUICulture)
    {
      TypeName = typeName;
      Criteria = criteria;
      Principal = principal;
      GlobalContext = globalContext;
      ClientContext = clientContext;
      ClientCulture = clientCulture;
      ClientUICulture = clientUICulture;
    }
  }
}