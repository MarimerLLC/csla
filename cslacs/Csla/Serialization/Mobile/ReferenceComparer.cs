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
    public sealed class ReferenceComparer<T> : IEqualityComparer<T>
    {
        #region IEqualityComparer<T> Members

        /// <summary>
        /// Determines if the two objects are reference-equal.
        /// </summary>
        public bool Equals(T x, T y)
        {
            return Object.ReferenceEquals(x, y);
        }

        public int GetHashCode(T obj)
        {
            return obj.GetHashCode();
        }

        #endregion
    }
}
