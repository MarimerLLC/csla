//-----------------------------------------------------------------------
// <copyright file="StateManager.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Get and save state from Blazor pages</summary>
//-----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using Csla.State;

namespace Csla.Blazor.State
{
  /// <summary>
  /// Get and save state from Blazor pages.
  /// </summary>
  public class StateManager
  {
    private readonly ISessionManager _sessionManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="StateManager"/> class.
    /// </summary>
    /// <param name="sessionManager">Session manager implementation used to persist page state.</param>
    /// <exception cref="ArgumentNullException"><paramref name="sessionManager"/> is <see langword="null"/>.</exception>
    public StateManager(ISessionManager sessionManager)
    {
      _sessionManager = sessionManager ?? throw new ArgumentNullException(nameof(sessionManager));
    }

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
    /// <param name="ct">Cancellation token</param>
    public Task InitializeAsync(CancellationToken ct)
    {
      return GetState(ct);
    }

    /// <summary>
    /// Get state from cache.
    /// </summary>
    /// <param name="timeout">Time to wait before timing out</param>
    private async Task GetState(TimeSpan timeout)
    {
      var isBrowser = OperatingSystem.IsBrowser();
      if (isBrowser)
        _ = await _sessionManager.RetrieveSession(timeout);
      else
        _ = _sessionManager.GetSession();
    }

    /// <summary>
    /// Get state from cache.
    /// </summary>
    /// <param name="ct">Cancellation token</param>
    private async Task GetState(CancellationToken ct)
    {
      var isBrowser = OperatingSystem.IsBrowser();
      if (isBrowser)
        _ = await _sessionManager.RetrieveSession(ct);
      else
        _ = _sessionManager.GetSession();
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
    public Task SaveState()
    {
      return SaveState(TimeSpan.FromSeconds(10));
    }

    /// <summary>
    /// Saves state from Blazor wasm to web server. Must call
    /// as user navigates to any server-side Blazor page.
    /// </summary>
    /// <param name="timeout">Timeout value</param>
    /// <remarks>
    /// Normally this method is called from the Dispose method
    /// of a Blazor page, which is the only reliable point
    /// at which you know the user is navigating to another
    /// page.
    /// </remarks>
    public async Task SaveState(TimeSpan timeout)
    {
      var isBrowser = OperatingSystem.IsBrowser();
      if (isBrowser)
        await _sessionManager.SendSession(timeout);
      else
        _sessionManager.GetSession().Touch();
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
    /// <param name="ct">Cancellation token</param>
    public async Task SaveState(CancellationToken ct)
    {
      var isBrowser = OperatingSystem.IsBrowser();
      if (isBrowser)
        await _sessionManager.SendSession(ct);
      else
        _sessionManager.GetSession().Touch();
    }
  }
}
