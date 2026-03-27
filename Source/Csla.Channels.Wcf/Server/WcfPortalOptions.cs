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

    // The Binding property for server configuration will be System.ServiceModel.Channels.Binding for .NET Framework
    // target frameworks or CoreWCF.Channels.Binding for other target frameworks. However, the client binding is always
    // System.ServiceModel.Channels.Binding which means the binding used for routing must be explicitly set in a
    // different property for modern .net targets.
#if NETFRAMEWORK
    internal Binding? RouterBinding => Binding;
#else
    /// <summary>
    /// <para>
    /// Gets or sets the WCF client binding that will be used to route calls to other data portal services.
    /// </para>
    /// <para>
    /// This property is <see langword="null"/> by default, but is required in order to enable routing.
    /// </para>
    /// </summary>
    public System.ServiceModel.Channels.Binding? RouterBinding { get; set; }
#endif
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
