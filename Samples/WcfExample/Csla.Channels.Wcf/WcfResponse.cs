//-----------------------------------------------------------------------
// <copyright file="WcfResponse.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Provides consistent context information between the client</summary>
//-----------------------------------------------------------------------

using System.Runtime.Serialization;

namespace Csla.Channels.Wcf
{
  /// <summary>
  /// Represents a response message from a data portal operation that is returned by the data portal server through a 
  /// WCF channel.
  /// </summary>
  [DataContract]
  public class WcfResponse
  {
    /// <summary>
    /// Gets or sets the body of the response that contains the result of the data portal operation that was called.
    /// </summary>
    [DataMember]
    public byte[] Body { get; set; } = [];
  }
}
