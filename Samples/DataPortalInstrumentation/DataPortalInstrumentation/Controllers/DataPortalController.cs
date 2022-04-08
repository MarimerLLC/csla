using Microsoft.AspNetCore.Mvc;
using Csla.Server.Hosts;
using Csla;
using Csla.Server.Dashboard;

namespace DataPortalInstrumentation.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class DataPortalController : HttpPortalController
  {
    public DataPortalController(ApplicationContext applicationContext, IDashboard dashboard)
      : base(applicationContext) 
    { 
      Dashboard = dashboard;
    }

    private IDashboard Dashboard { get; set; }

    public Csla.Server.Dashboard.IDashboard Get()
    {

      return Dashboard;
    }
  }
}