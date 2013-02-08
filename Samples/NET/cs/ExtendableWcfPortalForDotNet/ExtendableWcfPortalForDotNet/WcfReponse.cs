using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Csla.Serialization;

namespace ExtendableWcfPortalForDotNet
{
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
    public byte[] Error { get; set; }
    /// <summary>
    /// Serialized global context data.
    /// </summary>
    [DataMember]
    public byte[] GlobalContext { get; set; }

    public WcfResponse(byte[] objectData, byte[] error, byte[] globalContext)
    {
      ObjectData = objectData;
      Error = error;
      GlobalContext = globalContext;
    }
    public WcfResponse() { }
  }
}
