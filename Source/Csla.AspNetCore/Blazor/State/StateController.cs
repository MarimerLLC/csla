using System.IO;
using System.Threading.Tasks;
using Csla.Serialization.Mobile;
using Microsoft.AspNetCore.Mvc;
using Csla.State;

namespace Csla.AspNetCore.Blazor.State
{
  /// <summary>
  /// Gets and puts the current user session data
  /// from the Blazor wasm client components.
  /// </summary>
  /// <param name="applicationContext"></param>
  /// <param name="sessionManager"></param>
  [ApiController]
  [Route("[controller]")]
  public class StateController(ApplicationContext applicationContext, ISessionManager sessionManager) : ControllerBase
  {
    private readonly ApplicationContext ApplicationContext = applicationContext;
    private readonly ISessionManager _sessionManager = sessionManager;

    /// <summary>
    /// Gets current user session data in a serialized
    /// format.
    /// </summary>
    /// <returns></returns>
    [HttpGet(Name = "GetState")]
    public async Task<byte[]> Get()
    {
      var session = await _sessionManager.GetSession();
      var formatter = new MobileFormatter(ApplicationContext);
      var buffer = new MemoryStream();
      formatter.Serialize(buffer, session);
      session.IsCheckedOut = true;
      return buffer.ToArray();
    }

    /// <summary>
    /// Sets the current user session data from a
    /// serialized format.
    /// </summary>
    /// <param name="updatedSessionData"></param>
    /// <returns></returns>
    [HttpPut(Name = "UpdateState")]
    public async Task Put(byte[] updatedSessionData)
    {
      var formatter = new MobileFormatter(ApplicationContext);
      var buffer = new MemoryStream(updatedSessionData)
      {
        Position = 0
      };
      var updatedSession = (Session)formatter.Deserialize(buffer);
      await _sessionManager.UpdateSession(updatedSession);
      
    }
  }
}
