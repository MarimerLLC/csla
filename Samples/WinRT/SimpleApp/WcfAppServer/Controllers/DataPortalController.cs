using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;

namespace WcfAppServer.Controllers
{
  public class DataPortalController : Csla.Server.Hosts.HttpPortalController
  {
    //[System.Web.Http.HttpPost]
    //public async override Task<ActionResult> PostAsync(string operation)
    //{
    //  var result = await base.PostAsync(operation).ConfigureAwait(false);
    //  return result;
    //}
  }
}
