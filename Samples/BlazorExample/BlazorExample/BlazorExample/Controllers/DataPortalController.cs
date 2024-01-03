using Microsoft.AspNetCore.Mvc;

namespace BlazorExample.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class DataPortalController(Csla.ApplicationContext applicationContext) : 
    Csla.Server.Hosts.HttpPortalController(applicationContext)
  {
    [HttpGet]
    public string Get()
    {
      return "Running";
    }

    public override Task PostAsync([FromQuery] string operation)
    {
      var result = base.PostAsync(operation);
      return result;
    }
  }
}