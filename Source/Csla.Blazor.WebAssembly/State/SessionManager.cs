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
using Csla.Configuration;
using Csla.Serialization.Mobile;
using Csla.State;

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
        _session = [];
        _session.Initialize();
        _session.Initialize();
      }
      return _session;
    }

    /// <summary>
    /// Updates the current user's session 
    /// data locally and on the web server.
    /// </summary>
    /// <param name="session">Current user session data</param>
    public void UpdateSession(Session session) => _session = session;

    /// <summary>
    /// Retrieves the current user's session from
    /// the web server to the wasm client.
    /// </summary>
    public async Task<Session> RetrieveSession()
    {
      if (_session == null)
      {
        var result = await client.GetFromJsonAsync<byte[]>(_options.StateControllerName);
        var formatter = new MobileFormatter(ApplicationContext);
        var buffer = new MemoryStream(result)
        {
          Position = 0
        };
        _session = (Session)formatter.Deserialize(buffer);
      }
      else
      {
        await client.PatchAsync(_options.StateControllerName, null);
      }
      _session.Initialize();
      _session.Initialize();
      return _session;
    }

    /// <summary>
    /// Sends the current user's session from
    /// the wasm client to the web server.
    /// </summary>
    /// <returns></returns>
    public async Task SendSession()
    {
      if (_session is not null)
      {
        var formatter = new MobileFormatter(ApplicationContext);
        var buffer = new MemoryStream();
        formatter.Serialize(buffer, _session);
        buffer.Position = 0;
        await client.PutAsJsonAsync<byte[]>(_options.StateControllerName, buffer.ToArray());
      }
    }
  }
}
