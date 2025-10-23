//-----------------------------------------------------------------------
// <copyright file="IMobileObjectMetastate.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Interface for types that can serialize their metastate.</summary>
//-----------------------------------------------------------------------

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
    /// <returns>Byte array containing the serialized field values.</returns>
    byte[] GetMetastate();

    /// <summary>
    /// Deserializes field values from a byte array into the object.
    /// </summary>
    /// <param name="metastate">Byte array containing the serialized field values.</param>
    void SetMetastate(byte[] metastate);

    /// <summary>
    /// Serializes the object's field values for undo purposes into a byte array.
    /// </summary>
    /// <returns>Byte array containing the serialized field values for undo.</returns>
    byte[] GetUndoMetastate();

    /// <summary>
    /// Deserializes undo field values from a byte array into the object.
    /// </summary>
    /// <param name="metastate">Byte array containing the serialized undo field values.</param>
    void SetUndoMetastate(byte[] metastate);
  }
}
