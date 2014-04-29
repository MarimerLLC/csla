using Csla.Serialization.Mobile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Csla.Server.Hosts.HttpChannel;
using Csla.Core;
using System.Security.Principal;

namespace Csla.Server.Hosts
{
  /// <summary>
  /// Exposes server-side DataPortal functionality
  /// through HTTP request/response.
  /// </summary>
#if NET40
  public class HttpPortalController : AsyncController
  {
    [System.Web.Http.HttpPost]
    public async Task<ActionResult> Post(string operation)
    {
      var requestData = await Request.Content.ReadAsByteArrayAsync();
      var responseData = await InvokePortal(operation, requestData);
      Response.BinaryWrite(responseData)
      return View(new NullView());
    }
  }
#else
  public class HttpPortalController : ApiController
  {
    /// <summary>
    /// Entry point for all data portal operations.
    /// </summary>
    /// <param name="operation">Name of the data portal operation to perform.</param>
    /// <returns>Results from the server-side data portal.</returns>
    public virtual async Task<HttpResponseMessage> Post(string operation)
    {
      var requestData = await Request.Content.ReadAsByteArrayAsync();
      var responseData = await InvokePortal(operation, requestData);
      var response = Request.CreateResponse();
      response.Content = new ByteArrayContent(responseData);
      return response;
    }

    private async Task<byte[]> InvokePortal(string operation, byte[] data)
    {
      var result = new HttpResponse();
      HttpErrorInfo errorData = null;
      try
      {
        var buffer = new MemoryStream(data);
        buffer.Position = 0;
        var request = MobileFormatter.Deserialize(buffer.ToArray());
        var portal = new HttpPortal();
        switch (operation)
        {
          case "create":
            result = await portal.Create((CriteriaRequest)request).ConfigureAwait(false);
            break;
          case "fetch":
            result = await portal.Fetch((CriteriaRequest)request).ConfigureAwait(false);
            break;
          case "update":
            result = await portal.Update((UpdateRequest)request).ConfigureAwait(false);
            break;
          case "delete":
            result = await portal.Delete((CriteriaRequest)request).ConfigureAwait(false);
            break;
          default:
            throw new InvalidOperationException(operation);
        }
      }
      catch (Exception ex)
      {
        errorData = new HttpErrorInfo(ex);
      }
      var portalResult = new HttpResponse { ErrorData = errorData, GlobalContext = result.GlobalContext, ObjectData = result.ObjectData };
      var bytes = MobileFormatter.Serialize(portalResult);
      return bytes;
    }
  }
#endif
}
