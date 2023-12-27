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
    Task<Session> GetSession();
    /// <summary>
    /// Updates the current user's
    /// session data.
    /// </summary>
    /// <param name="session">Current user session data</param>
    Task UpdateSession(Session session);
  }
}
