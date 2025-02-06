﻿//-----------------------------------------------------------------------
// <copyright file="StateManager.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Get and save state from Blazor pages</summary>
//-----------------------------------------------------------------------

using Csla.State;

namespace Csla.Blazor.State
{
  /// <summary>
  /// Get and save state from Blazor pages.
  /// </summary>
  /// <param name="sessionIdManager"></param>
  /// <param name="sessionManager"></param>
  public class StateManager(ISessionIdManager sessionIdManager, ISessionManager sessionManager)
  {
    private readonly ISessionManager _sessionManager = sessionManager;

    /// <summary>
    /// Get state from cache.
    /// </summary>
    public Task InitializeAsync()
    {
      return InitializeAsync(TimeSpan.FromSeconds(10));
    }

    /// <summary>
    /// Get state from cache.
    /// </summary>
    /// <param name="timeout">Time to wait before timing out</param>
    public Task InitializeAsync(TimeSpan timeout)
    {
      return GetState(timeout);
    }

    /// <summary>
    /// Get state from cache.
    /// </summary>
    /// <param name="timeout">Time to wait before timing out</param>
    private async Task GetState(TimeSpan timeout)
    {
      var isBrowser = OperatingSystem.IsBrowser();
      if (isBrowser)
        _ = await _sessionManager.RetrieveSession();
      else
        _ = sessionIdManager.GetSessionId();
    }

    /// <summary>
    /// Saves state from Blazor wasm to web server. Must call
    /// as user navigates to any server-side Blazor page.
    /// </summary>
    /// <remarks>
    /// Normally this method is called from the Dispose method
    /// of a Blazor page, which is the only reliable point
    /// at which you know the user is navigating to another
    /// page.
    /// </remarks>
    public void SaveState()
    {
      var isBrowser = OperatingSystem.IsBrowser();
      if (isBrowser)
      {
        _sessionManager.SendSession();
      }
    }
  }
}
