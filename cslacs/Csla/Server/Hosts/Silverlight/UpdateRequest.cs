using System;
using System.Runtime.Serialization;
using System.Security.Principal;

namespace Csla.Server.Hosts.Silverlight
{
  /// <summary>
  /// Message sent to the Silverlight
  /// WCF data portal.
  /// </summary>
  [DataContract]
  public class UpdateRequest
  {
    /// <summary>
    /// Serialized object data.
    /// </summary>
    [DataMember]
    public byte[] ObjectData { get; set; }

    /// <summary>
    /// Serialized principal object.
    /// </summary>
    [DataMember]
    public byte[] Principal { get; set; }

    /// <summary>
    /// Serialized global context object.
    /// </summary>
    [DataMember]
    public byte[] GlobalContext { get; set; }

    /// <summary>
    /// Serialized client context object.
    /// </summary>
    [DataMember]
    public byte[] ClientContext { get; set; }
  }
}
