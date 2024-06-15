//-----------------------------------------------------------------------
// <copyright file="TypeMap.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Maps a type to a serializer type.</summary>
//-----------------------------------------------------------------------

namespace Csla.Serialization.Mobile;

/// <inheritdoc />
public class TypeMap<T, S> : ITypeMap
  where S : IMobileSerializer
{
  /// <summary>
  /// Creates an instance of the class.
  /// </summary>
  public TypeMap()
  {
    CanSerialize = (t) => t == OriginalType;
  }

  /// <summary>
  /// Creates an instance of the class.
  /// </summary>
  /// <param name="canSerialize">A function that determines if the type can be serialized.</param>
  public TypeMap(Func<Type, bool> canSerialize)
  {
    CanSerialize = canSerialize;
  }

  /// <inheritdoc />
  public Type OriginalType => typeof(T);
  /// <inheritdoc />
  public Type SerializerType => typeof(S);
  /// <inheritdoc />
  public Func<Type, bool> CanSerialize { get; set; }
}
