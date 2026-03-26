//-----------------------------------------------------------------------
// <copyright file="WcfProxyOptions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Provides consistent context information between the client</summary>
//-----------------------------------------------------------------------

using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Csla.Channels.Wcf.Client
{
  /// <summary>
  /// Represents options that are used to configure a WCF data portal proxy.
  /// </summary>
  public class WcfProxyOptions
  {
    /// <summary>
    /// Gets or sets the WCF binding that should be used to communicate with the remote WCF data portal service.
    /// </summary>
    public Binding Binding { get; set; } = new BasicHttpBinding();

    /// <summary>
    /// Gets or sets the URL that should be used to communicate with the remote WCF data portal service.
    /// </summary>
    public string DataPortalUrl { get; set; } = "http://localhost";
  }
}
