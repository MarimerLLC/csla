using System;
using System.Threading.Tasks;
using Csla;
using Microsoft.AspNetCore.Mvc;

namespace DataPortalServer.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class DataPortalController : Csla.Server.Hosts.HttpPortalController
  {
    public DataPortalController(ApplicationContext applicationContext)
      : base(applicationContext) { }

    [HttpGet]
    public string Get()
    {
      return "DataPortal running...";
    }
  }
}