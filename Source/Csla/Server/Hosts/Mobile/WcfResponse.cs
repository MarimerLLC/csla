#if !NETFX_CORE
//-----------------------------------------------------------------------
// <copyright file="WcfResponse.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Response message returned from the </summary>
//-----------------------------------------------------------------------
using System;
using System.Runtime.Serialization;

namespace Csla.Server.Hosts.Mobile
{
  /// <summary>
  /// Response message returned from the 
  /// WCF data portal methods.
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
#endif