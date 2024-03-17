using Microsoft.AspNetCore.Mvc;

namespace Csla.TestHelpers.Http;

[Route("api/[controller]")]
[ApiController]
public class TestDataPortalController(ApplicationContext applicationContext)
    : Csla.Server.Hosts.HttpPortalController(applicationContext) {
  [HttpGet]
  public string Get() {
    return "DataPortal running...";
  }
}
