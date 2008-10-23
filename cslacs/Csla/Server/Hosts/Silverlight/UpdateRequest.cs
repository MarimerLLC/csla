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

    [DataMember]
    public byte[] Principal { get; set; }

    [DataMember]
    public byte[] GlobalContext { get; set; }

    [DataMember]
    public byte[] ClientContext { get; set; }
  }
}
