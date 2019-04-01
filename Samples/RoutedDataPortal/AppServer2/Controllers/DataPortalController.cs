using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AppServer2.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class DataPortalController : Csla.Server.Hosts.HttpPortalController
  {
    public DataPortalController()
    {
      Csla.ApplicationContext.LocalContext.Add("dpv", "v2");
    }

    [HttpGet]
    public ActionResult<string> Get()
    {
      return "AppServer2";
    }
  }
}