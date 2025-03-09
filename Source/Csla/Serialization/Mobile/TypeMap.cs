//-----------------------------------------------------------------------
// <copyright file="TypeMap.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Maps a type to a serializer type.</summary>
//-----------------------------------------------------------------------

namespace Csla.Serialization.Mobile;

/// <inheritdoc />
/// <summary>
/// Creates an instance of the class.
/// </summary>
/// <param name="canSerialize">A function that determines if the type can be serialized.</param>
/// <exception cref="ArgumentNullException"><paramref name="canSerialize"/> is <see langword="null"/>.</exception>
public class TypeMap<T, S>(Func<Type, bool> canSerialize) : ITypeMap
  where S : IMobileSerializer
{
  /// <summary>
  /// Creates an instance of the class.
  /// </summary>
  public TypeMap()
    : this(type => type == typeof(T))
  { }

  /// <inheritdoc />
  public Type SerializerType => typeof(S);
  /// <inheritdoc />
  public Func<Type, bool> CanSerialize { get; } = canSerialize ?? throw new ArgumentNullException(nameof(canSerialize));
}
