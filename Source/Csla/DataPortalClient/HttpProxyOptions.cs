#nullable enable
//-----------------------------------------------------------------------
// <copyright file="HttpProxyOptions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Options for HttpProxy</summary>
//-----------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
#if !NETSTANDARD2_0 && !NET8_0_OR_GREATER
using System.Net.Http;
#endif

namespace Csla.Channels.Http
{
  /// <summary>
  /// Options for HttpProxy
  /// </summary>
  public class HttpProxyOptions
  {
    /// <summary>
    /// Data portal server endpoint URL
    /// </summary>
    public string DataPortalUrl { get; set; } = "";

    /// <summary>
    /// Gets or sets a value indicating whether to use
    /// text/string serialization instead of the default
    /// binary serialization.
    /// </summary>
    public bool UseTextSerialization { get; set; }

    /// <summary>
    /// Gets or sets the factory to obtain an <see cref="HttpClient"/> to make server calls.
    /// </summary>
    public Func<IServiceProvider, HttpClient> HttpClientFactory { get; set; } = DefaultHttpFactory;

    /// <summary>
    /// Default <see cref="HttpClientFactory"/> implementation.
    /// </summary>
    /// <param name="serviceProvider">The service provider to obtain the <see cref="HttpClient"/>.</param>
    /// <returns>The <see cref="HttpClient"/>.</returns>
    private static HttpClient DefaultHttpFactory(IServiceProvider serviceProvider)
    {
      return serviceProvider.GetRequiredService<HttpClient>();
    }
  }
}