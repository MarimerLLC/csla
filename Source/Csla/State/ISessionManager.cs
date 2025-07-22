//-----------------------------------------------------------------------
// <copyright file="ISessionManager.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Manages session state</summary>
//-----------------------------------------------------------------------

namespace Csla.State
{
  /// <summary>
  /// Manages session state.
  /// </summary>
  public interface ISessionManager
  {
    #region Client
    /// <summary>
    /// Retrieves the current user's session from
    /// the web server to the wasm client.
    /// </summary>
    Task<Session> RetrieveSession(TimeSpan timeout);
    /// <summary>
    /// Retrieves the current user's session from
    /// the web server to the wasm client.
    /// </summary>
    Task<Session> RetrieveSession(CancellationToken ct);
    /// <summary>
    /// Gets the current user's session from the cache.
    /// </summary>
    Session? GetCachedSession();
    /// <summary>
    /// Sends the current user's session from
    /// the wasm client to the web server.
    /// </summary>
    Task SendSession(TimeSpan timeout);
    /// <summary>
    /// Sends the current user's session from
    /// the wasm client to the web server.
    /// </summary>
    Task SendSession(CancellationToken ct);
    #endregion

    #region Server
    /// <summary>
    /// Gets the session data for the
    /// current user.
    /// </summary>
    Session GetSession();
    /// <summary>
    /// Updates the current user's session data.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="newSession"/> is <see langword="null"/>.</exception>
    void UpdateSession(Session newSession);
    /// <summary>
    /// Remove all expired session data.
    /// </summary>
    /// <param name="expiration">Expiration duration</param>
    /// <remarks>
    /// Remove all session data that has not been touched
    /// in the last "expiration" timeframe.
    /// </remarks>
    void PurgeSessions(TimeSpan expiration);
    #endregion
  }
}
