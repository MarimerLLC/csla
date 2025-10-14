//-----------------------------------------------------------------------
// <copyright file="SessionServerManager.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Manages all user session data</summary>
//-----------------------------------------------------------------------

using System.Globalization;
using System.Net.Http.Json;
using Csla.Blazor.Authentication;
using Csla.Blazor.State.Messages;
using Csla.Blazor.WebAssembly.Configuration;
using Csla.Serialization;
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
  /// <exception cref="ArgumentNullException"><paramref name="applicationContext"/>, <paramref name="httpClient"/> or <paramref name="options"/> is <see langword="null"/>.</exception>
  public class SessionManager(ApplicationContext applicationContext, HttpClient httpClient, BlazorWebAssemblyConfigurationOptions options) : ISessionManager
  {
    private readonly ApplicationContext ApplicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
    private readonly HttpClient client = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    private readonly BlazorWebAssemblyConfigurationOptions _options = options ?? throw new ArgumentNullException(nameof(options));
    private Session? _session;

    /// <summary>
    /// Gets the current user's session from the cache.
    /// </summary>
    public Session? GetCachedSession()
    {
      if (!_options.SyncContextWithServer && _session == null)
      {
        _session = GetSession();
      }
      return _session;
    }

    /// <summary>
    /// Retrieves the current user's session from
    /// the web server to the wasm client
    /// if SyncContextWithServer is true.
    /// </summary>
    /// <param name="timeout">The timeout duration for the operation.</param>
    /// <returns>The retrieved session.</returns>
    /// <exception cref="TimeoutException">Thrown when the specified timeout is exceeded.</exception>
    public async Task<Session> RetrieveSession(TimeSpan timeout)
    {
      try
      {
        using var cts = timeout.ToCancellationTokenSource();
        return await RetrieveSession(cts.Token);
      }
      catch (TaskCanceledException tcex)
      {
        throw new TimeoutException($"{this.GetType().FullName}.{nameof(RetrieveSession)}.", tcex);
      }
    }

    /// <summary>
    /// Retrieves the current user's session from
    /// the web server to the wasm client
    /// if SyncContextWithServer is true.
    /// </summary>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The retrieved session.</returns>
    public async Task<Session> RetrieveSession(CancellationToken ct)
    {
      if (_options.SyncContextWithServer)
      {
        long lastTouched = 0;
        if (_session != null)
          lastTouched = _session.LastTouched;
        var url = $"{_options.StateControllerName}?lastTouched={lastTouched}";
        var stateResult = (await client.GetFromJsonAsync<StateResult>(url, ct).ConfigureAwait(false)) ?? throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Csla.Properties.Resources.SessionManagerSessionStateCouldNotBeRetrieved, _options.StateControllerName, nameof(StateResult)));
        if (stateResult.IsSuccess)
        {
          var formatter = ApplicationContext.GetRequiredService<ISerializationFormatter>();
          var message = (SessionMessage)formatter.Deserialize(stateResult.SessionData)!;
          _session = message.Session;
          if (message.Principal is not null && ApplicationContext.GetRequiredService<AuthenticationStateProvider>() is CslaAuthenticationStateProvider provider)
          {
            provider.SetPrincipal(message.Principal);
          }
        }
        else // NoUpdates
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
    /// the wasm client to the web server 
    /// if SyncContextWithServer is true.
    /// </summary>
    /// <param name="timeout">The timeout duration for the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="TimeoutException">Thrown when the specified timeout is exceeded.</exception>
    public async Task SendSession(TimeSpan timeout)
    {
      try
      {
        using var cts = timeout.ToCancellationTokenSource();
        await SendSession(cts.Token);
      }
      catch (TaskCanceledException tcex)
      {
        throw new TimeoutException($"{this.GetType().FullName}.{nameof(SendSession)}.", tcex);
      }
    }

    /// <summary>
    /// Sends the current user's session from
    /// the wasm client to the web server 
    /// if SyncContextWithServer is true.
    /// </summary>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task SendSession(CancellationToken ct)
    {
      _session?.Touch();
      if (_options.SyncContextWithServer)
      {
        ExceptionLocalizer.ThrowIfNullSessionNotRetrieved(_session);

        var formatter = ApplicationContext.GetRequiredService<ISerializationFormatter>();
        var buffer = new MemoryStream();
        formatter.Serialize(buffer, _session);
        buffer.Position = 0;
        await client.PutAsJsonAsync<byte[]>(_options.StateControllerName, buffer.ToArray(), ct).ConfigureAwait(false);
      }
    }


    /// <summary>
    /// Gets or creates the session data.
    /// </summary>
    /// <returns>The session data.</returns>
    private Session GetSession()
    {
      Session result;
      if (_session != null)
      {
        result = _session;
      }
      else
      {
        result = [];
        result.Touch();
      }
      return result;
    }

    // server-side methods
    Session ISessionManager.GetSession() => throw new NotImplementedException();
    void ISessionManager.UpdateSession(Session newSession) => throw new NotImplementedException();
    void ISessionManager.PurgeSessions(TimeSpan expiration) => throw new NotImplementedException();
  }
}
