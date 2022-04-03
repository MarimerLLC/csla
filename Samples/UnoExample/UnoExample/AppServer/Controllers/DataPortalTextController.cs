using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Csla;

namespace AppServer.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class DataPortalTextController : Csla.Server.Hosts.HttpPortalController
  {
    public DataPortalTextController(ApplicationContext applicationContext)
      : base(applicationContext)
    {
      UseTextSerialization = true;
    }

    [HttpGet]
    public string Get() => "DataPortal running (text)";

    [HttpPost]
    public override Task PostAsync([FromQuery] string operation)
    {
      return base.PostAsync(operation);
    }
  }
}