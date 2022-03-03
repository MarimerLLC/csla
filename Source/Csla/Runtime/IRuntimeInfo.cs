//-----------------------------------------------------------------------
// <copyright file="IRuntimeInfo.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Information about the current runtime environment.</summary>
//-----------------------------------------------------------------------
namespace Csla.Runtime
{
  /// <summary>
  /// Information about the current runtime environment.
  /// </summary>
  public interface IRuntimeInfo
  {
    /// <summary>
    /// Gets a value indicating whether a LocalProxy (if configured and used)
    /// has created a new scope in preparation for logical server side
    /// operations.
    /// </summary>
    bool LocalProxyNewScopeExists { get; set; }
  }
}
