using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Linq
{
  /// <summary>
  /// Implemented by objects that support range
  /// comparisons.
  /// </summary>
  /// <typeparam name="T">Type of object.</typeparam>
  public interface IRangeTestableIndex<T> : IIndex<T>
  {
    /// <summary>
    /// Implements a less than clause.
    /// </summary>
    /// <param name="pivotVal">Pivot value.</param>
    IEnumerable<T> WhereLessThan(object pivotVal);
    /// <summary>
    /// Implements a greater than clause.
    /// </summary>
    /// <param name="pivotVal">Pivot value.</param>
    IEnumerable<T> WhereGreaterThan(object pivotVal);
    /// <summary>
    /// Implements a less than or equal clause.
    /// </summary>
    /// <param name="pivotVal">Pivot value.</param>
    IEnumerable<T> WhereLessThanOrEqualTo(object pivotVal);
    /// <summary>
    /// Implements a greater than or equal clause.
    /// </summary>
    /// <param name="pivotVal">Pivot value.</param>
    IEnumerable<T> WhereGreaterThanOrEqualTo(object pivotVal);
  }
}
