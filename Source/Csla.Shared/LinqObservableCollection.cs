//-----------------------------------------------------------------------
// <copyright file="LinqObservableCollection.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Synchronized view over a source list, </summary>
//-----------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Expressions;

namespace Csla
{
  /// <summary>
  /// Synchronized view over a source list, 
  /// filtered, sorted and ordered based
  /// on a query result.
  /// </summary>
  /// <typeparam name="T">Type of objects contained in the list/collection.</typeparam>
  public class LinqObservableCollection<T> : IList<T>, ICollection<T>, IEnumerable<T>, 
    IList, ICollection, INotifyCollectionChanged
  {
    /// <summary>
    /// Event raised when the underlying source list is changed.
    /// </summary>
    public event System.Collections.Specialized.NotifyCollectionChangedEventHandler CollectionChanged;

    private System.Collections.ObjectModel.ObservableCollection<T> _baseCollection;
    private List<T> _filteredCollection;
    private bool _suppressEvents = false;
        /// <summary>
    /// Creates a new instance of the observable
    /// view.
    /// </summary>
    /// <param name="source">Source list containing/managing all items.</param>
    /// <param name="queryResult">Filtered query result over source list.</param>
    public LinqObservableCollection(System.Collections.ObjectModel.ObservableCollection<T> source, IEnumerable<T> queryResult)
      : this(source, queryResult.ToList())
    { }

    /// <summary>
    /// Creates a new instance of the observable
    /// view.
    /// </summary>
    /// <param name="source">Source list containing/managing all items.</param>
    /// <param name="queryResult">Filtered query result over source list.</param>
    public LinqObservableCollection(System.Collections.ObjectModel.ObservableCollection<T> source, List<T> queryResult)
    {
      _filteredCollection = queryResult;
      _baseCollection = source;
      _baseCollection.CollectionChanged += (o, e) =>
        {
          if (!_suppressEvents)
          {
            NotifyCollectionChangedEventArgs newE = null;
            T item;
            int index;
            switch (e.Action)
            {
              case NotifyCollectionChangedAction.Add:
                item = (T)e.NewItems[0];
                index = e.NewStartingIndex;
                if (index > _filteredCollection.Count)
                  index = _filteredCollection.Count;
                _filteredCollection.Insert(index, item);
                newE = new NotifyCollectionChangedEventArgs(
                  e.Action, item, _filteredCollection.IndexOf(item));
                break;
              case NotifyCollectionChangedAction.Remove:
                item = (T)e.OldItems[0];
                index = _filteredCollection.IndexOf(item);
                if (index > -1)
                {
                  _filteredCollection.Remove(item);
                  newE = new NotifyCollectionChangedEventArgs(e.Action, item, index);
                }
                break;
              case NotifyCollectionChangedAction.Replace:
                index = _filteredCollection.IndexOf((T)e.OldItems[0]);
                if (index > -1)
                {
                  _filteredCollection[index] = (T)e.NewItems[0];
                  newE = new NotifyCollectionChangedEventArgs(
                    e.Action,
                    e.NewItems, e.OldItems, index);
                }
                break;
              case NotifyCollectionChangedAction.Reset:
                if (source.Count == 0)
                {
                  _filteredCollection.Clear();
                  newE = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
                }
                break;
              default:
                break;
            }
            if (newE != null)
              OnCollectionChanged(newE);
          }
        };
    }

    /// <summary>
    /// Raises the CollectionChanged event.
    /// </summary>
    /// <param name="e">EventArgs for event.</param>
    protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
      if (!_suppressEvents && CollectionChanged != null)
        CollectionChanged(this, e);
    }

    /// <summary>
    /// Gets the source list (usually
    /// a BusinessListBase) object that
    /// contains/manages all data items.
    /// </summary>
    public IList<T> Source
    {
      get { return _baseCollection; }
    }

    /// <summary>
    /// Gets the query result object used to
    /// filter the view.
    /// </summary>
    public List<T> QueryResult
    {
      get { return _filteredCollection; }
    }

    /// <summary>
    /// Gets the positional index of the item.
    /// </summary>
    /// <param name="item">Item to find.</param>
    public int IndexOf(T item)
    {
      return _filteredCollection.IndexOf(item);
    }

    /// <summary>
    /// Inserts item into specified position in list.
    /// </summary>
    /// <param name="index">Location to insert item.</param>
    /// <param name="item">Item to insert.</param>
    public void Insert(int index, T item)
    {
      _baseCollection.Insert(index, item);
    }

    /// <summary>
    /// Removes item at specified index.
    /// </summary>
    /// <param name="index">Index of item to remove.</param>
    public void RemoveAt(int index)
    {
      _baseCollection.Remove(_filteredCollection[index]);
    }

    /// <summary>
    /// Gets or sets the value at the specified
    /// index.
    /// </summary>
    /// <param name="index">Index location.</param>
    public T this[int index]
    {
      get
      {
        return _filteredCollection[index];
      }
      set
      {
        var idx = _baseCollection.IndexOf(_filteredCollection[index]);
        _baseCollection[idx] = value;
      }
    }

    /// <summary>
    /// Adds an item to the end of the list.
    /// </summary>
    /// <param name="item">Item to add.</param>
    public void Add(T item)
    {
      _baseCollection.Add(item);
    }

    /// <summary>
    /// Clears the list.
    /// </summary>
    /// <remarks>
    /// Items in the LinqObservableCollection are cleared,
    /// and are also removed from the source list. Other
    /// items in the source list are unaffected.
    /// </remarks>
    public void Clear()
    {
      _suppressEvents = true;
      foreach (var item in _filteredCollection)
        _baseCollection.Remove(item);
      _suppressEvents = false;
      _filteredCollection.Clear();
      OnCollectionChanged(
        new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

    /// <summary>
    /// Gets a value indicating whether the list
    /// contains the specified item.
    /// </summary>
    /// <param name="item">Item to find.</param>
    public bool Contains(T item)
    {
      return _filteredCollection.Contains(item);
    }

    /// <summary>
    /// Copies the contents of the list to an array.
    /// </summary>
    /// <param name="array">Target array.</param>
    /// <param name="arrayIndex">Index in array where copying begins.</param>
    public void CopyTo(T[] array, int arrayIndex)
    {
      _filteredCollection.CopyTo(array, arrayIndex);
    }

    /// <summary>
    /// Gets the number of items in the list.
    /// </summary>
    public int Count
    {
      get { return _filteredCollection.Count; }
    }

    /// <summary>
    /// Gets a value indicating whether the source
    /// list is read-only.
    /// </summary>
    public bool IsReadOnly
    {
      get 
      {
        var obj = _baseCollection as ICollection<T>;
        if (obj != null)
          return obj.IsReadOnly;
        else
          return false; 
      }
    }

    /// <summary>
    /// Removes specified item from the list.
    /// </summary>
    /// <param name="item">Item to remove.</param>
    public bool Remove(T item)
    {
      return _baseCollection.Remove(item);
    }

    /// <summary>
    /// Gets an enumerator for the list.
    /// </summary>
    public IEnumerator<T> GetEnumerator()
    {
      return _filteredCollection.GetEnumerator();
    }

    /// <summary>
    /// Gets an enumerator for the list.
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator()
    {
      return _filteredCollection.GetEnumerator();
    }

    /// <summary>
    /// Adds an item to the end of the list.
    /// </summary>
    /// <param name="value">Item to add.</param>
    public int Add(object value)
    {
      return ((IList)_baseCollection).Add(value);
    }

    /// <summary>
    /// Gets a value indicating whether the list
    /// contains the specified item.
    /// </summary>
    /// <param name="value">Item to find.</param>
    public bool Contains(object value)
    {
      return _filteredCollection.Contains((T)value);
    }

    /// <summary>
    /// Gets the positional index of the item.
    /// </summary>
    /// <param name="value">Item to find.</param>
    public int IndexOf(object value)
    {
      return _filteredCollection.IndexOf((T)value);
    }

    /// <summary>
    /// Inserts item into specified position in list.
    /// </summary>
    /// <param name="index">Location to insert item.</param>
    /// <param name="value">Item to insert.</param>
    public void Insert(int index, object value)
    {
      _baseCollection.Insert(index, (T)value);
    }

    /// <summary>
    /// Gets a value indicating whether the source
    /// list has a fixed size.
    /// </summary>
    public bool IsFixedSize
    {
      get { return ((IList)_baseCollection).IsFixedSize; }
    }

    /// <summary>
    /// Removes specified item from the list.
    /// </summary>
    /// <param name="value">Item to remove.</param>
    public void Remove(object value)
    {
      _baseCollection.Remove((T)value);
    }

    object IList.this[int index]
    {
      get
      {
        return this[index];
      }
      set
      {
        this[index] = (T)value;
      }
    }

    /// <summary>
    /// Copies the contents of the list to an array.
    /// </summary>
    /// <param name="array">Target array.</param>
    /// <param name="index">Index in array where copying begins.</param>
    public void CopyTo(Array array, int index)
    {
      ((IList)_filteredCollection).CopyTo(array, index);
    }

    /// <summary>
    /// Gets a value indicating whether the source list
    /// is synchronized.
    /// </summary>
    public bool IsSynchronized
    {
      get { return ((IList)_baseCollection).IsSynchronized; }
    }

    /// <summary>
    /// Gets the SyncRoot from the source list.
    /// </summary>
    public object SyncRoot
    {
      get { return ((IList)_baseCollection).SyncRoot; }
    }
  }

  /// <summary>
  /// Extension method for implementation of LINQ methods on BusinessListBase
  /// </summary>
  public static class LinqObservableCollectionExtension
  {
    /// <summary>
    /// Gets a LinqObservableCollection that is a live view
    /// of the original list based on the query result.
    /// </summary>
    public static LinqObservableCollection<C> ToSyncList<C>(this IEnumerable<C> queryResult, System.Collections.ObjectModel.ObservableCollection<C> source)
    {
      return new LinqObservableCollection<C>(source, queryResult);
    }

    /// <summary>
    /// Gets a LinqObservableCollection that is a live view
    /// of the original list based on the query result.
    /// </summary>
    public static LinqObservableCollection<C> ToSyncList<C>(this System.Collections.ObjectModel.ObservableCollection<C> source, IEnumerable<C> queryResult)
    {
      return new LinqObservableCollection<C>(source, queryResult);
    }

    /// <summary>
    /// Gets a LinqObservableCollection that is a live view
    /// of the original list based on the query result.
    /// </summary>
    public static LinqObservableCollection<C> ToSyncList<C>(this System.Collections.ObjectModel.ObservableCollection<C> source, Expression<Func<C, bool>> expr)
    {
      IEnumerable<C> sourceEnum = source.AsEnumerable<C>();
      var output = sourceEnum.Where<C>(expr.Compile());
      return new LinqObservableCollection<C>(source, output.ToList());
    }
  }
}