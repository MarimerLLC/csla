//-----------------------------------------------------------------------
// <copyright file="ReferenceComparer.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implements an equality comparer for <see cref="IMobileObject" /> that compares</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Serialization.Mobile
{
  /// <summary>
  /// Implements an equality comparer for <see cref="IMobileObject" /> that compares
  /// the objects only on the basis is the reference value.
  /// </summary>
  /// <typeparam name="T">Type of objects to compare.</typeparam>
  public sealed class ReferenceComparer<T> : IEqualityComparer<T>
  {
    #region IEqualityComparer<T> Members

    /// <summary>
    /// Determines if the two objects are reference-equal.
    /// </summary>
    /// <param name="x">First object.</param>
    /// <param name="y">Second object.</param>
    public bool Equals(T x, T y)
    {
      return Object.ReferenceEquals(x, y);
    }

    /// <summary>
    /// Gets the hash code value for the object.
    /// </summary>
    /// <param name="obj">Object reference.</param>
    public int GetHashCode(T obj)
    {
      return obj.GetHashCode();
    }

    #endregion
  }
}