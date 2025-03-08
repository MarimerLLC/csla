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
      Guard.NotNull(source);
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
      Guard.NotNull(source);

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
      Guard.NotNull(source);

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
      Guard.NotNull(source);
      Guard.NotNull(factory);

      source.HttpClientFactory = factory;
      return source;
    }

    /// <summary>
    /// Configures the <see cref="HttpProxyOptions.Timeout"/>.
    /// </summary>
    /// <param name="source">The options object.</param>
    /// <param name="timeout">The timeout value to set.</param>
    /// <returns>The options object.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
    public static HttpProxyOptions WithTimeout(this HttpProxyOptions source, TimeSpan timeout)
    {
      Guard.NotNull(source);

      source.Timeout = timeout;
      return source;
    }

    /// <summary>
    /// Configures the <see cref="HttpProxyOptions.ReadWriteTimeout"/>.
    /// </summary>
    /// <param name="source">The options object.</param>
    /// <param name="readwriteTimeout">The read-write timeout value to set.</param>
    /// <returns>The options object.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
    public static HttpProxyOptions WithReadWriteTimeout(this HttpProxyOptions source, TimeSpan readwriteTimeout)
    {
      Guard.NotNull(source);

      source.ReadWriteTimeout = readwriteTimeout;
      return source;
    }

  }
}