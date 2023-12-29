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
    /// Get state from web server for use in Blazor wasm. Must call
    /// as user navigates from any server-side Blazor page to a wasm
    /// Blazor page.
    /// client.
    /// </summary>
    /// <param name="calledFrom">Location method is invoked in page</param>
    /// <param name="firstRender">firstRender value from OnAfterRender method</param>
    /// <returns></returns>
    /// <remarks>
    /// Normally this is called in both the initialize and after render
    /// methods of a Blazor page. This way the page can be flexibly
    /// rendered in server-rendered, server-interactive, and wasm-interactive
    /// modes and the state will always be available.
    /// </remarks>
    public async Task GetState(CalledFrom calledFrom, bool firstRender)
    {
      await GetState(calledFrom, firstRender, 10);
    }

    /// <summary>
    /// Get state from web server for use in Blazor wasm. Must call
    /// as user navigates from any server-side Blazor page to a wasm
    /// Blazor page.
    /// client.
    /// </summary>
    /// <param name="calledFrom">Location method is invoked in page</param>
    /// <param name="firstRender">firstRender value from OnAfterRender method</param>
    /// <param name="timeout">Milliseconds to wait before timing out</param>
    /// <returns></returns>
    /// <remarks>
    /// Normally this is called in both the initialize and after render
    /// methods of a Blazor page. This way the page can be flexibly
    /// rendered in server-rendered, server-interactive, and wasm-interactive
    /// modes and the state will always be available.
    /// </remarks>
    public async Task GetState(CalledFrom calledFrom, bool firstRender, int timeout)
    {
      var isBrowser = OperatingSystem.IsBrowser();
      if (isBrowser)
      {
        if (calledFrom == CalledFrom.AfterRender && firstRender)
        {
            await _sessionManager.RetrieveSession();
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
    /// <param name="timeout">Milliseconds to wait before timing out</param>
    private async Task WaitForState(int timeout)
    {
      var session = _sessionManager.GetSession();
      timeout *= 10;
      var count = 0;
      while (session.IsCheckedOut)
      {
        count++;
        if (count == timeout)
          throw new TimeoutException();
        await Task.Delay(1000);
      }
    }

    /// <summary>
    /// Saves state from Blazor wasm to web server. Must call
    /// as user navigates to any server-side Blazor page.
    /// </summary>
    /// <remarks>
    /// Normally this method is called from the Dispose method
    /// of a Blazor wasm page, which is the only reliable point
    /// at which you know the user is navigating to another
    /// page.
    /// </remarks>
    public async Task SaveState()
    {
      var isBrowser = OperatingSystem.IsBrowser();
      if (isBrowser)
        await _sessionManager.SendSession();
    }

    /// <summary>
    /// Indicate the location from which the StateManager 
    /// is invoked.
    /// </summary>
    public enum CalledFrom
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
