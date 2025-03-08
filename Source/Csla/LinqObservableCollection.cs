//-----------------------------------------------------------------------
// <copyright file="LinqObservableCollection.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Synchronized view over a source list, </summary>
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Specialized;
using System.Linq.Expressions;

namespace Csla
{
  /// <summary>
  /// Synchronized view over a source list,
  /// filtered, sorted and ordered based
  /// on a query result.
  /// </summary>
  /// <typeparam name="T">Type of objects contained in the list/collection.</typeparam>
  public class LinqObservableCollection<T> :
    IList<T>,
    IList,
    INotifyCollectionChanged
    where T: notnull
  {
    /// <summary>
    /// Event raised when the underlying source list is changed.
    /// </summary>
    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    private System.Collections.ObjectModel.ObservableCollection<T> _baseCollection;
    private bool _suppressEvents = false;
    /// <summary>
    /// Creates a new instance of the observable
    /// view.
    /// </summary>
    /// <param name="source">Source list containing/managing all items.</param>
    /// <param name="queryResult">Filtered query result over source list.</param>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="queryResult"/> is <see langword="null"/>.</exception>
    public LinqObservableCollection(System.Collections.ObjectModel.ObservableCollection<T> source, IEnumerable<T> queryResult)
      : this(source, queryResult?.ToList() ?? throw new ArgumentNullException(nameof(queryResult)))
    {
    }

    /// <summary>
    /// Creates a new instance of the observable
    /// view.
    /// </summary>
    /// <param name="source">Source list containing/managing all items.</param>
    /// <param name="queryResult">Filtered query result over source list.</param>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="queryResult"/> is <see langword="null"/>.</exception>
    public LinqObservableCollection(System.Collections.ObjectModel.ObservableCollection<T> source, List<T> queryResult)
    {
      QueryResult = Guard.NotNull(queryResult);
      _baseCollection = Guard.NotNull(source);
      _baseCollection.CollectionChanged += (_, e) =>
        {
          Guard.NotNull(e);

          if (!_suppressEvents)
          {
            NotifyCollectionChangedEventArgs? newE = null;
            T item;
            int index;
            switch (e.Action)
            {
              case NotifyCollectionChangedAction.Add:
                item = (T)e.NewItems![0]!;
                index = e.NewStartingIndex;
                if (index > QueryResult.Count)
                  index = QueryResult.Count;
                QueryResult.Insert(index, item);
                newE = new NotifyCollectionChangedEventArgs(e.Action, item, QueryResult.IndexOf(item));
                break;
              case NotifyCollectionChangedAction.Remove:
                item = (T)e.OldItems![0]!;
                index = QueryResult.IndexOf(item);
                if (index > -1)
                {
                  QueryResult.Remove(item);
                  newE = new NotifyCollectionChangedEventArgs(e.Action, item, index);
                }
                break;
              case NotifyCollectionChangedAction.Replace:
                index = QueryResult.IndexOf((T)e.OldItems![0]!);
                if (index > -1)
                {
                  QueryResult[index] = (T)e.NewItems![0]!;
                  newE = new NotifyCollectionChangedEventArgs(e.Action,e.NewItems, e.OldItems, index);
                }
                break;
              case NotifyCollectionChangedAction.Reset:
                if (source.Count == 0)
                {
                  QueryResult.Clear();
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
    /// <exception cref="ArgumentNullException"><paramref name="e"/> is <see langword="null"/>.</exception>
    protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
      Guard.NotNull(e);

      if (!_suppressEvents)
        CollectionChanged?.Invoke(this, e);
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
    public List<T> QueryResult { get; }

    /// <summary>
    /// Gets the positional index of the item.
    /// </summary>
    /// <param name="item">Item to find.</param>
    /// <exception cref="ArgumentNullException"><paramref name="item"/> is <see langword="null"/>.</exception>
    public int IndexOf(T item)
    {
      if(item is null) 
        throw new ArgumentNullException(nameof(item));

      return QueryResult.IndexOf(item);
    }

    /// <summary>
    /// Inserts item into specified position in list.
    /// </summary>
    /// <param name="index">Location to insert item.</param>
    /// <param name="item">Item to insert.</param>
    /// <exception cref="ArgumentNullException"><paramref name="item"/> is <see langword="null"/>.</exception>
    public void Insert(int index, T item)
    {
      if (item is null)
        throw new ArgumentNullException(nameof(item));

      _baseCollection.Insert(index, item);
    }

    /// <summary>
    /// Removes item at specified index.
    /// </summary>
    /// <param name="index">Index of item to remove.</param>
    public void RemoveAt(int index)
    {
      _baseCollection.Remove(QueryResult[index]);
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
        return QueryResult[index];
      }
      set
      {
        var idx = _baseCollection.IndexOf(QueryResult[index]);
        _baseCollection[idx] = value ?? throw new ArgumentNullException(nameof(value));
      }
    }

    /// <summary>
    /// Adds an item to the end of the list.
    /// </summary>
    /// <param name="item">Item to add.</param>
    /// <exception cref="ArgumentNullException"><paramref name="item"/> is <see langword="null"/>.</exception>
    public void Add(T item)
    {
      if (item is null)
        throw new ArgumentNullException(nameof(item));

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
      foreach (var item in QueryResult)
        _baseCollection.Remove(item);
      _suppressEvents = false;
      QueryResult.Clear();
      OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

    /// <summary>
    /// Gets a value indicating whether the list
    /// contains the specified item.
    /// </summary>
    /// <param name="item">Item to find.</param>
    /// <exception cref="ArgumentNullException"><paramref name="item"/> is <see langword="null"/>.</exception>
    public bool Contains(T item)
    {
      if (item is null)
        throw new ArgumentNullException(nameof(item));

      return QueryResult.Contains(item);
    }

    /// <summary>
    /// Copies the contents of the list to an array.
    /// </summary>
    /// <param name="array">Target array.</param>
    /// <param name="arrayIndex">Index in array where copying begins.</param>
    /// <exception cref="ArgumentNullException"><paramref name="array"/> is <see langword="null"/>.</exception>
    public void CopyTo(T[] array, int arrayIndex)
    {
      Guard.NotNull(array);

      QueryResult.CopyTo(array, arrayIndex);
    }

    /// <summary>
    /// Gets the number of items in the list.
    /// </summary>
    public int Count
    {
      get { return QueryResult.Count; }
    }

    /// <summary>
    /// Gets a value indicating whether the source
    /// list is read-only.
    /// </summary>
    public bool IsReadOnly
    {
      get 
      {
        if (_baseCollection is ICollection<T> obj)
          return obj.IsReadOnly;
        else
          return false; 
      }
    }

    /// <summary>
    /// Removes specified item from the list.
    /// </summary>
    /// <param name="item">Item to remove.</param>
    /// <exception cref="ArgumentNullException"><paramref name="item"/> is <see langword="null"/>.</exception>
    public bool Remove(T item)
    {
      if (item is null)
        throw new ArgumentNullException(nameof(item));

      return _baseCollection.Remove(item);
    }

    /// <summary>
    /// Gets an enumerator for the list.
    /// </summary>
    public IEnumerator<T> GetEnumerator()
    {
      return QueryResult.GetEnumerator();
    }

    /// <summary>
    /// Gets an enumerator for the list.
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator()
    {
      return QueryResult.GetEnumerator();
    }

    /// <summary>
    /// Adds an item to the end of the list.
    /// </summary>
    /// <param name="value">Item to add.</param>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
    public int Add(object? value)
    {
      if (value == null) 
        throw new ArgumentNullException(nameof(value));

      return ((IList)_baseCollection).Add(value);
    }

    /// <summary>
    /// Gets a value indicating whether the list
    /// contains the specified item.
    /// </summary>
    /// <param name="value">Item to find.</param>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
    public bool Contains(object? value)
    {
      if (value == null)
        throw new ArgumentNullException(nameof(value));

      return QueryResult.Contains((T)value);
    }

    /// <summary>
    /// Gets the positional index of the item.
    /// </summary>
    /// <param name="value">Item to find.</param>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
    public int IndexOf(object? value)
    {
      if (value == null)
        throw new ArgumentNullException(nameof(value));

      return QueryResult.IndexOf((T)value);
    }

    /// <summary>
    /// Inserts item into specified position in list.
    /// </summary>
    /// <param name="index">Location to insert item.</param>
    /// <param name="value">Item to insert.</param>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
    public void Insert(int index, object? value)
    {
      if (value == null)
        throw new ArgumentNullException(nameof(value));

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
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
    public void Remove(object? value)
    {
      if (value == null)
        throw new ArgumentNullException(nameof(value));

      _baseCollection.Remove((T)value);
    }

    object? IList.this[int index]
    {
      get
      {
        return this[index];
      }
      set
      {
        this[index] = (T)(value ?? throw new ArgumentNullException(nameof(value)));
      }
    }

    /// <summary>
    /// Copies the contents of the list to an array.
    /// </summary>
    /// <param name="array">Target array.</param>
    /// <param name="index">Index in array where copying begins.</param>
    /// <exception cref="ArgumentNullException"><paramref name="array"/> is <see langword="null"/>.</exception>
    public void CopyTo(Array array, int index)
    {
      Guard.NotNull(array);

      ((IList)QueryResult).CopyTo(array, index);
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
    /// <exception cref="ArgumentNullException"><paramref name="queryResult"/> or <paramref name="source"/> is <see langword="null"/>.</exception>
    public static LinqObservableCollection<C> ToSyncList<C>(this IEnumerable<C> queryResult, System.Collections.ObjectModel.ObservableCollection<C> source) where C: notnull
    {
      Guard.NotNull(queryResult);
      Guard.NotNull(source);

      return new LinqObservableCollection<C>(source, queryResult);
    }

    /// <summary>
    /// Gets a LinqObservableCollection that is a live view
    /// of the original list based on the query result.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="queryResult"/> or <paramref name="source"/> is <see langword="null"/>.</exception>
    public static LinqObservableCollection<C> ToSyncList<C>(this System.Collections.ObjectModel.ObservableCollection<C> source, IEnumerable<C> queryResult) where C : notnull
    {
      Guard.NotNull(source);
      Guard.NotNull(queryResult);

      return new LinqObservableCollection<C>(source, queryResult);
    }

    /// <summary>
    /// Gets a LinqObservableCollection that is a live view
    /// of the original list based on the query result.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="expr"/> or <paramref name="source"/> is <see langword="null"/>.</exception>
    public static LinqObservableCollection<C> ToSyncList<C>(this System.Collections.ObjectModel.ObservableCollection<C> source, Expression<Func<C, bool>> expr) where C : notnull
    {
      Guard.NotNull(source);
      if (expr is null)
        throw new ArgumentNullException(nameof(expr));

      IEnumerable<C> sourceEnum = source.AsEnumerable<C>();
      var output = sourceEnum.Where<C>(expr.Compile());
      return new LinqObservableCollection<C>(source, output.ToList());
    }
  }
}