using Microsoft.AspNetCore.Mvc;

namespace BlazorApp.Server.Controllers
{
  [Route("api/[controller]")]
  public class DataPortalController : Csla.Server.Hosts.HttpPortalController
  {
    public DataPortalController()
    {
      UseTextSerialization = true;
    }
  }
}
