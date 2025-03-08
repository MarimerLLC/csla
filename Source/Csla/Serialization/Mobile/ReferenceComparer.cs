//-----------------------------------------------------------------------
// <copyright file="ReferenceComparer.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implements an equality comparer for <see cref="IMobileObject" /> that compares</summary>
//-----------------------------------------------------------------------
namespace Csla.Serialization.Mobile;

/// <summary>
/// Implements an equality comparer for <see cref="IMobileObject" /> that compares
/// the objects only on the basis is the reference value.
/// </summary>
/// <typeparam name="T">Type of objects to compare.</typeparam>
public sealed class ReferenceComparer<T> : IEqualityComparer<T>
{
  /// <inheritdoc />
  public bool Equals(T? x, T? y)
  {
    return ReferenceEquals(x, y);
  }

  /// <inheritdoc />
  public int GetHashCode(T obj)
  {
    if (obj == null)
      throw new ArgumentNullException(nameof(obj));

    return obj.GetHashCode();
  }
}