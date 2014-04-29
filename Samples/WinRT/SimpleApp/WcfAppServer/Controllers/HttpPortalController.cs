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
  public class HttpPortalController : AsyncController
  {
    [System.Web.Http.HttpPost]
    public async Task<ActionResult> Post(string operation)
    {
      var req = Request;
      var res = Response;
      var portal = new HttpPortalControllerX();
      var result = await portal.Post(operation);
      return View(new NullView());
    }
  }

  public class NullView : ViewPage
  { }

  public class HttpPortalControllerX : Csla.Server.Hosts.HttpPortal
  {
    public override System.Threading.Tasks.Task<HttpResponseMessage> Post(string operation)
    {
      return base.Post(operation);
    }

    protected override Csla.Server.Hosts.HttpChannel.CriteriaRequest ConvertRequest(Csla.Server.Hosts.HttpChannel.CriteriaRequest request)
    {
      return base.ConvertRequest(request);
    }

    protected override Csla.Server.Hosts.HttpChannel.UpdateRequest ConvertRequest(Csla.Server.Hosts.HttpChannel.UpdateRequest request)
    {
      return base.ConvertRequest(request);
    }

    protected override Csla.Server.Hosts.HttpChannel.HttpResponse ConvertResponse(Csla.Server.Hosts.HttpChannel.HttpResponse response)
    {
      return base.ConvertResponse(response);
    }
  }
}
