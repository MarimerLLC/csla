using System;
using System.Threading.Tasks;
using Csla;
using Microsoft.AspNetCore.Mvc;

namespace AppServer.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class DataPortalController : Csla.Server.Hosts.HttpPortalController
  {
    public DataPortalController(ApplicationContext applicationContext)
      : base(applicationContext) { }

    [HttpGet]
    public string Get()
    {
      return "DataPortal running";
    }

    public override Task PostAsync([FromQuery] string operation)
    {
      var result = base.PostAsync(operation);
      return result;
    }

    protected override Task PostAsync(string operation, string routingTag) => base.PostAsync(operation, routingTag);
  }
}