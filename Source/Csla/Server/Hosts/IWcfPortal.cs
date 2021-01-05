#if !NETSTANDARD2_0 && !NET5_0
//-----------------------------------------------------------------------
// <copyright file="IWcfPortal.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Defines the service contract for the WCF data</summary>
//-----------------------------------------------------------------------
using System.ServiceModel;
using Csla.Server.Hosts.WcfChannel;
using System.Threading.Tasks;

namespace Csla.Server.Hosts
{
  /// <summary>
  /// Defines the service contract for the WCF data
  /// portal.
  /// </summary>
  [ServiceContract(Namespace="http://ws.lhotka.net/WcfDataPortal")]
  public interface IWcfPortal
  {
    /// <summary>
    /// Create a new business object.
    /// </summary>
    /// <param name="request">The request parameter object.</param>
    [OperationContract]
    [UseNetDataContract]
    Task<WcfResponse> Create(CreateRequest request);
    /// <summary>
    /// Get an existing business object.
    /// </summary>
    /// <param name="request">The request parameter object.</param>
    [OperationContract]
    [UseNetDataContract]
    Task<WcfResponse> Fetch(FetchRequest request);
    /// <summary>
    /// Update a business object.
    /// </summary>
    /// <param name="request">The request parameter object.</param>
    [OperationContract]
    [UseNetDataContract]
    Task<WcfResponse> Update(UpdateRequest request);
    /// <summary>
    /// Delete a business object.
    /// </summary>
    /// <param name="request">The request parameter object.</param>
    [OperationContract]
    [UseNetDataContract]
    Task<WcfResponse> Delete(DeleteRequest request);
  }
}
#endif