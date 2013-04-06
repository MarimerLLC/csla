using System;
using System.Runtime.Serialization;

namespace Csla.Server.Hosts.Silverlight
{
  /// <summary>
  /// Response message returned from the 
  /// Silverlight WCF data portal methods.
  /// </summary>
  [DataContract]
  public class WcfResponse
  {
    /// <summary>
    /// Serialized object data.
    /// </summary>
    [DataMember]
    public byte[] ObjectData { get; set; }
    /// <summary>
    /// Serialized error/exception data.
    /// </summary>
    [DataMember]
    public WcfErrorInfo ErrorData { get; set; }
    /// <summary>
    /// Serialized global context data.
    /// </summary>
    [DataMember]
    public byte[] GlobalContext { get; set; }
  }
}
