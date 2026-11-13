//-----------------------------------------------------------------------
// <copyright file="RegisteredTypeMap.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Maps a type to a serializer type from an assembly registration</summary>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace Csla.Serialization.Mobile;

/// <summary>
/// Maps exactly one type to a serializer type, as declared by a
/// <see cref="MobileSerializerRegistrationAttribute"/>.
/// </summary>
internal sealed class RegisteredTypeMap : ITypeMap
{
  public RegisteredTypeMap(Type type, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type serializerType)
  {
    Type = type ?? throw new ArgumentNullException(nameof(type));
    SerializerType = serializerType ?? throw new ArgumentNullException(nameof(serializerType));
    CanSerialize = t => t == Type;
  }

  /// <summary>
  /// Gets the type that is serialized.
  /// </summary>
  public Type Type { get; }

  /// <inheritdoc />
  [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
  public Type SerializerType { get; }

  /// <inheritdoc />
  public Func<Type, bool> CanSerialize { get; }
}
