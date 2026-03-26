//-----------------------------------------------------------------------
// <copyright file="WcfPortalOptions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Provides consistent context information between the client</summary>
//-----------------------------------------------------------------------

#if NETFRAMEWORK
using System.ServiceModel;
using System.ServiceModel.Channels;
#else
using CoreWCF;
using CoreWCF.Channels;
#endif

namespace Csla.Channels.Wcf.Server
{
  /// <summary>
  /// Represents options that are used to configure a WCF data portal server.
  /// </summary>
  public class WcfPortalOptions
  {
    /// <summary>
    /// Gets or sets the WCF binding that should be used to host the remote WCF data portal service.
    /// </summary>
    public Binding Binding { get; set; } = new BasicHttpBinding();

    /// <summary>
    /// Gets or sets the URL that should be used to host the remote WCF data portal service.
    /// </summary>
    public string DataPortalUrl { get; set; }
#if NETFRAMEWORK
      = "http://localhost";
#else
      ="/";
#endif
  }
}
