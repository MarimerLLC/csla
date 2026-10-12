//-----------------------------------------------------------------------
// <copyright file="EquatableArray.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Immutable array with value equality for incremental generator models</summary>
//-----------------------------------------------------------------------

using System.Collections;

namespace Csla.Generator.AutoImplementProperties.CSharp
{
  /// <summary>
  /// An immutable array with value (sequence) equality, so incremental
  /// generator models that contain collections compare correctly and
  /// the pipeline can cache unchanged results.
  /// </summary>
  internal readonly struct EquatableArray<T> : IEquatable<EquatableArray<T>>, IReadOnlyList<T>
    where T : IEquatable<T>
  {
    public static readonly EquatableArray<T> Empty = new([]);

    private readonly T[]? _array;

    public EquatableArray(T[] array)
    {
      _array = array;
    }

    public EquatableArray(IEnumerable<T> items)
    {
      _array = items.ToArray();
    }

    public T this[int index] => (_array ?? [])[index];

    public int Count => _array?.Length ?? 0;

    public bool IsEmpty => Count == 0;

    public bool Equals(EquatableArray<T> other)
    {
      var left = _array ?? [];
      var right = other._array ?? [];
      if (left.Length != right.Length)
        return false;
      for (var i = 0; i < left.Length; i++)
      {
        if (!EqualityComparer<T>.Default.Equals(left[i], right[i]))
          return false;
      }
      return true;
    }

    public override bool Equals(object? obj) => obj is EquatableArray<T> other && Equals(other);

    public override int GetHashCode()
    {
      var hash = new HashCode();
      foreach (var item in _array ?? [])
        hash.Add(item);
      return hash.ToHashCode();
    }

    public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)(_array ?? [])).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public static bool operator ==(EquatableArray<T> left, EquatableArray<T> right) => left.Equals(right);

    public static bool operator !=(EquatableArray<T> left, EquatableArray<T> right) => !left.Equals(right);
  }
}
