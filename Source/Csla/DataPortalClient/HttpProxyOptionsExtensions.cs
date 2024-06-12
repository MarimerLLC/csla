﻿#nullable enable
//-----------------------------------------------------------------------
// <copyright file="HttpProxyOptions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Extensions for HttpProxyOptions</summary>
//-----------------------------------------------------------------------

#if !NETSTANDARD2_0 && !NET8_0_OR_GREATER
using System.Net.Http;
#endif

namespace Csla.Channels.Http
{
  /// <summary>
  /// Provides extension methods for <see cref="HttpProxyOptions"/>.
  /// </summary>
  public static class HttpProxyOptionsExtensions
  {
    /// <summary>
    /// Configures the <see cref="HttpProxyOptions.DataPortalUrl"/>.
    /// </summary>
    /// <param name="source">The options object.</param>
    /// <param name="dataPortalUrl">The data portal server endpoint url to use.</param>
    /// <returns>The options object.</returns>
    public static HttpProxyOptions WithDataPortalUrl(this HttpProxyOptions source, string dataPortalUrl)
    {
      if (source is null)
        throw new ArgumentNullException(nameof(source));
      if (string.IsNullOrEmpty(dataPortalUrl))
        throw new ArgumentException($"'{nameof(dataPortalUrl)}' cannot be null or empty.", nameof(dataPortalUrl));

      source.DataPortalUrl = dataPortalUrl;
      return source;
    }

    /// <summary>
    /// Configures the <see cref="HttpProxyOptions.UseTextSerialization"/> to <see langword="true"/>.
    /// </summary>
    /// <param name="source">The options object.</param>
    /// <returns>The options object.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
    public static HttpProxyOptions UseTextForSerialization(this HttpProxyOptions source)
    {
      if (source is null)
        throw new ArgumentNullException(nameof(source));

      source.UseTextSerialization = true;
      return source;
    }

    /// <summary>
    /// Configures the <see cref="HttpProxyOptions.UseTextSerialization"/> to <see langword="false"/> to use the default serialization.
    /// </summary>
    /// <param name="source">The options object.</param>
    /// <returns>The options object.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
    public static HttpProxyOptions UseDefaultForSerialization(this HttpProxyOptions source)
    {
      if (source is null)
        throw new ArgumentNullException(nameof(source));

      source.UseTextSerialization = false;
      return source;
    }

    /// <summary>
    /// Configures the <see cref="HttpProxyOptions.HttpClientFactory"/> to use the provided factory instead of the default.
    /// </summary>
    /// <param name="source">The options object.</param>
    /// <param name="factory">The factory to obtain an <see cref="HttpClient"/> for use.</param>
    /// <returns>The options object.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="factory"/> is <see langword="null"/>.</exception>
    public static HttpProxyOptions WithHttpClientFactory(this HttpProxyOptions source, Func<IServiceProvider, HttpClient> factory)
    {
      if (source is null)
        throw new ArgumentNullException(nameof(source));
      if (factory is null)
        throw new ArgumentNullException(nameof(factory));

      source.HttpClientFactory = factory;
      return source;
    }
  }
}