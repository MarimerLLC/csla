//-----------------------------------------------------------------------
// <copyright file="SessionServerManager.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Manages all user session data</summary>
//-----------------------------------------------------------------------
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Csla.Blazor.Authentication;
using Csla.Configuration;
using Csla.Serialization;
using Csla.Serialization.Mobile;
using Csla.State;
using Microsoft.AspNetCore.Components.Authorization;

namespace Csla.Blazor.WebAssembly.State
{
  /// <summary>
  /// Manages all user session data for a given
  /// root DI container.
  /// </summary>
  /// <param name="applicationContext"></param>
  /// <param name="httpClient"></param>
  /// <param name="options"></param>
  public class SessionManager(
    ApplicationContext applicationContext, HttpClient httpClient, BlazorWebAssemblyConfigurationOptions options) : ISessionManager
  {
    private readonly ApplicationContext ApplicationContext = applicationContext;
    private readonly HttpClient client = httpClient;
    private readonly BlazorWebAssemblyConfigurationOptions _options = options;
    private Session _session;

    /// <summary>
    /// Gets the session data for the
    /// current user from the web server
    /// or local cache.
    /// </summary>
    public Session GetSession()
    {
      if (_session == null)
      {
        _session ??= [];
        _session.LastTouched = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
      }
      return _session;
    }

    /// <summary>
    /// Updates the current user's session 
    /// data locally and on the web server.
    /// </summary>
    /// <param name="session">Current user session data</param>
    public void UpdateSession(Session session)
    {
      _session.LastTouched = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
      _session = session;
    }

    /// <summary>
    /// Retrieves the current user's session from
    /// the web server to the wasm client.
    /// </summary>
    public async Task<Session> RetrieveSession()
    {
      if (_options.SyncContextWithServer)
      {
        if (_session == null) _session = GetSession(); // make sure we have a session
        var url = $"{_options.StateControllerName}?lastTouched={_session.LastTouched}";
        var stateResult = await client.GetFromJsonAsync<StateResult>(url);
        if (stateResult.ResultStatus == ResultStatuses.Success)
        {
          var formatter = SerializationFormatterFactory.GetFormatter(ApplicationContext);
          var buffer = new MemoryStream(stateResult.SessionData)
          {
            Position = 0
          };
          var message = (SessionMessage)formatter.Deserialize(buffer);
          _session = message.Session;
          if (message.Principal is not null &&
              ApplicationContext.GetRequiredService<AuthenticationStateProvider>() is CslaAuthenticationStateProvider provider)
          {
            provider.SetPrincipal(message.Principal);
          }
        }
        else // NoResult
        {
          _session = GetSession();
        }
      }
      else
      {
        _session = GetSession();
      }
      return _session;
    }

    /// <summary>
    /// Sends the current user's session from
    /// the wasm client to the web server.
    /// </summary>
    /// <returns></returns>
    public async Task SendSession()
    {
      _session.LastTouched = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
      if (_options.SyncContextWithServer)
      {
        var formatter = SerializationFormatterFactory.GetFormatter(ApplicationContext);
        var buffer = new MemoryStream();
        formatter.Serialize(buffer, _session);
        buffer.Position = 0;
        await client.PutAsJsonAsync<byte[]>(_options.StateControllerName, buffer.ToArray());
      }
    }

    /// <summary>
    /// Remove all expired session data. Not supported on wasm.
    /// </summary>
    /// <param name="expiration">Expiration duration</param>
    public void PurgeSessions(TimeSpan expiration) => throw new NotSupportedException();
  }

  /// <summary>
  /// Message type for communication between StateController
  /// and the Blazor wasm client.
  /// </summary>
  public class StateResult
  {
    /// <summary>
    /// Gets or sets the result status of the message.
    /// </summary>
    public ResultStatuses ResultStatus { get; set; }
    /// <summary>
    /// Gets or sets the serialized session data.
    /// </summary>
    public byte[] SessionData { get; set; }
  }

  /// <summary>
  /// Result status values for StateResult.
  /// </summary>
  public enum ResultStatuses
  {
    /// <summary>
    /// Success
    /// </summary>
    Success = 0,
    /// <summary>
    /// No updates
    /// </summary>
    NoUpdates = 1,
  }
}
