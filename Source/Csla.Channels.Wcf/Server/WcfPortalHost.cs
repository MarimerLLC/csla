//-----------------------------------------------------------------------
// <copyright file="WcfPortalHost.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Provides consistent context information between the client</summary>
//-----------------------------------------------------------------------

#if NETFRAMEWORK
using Csla.Server;
using System.ServiceModel;

namespace Csla.Channels.Wcf.Server
{
  /// <summary>
  /// Represents a custom <see cref="ServiceHost"/> that is used to provide WCF configuration to the server side WCF
  /// data portal via dependency injection.
  /// </summary>
  public class WcfPortalHost : ServiceHost
  {
    /// <summary>
    /// Creates an instance of <see cref="WcfPortalHost"/>.
    /// </summary>
    /// <param name="dataPortal">
    /// The server side data portal that processes the data portal requests.
    /// </param>
    /// <param name="applicationContext">
    /// The server side context for the data portal.
    /// </param>
    /// <param name="serviceType">
    /// The type that implements the WCF service contract that is being hosted.
    /// </param>
    /// <param name="baseAddresses">
    /// The base address where the WCF service is being hosted.
    /// </param>
    public WcfPortalHost(IDataPortalServer dataPortal, ApplicationContext applicationContext, Type serviceType, params Uri[] baseAddresses)
      : base(serviceType, baseAddresses)
    {
      foreach (var cd in ImplementedContracts.Values)
      {
        cd.Behaviors.Add(new WcfPortalInstanceProvider(dataPortal, applicationContext));
      }
    }
  }
}
#endif
