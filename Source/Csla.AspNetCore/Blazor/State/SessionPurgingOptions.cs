//-----------------------------------------------------------------------
// <copyright file="SessionPurgingOptions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Options for the session purging background service</summary>
//-----------------------------------------------------------------------

namespace Csla.Configuration;

/// <summary>
/// Options for the background service that purges expired
/// Blazor sessions, registered by
/// <see cref="SessionPurgingConfigurationExtensions.AddSessionPurging"/>.
/// </summary>
public class SessionPurgingOptions
{
  /// <summary>
  /// Gets or sets how often expired sessions are purged
  /// (default = 5 minutes).
  /// </summary>
  /// <remarks>
  /// Keep the interval well below <see cref="Expiration"/>. A shared
  /// session store, such as the Azure Table Storage store, refreshes
  /// the stored last-touched time of live sessions on each purge, so
  /// other servers don't remove them.
  /// </remarks>
  public TimeSpan Interval { get; set; } = TimeSpan.FromMinutes(5);

  /// <summary>
  /// Gets or sets how long a session can go untouched
  /// before it is purged (default = 1 hour).
  /// </summary>
  public TimeSpan Expiration { get; set; } = TimeSpan.FromHours(1);
}
