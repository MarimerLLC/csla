using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DataPortalServer.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class DataPortalController : Csla.Server.Hosts.HttpPortalController
  {
    [HttpGet]
    public string Get()
    {
      return "Running...";
    }
  }
}