using System;
using System.Collections.Generic;
using System.Reflection;

namespace Csla.Linq
{
  /// <summary>
  /// Interface that determines functionality of an index
  /// </summary>
  public interface IIndex<T> : ICollection<T>
  {
    /// <summary>
    /// Field this index is indexing on
    /// </summary>
    PropertyInfo IndexField { get; }
    /// <summary>
    /// Iterator that returns objects where there is a match based on the passed item
    /// </summary>
    IEnumerable<T> WhereEqual(T item);
    /// <summary>
    /// Iterator that returns objects based on the expression and the value passed in
    /// </summary>
    IEnumerable<T> WhereEqual(object pivotVal, Func<T, bool> expr);
    /// <summary>
    /// Reindex an item in this index
    /// </summary>
    void ReIndex(T item);
    /// <summary>
    /// Determine whether the given index is loaded or not
    /// </summary>
    bool Loaded { get; }
    /// <summary>
    /// Set the index as not loaded anymore
    /// </summary>
    void InvalidateIndex();
    /// <summary>
    /// Set the index as as loaded
    /// </summary>
    void LoadComplete();
    /// <summary>
    /// Determine the index mode (always, ondemand, never)
    /// </summary>
    IndexModeEnum IndexMode { get; set; }
  }
}
