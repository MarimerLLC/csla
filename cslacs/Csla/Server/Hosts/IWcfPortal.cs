using System;
using System.ServiceModel;
using Csla.Server.Hosts.WcfChannel;

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
    WcfResponse Create(CreateRequest request);
    /// <summary>
    /// Get an existing business object.
    /// </summary>
    /// <param name="request">The request parameter object.</param>
    [OperationContract]
    [UseNetDataContract]
    WcfResponse Fetch(FetchRequest request);
    /// <summary>
    /// Update a business object.
    /// </summary>
    /// <param name="request">The request parameter object.</param>
    [OperationContract]
    [UseNetDataContract]
    WcfResponse Update(UpdateRequest request);
    /// <summary>
    /// Delete a business object.
    /// </summary>
    /// <param name="request">The request parameter object.</param>
    [OperationContract]
    [UseNetDataContract]
    WcfResponse Delete(DeleteRequest request);
  }
}
