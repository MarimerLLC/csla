using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppServer.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class DataPortalTextController : Csla.Server.Hosts.HttpPortalController
  {
    public DataPortalTextController()
    {
      UseTextSerialization = true;
    }

    [HttpGet]
    public string Get()
    {
      return "Running";
    }

    [HttpPost]
    public override Task PostAsync([FromQuery] string operation)
    {
      return base.PostAsync(operation);
    }
  }
}