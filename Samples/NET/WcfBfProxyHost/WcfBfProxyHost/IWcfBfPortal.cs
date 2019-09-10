using System;
using System.ServiceModel;
using Csla.Server.Hosts.WcfBfChannel;

namespace Csla.Server.Hosts
{
  /// <summary>
  /// Defines the service contract for the WCF data
  /// portal using the BinaryFormatter.
  /// </summary>
  [ServiceContract(Namespace = "http://ws.lhotka.net/WcfDataPortal")]
  public interface IWcfBfPortal
  {
    /// <summary>
    /// Create a new business object.
    /// </summary>
    /// <param name="request">The request parameter object.</param>
    [OperationContract]
    byte[] Create(byte[] request);
    /// <summary>
    /// Get an existing business object.
    /// </summary>
    /// <param name="request">The request parameter object.</param>
    [OperationContract]
    byte[] Fetch(byte[] request);
    /// <summary>
    /// Update a business object.
    /// </summary>
    /// <param name="request">The request parameter object.</param>
    [OperationContract]
    byte[] Update(byte[] request);
    /// <summary>
    /// Delete a business object.
    /// </summary>
    /// <param name="request">The request parameter object.</param>
    [OperationContract]
    byte[] Delete(byte[] request);
  }
}
