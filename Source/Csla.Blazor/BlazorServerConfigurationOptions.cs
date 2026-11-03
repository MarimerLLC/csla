//-----------------------------------------------------------------------
// <copyright file="BlazorBlazorServerConfigurationOptions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implement extension methods for .NET Core configuration</summary>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

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
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
    public Type SessionManagerType { get; set; } = Type.GetType("Csla.Blazor.State.SessionManager, Csla.AspNetCore", true)!;

    /// <summary>
    /// Gets or sets the type of the ISessionIdManager service.
    /// </summary>
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
    public Type SessionIdManagerType { get; set; } = Type.GetType("Csla.Blazor.State.SessionIdManager, Csla.AspNetCore", true)!;

    /// <summary>
    /// Gets or sets the type of the ISessionStore service.
    /// </summary>
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
    public Type SessionStoreType { get; set; } = Type.GetType("Csla.Blazor.State.InMemorySessionStore, Csla.AspNetCore", true)!;

    /// <summary>
    /// Gets or sets a value indicating whether a hosted background
    /// service periodically purges expired sessions (default = true).
    /// </summary>
    /// <remarks>
    /// Only used when <see cref="UseInMemoryApplicationContextManager"/>
    /// is <see langword="false"/>. The service calls
    /// <see cref="Csla.State.ISessionManager.PurgeSessions"/> every
    /// <see cref="SessionPurgeInterval"/>, removing sessions that have not
    /// been touched within <see cref="SessionExpiration"/>. Set this to
    /// <see langword="false"/> if the app purges sessions itself.
    /// </remarks>
    public bool PurgeExpiredSessions { get; set; } = true;

    /// <summary>
    /// Gets or sets how often expired sessions are purged
    /// (default = 5 minutes).
    /// </summary>
    /// <remarks>
    /// Must be from 1 millisecond to 4,294,967,294 milliseconds (about
    /// 49.7 days). Keep it well below <see cref="SessionExpiration"/>: a
    /// shared session store, such as the Azure Table Storage store,
    /// refreshes the stored last-touched time of live sessions on each
    /// purge, so other servers don't remove them.
    /// </remarks>
    public TimeSpan SessionPurgeInterval { get; set; } = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Gets or sets how long a session can go untouched
    /// before it is purged (default = 1 hour).
    /// </summary>
    public TimeSpan SessionExpiration { get; set; } = TimeSpan.FromHours(1);
  }
}
