#if !NETFX_CORE
//-----------------------------------------------------------------------
// <copyright file="IWcfPortal.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Defines the service contract for the WCF data</summary>
//-----------------------------------------------------------------------
using System;
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
#if NET40
    /// <summary>
    /// Create a new business object.
    /// </summary>
    /// <param name="request">The request parameter object.</param>
    [OperationContract]
    WcfResponse Create(CriteriaRequest request);
    /// <summary>
    /// Get an existing business object.
    /// </summary>
    /// <param name="request">The request parameter object.</param>
    [OperationContract]
    WcfResponse Fetch(CriteriaRequest request);
    /// <summary>
    /// Update a business object.
    /// </summary>
    /// <param name="request">The request parameter object.</param>
    [OperationContract]
    WcfResponse Update(UpdateRequest request);
    /// <summary>
    /// Delete a business object.
    /// </summary>
    /// <param name="request">The request parameter object.</param>
    [OperationContract]
    WcfResponse Delete(CriteriaRequest request);
#else
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
#endif
  }
}
#endif