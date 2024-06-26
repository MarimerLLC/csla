//-----------------------------------------------------------------------
// <copyright file="ISessionIdManager.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Manages the per-user id value</summary>
//-----------------------------------------------------------------------
#nullable enable

namespace Csla.State
{
  /// <summary>
  /// Manages the per-user id value
  /// for state management.
  /// </summary>
  public interface ISessionIdManager
  {
    /// <summary>
    /// Gets the per-user id value
    /// for the current user state.
    /// </summary>
    string GetSessionId();
  }
}
