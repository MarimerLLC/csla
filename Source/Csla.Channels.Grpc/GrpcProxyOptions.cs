//-----------------------------------------------------------------------
// <copyright file="GrpcProxyOptions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Options for GrpcProxy</summary>
//-----------------------------------------------------------------------

namespace Csla.Channels.Grpc
{
  /// <summary>
  /// Options for GrpcProxy
  /// </summary>
  public class GrpcProxyOptions
  {
    /// <summary>
    /// Data portal server endpoint URL
    /// </summary>
    public string DataPortalUrl { get; set; }
  }
}
