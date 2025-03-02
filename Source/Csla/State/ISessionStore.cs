//-----------------------------------------------------------------------
// <copyright file="ISessionStore.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Manages session storage</summary>
//-----------------------------------------------------------------------
#nullable enable

namespace Csla.State
{
  /// <summary>
  /// Session store
  /// </summary>
  public interface ISessionStore
  {
    /// <summary>
    /// Retrieves a session
    /// </summary>
    /// <param name="key"></param>
    /// <returns>The session for the given key, or default if not found</returns>
    Session? GetSession(string key);

    /// <summary>
    /// Creates a session
    /// </summary>
    /// <param name="key"></param>
    /// <param name="session"></param>
    void CreateSession(string key, Session session);

    /// <summary>
    /// Updates a session
    /// </summary>
    /// <param name="key"></param>
    /// <param name="session"></param>
    void UpdateSession(string key, Session session);

    /// <summary>
    /// Deletes a session
    /// </summary>
    /// <param name="key"></param>
    void DeleteSession(string key);

    /// <summary>
    /// Deletes sessions based on the filter.
    /// </summary>
    /// <param name="filter"></param>
    void DeleteSessions(SessionsFilter filter);
  }
}
