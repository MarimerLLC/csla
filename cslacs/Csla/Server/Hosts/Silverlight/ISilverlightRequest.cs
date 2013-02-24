using System;
using Csla.Core;
using System.Security.Principal;

namespace Csla.Server.Hosts.Silverlight
{
  /// <summary>
  /// Interface for all requests from Silverlight client
  /// </summary>
  public interface ISilverlightRequest
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
  }
}
