//-----------------------------------------------------------------------
// <copyright file="ITypeMap.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Maps a type to a serializer type.</summary>
//-----------------------------------------------------------------------

namespace Csla.Serialization.Mobile;

/// <summary>
/// Maps a type to a serializer type.
/// </summary>
public interface ITypeMap
{
  /// <summary>
  /// Gets the original type.
  /// </summary>
  Type OriginalType { get; }
  /// <summary>
  /// Gets the serializer type.
  /// </summary>
  Type SerializerType { get; }
  /// <summary>
  /// Gets a function that determines 
  /// if the type can be serialized.
  /// </summary>
  Func<Type, bool> CanSerialize { get; }
}
