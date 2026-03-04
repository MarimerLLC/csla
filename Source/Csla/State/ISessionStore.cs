//-----------------------------------------------------------------------
// <copyright file="ISessionStore.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Defines the interface for a session storage provider</summary>
//-----------------------------------------------------------------------

namespace Csla.State;

/// <summary>
/// Defines the interface for a session storage provider.
/// </summary>
public interface ISessionStore
{
  /// <summary>
  /// Gets the session for the specified key,
  /// or <see langword="null"/> if not found.
  /// </summary>
  /// <param name="key">The session key.</param>
  ValueTask<Session?> GetSessionAsync(string key);

  /// <summary>
  /// Adds or updates the session for the specified key.
  /// </summary>
  /// <param name="key">The session key.</param>
  /// <param name="session">The session data.</param>
  ValueTask AddOrUpdateSessionAsync(string key, Session session);

  /// <summary>
  /// Removes all sessions that have not been touched
  /// within the specified expiration duration.
  /// </summary>
  /// <param name="expiration">The expiration duration.</param>
  ValueTask PurgeExpiredSessionsAsync(TimeSpan expiration);
}
