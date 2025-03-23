//-----------------------------------------------------------------------
// <copyright file="SessionServerManager.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Manages all user session data</summary>
//-----------------------------------------------------------------------

using System.Collections.Concurrent;
using Csla.State;

namespace Csla.Blazor.State
{
  /// <summary>
  /// Manages all user session data for a given
  /// root DI container.
  /// </summary>
  /// <param name="sessionIdManager"></param>
  public class SessionManager(ISessionIdManager sessionIdManager) : ISessionManager
  {
    private readonly ConcurrentDictionary<string, Session> _sessions = [];
    private readonly ISessionIdManager _sessionIdManager = sessionIdManager;

    /// <summary>
    /// Gets the session data for the current user.
    /// </summary>
    public Session GetSession()
    {
      var key = _sessionIdManager.GetSessionId();
      var result = _sessions.GetOrAdd(key, []);
      result.Touch();
      return result;
    }

    /// <summary>
    /// Updates the current user's session data.
    /// </summary>
    /// <param name="newSession">Current user session data</param>
    /// <exception cref="ArgumentNullException"><paramref name="newSession"/> is <see langword="null"/>.</exception>
    public void UpdateSession(Session newSession)
    {
      ArgumentNullException.ThrowIfNull(newSession);
      var key = _sessionIdManager.GetSessionId();
      var existingSession = _sessions[key];
      Replace(newSession, existingSession);
      existingSession.Touch();
    }

    /// <summary>
    /// Replace the contents of oldSession with the items
    /// in newSession.
    /// </summary>
    /// <param name="newSession"></param>
    /// <param name="oldSession"></param>
    private static void Replace(Session newSession, Session oldSession)
    {
      oldSession.Clear();
      foreach (var (key, value) in newSession)
        oldSession.Add(key, value);
    }

    /// <summary>
    /// Remove all expired session data.
    /// </summary>
    /// <param name="expiration">Expiration duration</param>
    public void PurgeSessions(TimeSpan expiration)
    {
      var expirationTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - expiration.TotalSeconds;
      List<string> toRemove = [];
      foreach (var session in _sessions)
        if (session.Value.LastTouched < expirationTime)
          toRemove.Add(session.Key);
      foreach (var key in toRemove)
        _sessions.TryRemove(key, out _);
    }

    // wasm client-side methods
    Task<Session> ISessionManager.RetrieveSession(TimeSpan timeout) => throw new NotImplementedException();
    Session ISessionManager.GetCachedSession() => throw new NotImplementedException();
    Task ISessionManager.SendSession(TimeSpan timeout) => throw new NotImplementedException();
    Task<Session> ISessionManager.RetrieveSession(CancellationToken ct) => throw new NotImplementedException();
    Task ISessionManager.SendSession(CancellationToken ct) => throw new NotImplementedException();
  }
}
