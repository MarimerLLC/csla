//-----------------------------------------------------------------------
// <copyright file="BlazorBlazorServerConfigurationOptions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implement extension methods for .NET Core configuration</summary>
//-----------------------------------------------------------------------

namespace Csla.Configuration
{
  /// <summary>
  /// Options for Blazor server-rendered and server-interactive.
  /// </summary>
  public class BlazorServerConfigurationOptions
  {
    /// <summary>
    /// Gets or sets a value indicating whether the app 
    /// should be configured to use CSLA permissions 
    /// policies (default = true).
    /// </summary>
    public bool UseCslaPermissionsPolicy { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to use a
    /// scoped DI container to manage the ApplicationContext; 
    /// false to use the Blazor 8 state management subsystem.
    /// </summary>
    public bool UseInMemoryApplicationContextManager { get; set; } = true;

    /// <summary>
    /// Gets or sets the type of the ISessionManager service.
    /// </summary>
    public Type SessionManagerType { get; set; } = Type.GetType("Csla.Blazor.State.SessionManager, Csla.AspNetCore", true)!;

    /// <summary>
    /// Gets or sets the type of the ISessionIdManager service.
    /// </summary>
    public Type SessionIdManagerType { get; set; } = Type.GetType("Csla.Blazor.State.SessionIdManager, Csla.AspNetCore", true)!;

    /// <summary>
    /// Gets or sets the type of the ISessionStore service.
    /// </summary>
    public Type SessionStoreType { get; set; } = Type.GetType("Csla.Blazor.State.InMemorySessionStore, Csla.AspNetCore", true)!;
  }
}
