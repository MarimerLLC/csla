using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectTracker.AppServerHost.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class DataPortalController : Csla.Server.Hosts.HttpPortalController
  {
    [HttpPost]
    public override Task PostAsync([FromQuery] string operation)
    {
      return base.PostAsync(operation);
    }

    // GET api/values
    [HttpGet]
    public async Task<ActionResult<string>> GetAsync()
    {
      try
      {
        var obj = await Csla.DataPortal.FetchAsync<ProjectTracker.Library.Dashboard>();
        return obj.ProjectCount.ToString();
      }
      catch (Exception ex)
      {
        return ex.Message;
      }
    }
  }
}