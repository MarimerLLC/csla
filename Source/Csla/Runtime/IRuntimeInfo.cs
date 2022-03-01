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
    /// Gets a value indicating whether the current runtime
    /// is stateful (like WPF, Blazor, etc.).
    /// </summary>
    bool IsStatefulRuntime { get; set; }
    /// <summary>
    /// Gets a valud indicating whether any HttpContext instance
    /// can be considered valid (false for all server-side Blazor
    /// scenarios).
    /// </summary>
    bool IsHttpContextValid { get; set; }
  }
}
