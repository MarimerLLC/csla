//-----------------------------------------------------------------------
// <copyright file="IWcfPortal.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Provides consistent context information between the client</summary>
//-----------------------------------------------------------------------

using System.ServiceModel;

namespace Csla.Channels.Wcf.Client
{
  /// <summary>
  /// Represents the WCF service contract that is used for communication between the data portal client and server.
  /// </summary>
  [ServiceContract]
  public interface IWcfPortal
  {
    /// <summary>
    /// Asynchronously invokes an operation on the remote data portal.
    /// </summary>
    /// <param name="request">
    /// The request that contains the name and parameters necessary to invoke the data portal operation.
    /// </param>
    /// <returns>
    /// As task containing the response from the remote data portal.
    /// </returns>
    [OperationContract(Action = "https://cslanet.com/IwcfPortal/Invoke", ReplyAction = "https://cslanet.com/IwcfPortal/InvokeResponse")]
    Task<WcfResponse> InvokeAsync(WcfRequest request);

    /// <summary>
    /// Synchronously invokes an operation on the remote data portal.
    /// </summary>
    /// <param name="request">
    /// The request that contains the name and parameters necessary to invoke the data portal operation.
    /// </param>
    /// <returns>
    /// The response from the remote data portal.
    /// </returns>
    [OperationContract(Action = "https://cslanet.com/IwcfPortal/Invoke", ReplyAction = "https://cslanet.com/IwcfPortal/InvokeResponse")]
    WcfResponse Invoke(WcfRequest request);
  }
}
