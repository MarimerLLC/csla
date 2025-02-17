//-----------------------------------------------------------------------
// <copyright file="ActiveCircuitHandler.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Circuit handler indicating if code in server-side Blazor</summary>
//-----------------------------------------------------------------------
#if NET8_0_OR_GREATER
using Microsoft.AspNetCore.Components.Server.Circuits;

namespace Csla.AspNetCore.Blazor
{
  /// <summary>
  /// Circuit handler indicating if code in server-side Blazor.
  /// </summary>
  public class ActiveCircuitHandler : CircuitHandler
  {
    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="activeCircuitState"></param>
    public ActiveCircuitHandler(ActiveCircuitState activeCircuitState)
    {
      _activeCircuitState = activeCircuitState;
    }

    private ActiveCircuitState _activeCircuitState;

    /// <summary>
    /// Handler for the OnCircuitOpenedAsync call
    /// </summary>
    /// <param name="circuit">The circuit in which we are running</param>
    /// <param name="cancellationToken">The cancellation token provided by the runtime</param>
    public override Task OnCircuitOpenedAsync(Circuit circuit, CancellationToken cancellationToken)
    {
      _activeCircuitState.CircuitExists = true;
      return base.OnCircuitOpenedAsync(circuit, cancellationToken);
    }
  }
}
#endif