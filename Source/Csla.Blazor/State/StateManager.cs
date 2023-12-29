//-----------------------------------------------------------------------
// <copyright file="StateManager.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Get and save state from Blazor pages</summary>
//-----------------------------------------------------------------------
using System;
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
    public bool GetState(CalledFrom calledFrom, bool firstRender)
    {
      bool result = false;
      var isBrowser = (Environment.OSVersion.Platform == PlatformID.Other);
      if (isBrowser && calledFrom == CalledFrom.AfterRender)
      {
        if (firstRender)
        {
          _sessionManager.RetrieveSession();
          result = true;
        }
      }
      else if (!isBrowser && calledFrom == CalledFrom.Initialize)
      {
        _sessionManager.RetrieveSession();
        result = true;
      }
      return result;
    }

    /// <summary>
    /// Saves state from Blazor wasm to web server. Must call
    /// as user navigates to any server-side Blazor page.
    /// </summary>
    public void SaveState()
    {
      _sessionManager.SendSession();
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
