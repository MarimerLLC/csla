//-----------------------------------------------------------------------
// <copyright file="ISessionManager.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Manages all user session data</summary>
//-----------------------------------------------------------------------
using System.Threading.Tasks;

namespace Csla.State
{
  /// <summary>
  /// Manages all user session data for a given
  /// root DI container.
  /// </summary>
  public interface ISessionManager
  {
    /// <summary>
    /// Gets the session data for the
    /// current user.
    /// </summary>
    Session GetSession();
    /// <summary>
    /// Updates the current user's
    /// session data.
    /// </summary>
    /// <param name="session">Current user session data</param>
    void UpdateSession(Session session);
    /// <summary>
    /// Retrieves the current user's session from
    /// the web server to the wasm client.
    /// </summary>
    Task<Session> RetrieveSession();
    /// <summary>
    /// Sends the current user's session from
    /// the wasm client to the web server.
    /// </summary>
    /// <returns></returns>
    Task SendSession();
  }
}
