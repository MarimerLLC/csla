//-----------------------------------------------------------------------
// <copyright file="SortedBindingList.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Provides a sorted view into an existing IList(Of T).</summary>
//-----------------------------------------------------------------------
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Collections;
using Csla.Properties;

namespace Csla
{

  /// <summary>
  /// Provides a sorted view into an existing IList(Of T).
  /// </summary>
  /// <typeparam name="T">
  /// Type of child object contained by
  /// the original list or collection.
  /// </typeparam>
  public class SortedBindingList<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]T> :
    IList<T>,
    IBindingList,
    ICancelAddNew
    where T : notnull
  {

    #region ListItem class

    private class ListItem : IComparable<ListItem>
    {
      public object Key { get; }

      public int BaseIndex { get; set; }

      public ListItem(object key, int baseIndex)
      {
        Key = key ?? throw new ArgumentNullException(nameof(key));
        BaseIndex = baseIndex;
      }

      public int CompareTo(ListItem? other)
      {
        if (other == null) 
          return 1;

        object target = other.Key;

        if (Key is IComparable comparable)
          return comparable.CompareTo(target);

        else
        {
          if (Key == null)
          {
            if (target == null)
              return 0;
            else
              return 1;
          }
          else if (Key.Equals(target))
            return 0;

          else if (target == null)
            return 1;

          return Key.ToString()!.CompareTo(target.ToString());
        }
      }

      public override string ToString()
      {
        return Key.ToString()!;
      }

    }

    #endregion

    #region Sorted enumerator

    private class SortedEnumerator : IEnumerator<T>
    {
      private readonly IList<T> _list;
      private readonly List<ListItem> _sortIndex;
      private readonly ListSortDirection _sortOrder;
      private int _index;

      public SortedEnumerator(IList<T> list, List<ListItem> sortIndex, ListSortDirection direction)
      {
        _list = list ?? throw new ArgumentNullException(nameof(list));
        _sortIndex = sortIndex ?? throw new ArgumentNullException(nameof(sortIndex));
        _sortOrder = direction;
        Reset();
      }

      public T Current => _list[_sortIndex[_index].BaseIndex];

      object IEnumerator.Current => _list[_sortIndex[_index].BaseIndex];

      public bool MoveNext()
      {
        if (_sortOrder == ListSortDirection.Ascending)
        {
          if (_index < _sortIndex.Count - 1)
          {
            _index++;
            return true;
          }
          else
            return false;
        }
        else
        {
          if (_index > 0)
          {
            _index--;
            return true;
          }
          else
            return false;
        }
      }

      public void Reset()
      {
        if (_sortOrder == ListSortDirection.Ascending)
          _index = -1;
        else
          _index = _sortIndex.Count;
      }

      #region IDisposable Support

      private bool _disposedValue = false; // To detect redundant calls.

      // IDisposable
      protected virtual void Dispose(bool disposing)
      {
        if (!_disposedValue)
        {
          if (disposing)
          {
            // free unmanaged resources when explicitly called
          }
          // free shared unmanaged resources
        }
        _disposedValue = true;
      }

      // this code added to correctly implement the disposable pattern.
      public void Dispose()
      {
        // Do not change this code.  Put cleanup code in Dispose(bool disposing) above.
        Dispose(true);
        GC.SuppressFinalize(this);
      }

      /// <summary>
      /// Allows an <see cref="T:System.Object"/> to attempt to free resources and perform other cleanup operations before the <see cref="T:System.Object"/> is reclaimed by garbage collection.
      /// </summary>
      ~SortedEnumerator()
      {
        Dispose(false);
      }

      #endregion

    }

    #endregion

    #region Sort/Unsort

    private void DoSort()
    {
      int index = 0;
      _sortIndex.Clear();

      if (SortProperty == null)
      {
        foreach (T obj in SourceList)
        {
          _sortIndex.Add(new ListItem(obj, index));
          index++;
        }
      }
      else
      {
        foreach (T obj in SourceList)
        {
          _sortIndex.Add(new ListItem(SortProperty.GetValue(obj) ?? throw new InvalidOperationException(), index));
          index++;
        }
      }

      _sortIndex.Sort();
      IsSorted = true;

      OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, 0));
    }

    private void UndoSort()
    {
      _sortIndex.Clear();
      SortProperty = null;
      SortDirection = ListSortDirection.Ascending;
      IsSorted = false;

      OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, 0));
    }

    #endregion

    #region IEnumerable<T>

    /// <summary>
    /// Returns an enumerator for the list, honoring
    /// any sort that is active at the time.
    /// </summary>
    public IEnumerator<T> GetEnumerator()
    {
      if (IsSorted)
        return new SortedEnumerator(SourceList, _sortIndex, SortDirection);
      else
        return SourceList.GetEnumerator();
    }

    #endregion

    #region IBindingList, IList<T>

    /// <summary>
    /// Implemented by IList source object.
    /// </summary>
    /// <param name="property">Property on which
    /// to build the index.</param>
    /// <exception cref="ArgumentNullException"><paramref name="property"/> is <see langword="null"/>.</exception>
    public void AddIndex(PropertyDescriptor property)
    {
      if (!SupportsBinding)
        return;
      
      if (property is null)
        throw new ArgumentNullException(nameof(property));
      
      _bindingList!.AddIndex(property);
    }

    /// <summary>
    /// Implemented by IList source object.
    /// </summary>
    public object? AddNew()
    {
      object? result;
      if (SupportsBinding)
      {
        _initiatedLocally = true;
        result = _bindingList!.AddNew();
        _initiatedLocally = false;
        OnListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, _bindingList!.Count - 1));
      }
      else
        result = null;

      return result;
    }

    /// <summary>
    /// Implemented by IList source object.
    /// </summary>
    public bool AllowEdit
    {
      get
      {
        if (SupportsBinding)
          return _bindingList!.AllowEdit;
        else
          return false;
      }
    }

    /// <summary>
    /// Implemented by IList source object.
    /// </summary>
    public bool AllowNew
    {
      get
      {
        if (SupportsBinding)
          return _bindingList!.AllowNew;
        else
          return false;
      }
    }

    /// <summary>
    /// Implemented by IList source object.
    /// </summary>
    public bool AllowRemove
    {
      get
      {
        if (SupportsBinding)
          return _bindingList!.AllowRemove;
        else
          return false;
      }
    }

    /// <summary>
    /// Applies a sort to the view.
    /// </summary>
    /// <param name="propertyName">The text name of the property on which to sort.</param>
    /// <param name="direction">The direction to sort the data.</param>
    public void ApplySort(string? propertyName, ListSortDirection direction)
    {
      PropertyDescriptor? newSortProperty = null;
      if (!string.IsNullOrWhiteSpace(propertyName))
      {
        newSortProperty = GetPropertyDescriptor(propertyName!);
      }

      SortProperty = newSortProperty;

      ApplySort(SortProperty, direction);
    }

    /// <summary>
    /// Applies a sort to the view.
    /// </summary>
    /// <param name="property">A PropertyDescriptor for the property on which to sort.</param>
    /// <param name="direction">The direction to sort the data.</param>
    public void ApplySort(PropertyDescriptor? property, ListSortDirection direction)
    {
      SortProperty = property;
      SortDirection = direction;
      DoSort();
    }

    /// <summary>
    /// Finds an item in the view
    /// </summary>
    /// <param name="propertyName">Name of the property to search</param>
    /// <param name="key">Value to find</param>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="propertyName"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    public int Find(string propertyName, object key)
    {
      if (key is null)
        throw new ArgumentNullException(nameof(key));

      if (string.IsNullOrWhiteSpace(propertyName))
      {
        if (!SupportsBinding)
          return -1;
        
        throw new ArgumentException(string.Format(Resources.StringNotNullOrWhiteSpaceException, nameof(propertyName)), nameof(propertyName));
      }

      var findProperty = GetPropertyDescriptor(propertyName)!;

      return Find(findProperty, key);
    }

    private static PropertyDescriptor GetPropertyDescriptor(string propertyName)
    {
        Type itemType = typeof(T);
        foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(itemType))
        {
          if (prop.Name == propertyName)
          {
            return prop;
          }
        }

        throw new ArgumentException(string.Format(Resources.SortedBindingListPropertyNameNotFound, propertyName), propertyName);
    }

    /// <summary>
    /// Implemented by IList source object.
    /// </summary>
    /// <param name="key">Key value for which to search.</param>
    /// <param name="property">Property to search for the key
    /// value.</param>
    /// <exception cref="ArgumentNullException"><paramref name="property"/> or <paramref name="key"/> is <see langword="null"/>.</exception>
    public int Find(PropertyDescriptor property, object key)
    {
      if (SupportsBinding)
      {
        if (property is null)
          throw new ArgumentNullException(nameof(property));
        if (key is null)
          throw new ArgumentNullException(nameof(key));

        var originalIndex = _bindingList!.Find(property, key);
        return originalIndex > -1 ? SortedIndex(originalIndex) : -1;
      }
      else
        return -1;
    }

    /// <summary>
    /// Gets a value indicating whether the view is currently sorted.
    /// </summary>
    public bool IsSorted { get; private set; }

    /// <summary>
    /// Raised to indicate that the list's data has changed.
    /// </summary>
    /// <remarks>
    /// This event is raised if the underling IList object's data changes
    /// (assuming the underling IList also implements the IBindingList
    /// interface). It is also raised if the sort property or direction
    /// is changed to indicate that the view's data has changed. See
    /// Chapter 5 for details.
    /// </remarks>
    public event ListChangedEventHandler? ListChanged;

    /// <summary>
    /// Raises the <see cref="ListChanged"/> event.
    /// </summary>
    /// <param name="e">Event arguments.</param>
    /// <exception cref="ArgumentNullException"><paramref name="e"/> is <see langword="null"/>.</exception>
    protected void OnListChanged(ListChangedEventArgs e)
    {
      if (e is null)
        throw new ArgumentNullException(nameof(e));

      ListChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Implemented by IList source object.
    /// </summary>
    /// <param name="property">Property for which the
    /// index should be removed.</param>
    /// <exception cref="ArgumentNullException"><paramref name="property"/> is <see langword="null"/>.</exception>
    public void RemoveIndex(PropertyDescriptor property)
    {
      if (!SupportsBinding)
        return;

      if (property is null)
        throw new ArgumentNullException(nameof(property));

      _bindingList!.RemoveIndex(property);
    }

    /// <summary>
    /// Removes any sort currently applied to the view.
    /// </summary>
    public void RemoveSort()
    {
      UndoSort();
    }

    /// <summary>
    /// Returns the direction of the current sort.
    /// </summary>
    public ListSortDirection SortDirection { get; private set; } = ListSortDirection.Ascending;

    /// <summary>
    /// Returns the PropertyDescriptor of the current sort.
    /// </summary>
    public PropertyDescriptor? SortProperty { get; private set; }

    /// <summary>
    /// Returns true since this object does raise the
    /// ListChanged event.
    /// </summary>
    public bool SupportsChangeNotification => true;

    /// <summary>
    /// Implemented by IList source object.
    /// </summary>
    public bool SupportsSearching
    {
      get
      {
        if (SupportsBinding)
          return _bindingList!.SupportsSearching;
        else
          return false;
      }
    }

    /// <summary>
    /// Returns true. Sorting is supported.
    /// </summary>
    public bool SupportsSorting => true;

    /// <summary>
    /// Implemented by IList source object.
    /// </summary>
    /// <param name="array">Array to receive the data.</param>
    /// <param name="arrayIndex">Starting array index.</param>
    /// <exception cref="ArgumentNullException"><paramref name="array"/> is <see langword="null"/>.</exception>
    public void CopyTo(T[] array, int arrayIndex)
    {
      if (array is null)
        throw new ArgumentNullException(nameof(array));

      int pos = arrayIndex;
      foreach (T child in this)
      {
        array[pos] = child;
        pos++;
      }
    }

    void ICollection.CopyTo(Array array, int index)
    {
      if (array is null)
        throw new ArgumentNullException(nameof(array));

      T[] tmp = new T[array.Length];
      CopyTo(tmp, index);
      Array.Copy(tmp, 0, array, index, array.Length);
    }

    /// <summary>
    /// Implemented by IList source object.
    /// </summary>
    public int Count => SourceList.Count;

    bool ICollection.IsSynchronized => false;

    object ICollection.SyncRoot => SourceList;

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    /// <summary>
    /// Implemented by IList source object.
    /// </summary>
    /// <param name="item">Item to add to the list.</param>
    /// <exception cref="ArgumentNullException"><paramref name="item"/> is <see langword="null"/>.</exception>
    public void Add(T item)
    {
      if(item is null) 
        throw new ArgumentNullException(nameof(item));

      SourceList.Add(item);
    }

    int IList.Add(object? value)
    {
      if(value is null) 
        throw new ArgumentNullException(nameof(value));

      Add((T)value);
      return SortedIndex(SourceList.Count - 1);
    }

    /// <summary>
    /// Implemented by IList source object.
    /// </summary>
    public void Clear()
    {
      SourceList.Clear();
    }

    /// <summary>
    /// Implemented by IList source object.
    /// </summary>
    /// <param name="item">Item for which to search.</param>
    /// <exception cref="ArgumentNullException"><paramref name="item"/> is <see langword="null"/>.</exception>
    public bool Contains(T item)
    {
      if (item is null)
        throw new ArgumentNullException(nameof(item));

      return SourceList.Contains(item);
    }

    bool IList.Contains(object? value)
    {
      if (value is null)
        throw new ArgumentNullException(nameof(value));

      return Contains((T)value);
    }

    /// <summary>
    /// Implemented by IList source object.
    /// </summary>
    /// <param name="item">Item for which to search.</param>
    /// <exception cref="ArgumentNullException"><paramref name="item"/> is <see langword="null"/>.</exception>
    public int IndexOf(T item)
    {
      if (item is null)
        throw new ArgumentNullException(nameof(item));

      return SortedIndex(SourceList.IndexOf(item));
    }

    int IList.IndexOf(object? value)
    {
      if (value is null)
        throw new ArgumentNullException(nameof(value));

      return IndexOf((T)value);
    }

    /// <summary>
    /// Implemented by IList source object.
    /// </summary>
    /// <param name="index">Index at
    /// which to insert the item.</param>
    /// <param name="item">Item to insert.</param>
    /// <exception cref="ArgumentNullException"><paramref name="item"/> is <see langword="null"/>.</exception>
    public void Insert(int index, T item)
    {
      if (item is null)
        throw new ArgumentNullException(nameof(item));

      SourceList.Insert(index, item);
    }

    void IList.Insert(int index, object? value)
    {
      if (value is null)
        throw new ArgumentNullException(nameof(value));

      Insert(index, (T)value);
    }

    bool IList.IsFixedSize => false;

    /// <summary>
    /// Implemented by IList source object.
    /// </summary>
    public bool IsReadOnly => SourceList.IsReadOnly;

    object? IList.this[int index]
    {
      get => this[index];
      set => this[index] = (T)(value ?? throw new ArgumentNullException(nameof(value)));
    }

    /// <summary>
    /// Implemented by IList source object.
    /// </summary>
    /// <param name="item">Item to be removed.</param>
    /// <exception cref="ArgumentNullException"><paramref name="item"/> is <see langword="null"/>.</exception>
    public bool Remove(T item)
    {
      if (item is null)
        throw new ArgumentNullException(nameof(item));

      return SourceList.Remove(item);
    }

    void IList.Remove(object? value)
    {
      if (value is null)
        throw new ArgumentNullException(nameof(value));

      Remove((T)value);
    }

    /// <summary>
    /// Removes the child object at the specified index
    /// in the list, resorting the display as needed.
    /// </summary>
    /// <param name="index">The index of the object to remove.</param>
    /// <remarks>
    /// See Chapter 5 for details on how and why the list is
    /// altered during the remove process.
    /// </remarks>
    public void RemoveAt(int index)
    {
      if (IsSorted)
      {
        _initiatedLocally = true;
        int baseIndex = OriginalIndex(index);

        // remove the item from the source list
        SourceList.RemoveAt(baseIndex);

        _initiatedLocally = false;
      }
      else
        SourceList.RemoveAt(index);
    }

    /// <summary>
    /// Gets the child item at the specified index in the list,
    /// honoring the sort order of the items.
    /// </summary>
    /// <param name="index">The index of the item in the sorted list.</param>
    /// <exception cref="ArgumentNullException">value is <see langword="null"/>.</exception>
    public T this[int index]
    {
      get
      {
        if (IsSorted)
          return SourceList[OriginalIndex(index)];
        else
          return SourceList[index];
      }
      set
      {
        if (value is null)
          throw new ArgumentNullException(nameof(value));
        
        if (IsSorted)
          SourceList[OriginalIndex(index)] = value;
        else
          SourceList[index] = value;
      }
    }

    #endregion

    #region SourceList

    /// <summary>
    /// Gets the source list over which this
    /// SortedBindingList is a view.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public IList<T> SourceList { get; }

    #endregion

    [MemberNotNullWhen(true, nameof(_bindingList))]
    private bool SupportsBinding { get; }
    private IBindingList? _bindingList;
    private bool _initiatedLocally;

    private readonly List<ListItem> _sortIndex = [];

    /// <summary>
    /// Creates a new view based on the provided IList object.
    /// </summary>
    /// <param name="list">The IList (collection) containing the data.</param>
    /// <exception cref="ArgumentNullException"><paramref name="list"/> is <see langword="null"/>.</exception>
    public SortedBindingList(IList<T> list)
    {
      SourceList = list ?? throw new ArgumentNullException(nameof(list));

      if (SourceList is IBindingList sourceList)
      {
        SupportsBinding = true;
        _bindingList = sourceList;
        _bindingList!.ListChanged += SourceChanged;
      }
    }

    private void SourceChanged(object? sender, ListChangedEventArgs e)
    {
      if (IsSorted)
      {
        switch (e.ListChangedType)
        {
          case ListChangedType.ItemAdded:
            T newItem = SourceList[e.NewIndex];
            if (e.NewIndex == SourceList.Count - 1)
            {
              object newKey;
              if (SortProperty != null)
                newKey = SortProperty.GetValue(newItem) ?? throw new InvalidOperationException();
              else
                newKey = newItem;

              if (SortDirection == ListSortDirection.Ascending)
                _sortIndex.Add(new ListItem(newKey, e.NewIndex));
              else
                _sortIndex.Insert(0, new ListItem(newKey, e.NewIndex));
              if (!_initiatedLocally)
                OnListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, SortedIndex(e.NewIndex)));
            }
            else
              DoSort();
            break;

          case ListChangedType.ItemChanged:
            // an item changed - just relay the event with
            // a translated index value
            OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, SortedIndex(e.NewIndex), e.PropertyDescriptor));
            break;

          case ListChangedType.ItemDeleted:
            var internalIndex = InternalIndex(e.NewIndex);
            var sortedIndex = internalIndex;
            if (IsSorted && (SortDirection == ListSortDirection.Descending))
              sortedIndex = _sortIndex.Count - 1 - internalIndex;

            // remove from internal list 
            _sortIndex.RemoveAt(internalIndex);

            // now fix up all index pointers in the sort index
            foreach (ListItem item in _sortIndex)
              if (item.BaseIndex > e.NewIndex)
                item.BaseIndex -= 1;

            OnListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, sortedIndex, e.PropertyDescriptor));
            break;

          default:
            // for anything other than add, delete or change
            // just re-sort the list
            if (!_initiatedLocally)
              DoSort();
            break;
        }
      }
      else
      {
        if (!_initiatedLocally)
          OnListChanged(e);
      }
    }

    private int OriginalIndex(int sortedIndex)
    {
      if (sortedIndex == -1) return -1;

      if (IsSorted)
      {
        if (SortDirection == ListSortDirection.Ascending)
          return _sortIndex[sortedIndex].BaseIndex;
        else
          return _sortIndex[_sortIndex.Count - 1 - sortedIndex].BaseIndex;
      }
      else
        return sortedIndex;
    }

    private int SortedIndex(int originalIndex)
    {
      if (originalIndex == -1) return -1;

      int result = 0;
      if (IsSorted)
      {
        for (int index = 0; index < _sortIndex.Count; index++)
        {
          if (_sortIndex[index].BaseIndex == originalIndex)
          {
            result = index;
            break;
          }
        }
        if (SortDirection == ListSortDirection.Descending)
          result = _sortIndex.Count - 1 - result;
      }
      else
        result = originalIndex;
      return result;
    }

    private int InternalIndex(int originalIndex)
    {
      int result = 0;
      if (IsSorted)
      {
        for (int index = 0; index < _sortIndex.Count; index++)
        {
          if (_sortIndex[index].BaseIndex == originalIndex)
          {
            result = index;
            break;
          }
        }
      }
      else
        result = originalIndex;
      return result;
    }


    #region ICancelAddNew Members

    void ICancelAddNew.CancelNew(int itemIndex)
    {
      if (itemIndex <= -1) return;

      if (SourceList is ICancelAddNew can)
        can.CancelNew(OriginalIndex(itemIndex));
      else
        SourceList.RemoveAt(OriginalIndex(itemIndex));
    }

    void ICancelAddNew.EndNew(int itemIndex)
    {
      if (SourceList is ICancelAddNew can)
        can.EndNew(OriginalIndex(itemIndex));
    }

    #endregion

    #region ToArray

    /// <summary>
    /// Get an array containing all items in the list.
    /// </summary>
    public T[] ToArray()
    {
      List<T> result = new List<T>();
      foreach (T item in this)
        result.Add(item);
      return result.ToArray();
    }

    #endregion

  }
}