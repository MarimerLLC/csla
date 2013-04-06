using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Csla.Linq
{
  /// <summary>
  /// Interface that defines a what a set of indexes should do
  /// </summary>
  public interface IIndexSet<T>
  {
    /// <summary>
    /// Insert an item into all indexes
    /// </summary>
    void InsertItem(T item);
    /// <summary>
    /// Insert an item into the index for the given property
    /// </summary>
    void InsertItem(T item, string property);
    /// <summary>
    /// Remove an item from all indexes
    /// </summary>
    void RemoveItem(T item);
    /// <summary>
    /// Remove an item from the index for the given property
    /// </summary>
    void RemoveItem(T item, string property);
    /// <summary>
    /// Reindex the item for all indexes
    /// </summary>
    void ReIndexItem(T item);
    /// <summary>
    /// Reindex the item in the index for a given property
    /// </summary>
    void ReIndexItem(T item, string property);
    /// <summary>
    /// Clear all the indexes
    /// </summary>
    void ClearIndexes();
    /// <summary>
    /// Clear the index for a given property
    /// </summary>
    void ClearIndex(string property);
    /// <summary>
    /// Search for items using a given index and a given expression
    /// </summary>
    IEnumerable<T> Search(Expression<Func<T, bool>> expr, string property);
    /// <summary>
    /// Determine whether there is an index for a given property present
    /// </summary>
    bool HasIndexFor(string property);
    /// <summary>
    /// Determine whether the index set has an index that enables search for a given expression
    /// </summary>
    string HasIndexFor(Expression<Func<T, bool>> expr);
    /// <summary>
    /// Return an index based on an indexer using a property name
    /// </summary>
    IIndex<T> this [string property] { get; }
    /// <summary>
    /// Tell the index set that it is time to allow for loading of an on demand index
    /// </summary>
    void LoadIndex(string property);
  }
}
