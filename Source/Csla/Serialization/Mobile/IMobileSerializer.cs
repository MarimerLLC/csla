﻿//-----------------------------------------------------------------------
// <copyright file="IMobileSerializer.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Custom serializer for a type</summary>
//-----------------------------------------------------------------------
using System.Text;
using Csla.Core;

namespace Csla.Serialization.Mobile
{
  /// <summary>
  /// Custom serializer for a type
  /// </summary>
  public interface IMobileSerializer
  {
#if NET8_0_OR_GREATER
    /// <summary>
    /// Gets a value indicating whether this serializer can
    /// serialize the specified type.
    /// </summary>
    /// <param name="type">Type to check</param>
    static bool CanSerialize(Type type) => false;
#endif
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
