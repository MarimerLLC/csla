using Microsoft.AspNetCore.Mvc;
using Csla;
using Csla.State;
using Csla.Blazor.State.Messages;

namespace ProjectTracker.Blazor.Controllers
{
  /// <summary>
  /// Gets and puts the current user session data
  /// from the Blazor wasm client components.
  /// </summary>
  /// <param name="applicationContext"></param>
  /// <param name="sessionManager"></param>
  [ApiController]
  [Route("[controller]")]
  public class CslaStateController : Csla.AspNetCore.Blazor.State.StateController
  {
    public CslaStateController(ApplicationContext applicationContext, ISessionManager sessionManager) :
      base(applicationContext, sessionManager)
    {
      ApplicationContext = applicationContext;
    }

    private ApplicationContext ApplicationContext { get; set; }

    public override StateResult Get(long lastTouched)
    {
      var user = ApplicationContext.User;
      var result = base.Get(lastTouched);
      return result;
    }
  }
}