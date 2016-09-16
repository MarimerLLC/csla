#if !NET40
//-----------------------------------------------------------------------
// <copyright file="HttpPortalController.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Exposes server-side DataPortal functionality</summary>
//-----------------------------------------------------------------------
using Csla.Serialization.Mobile;
using System;
using System.IO;
using System.Threading.Tasks;
using Csla.Server.Hosts.HttpChannel;
#if MVC4
using System.Web.Mvc;
#endif
using System.Net.Http;
using System.Web.Http;

namespace Csla.Server.Hosts
{
  /// <summary>
  /// Exposes server-side DataPortal functionality
  /// through HTTP request/response.
  /// </summary>
#if MVC4
  public class HttpPortalController : Controller
  {
    /// <summary>
    /// Entry point for all data portal operations.
    /// </summary>
    /// <param name="operation">Name of the data portal operation to perform.</param>
    /// <returns>Results from the server-side data portal.</returns>
    [HttpPost]
    public virtual async Task<ActionResult> PostAsync(string operation)
    {
      var requestData = Request.BinaryRead(Request.TotalBytes);
      var responseData = await InvokePortal(operation, requestData).ConfigureAwait(false);
      Response.BinaryWrite(responseData);
      return new EmptyResult();
    }
#else
  public class HttpPortalController : ApiController
  {
    /// <summary>
    /// Entry point for all data portal operations.
    /// </summary>
    /// <param name="operation">Name of the data portal operation to perform.</param>
    /// <returns>Results from the server-side data portal.</returns>
    public virtual async Task<HttpResponseMessage> PostAsync(string operation)
    {
      var requestData = await Request.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
      var responseData = await InvokePortal(operation, requestData).ConfigureAwait(false);
      var response = Request.CreateResponse();
      response.Content = new ByteArrayContent(responseData);
      return response;
    }
#endif

    private HttpPortal _portal;
    /// <summary>
    /// Gets or sets the HttpPortal implementation
    /// used to coordinate the data portal
    /// operations.
    /// </summary>
    public HttpPortal Portal 
    {
      get 
      {
        if (_portal == null)
          _portal = new HttpPortal();
        return _portal; 
      }
      set { _portal = value; } 
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
        var portal = Portal;
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
}
#endif