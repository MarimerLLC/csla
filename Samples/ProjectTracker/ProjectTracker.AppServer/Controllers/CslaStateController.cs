using Microsoft.AspNetCore.Mvc;
using Csla;
using Csla.State;

namespace ProjectTracker.AppServer.Controllers
{
  /// <summary>
  /// Gets and puts the current user session data
  /// from the Blazor wasm client components.
  /// </summary>
  /// <param name="applicationContext"></param>
  /// <param name="sessionManager"></param>
  [ApiController]
  [Route("[controller]")]
  public class CslaStateController(ApplicationContext applicationContext, ISessionManager sessionManager) :
      Csla.AspNetCore.Blazor.State.StateController(applicationContext, sessionManager)
  {
    public override byte[] Get() => base.Get();

    public override void Put(byte[] updatedSessionData) => base.Put(updatedSessionData);
  }
}