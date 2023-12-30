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
using static System.Collections.Specialized.BitVector32;

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
    /// Get state from web server for use in Blazor wasm.
    /// client.
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// This is called in Initialize methods of a Blazor page (sync
    /// or async). This way the page can be flexibly
    /// rendered in server-rendered, server-interactive, and wasm-interactive
    /// modes and the state will always be available.
    /// </remarks>
    public async Task Initialize()
    {
      await Initialize(TimeSpan.FromSeconds(10));
    }

    /// <summary>
    /// Get state from web server for use in Blazor wasm.
    /// client.
    /// </summary>
    /// <param name="timeout">Time to wait before timing out</param>
    /// <returns></returns>
    /// <remarks>
    /// This is called in Initialize methods of a Blazor page (sync
    /// or async). This way the page can be flexibly
    /// rendered in server-rendered, server-interactive, and wasm-interactive
    /// modes and the state will always be available.
    /// </remarks>
    public async Task Initialize(TimeSpan timeout)
    {
      await GetState(CalledFrom.Initialize, true, timeout);
    }

    /// <summary>
    /// Get state from web server for use in Blazor wasm.
    /// client.
    /// </summary>
    /// <param name="firstRender">firstRender value from OnAfterRender method</param>
    /// <returns></returns>
    /// <remarks>
    /// This is called in OnAfterRender methods of a Blazor page (sync
    /// or async). This way the page can be flexibly
    /// rendered in server-rendered, server-interactive, and wasm-interactive
    /// modes and the state will always be available.
    /// </remarks>
    public async Task AfterRender(bool firstRender)
    {
      await AfterRender(firstRender, TimeSpan.FromSeconds(10));
    }

    /// <summary>
    /// Get state from web server for use in Blazor wasm.
    /// client.
    /// </summary>
    /// <param name="firstRender">firstRender value from OnAfterRender method</param>
    /// <param name="timeout">Time to wait before timing out</param>
    /// <returns></returns>
    /// <remarks>
    /// This is called in OnAfterRender methods of a Blazor page (sync
    /// or async). This way the page can be flexibly
    /// rendered in server-rendered, server-interactive, and wasm-interactive
    /// modes and the state will always be available.
    /// </remarks>
    public async Task AfterRender(bool firstRender, TimeSpan timeout)
    {
      await GetState(CalledFrom.AfterRender, firstRender, timeout);
    }

    /// <summary>
    /// Get state from web server for use in Blazor wasm. Must call
    /// as user navigates from any server-side Blazor page to a wasm
    /// Blazor page.
    /// client.
    /// </summary>
    /// <param name="calledFrom">Location method is invoked in page</param>
    /// <param name="firstRender">firstRender value from OnAfterRender method</param>
    /// <param name="timeout">Time to wait before timing out</param>
    /// <returns></returns>
    /// <remarks>
    /// Normally this is called in both the initialize and after render
    /// methods of a Blazor page. This way the page can be flexibly
    /// rendered in server-rendered, server-interactive, and wasm-interactive
    /// modes and the state will always be available.
    /// </remarks>
    private async Task GetState(CalledFrom calledFrom, bool firstRender, TimeSpan timeout)
    {
      var isBrowser = OperatingSystem.IsBrowser();
      if (isBrowser)
      {
        if (calledFrom == CalledFrom.Initialize && firstRender)
        {
          var session = await _sessionManager.RetrieveSession();
          IsStateAvailable = session.IsFullyInitialized;
        }
      }
      else
      {
        if (calledFrom == CalledFrom.Initialize)
        {
          await WaitForState(timeout);
        }
      }
    }

    /// <summary>
    /// Waits for state to be available as it is transferred
    /// from Blazor wasm to the web server.
    /// </summary>
    /// <param name="timeout">Time to wait before timing out</param>
    private async Task WaitForState(TimeSpan timeout)
    {
      var session = _sessionManager.GetSession();
      session.Initialize();
      if (session != null && session.IsCheckedOut)
      {
        var endTime = DateTime.Now + timeout;
        while (session.IsCheckedOut)
        {
          if (DateTime.Now > endTime)
            throw new TimeoutException();
          await Task.Delay(5);
        }
      }
      IsStateAvailable = session.IsFullyInitialized;
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

    /// <summary>
    /// Indicate the location from which the StateManager 
    /// is invoked.
    /// </summary>
    private enum CalledFrom
    {
      /// <summary>
      /// Blazor page initialize method (sync or async).
      /// </summary>
      Initialize,
      /// <summary>
      /// Blazor page after render method (sync or async).
      /// </summary>
      AfterRender
    }
  }
}
