using System;
using System.Runtime.Serialization;
using System.Security.Principal;
using Csla.Core;

namespace Csla.Server.Hosts.Silverlight
{
  /// <summary>
  /// Message sent to the Silverlight
  /// WCF data portal.
  /// </summary>
  [DataContract]
  public class CriteriaRequest
  {
    /// <summary>
    /// Assembly qualified name of the 
    /// business object type to create.
    /// </summary>
    [DataMember]
    public string TypeName { get; set; }
    /// <summary>
    /// Serialized data for the criteria object.
    /// </summary>
    [DataMember]
    public byte[] CriteriaData { get; set; }
    /// <summary>
    /// Serialized data for the principal object.
    /// </summary>
    [DataMember]
    public byte[] Principal { get; set; }
    /// <summary>
    /// Serialized data for the global context object.
    /// </summary>
    [DataMember]
    public byte[] GlobalContext { get; set; }
    /// <summary>
    /// Serialized data for the client context object.
    /// </summary>
    [DataMember]
    public byte[] ClientContext { get; set; }
  }
}
