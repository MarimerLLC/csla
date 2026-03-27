//-----------------------------------------------------------------------
// <copyright file="WcfPortalClient.cs" company="Marimer LLC">
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
  /// Represents a WCF client that is used by the <see cref="WcfProxy"/> to communicate with a remote data portal.
  /// </summary>
  /// <param name="binding">
  /// The WCF binding that is used to communicate with the remote WCF data portal service.
  /// </param>
  /// <param name="address">
  /// The endpoint address that is used to communicate with the remote WCF data portal service.
  /// </param>
  internal class WcfPortalClient(Binding binding, EndpointAddress address) : ClientBase<IWcfPortal>(binding, address), IWcfPortal
  {
    /// <summary>
    /// Asynchronously invokes an operation on the remote data portal.
    /// </summary>
    /// <param name="request">
    /// The request that contains the name and parameters necessary to invoke the data portal operation.
    /// </param>
    /// <returns>
    /// A task containing the response from the remote data portal.
    /// </returns>
    public Task<WcfResponse> InvokeAsync(WcfRequest request) => Channel.InvokeAsync(request);

    /// <summary>
    /// Synchronously invokes an operation on the remote data portal.
    /// </summary>
    /// <param name="request">
    /// The request that contains the name and parameters necessary to invoke the data portal operation.
    /// </param>
    /// <returns>
    /// The response from the remote data portal.
    /// </returns>
    public WcfResponse Invoke(WcfRequest request) => Channel.Invoke(request);
  }
}
