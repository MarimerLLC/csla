using Microsoft.AspNetCore.Mvc;
using Csla;

namespace AppServer.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class DataPortalController : Csla.Server.Hosts.HttpPortalController
  {
    public DataPortalController(ApplicationContext applicationContext)
      : base(applicationContext) { }

    [HttpGet]
    public string Get() => "DataPortal running (binary)";
  }
}