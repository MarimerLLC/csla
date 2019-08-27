using System;
using System.Threading.Tasks;
using Csla;
using Microsoft.AspNetCore.Mvc;

namespace ProjectTracker.AppServerHost.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class DataPortalController : Csla.Server.Hosts.HttpPortalController
  {
    // This override exists for demo purposes only, normally you
    // wouldn't implement this code.
    [HttpPost]
    public override Task PostAsync([FromQuery] string operation)
    {
      return base.PostAsync(operation);
    }

    // Implementing a GET is totally optional, but is nice
    // for quick diagnosis as to whether a service is running.
    // GET api/values
    [HttpGet]
    public async Task<ActionResult<string>> GetAsync()
    {
      try
      {
        var obj = await DataPortal.FetchAsync<Library.Dashboard>();
        return $"{obj.ProjectCount} projects exist; {obj.ResourceCount} resources exist";
      }
      catch (Exception ex)
      {
        return ex.ToString();
      }
    }
  }
}