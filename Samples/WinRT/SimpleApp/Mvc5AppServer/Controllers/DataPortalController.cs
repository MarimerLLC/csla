using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Mvc5AppServer.Controllers
{
    public class DataPortalController : Csla.Server.Hosts.HttpPortalController
    {
      public async override System.Threading.Tasks.Task<HttpResponseMessage> PostAsync(string operation)
      {
        return await base.PostAsync(operation).ConfigureAwait(false);
      }
    }
}
