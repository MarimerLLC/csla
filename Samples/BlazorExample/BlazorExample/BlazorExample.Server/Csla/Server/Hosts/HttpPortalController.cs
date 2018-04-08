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
using Microsoft.AspNetCore.Mvc;

namespace Csla.Server.Hosts
{
  /// <summary>
  /// Exposes server-side DataPortal functionality
  /// through HTTP request/response.
  /// </summary>
  public class HttpPortalController : Controller
  {
    /// <summary>
    /// Entry point for all data portal operations.
    /// </summary>
    /// <param name="operation">Name of the data portal operation to perform.</param>
    /// <returns>Results from the server-side data portal.</returns>
    [HttpPost]
    public async Task PostAsync([FromQuery]string operation)
    {
      await InvokePortal(operation, Request.Body, Response.Body).ConfigureAwait(false);
    }

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

    private async Task InvokePortal(string operation, Stream requestStream, Stream responseStream)
    {
      string requestString;
      using (var reader = new StreamReader(requestStream))
        requestString = await reader.ReadToEndAsync();
      var requestArray = System.Convert.FromBase64String(requestString);
      var requestBuffer = new MemoryStream(requestArray);

      var serializer = new MobileFormatter();
      var result = new HttpResponse();
      HttpErrorInfo errorData = null;
      try
      {
        var request = serializer.Deserialize(requestBuffer);
        result = await CallPortal(operation, request);
      }
      catch (Exception ex)
      {
        errorData = new HttpErrorInfo(ex);
      }
      var portalResult = new HttpResponse { ErrorData = errorData, GlobalContext = result.GlobalContext, ObjectData = result.ObjectData };

      var responseBuffer = new MemoryStream();
      serializer.Serialize(responseBuffer, portalResult);
      responseBuffer.Position = 0;
      using (var writer = new StreamWriter(responseStream))
      {
        await writer.WriteAsync(System.Convert.ToBase64String(responseBuffer.ToArray()));
      }
    }

    private async Task<HttpResponse> CallPortal(string operation, object request)
    {
      HttpResponse result = null;
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
      return result;
    }
  }
}