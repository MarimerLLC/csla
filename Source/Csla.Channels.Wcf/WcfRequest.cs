//-----------------------------------------------------------------------
// <copyright file="WcfRequest.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Provides consistent context information between the client</summary>
//-----------------------------------------------------------------------

using System.Runtime.Serialization;
using Csla.Core;

namespace Csla.Channels.Wcf
{
  /// <summary>
  /// Represents a request message that is used to invoke a remote data portal operation through a WCF channel.
  /// </summary>
  [DataContract]
  public class WcfRequest : MobileObject
  {
    /// <summary>
    /// Gets or sets the name of the data portal operation to invoke.
    /// </summary>
    [DataMember]
    public string Operation { get; set; } = "";

    /// <summary>
    /// Gets or sets the request body that contains the criteria for the data portal operation.
    /// </summary>
    [DataMember]
    public byte[] Body { get; set; } = [];
  }
}
