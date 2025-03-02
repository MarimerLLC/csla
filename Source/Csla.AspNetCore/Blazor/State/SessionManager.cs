//-----------------------------------------------------------------------
// <copyright file="SessionServerManager.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Manages all user session data</summary>
//-----------------------------------------------------------------------

using Csla.State;

namespace Csla.Blazor.State
{
  /// <summary>
  /// Manages all user session data for a given
  /// root DI container.
  /// </summary>
  /// <param name="sessionIdManager"></param>
  /// <param name="sessionStore"></param>
  public class SessionManager(ISessionIdManager sessionIdManager, ISessionStore sessionStore) : ISessionManager
  {
    private readonly ISessionIdManager _sessionIdManager = sessionIdManager;
    private readonly ISessionStore _sessionStore = sessionStore;

    /// <summary>
    /// Gets the session data for the current user.
    /// </summary>
    public Session GetSession()
    {
      var key = _sessionIdManager.GetSessionId();
      var session = _sessionStore.GetSession(key);
      if (session == null)
      {
        session = [];
        session.Touch();
        _sessionStore.CreateSession(key, session);
        return session;
      }

      session.Touch();
      _sessionStore.UpdateSession(key, session);
      return session;
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
      var existingSession = _sessionStore.GetSession(key)!;
      Replace(newSession, existingSession);
      existingSession.Touch();
      _sessionStore.UpdateSession(key, existingSession);
    }

    /// <summary>
    /// Remove all expired session data.
    /// </summary>
    /// <param name="expiration">Expiration duration</param>
    public void PurgeSessions(TimeSpan expiration)
    {
      _sessionStore.DeleteSessions(new SessionsFilter { Expiration = expiration });
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
      foreach (var key in newSession.Keys)
        oldSession.Add(key, newSession[key]);
    }

    // wasm client-side methods
    Task<Session> ISessionManager.RetrieveSession(TimeSpan timeout) => throw new NotImplementedException();
    Session ISessionManager.GetCachedSession() => throw new NotImplementedException();
    Task ISessionManager.SendSession(TimeSpan timeout) => throw new NotImplementedException();
    Task<Session> ISessionManager.RetrieveSession(CancellationToken ct) => throw new NotImplementedException();
    Task ISessionManager.SendSession(CancellationToken ct) => throw new NotImplementedException();
  }
}
