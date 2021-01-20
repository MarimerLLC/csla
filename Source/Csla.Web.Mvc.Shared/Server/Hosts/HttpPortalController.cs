//-----------------------------------------------------------------------
// <copyright file="HttpPortalController.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Exposes server-side DataPortal functionality</summary>
//-----------------------------------------------------------------------
using Csla.Serialization.Mobile;
using System;
using System.IO;
using System.Threading.Tasks;
using Csla.Server.Hosts.HttpChannel;
using System.Net.Http;
using Csla.Serialization;
#if NETSTANDARD2_0 || NET5_0 || NETCORE3_1
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
#else
using System.Web.Http;
#endif

namespace Csla.Server.Hosts
{
  /// <summary>
  /// Exposes server-side DataPortal functionality
  /// through HTTP request/response.
  /// </summary>
#if NETSTANDARD2_0 || NET5_0 || NETCORE3_1
  public class HttpPortalController : Controller
  {
    /// <summary>
    /// Gets or sets a value indicating whether to use
    /// text/string serialization instead of the default
    /// binary serialization.
    /// </summary>
    public bool UseTextSerialization { get; set; } = false;

    /// <summary>
    /// Entry point for all data portal operations.
    /// </summary>
    /// <param name="operation">Name of the data portal operation to perform.</param>
    /// <returns>Results from the server-side data portal.</returns>
    [HttpPost]
    public virtual async Task PostAsync([FromQuery]string operation)
    {
      if (operation.Contains("/"))
      {
        var temp = operation.Split('/');
        await PostAsync(temp[0], temp[1]);
      }
      else
      {
        if (UseTextSerialization)
          await InvokeTextPortal(operation, Request.Body, Response.Body).ConfigureAwait(false);
        else
          await InvokePortal(operation, Request.Body, Response.Body).ConfigureAwait(false);
      }
    }

    private static Dictionary<string, string> routingTagUrls = new Dictionary<string, string>();
    private static HttpClient _client;

    /// <summary>
    /// Gets a dictionary containing the URLs for each
    /// data portal route, where each key is the 
    /// routing tag identifying the route URL.
    /// </summary>
    protected static Dictionary<string, string> RoutingTagUrls { get => routingTagUrls; set => routingTagUrls = value; }

    /// <summary>
    /// Gets or sets the HttpClient timeout
    /// in milliseconds (0 uses default HttpClient timeout).
    /// </summary>
    protected int HttpClientTimeout { get; set; }

    /// <summary>
    /// Gets an HttpClient object for use in
    /// communication with the server.
    /// </summary>
    protected virtual HttpClient GetHttpClient()
    {
      if (_client == null)
      {
        _client = new HttpClient();
        if (this.HttpClientTimeout > 0)
        {
          _client.Timeout = TimeSpan.FromMilliseconds(this.HttpClientTimeout);
        }
      }

      return _client;
    }

    /// <summary>
    /// Entry point for routing tag based data portal operations.
    /// </summary>
    /// <param name="operation">Name of the data portal operation to perform.</param>
    /// <param name="routingTag">Routing tag from caller</param>
    protected virtual async Task PostAsync(string operation, string routingTag)
    {
      if (RoutingTagUrls.TryGetValue(routingTag, out string route) && route != "localhost")
      {
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{route}?operation={operation}");
        using (var buffer = new MemoryStream())
        {
          await Request.Body.CopyToAsync(buffer);
          httpRequest.Content = new ByteArrayContent(buffer.ToArray());
        }
        var response = await GetHttpClient().SendAsync(httpRequest);
        await response.Content.CopyToAsync(Response.Body);
      }
      else
      {
        await PostAsync(operation).ConfigureAwait(false);
      }
    }
#elif MVC4
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

#if NETSTANDARD2_0 || NET5_0 || NETCORE3_1
    private async Task InvokePortal(string operation, Stream requestStream, Stream responseStream)
    {
      var serializer = SerializationFormatterFactory.GetFormatter();
      var result = new HttpResponse();
      HttpErrorInfo errorData = null;
      if (UseTextSerialization)
      {
        Response.Headers.Add("Content-type", "application/base64,text/plain");
      }
      else
      {
        Response.Headers.Add("Content-type", "application/octet-stream");
      }
      Response.Headers.Add("Accept-Encoding", "gzip,deflate");
      try
      {
        var request = serializer.Deserialize(requestStream);
        result = await CallPortal(operation, request);
      }
#pragma warning disable CA1031 // Do not catch general exception types
      catch (Exception ex)
      {
        errorData = new HttpErrorInfo(ex);
      }
#pragma warning restore CA1031 // Do not catch general exception types
      var portalResult = new HttpResponse { ErrorData = errorData, GlobalContext = result.GlobalContext, ObjectData = result.ObjectData };
      serializer.Serialize(responseStream, portalResult);
    }

    private async Task InvokeTextPortal(string operation, Stream requestStream, Stream responseStream)
    {
      string requestString;
      using (var reader = new StreamReader(requestStream))
        requestString = await reader.ReadToEndAsync();
      var requestArray = System.Convert.FromBase64String(requestString);
      var requestBuffer = new MemoryStream(requestArray);

      var serializer = SerializationFormatterFactory.GetFormatter();
      var result = new HttpResponse();
      HttpErrorInfo errorData = null;
      try
      {
        var request = serializer.Deserialize(requestBuffer);
        result = await CallPortal(operation, request);
      }
#pragma warning disable CA1031 // Do not catch general exception types
      catch (Exception ex)
      {
        errorData = new HttpErrorInfo(ex);
      }
#pragma warning restore CA1031 // Do not catch general exception types
      var portalResult = new HttpResponse { ErrorData = errorData, GlobalContext = result.GlobalContext, ObjectData = result.ObjectData };

      var responseBuffer = new MemoryStream();
      serializer.Serialize(responseBuffer, portalResult);
      responseBuffer.Position = 0;
      using var writer = new StreamWriter(responseStream)
      {
        AutoFlush = true
      };
      await writer.WriteAsync(System.Convert.ToBase64String(responseBuffer.ToArray()));
    }
#else
    private async Task<byte[]> InvokePortal(string operation, byte[] data)
    {
      var result = new HttpResponse();
      HttpErrorInfo errorData = null;
      try
      {
        var buffer = new MemoryStream(data)
        {
          Position = 0
        };
        var request = SerializationFormatterFactory.GetFormatter().Deserialize(buffer.ToArray());
        result = await CallPortal(operation, request);
      }
#pragma warning disable CA1031 // Do not catch general exception types
      catch (Exception ex)
      {
        errorData = new HttpErrorInfo(ex);
      }
#pragma warning restore CA1031 // Do not catch general exception types
      var portalResult = new HttpResponse { ErrorData = errorData, GlobalContext = result.GlobalContext, ObjectData = result.ObjectData };
      var bytes = SerializationFormatterFactory.GetFormatter().Serialize(portalResult);
      return bytes;
    }
#endif

    private async Task<HttpResponse> CallPortal(string operation, object request)
    {
      var portal = Portal;
      HttpResponse result = operation switch
      {
        "create" => await portal.Create((CriteriaRequest)request).ConfigureAwait(false),
        "fetch" => await portal.Fetch((CriteriaRequest)request).ConfigureAwait(false),
        "update" => await portal.Update((UpdateRequest)request).ConfigureAwait(false),
        "delete" => await portal.Delete((CriteriaRequest)request).ConfigureAwait(false),
        _ => throw new InvalidOperationException(operation),
      };
      return result;
    }
  }
}
