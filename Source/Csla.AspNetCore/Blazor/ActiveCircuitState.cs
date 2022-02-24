//-----------------------------------------------------------------------
// <copyright file="ActiveCircuitState.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Service which interacts with ActiveCircuitHandler and indicates if code is in server-side Blazor</summary>
//-----------------------------------------------------------------------
#if NET5_0_OR_GREATER

namespace Csla.AspNetCore.Blazor
{
  /// <summary>
  /// Provides access to server-side Blazor
  /// circuit information required by CSLA .NET.
  /// </summary>
  public class ActiveCircuitState
  {
    /// <summary>
    /// Gets a value indicating whether the current scope
    /// has a circuit (is running in server-side Blazor).
    /// </summary>
    public bool CircuitExists { get; set; } = false;
  }
}
#endif
