using Microsoft.AspNetCore.Mvc;

namespace BlazorExample.Server.Controllers
{
  [Route("api/[controller]")]
  public class DataPortalController : Csla.Server.Hosts.HttpPortalController
  {
  }
}