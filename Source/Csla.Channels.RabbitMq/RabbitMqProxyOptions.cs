//-----------------------------------------------------------------------
// <copyright file="RabbitMqProxyOptions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Options for RabbitMqProxy</summary>
//-----------------------------------------------------------------------

namespace Csla.Channels.RabbitMq
{
  /// <summary>
  /// Options for RabbitMqProxy
  /// </summary>
  public class RabbitMqProxyOptions
  {
    /// <summary>
    /// Data portal server endpoint URL
    /// </summary>
    public string DataPortalUrl { get; set; }
  }
}
