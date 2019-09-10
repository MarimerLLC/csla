using Microsoft.AspNetCore.Mvc;
using Csla.Server.Hosts;

namespace DataPortalInstrumentation.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class DataPortalController : HttpPortalController
  {
    public Csla.Server.Dashboard.IDashboard Get()
    {
      return Csla.Server.Dashboard.DashboardFactory.GetDashboard();
    }
  }
}