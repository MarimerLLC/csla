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
  /// Interface for types that can round trip their non-public state - the
  /// bookkeeping an object maintains about itself, such as whether it is new,
  /// dirty or deleted - to and from a byte array representation.
  /// </summary>
  /// <remarks>
  /// <para>
  /// This exists so that a serializer written outside CSLA .NET can preserve
  /// state it has no other way to reach, without resorting to reflection over
  /// private fields. The serializer remains responsible for the object's
  /// property values; this interface covers only the state those values are
  /// wrapped in.
  /// </para>
  /// <para>
  /// The byte array is opaque. Its layout is decided entirely by the type
  /// producing it, is not a documented format, and may change between releases,
  /// so it is suitable for a single serialization round trip and not for
  /// long-term storage.
  /// </para>
  /// <para>
  /// A type contributes to its metastate by overriding
  /// <c>OnGetMetastate</c> and <c>OnSetMetastate</c>. A type with no non-public
  /// state to carry - a <see cref="CommandBase{T}"/> or a
  /// <see cref="Csla.Rules.BrokenRule"/>, for instance - overrides neither, and
  /// its metastate is legitimately an empty array.
  /// </para>
  /// </remarks>
  public interface IMobileObjectMetastate
  {
    /// <summary>
    /// Serializes the object's non-public state into a byte array.
    /// </summary>
    /// <returns>
    /// Byte array containing the serialized state, which is empty when the type
    /// has no non-public state to carry.
    /// </returns>
    byte[] GetMetastate();

    /// <summary>
    /// Restores the object's non-public state from a byte array.
    /// </summary>
    /// <param name="metastate">
    /// Byte array previously returned by <see cref="GetMetastate"/> on the same
    /// type. It is empty when that type carries no non-public state, and an
    /// empty array is accepted rather than treated as an error.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="metastate"/> is <see langword="null"/>.</exception>
    void SetMetastate(byte[] metastate);
  }
}
