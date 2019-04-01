using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppServer1.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class DataPortalController : Csla.Server.Hosts.HttpPortalController
  {
    public DataPortalController()
    {
      Csla.ApplicationContext.LocalContext.Add("dpv", "v1");
    }

    [HttpGet]
    public ActionResult<string> Get()
    {
      return "AppServer2";
    }
  }
}
