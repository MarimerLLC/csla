using System.Net.Http;

namespace ProjectTracker.AppServerHost.Controllers
{
  public class DataPortalController : Csla.Server.Hosts.HttpPortalController
  {
    public async override System.Threading.Tasks.Task<HttpResponseMessage> PostAsync(string operation)
    {
      return await base.PostAsync(operation).ConfigureAwait(false);
    }
  }
}