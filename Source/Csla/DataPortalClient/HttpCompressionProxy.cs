//-----------------------------------------------------------------------
// <copyright file="HttpCompressionProxy.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implements a data portal proxy to relay data portal</summary>
//-----------------------------------------------------------------------

using System.Net;
using System.Net.Http;
using Csla.Configuration;

namespace Csla.Channels.Http
{
  /// <summary>
  /// Implements a data portal proxy to relay data portal
  /// calls to a remote application server by using http.
  /// </summary>
  public class HttpCompressionProxy : HttpProxy
  {
    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="applicationContext"></param>
    /// <param name="httpClient">HttpClient instance</param>
    /// <param name="options">Options for HttpProxy</param>
    /// <param name="dataPortalOptions">Data portal options</param>
    public HttpCompressionProxy(ApplicationContext applicationContext, HttpClient httpClient, HttpProxyOptions options, DataPortalOptions dataPortalOptions)
      : base(applicationContext, httpClient, options, dataPortalOptions)
    { }

    /// <summary>
    /// Gets an HttpClientHandler for use
    /// in initializing the HttpClient instance.
    /// </summary>
    protected override HttpClientHandler GetHttpClientHandler()
    {
      var result = new HttpClientHandler();
      if (!Options.UseTextSerialization)
        result.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
      return result;
    }

    /// <summary>
    /// Override to set headers or other properties of the
    /// HttpRequestMessage before it is sent to the server.
    /// </summary>
    /// <param name="request">HttpRequestMessage instance</param>
    protected override void SetHttpRequestHeaders(HttpRequestMessage request)
    {
      request.Headers.Add("Accept", "*/*");
      request.Headers.Add("Accept-Encoding", "gzip,deflate,*");
    }

    /// <summary>
    /// Gets an WebClient object for use in
    /// communication with the server.
    /// </summary>
    protected override WebClient GetWebClient()
    {
      var client = new CompressionWebClient(Timeout);
      client.Headers.Set("Accept", "*/*");
      client.Headers.Set("Accept-Encoding", "gzip,deflate,*");
      return client;
    }

#pragma warning disable SYSLIB0014
    private class CompressionWebClient(int timeout) : WebClient
    {
      protected override WebRequest GetWebRequest(Uri address)
      {
        var webRequest = base.GetWebRequest(address)!;

        if (webRequest is HttpWebRequest httpWebRequest)
        {
          httpWebRequest.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
        }

        if (timeout > 0)
        {
          webRequest.Timeout = timeout;
        }

        return webRequest;
      }
    }
#pragma warning restore SYSLIB0014
  }
}