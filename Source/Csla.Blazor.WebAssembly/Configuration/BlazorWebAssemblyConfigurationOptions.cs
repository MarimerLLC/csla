//-----------------------------------------------------------------------
// <copyright file="BlazorWebAssemblyConfigurationOptions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Options for Blazor wasm-interactive</summary>
//-----------------------------------------------------------------------

using Csla.Blazor.WebAssembly.State;

namespace Csla.Blazor.WebAssembly.Configuration
{
  /// <summary>
  /// Options for Blazor wasm-interactive.
  /// </summary>
  public class BlazorWebAssemblyConfigurationOptions
  {
    /// <summary>
    /// Gets or sets the type of the ISessionManager service.
    /// </summary>
    public Type SessionManagerType { get; set; } = typeof(SessionManager);

    /// <summary>
    /// Gets or sets the name of the controller providing state
    /// data from the Blazor web server.
    /// </summary>
    public string StateControllerName { get; set; } = "CslaState";
    /// <summary>
    /// Gets or sets a value indicating whether the LocalContext
    /// and ClientContext values on ApplicationContext should be
    /// synchronized with a Blazor web server host.
    /// </summary>
    /// <remarks>
    /// If this value is true, the Blazor web server host must
    /// provide a state controller to allow the wasm client to
    /// get and send the state from/to the server as necessary.
    /// </remarks>
    public bool SyncContextWithServer { get; set; } = false;
  }
}