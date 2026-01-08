using Microsoft.AspNetCore.Mvc;
using Csla;
using Csla.State;
using Csla.Blazor.State.Messages;

namespace BlazorExample.Controllers
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
  {  }
}