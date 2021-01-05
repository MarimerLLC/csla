#if !NETSTANDARD2_0 && !NET5_0
//-----------------------------------------------------------------------
// <copyright file="IWcfPortal.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Defines the service contract for the WCF data</summary>
//-----------------------------------------------------------------------
using System.ServiceModel;
using System.Threading.Tasks;

namespace Csla.Server.Hosts.Mobile
{
  /// <summary>
  /// Defines the service contract for the WCF data
  /// portal.
  /// </summary>
  [ServiceContract(Namespace = "http://ws.lhotka.net/WcfDataPortal")]
  public interface IWcfPortal
  {
    /// <summary>
    /// Create a new business object.
    /// </summary>
    /// <param name="request">The request parameter object.</param>
    [OperationContract]
    Task<WcfResponse> Create(CriteriaRequest request);
    /// <summary>
    /// Get an existing business object.
    /// </summary>
    /// <param name="request">The request parameter object.</param>
    [OperationContract]
    Task<WcfResponse> Fetch(CriteriaRequest request);
    /// <summary>
    /// Update a business object.
    /// </summary>
    /// <param name="request">The request parameter object.</param>
    [OperationContract]
    Task<WcfResponse> Update(UpdateRequest request);
    /// <summary>
    /// Delete a business object.
    /// </summary>
    /// <param name="request">The request parameter object.</param>
    [OperationContract]
    Task<WcfResponse> Delete(CriteriaRequest request);
  }
}
#endif