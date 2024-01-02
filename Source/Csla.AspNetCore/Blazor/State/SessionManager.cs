//-----------------------------------------------------------------------
// <copyright file="SessionServerManager.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Manages all user session data</summary>
//-----------------------------------------------------------------------
using System;
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
    /// Gets the session data for the current user.
    /// </summary>
    public Session GetSession()
    {
      Session result;
      var key = _sessionIdManager.GetSessionId();
      if (!_sessions.ContainsKey(key))
        _sessions.Add(key, new Session());
      result = _sessions[key];
      result.LastTouched = DateTimeOffset.UtcNow;
      return result;
    }

    /// <summary>
    /// Updates the current user's session data.
    /// </summary>
    /// <param name="newSession">Current user session data</param>
    public void UpdateSession(Session newSession)
    {
      if (newSession != null)
      {
        var key = _sessionIdManager.GetSessionId();
        var existingSession = _sessions[key];
        Replace(newSession, existingSession);
        existingSession.LastTouched = DateTimeOffset.UtcNow;
        existingSession.IsCheckedOut = false;
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

    /// <summary>
    /// Retrieves the current user's session from
    /// the web server to the wasm client.
    /// </summary>
    public Task<Session> RetrieveSession() => throw new NotSupportedException();

    /// <summary>
    /// Sends the current user's session from
    /// the wasm client to the web server.
    /// </summary>
    /// <returns></returns>
    public Task SendSession() => throw new NotSupportedException();
  }
}
