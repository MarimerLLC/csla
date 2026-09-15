//-----------------------------------------------------------------------
// <copyright file="HashCode.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Minimal hash code combiner for netstandard2.0</summary>
//-----------------------------------------------------------------------

namespace Csla.Generator.AutoImplementProperties.CSharp
{
  /// <summary>
  /// Minimal replacement for System.HashCode, which is not available
  /// on netstandard2.0. Keeps the generator free of extra dependencies
  /// so it ships as a single analyzer assembly.
  /// </summary>
  internal struct HashCode
  {
    private const int Seed = unchecked((int)2166136261);
    private const int Prime = 16777619;

    private int _value;
    private bool _started;

    public void Add<T>(T value)
    {
      if (!_started)
      {
        _value = Seed;
        _started = true;
      }
      _value = unchecked((_value ^ (value?.GetHashCode() ?? 0)) * Prime);
    }

    public readonly int ToHashCode() => _started ? _value : Seed;

    public static int Combine<T1>(T1 v1)
    {
      var hash = new HashCode();
      hash.Add(v1);
      return hash.ToHashCode();
    }

    public static int Combine<T1, T2>(T1 v1, T2 v2)
    {
      var hash = new HashCode();
      hash.Add(v1);
      hash.Add(v2);
      return hash.ToHashCode();
    }

    public static int Combine<T1, T2, T3>(T1 v1, T2 v2, T3 v3)
    {
      var hash = new HashCode();
      hash.Add(v1);
      hash.Add(v2);
      hash.Add(v3);
      return hash.ToHashCode();
    }

    public static int Combine<T1, T2, T3, T4>(T1 v1, T2 v2, T3 v3, T4 v4)
    {
      var hash = new HashCode();
      hash.Add(v1);
      hash.Add(v2);
      hash.Add(v3);
      hash.Add(v4);
      return hash.ToHashCode();
    }

    public static int Combine<T1, T2, T3, T4, T5>(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5)
    {
      var hash = new HashCode();
      hash.Add(v1);
      hash.Add(v2);
      hash.Add(v3);
      hash.Add(v4);
      hash.Add(v5);
      return hash.ToHashCode();
    }

    public static int Combine<T1, T2, T3, T4, T5, T6>(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6)
    {
      var hash = new HashCode();
      hash.Add(v1);
      hash.Add(v2);
      hash.Add(v3);
      hash.Add(v4);
      hash.Add(v5);
      hash.Add(v6);
      return hash.ToHashCode();
    }

    public static int Combine<T1, T2, T3, T4, T5, T6, T7>(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6, T7 v7)
    {
      var hash = new HashCode();
      hash.Add(v1);
      hash.Add(v2);
      hash.Add(v3);
      hash.Add(v4);
      hash.Add(v5);
      hash.Add(v6);
      hash.Add(v7);
      return hash.ToHashCode();
    }
  }
}
