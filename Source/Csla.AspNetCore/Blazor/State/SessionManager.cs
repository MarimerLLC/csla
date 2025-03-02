//-----------------------------------------------------------------------
// <copyright file="SessionManager.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Manages all user session data</summary>
//-----------------------------------------------------------------------

using Csla.State;

namespace Csla.Blazor.State;

/// <summary>
/// Manages all user session data for a given
/// root DI container.
/// </summary>
/// <param name="sessionIdManager">Session ID manager</param>
/// <param name="sessionStore">Session storage provider</param>
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
    var getTask = _sessionStore.GetSessionAsync(key);
    var result = getTask.IsCompletedSuccessfully
      ? getTask.Result
      : getTask.AsTask().GetAwaiter().GetResult();
    if (result is null)
    {
      result = [];
      var storeTask = _sessionStore.AddOrUpdateSessionAsync(key, result);
      if (!storeTask.IsCompletedSuccessfully)
        storeTask.AsTask().GetAwaiter().GetResult();
    }
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
    newSession.Touch();
    var task = _sessionStore.AddOrUpdateSessionAsync(key, newSession);
    if (!task.IsCompletedSuccessfully)
      task.AsTask().GetAwaiter().GetResult();
  }

  /// <summary>
  /// Remove all expired session data.
  /// </summary>
  /// <param name="expiration">Expiration duration</param>
  public void PurgeSessions(TimeSpan expiration)
  {
    var task = _sessionStore.PurgeExpiredSessionsAsync(expiration);
    if (!task.IsCompletedSuccessfully)
      task.AsTask().GetAwaiter().GetResult();
  }

  // wasm client-side methods
  Task<Session> ISessionManager.RetrieveSession(TimeSpan timeout) => throw new NotImplementedException();
  Session ISessionManager.GetCachedSession() => throw new NotImplementedException();
  Task ISessionManager.SendSession(TimeSpan timeout) => throw new NotImplementedException();
  Task<Session> ISessionManager.RetrieveSession(CancellationToken ct) => throw new NotImplementedException();
  Task ISessionManager.SendSession(CancellationToken ct) => throw new NotImplementedException();
}
