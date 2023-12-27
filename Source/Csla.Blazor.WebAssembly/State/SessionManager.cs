//-----------------------------------------------------------------------
// <copyright file="SessionServerManager.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Manages all user session data</summary>
//-----------------------------------------------------------------------
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
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
  public class SessionManager(ApplicationContext applicationContext, HttpClient httpClient) : ISessionManager
  {
    private readonly ApplicationContext ApplicationContext = applicationContext;
    private readonly HttpClient client = httpClient;
    private Session _session;

    /// <summary>
    /// Gets the session data for the
    /// current user from the web server
    /// or local cache.
    /// </summary>
    public async Task<Session> GetSession()
    {
      if (_session == null)
      {
        var result = await client.GetFromJsonAsync<byte[]>("state");
        var formatter = new MobileFormatter(ApplicationContext);
        var buffer = new MemoryStream(result)
        {
          Position = 0
        };
        _session = (Session)formatter.Deserialize(buffer);
      }
      return _session;
    }

    /// <summary>
    /// Updates the current user's session 
    /// data locally and on the web server.
    /// </summary>
    /// <param name="session">Current user session data</param>
    public async Task UpdateSession(Session session)
    {
      if (session is not null)
      {
        var formatter = new MobileFormatter(ApplicationContext);
        var buffer = new MemoryStream();
        formatter.Serialize(buffer, session);
        buffer.Position = 0;
        await client.PutAsJsonAsync<byte[]>("state", buffer.ToArray());
      }
      _session = session;
    }
  }
}
