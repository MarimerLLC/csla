using Csla;
using Microsoft.AspNetCore.Mvc;

namespace DataPortalServer.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class DataPortalController(ApplicationContext applicationContext) 
    : Csla.Server.Hosts.HttpPortalController(applicationContext)
  {
    [HttpGet]
    public string Get()
    {
      return "DataPortal running...";
    }
  }
}