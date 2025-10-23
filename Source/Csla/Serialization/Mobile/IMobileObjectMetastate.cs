//-----------------------------------------------------------------------
// <copyright file="IMobileObjectMetastate.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Interface for types that can serialize their metastate.</summary>
//-----------------------------------------------------------------------

using Csla.Core;

namespace Csla.Serialization.Mobile
{
  /// <summary>
  /// Interface for types that can serialize their metastate
  /// (field values) to and from a byte array representation.
  /// </summary>
  /// <remarks>
  /// This interface provides a lightweight serialization mechanism
  /// that captures the same field values as OnGetState/OnSetState
  /// without including child object references. It is intended for
  /// transitory data serialization scenarios, not long-term storage.
  /// </remarks>
  public interface IMobileObjectMetastate
  {
    /// <summary>
    /// Serializes the object's field values into a byte array.
    /// </summary>
    /// <param name="mode">The state mode indicating the purpose of serialization.</param>
    /// <returns>Byte array containing the serialized field values.</returns>
    byte[] GetMetastate(StateMode mode);

    /// <summary>
    /// Deserializes field values from a byte array into the object.
    /// </summary>
    /// <param name="metastate">Byte array containing the serialized field values.</param>
    /// <param name="mode">The state mode indicating the purpose of deserialization.</param>
    void SetMetastate(byte[] metastate, StateMode mode);
  }
}
