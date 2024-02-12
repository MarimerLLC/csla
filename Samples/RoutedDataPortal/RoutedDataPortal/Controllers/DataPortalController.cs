using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Csla;

namespace RoutedDataPortal.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class DataPortalController : Csla.Server.Hosts.HttpPortalController
  {
    public DataPortalController(ApplicationContext applicationContext)
      : base(applicationContext)
    {
      RoutingTagUrls["-v1"] = "http://localhost:64897/api/DataPortal";
      RoutingTagUrls["-v2"] = "http://localhost:64903/api/DataPortal";
      applicationContext.LocalContext.Add("dpv", "v0");
    }

    [HttpPost]
    public override async Task PostAsync([FromQuery] string operation)
    {
      await base.PostAsync(operation).ConfigureAwait(false);
    }

    [HttpPost]
    protected override async Task PostAsync(string operation, string version)
    {
      await base.PostAsync(operation, version);
    }


    [HttpGet]
    public ActionResult<string> Get()
    {
      return "RoutedServer";
    }
  }
}
