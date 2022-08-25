//-----------------------------------------------------------------------
// <copyright file="RuntimeInfo.cs" company="Marimer LLC">
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
  public class RuntimeInfo : IRuntimeInfo
  {
    /// <summary>
    /// Gets or sets a value indicating whether
    /// the current runtime is in a DI scope
    /// created by LocalProxy.
    /// </summary>
    public bool LocalProxyNewScopeExists { get; set; }
  }
}
