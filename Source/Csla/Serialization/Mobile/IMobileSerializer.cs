//-----------------------------------------------------------------------
// <copyright file="IMobileSerializer.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Custom serializer for a type</summary>
//-----------------------------------------------------------------------
using Csla.Core;

namespace Csla.Serialization.Mobile
{
  /// <summary>
  /// Custom serializer for a type
  /// </summary>
  public interface IMobileSerializer : IUseApplicationContext
  {
    /// <summary>
    /// Serialize the object into the SerializationInfo
    /// </summary>
    /// <param name="obj">Source object</param>
    /// <param name="info">SerializationInfo instance</param>
    void Serialize(object obj, SerializationInfo info);
    /// <summary>
    /// Deserialize the object from the SerializationInfo
    /// </summary>
    /// <param name="info">SerializationInfo instance</param>
    /// <returns></returns>
    object Deserialize(SerializationInfo info);
  }
}
