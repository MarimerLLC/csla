﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Csla.Server;

namespace ExtendableWcfPortalForDotNet
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

    /// <summary>
    /// Serialized client culture.
    /// </summary>
    /// <value>The client culture.</value>
    [DataMember]
    public string ClientCulture { get; set; }

    /// <summary>
    /// Serialized client UI culture.
    /// </summary>
    /// <value>The client UI culture.</value>
    [DataMember]
    public string ClientUICulture { get; set; }

  }
}
