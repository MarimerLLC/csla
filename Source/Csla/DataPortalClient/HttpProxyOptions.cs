//-----------------------------------------------------------------------
// <copyright file="HttpProxyOptions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Options for HttpProxy</summary>
//-----------------------------------------------------------------------

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
    public string DataPortalUrl { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether to use
    /// text/string serialization instead of the default
    /// binary serialization.
    /// </summary>
    public bool UseTextSerialization { get; set; }
  }
}
