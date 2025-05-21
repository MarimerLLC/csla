//-----------------------------------------------------------------------
// <copyright file="HttpProxy.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implements a data portal proxy to relay data portal</summary>
//-----------------------------------------------------------------------
using Csla.Configuration;
using Csla.DataPortalClient;
using Csla.Properties;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
#if NET8_0_OR_GREATER
using System.Runtime.Versioning;
#endif
using System.Text;

namespace Csla.Channels.Http
{
  /// <summary>
  /// Implements a data portal proxy to relay data portal
  /// calls to a remote application server by using http.
  /// </summary>
  public class HttpProxy : DataPortalProxy
  {
    private HttpClient _httpClient;

    /// <summary>
    /// Creates an instance of the type, initializing
    /// it to use the supplied HttpClient object and options.
    /// </summary>
    /// <param name="applicationContext"></param>
    /// <param name="httpClient">HttpClient instance</param>
    /// <param name="options">Options for HttpProxy</param>
    /// <param name="dataPortalOptions">Data portal options</param>
    public HttpProxy(ApplicationContext applicationContext, HttpClient httpClient, HttpProxyOptions options, DataPortalOptions dataPortalOptions)
      : base(applicationContext)
    {
      _httpClient = httpClient;
      Options = options;
      DataPortalUrl = options.DataPortalUrl;
      _versionRoutingTag = dataPortalOptions.VersionRoutingTag;
    }

    /// <summary>
    /// Current options for the proxy.
    /// </summary>
    protected HttpProxyOptions Options { get; set; }

    private string _versionRoutingTag;

#nullable enable
    /// <summary>
    /// Gets an HttpClientHandler for use
    /// in initializing the HttpClient instance.
    /// </summary>
    protected virtual HttpClientHandler? GetHttpClientHandler()
    {
      return null;
    }
#nullable disable

    /// <summary>
    /// Gets an HttpClient object for use in
    /// asynchronous communication with the server.
    /// </summary>
    protected virtual HttpClient GetHttpClient()
    {
      if (_httpClient == null)
      {
        var handler = GetHttpClientHandler() ?? CreateDefaultHandler();

        _httpClient = new HttpClient(handler);
        if (Options.Timeout > TimeSpan.Zero)
        {
          _httpClient.Timeout = Options.Timeout;
        }
      }

      return _httpClient;

      HttpClientHandler CreateDefaultHandler()
      {
        var handler = new HttpClientHandler();
#if NET8_0_OR_GREATER
        // Browser does not support customization of HttpClientHandler, since it's provided by browser.
        if (OperatingSystem.IsBrowser())
        {
          return handler;
        }
#endif
        if (!Options.UseTextSerialization)
        {
          handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
        }

        return handler;
      }
    }

    /// <summary>
    /// Gets an WebClient object for use in
    /// synchronous communication with the server.
    /// </summary>
#if NET8_0_OR_GREATER
    [UnsupportedOSPlatform("browser")]
#endif
    protected virtual WebClient GetWebClient()
    {
      return new DefaultWebClient(Options.Timeout, Options.ReadWriteTimeout);
    }

    /// <summary>
    /// Select client to make request based on isSync parameter and return response from server
    /// </summary>
    /// <param name="serialized">Serialized request</param>
    /// <param name="operation">DataPortal operation</param>
    /// <param name="routingToken">Routing Tag for server</param>
    /// <param name="isSync">True if the client-side proxy should synchronously invoke the server.</param>
    /// <returns>Serialized response from server</returns>
    protected override async Task<byte[]> CallDataPortalServer(byte[] serialized, string operation, string routingToken, bool isSync)
    {
      return isSync
        ? CallViaWebClient(serialized, operation, routingToken)
        : await CallViaHttpClient(serialized, operation, routingToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Override to set headers or other properties of the
    /// HttpRequestMessage before it is sent to the server
    /// (asynchronous only).
    /// </summary>
    /// <param name="request">HttpRequestMessage instance</param>
    protected virtual void SetHttpRequestHeaders(HttpRequestMessage request)
    { }

    /// <summary>
    /// Override to set headers or other properties of the
    /// WebClient before it is sent to the server
    /// (synchronous only).
    /// </summary>
    /// <param name="client">WebClient instance</param>
    protected virtual void SetWebClientHeaders(WebClient client)
    { }

    private async Task<byte[]> CallViaHttpClient(byte[] serialized, string operation, string routingToken)
    {
      var client = GetHttpClient();
      using var httpRequest = new HttpRequestMessage(
        HttpMethod.Post,
        $"{DataPortalUrl}?operation={CreateOperationTag(operation, _versionRoutingTag, routingToken)}");
      SetHttpRequestHeaders(httpRequest);
#if NET8_0_OR_GREATER
      if (Options.UseTextSerialization)
      {
        httpRequest.Content = new StringContent(
          Convert.ToBase64String(serialized),
          mediaType: new MediaTypeHeaderValue("application/base64,text/plain"));
      }
      else
      {
        httpRequest.Content = new ByteArrayContent(serialized);
        httpRequest.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
      }
#else
      if (Options.UseTextSerialization)
      {
        httpRequest.Content = new StringContent(Convert.ToBase64String(serialized));
        httpRequest.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
      }
      else
      {
        httpRequest.Content = new ByteArrayContent(serialized);
        httpRequest.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
      }
#endif
      using var httpResponse = await client.SendAsync(httpRequest).ConfigureAwait(false);
      await VerifyResponseSuccess(httpResponse).ConfigureAwait(false);
      if (Options.UseTextSerialization)
        serialized = Convert.FromBase64String(await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false));
      else
        serialized = await httpResponse.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
      return serialized;
    }

    private byte[] CallViaWebClient(byte[] serialized, string operation, string routingToken)
    {
#if NET8_0_OR_GREATER
      if (OperatingSystem.IsBrowser())
      {
        throw new PlatformNotSupportedException(Resources.SyncDataAccessNotSupportedException);
      }
#endif
      WebClient client = GetWebClient();
      var url = $"{DataPortalUrl}?operation={CreateOperationTag(operation, _versionRoutingTag, routingToken)}";
      client.Headers["Content-Type"] = Options.UseTextSerialization ? "application/base64,text/plain" : "application/octet-stream";
      SetWebClientHeaders(client);
      try
      {
        if (Options.UseTextSerialization)
        {
          var result = client.UploadString(url, Convert.ToBase64String(serialized));
          serialized = Convert.FromBase64String(result);
        }
        else
        {
          var result = client.UploadData(url, serialized);
          serialized = result;
        }
        return serialized;
      }
      catch (WebException ex)
      {
        string message;
        if (ex.Response != null)
        {
          using var reader = new StreamReader(ex.Response.GetResponseStream());
          message = reader.ReadToEnd();
        }
        else
        {
          message = ex.Message;
        }
        throw new DataPortalException(message, ex);
      }
    }

    private static async Task VerifyResponseSuccess(HttpResponseMessage httpResponse)
    {
      if (!httpResponse.IsSuccessStatusCode)
      {
        var message = new StringBuilder();
        message.Append((int)httpResponse.StatusCode);
        message.Append(": ");
        message.Append(httpResponse.ReasonPhrase);
        var content = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
        if (!string.IsNullOrWhiteSpace(content))
        {
          message.AppendLine();
          message.Append(content);
        }
        throw new HttpRequestException(message.ToString());
      }
    }

    private string CreateOperationTag(string operation, string versionToken, string routingToken)
    {
      if (!string.IsNullOrWhiteSpace(versionToken) || !string.IsNullOrWhiteSpace(routingToken))
        return $"{operation}/{routingToken}-{versionToken}";
      return operation;
    }

#pragma warning disable SYSLIB0014
#if NET8_0_OR_GREATER
    [UnsupportedOSPlatform("browser")]
#endif
    private class DefaultWebClient(TimeSpan timeout, TimeSpan readWriteTimeout) : WebClient
    {

      protected override WebRequest GetWebRequest(Uri address)
      {
        var req = base.GetWebRequest(address)!;
        if (req is HttpWebRequest httpWebRequest)
        {
          if (readWriteTimeout > TimeSpan.Zero)
          {
            httpWebRequest.ReadWriteTimeout = (int)readWriteTimeout.TotalMilliseconds;
          }
        }
        if (timeout > TimeSpan.Zero)
        {
          req.Timeout = (int)timeout.TotalMilliseconds;
        }
        return req;
      }
    }
#pragma warning restore SYSLIB0014
  }
}