//-----------------------------------------------------------------------
// <copyright file="StateManager.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Default implementation of session storage.</summary>
//-----------------------------------------------------------------------
#nullable enable

using System.Collections.Concurrent;
using Csla.State;

namespace Csla.Blazor.State
{
  /// <summary>
  /// Default implementation of <see cref="ISessionStore"/>
  /// </summary>
  public class DefaultSessionStore : ISessionStore
  {
    private readonly ConcurrentDictionary<string, Session> _store = new();

    /// <inheritdoc />
    public Session? GetSession(string key)
    {
      _store.TryGetValue(key, out var item);
      return item;
    }

    /// <inheritdoc />
    public void CreateSession(string key, Session session)
    {
      if (!_store.TryAdd(key, session))
      {
        throw new Exception("Key already exists");
      }
    }

    /// <inheritdoc />
    public void UpdateSession(string key, Session session)
    {
      ArgumentNullException.ThrowIfNull(session);
      _store[key] = session;
    }

    /// <inheritdoc />
    public void DeleteSession(string key)
    {
      _store.TryRemove(key, out _);
    }

    /// <inheritdoc />
    public void DeleteSessions(SessionsFilter filter)
    {
      filter.Validate();

      var query = _store.AsQueryable();
      if (filter.Expiration.HasValue)
      {
        var expirationTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - filter.Expiration.Value.TotalSeconds;
        query = query.Where(x => x.Value.LastTouched < expirationTime);
      }

      var keys = query.Select(x => x.Key).ToArray();

      foreach (var key in keys)
      {
        _store.TryRemove(key, out _);
      }
    }
  }
}
