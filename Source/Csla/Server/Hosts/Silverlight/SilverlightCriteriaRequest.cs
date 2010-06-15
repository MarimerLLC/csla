//-----------------------------------------------------------------------
// <copyright file="SilverlightCriteriaRequest.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Class that will be used to execute a request from a Silverlight client.</summary>
//-----------------------------------------------------------------------
using System;
using Csla.Core;
using System.Security.Principal;

namespace Csla.Server.Hosts.Silverlight
{
  /// <summary>
  /// Class that will be used to execute a request from a Silverlight client.
  /// This will inlcude Execute, Fetch, Create and Delete requests
  /// </summary>
  public class SilverlightCriteriaRequest : ISilverlightRequest
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
    /// New instance of criteria object
    /// </summary>
    /// <param name="typeName">Type of object that is the target of the request</param>
    /// <param name="criteria">Criteria object.</param>
    /// <param name="principal">Principal that will be set for the request</param>
    /// <param name="globalContext">Global context object.</param>
    /// <param name="clientContext">Client context object.</param>
    public SilverlightCriteriaRequest(
      string typeName,
      object criteria,
      IPrincipal principal,
      ContextDictionary globalContext,
      ContextDictionary clientContext)
    {
      TypeName = typeName;
      Criteria = criteria;
      Principal = principal;
      GlobalContext = globalContext;
      ClientContext = clientContext;
    }
  }
}