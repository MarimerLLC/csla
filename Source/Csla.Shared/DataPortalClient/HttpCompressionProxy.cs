//-----------------------------------------------------------------------
// <copyright file="HttpCompressionProxy.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implements a data portal proxy to relay data portal</summary>
//-----------------------------------------------------------------------
using System;
using System.Net;
using System.Net.Http;

namespace Csla.DataPortalClient
{
  /// <summary>
  /// Implements a data portal proxy to relay data portal
  /// calls to a remote application server by using http.
  /// </summary>
  public class HttpCompressionProxy : HttpProxy
  {
    /// <summary>
    /// Gets an HttpClientHandler for use
    /// in initializing the HttpClient instance.
    /// </summary>
    protected override HttpClientHandler GetHttpClientHandler()
    {
      var result = new HttpClientHandler();
      if (!UseTextSerialization)
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

    private class CompressionWebClient : WebClient
    {
      private int Timeout { get; set; }

      public CompressionWebClient(int timeout)
      {
        Timeout = timeout;
      }

      protected override WebRequest GetWebRequest(Uri address)
      {
        var req = base.GetWebRequest(address) as HttpWebRequest;
        req.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
        if (Timeout > 0)
          req.Timeout = Timeout;
        return req;
      }
    }
  }
}
