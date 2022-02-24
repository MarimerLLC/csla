//-----------------------------------------------------------------------
// <copyright file="LocalProxyState.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Service which interacts with LocalProxy and indicates if a new scope was created</summary>
//-----------------------------------------------------------------------

namespace Csla.Channels.Local
{
  /// <summary>
  /// Provides information about the state of the LocalProxy if used
  /// </summary>
  public class LocalProxyState
  {
    /// <summary>
    /// True if LocalProxy is in use and has created a new scope.  
    /// Only happens in stateful technologies where <see cref="ApplicationContext.IsStatefulRuntime"/> equals true)
    /// </summary>
    public bool NewScopeExists { get; set; }
  }
}
