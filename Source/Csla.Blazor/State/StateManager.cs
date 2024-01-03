//-----------------------------------------------------------------------
// <copyright file="StateManager.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Get and save state from Blazor pages</summary>
//-----------------------------------------------------------------------
using System;
using System.Threading.Tasks;
using Csla.State;

namespace Csla.Blazor.State
{
  /// <summary>
  /// Get and save state from Blazor pages.
  /// </summary>
  /// <param name="sessionManager"></param>
  public class StateManager(ISessionManager sessionManager)
  {
    private readonly ISessionManager _sessionManager = sessionManager;

    /// <summary>
    /// Gets a value indicating whether state is valid
    /// for use in the current component.
    /// </summary>
    public bool IsStateAvailable { get; private set; } = false;

    /// <summary>
    /// Get state from cache.
    /// </summary>
    /// <returns></returns>
    public async Task InitializeAsync()
    {
      await InitializeAsync(TimeSpan.FromSeconds(10));
    }

    /// <summary>
    /// Get state from cache.
    /// </summary>
    /// <param name="timeout">Time to wait before timing out</param>
    /// <returns></returns>
    public async Task InitializeAsync(TimeSpan timeout)
    {
      await GetState(timeout);
    }

    /// <summary>
    /// Get state from cache.
    /// </summary>
    /// <param name="timeout">Time to wait before timing out</param>
    /// <returns></returns>
    private async Task GetState(TimeSpan timeout)
    {
      Session session;
      var isBrowser = OperatingSystem.IsBrowser();
      if (isBrowser)
        session = await _sessionManager.RetrieveSession();
      else
        session = await WaitForState(timeout);
      session.Initialize();
      IsStateAvailable = session.IsFullyInitialized;
    }

    /// <summary>
    /// Waits for state to be available as it is transferred
    /// from Blazor wasm to the web server.
    /// </summary>
    /// <param name="timeout">Time to wait before timing out</param>
    private async Task<Session> WaitForState(TimeSpan timeout)
    {
      var session = _sessionManager.GetSession();
      if (session.IsCheckedOut)
      {
        var tcs = new TaskCompletionSource();
        session.PropertyChanged += (s, e) =>
        {
          if (e.PropertyName == "IsCheckedOut" && !session.IsCheckedOut && !tcs.Task.IsCompleted)
            tcs.SetResult();
        };
        if (session.IsCheckedOut)
          await tcs.Task.WaitAsync(timeout);
      }
      return session;
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
        IsStateAvailable = false;
      }
    }
  }
}
