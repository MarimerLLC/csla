//-----------------------------------------------------------------------
// <copyright file="InMemorySessionStore.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Default in-memory session store</summary>
//-----------------------------------------------------------------------

using System.Collections.Concurrent;
using Csla.State;

namespace Csla.Blazor.State;

/// <summary>
/// Default in-memory implementation of <see cref="ISessionStore"/>
/// using a <see cref="ConcurrentDictionary{TKey, TValue}"/>.
/// </summary>
public class InMemorySessionStore : ISessionStore
{
  private readonly ConcurrentDictionary<string, Session> _sessions = [];

  /// <inheritdoc />
  public ValueTask<Session?> GetSessionAsync(string key)
  {
    _sessions.TryGetValue(key, out var session);
    return new ValueTask<Session?>(session);
  }

  /// <inheritdoc />
  public ValueTask AddOrUpdateSessionAsync(string key, Session session)
  {
    _sessions.AddOrUpdate(key, session, (_, _) => session);
    return ValueTask.CompletedTask;
  }

  /// <inheritdoc />
  public ValueTask PurgeExpiredSessionsAsync(TimeSpan expiration)
  {
    var expirationTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - expiration.TotalSeconds;
    List<string> toRemove = [];
    foreach (var kvp in _sessions)
      if (kvp.Value.LastTouched < expirationTime)
        toRemove.Add(kvp.Key);
    foreach (var key in toRemove)
      _sessions.TryRemove(key, out _);
    return ValueTask.CompletedTask;
  }
}
