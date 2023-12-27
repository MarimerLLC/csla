//-----------------------------------------------------------------------
// <copyright file="SessionServerManager.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Manages all user session data</summary>
//-----------------------------------------------------------------------
using System.Collections.Generic;
using System.Threading.Tasks;
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
    private readonly Dictionary<string, Session> _sessions = [];
    private readonly ISessionIdManager _sessionIdManager = sessionIdManager;

    /// <summary>
    /// Gets the session data for the
    /// current user.
    /// </summary>
    public async Task<Session> GetSession()
    {
      var key = await _sessionIdManager.GetSessionId();
      if (!_sessions.ContainsKey(key))
        _sessions.Add(key, []);
      var session = _sessions[key];
      // ensure session isn't checked out by wasm
      while (session.IsCheckedOut)
        await Task.Delay(5);

      return session;
    }

    /// <summary>
    /// Updates the current user's
    /// session data.
    /// </summary>
    /// <param name="session">Current user session data</param>
    public async Task UpdateSession(Session session)
    {
      if (session != null)
      {
        var key = await _sessionIdManager.GetSessionId();
        session.SessionId = key;
        Replace(session, _sessions[key]);
        _sessions[key].IsCheckedOut = false;
      }
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
  }
}
