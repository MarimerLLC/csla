using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections;

namespace Csla
{

  /// <summary>
  /// Provides a sorted view into an existing IList(Of T).
  /// </summary>
  /// <typeparam name="T">
  /// Type of child object contained by
  /// the original list or collection.
  /// </typeparam>
  public class SortedBindingList<T> : 
    IList<T>, IBindingList, IEnumerable<T>, ICancelAddNew
  {

    #region ListItem class

    private class ListItem : IComparable<ListItem>
    {

      private object _key;
      private int _baseIndex;

      public object Key
      {
        get { return _key; }
      }

      public int BaseIndex
      {
        get { return _baseIndex; }
        set { _baseIndex = value; }
      }

      public ListItem(object key, int baseIndex)
      {
        _key = key;
        _baseIndex = baseIndex;
      }

      public int CompareTo(ListItem other)
      {
        object target = other.Key;

        if (Key is IComparable)
          return ((IComparable)Key).CompareTo(target);

        else
        {
          if (Key == null)
          {
            if (target == null)
              return 0;
            else
              return -1;
          }
          else if (Key.Equals(target))
            return 0;
          else
            return Key.ToString().CompareTo(target.ToString());
        }
      }

      public override string ToString()
      {
        return Key.ToString();
      }

    }

    #endregion

    #region Sorted enumerator

    private class SortedEnumerator : IEnumerator<T>
    {
      private IList<T> _list;
      private List<ListItem> _sortIndex;
      private ListSortDirection _sortOrder;
      private int _index;

      public SortedEnumerator(
        IList<T> list, 
        List<ListItem> sortIndex, 
        ListSortDirection direction)
      {
        _list = list;
        _sortIndex = sortIndex;
        _sortOrder = direction;
        Reset();
      }

      public T Current
      {
        get { return _list[_sortIndex[_index].BaseIndex]; }
      }

      Object System.Collections.IEnumerator.Current
      {
        get { return _list[_sortIndex[_index].BaseIndex]; }
      }

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
            // TODO: free unmanaged resources when explicitly called
          }
          // TODO: free shared unmanaged resources
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

      if (_sortBy == null)
      {
        foreach (T obj in _list)
        {
          _sortIndex.Add(new ListItem(obj, index));
          index++;
        }
      }
      else
      {
        foreach (T obj in _list)
        {
          _sortIndex.Add(new ListItem(_sortBy.GetValue(obj), index));
          index++;
        }
      }

      _sortIndex.Sort();
      _sorted = true;

      OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, 0));

    }

    private void UndoSort()
    {
      _sortIndex.Clear();
      _sortBy = null;
      _sortOrder = ListSortDirection.Ascending;
      _sorted = false;

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
      if (_sorted)
        return new SortedEnumerator(_list, _sortIndex, _sortOrder);
      else
        return _list.GetEnumerator();
    }

    #endregion

    #region IBindingList, IList<T>

    /// <summary>
    /// Implemented by IList source object.
    /// </summary>
    /// <param name="property">Property on which
    /// to build the index.</param>
    public void AddIndex(PropertyDescriptor property)
    {
      if (_supportsBinding)
        _bindingList.AddIndex(property);
    }

    /// <summary>
    /// Implemented by IList source object.
    /// </summary>
    public object AddNew()
    {
      object result;
      if (_supportsBinding)
      {
        _initiatedLocally = true;
        result = _bindingList.AddNew();
        _initiatedLocally = false;
        OnListChanged(
          new ListChangedEventArgs(
          ListChangedType.ItemAdded, _bindingList.Count - 1));
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
        if (_supportsBinding)
          return _bindingList.AllowEdit;
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
        if (_supportsBinding)
          return _bindingList.AllowNew;
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
        if (_supportsBinding)
          return _bindingList.AllowRemove;
        else
          return false;
      }
    }

    /// <summary>
    /// Applies a sort to the view.
    /// </summary>
    /// <param name="propertyName">The text name of the property on which to sort.</param>
    /// <param name="direction">The direction to sort the data.</param>
    public void ApplySort(string propertyName, ListSortDirection direction)
    {
      _sortBy = null;

      if (!String.IsNullOrEmpty(propertyName))
      {
        Type itemType = typeof(T);
        foreach (PropertyDescriptor prop in 
          TypeDescriptor.GetProperties(itemType))
        {
          if (prop.Name == propertyName)
          {
            _sortBy = prop;
            break;
          }
        }
      }
      
      ApplySort(_sortBy, direction);

    }

    /// <summary>
    /// Applies a sort to the view.
    /// </summary>
    /// <param name="property">A PropertyDescriptor for the property on which to sort.</param>
    /// <param name="direction">The direction to sort the data.</param>
    public void ApplySort(
      PropertyDescriptor property, ListSortDirection direction)
    {
      _sortBy = property;
      _sortOrder = direction;
      DoSort();
    }

    /// <summary>
    /// Finds an item in the view
    /// </summary>
    /// <param name="propertyName">Name of the property to search</param>
    /// <param name="key">Value to find</param>
    public int Find(string propertyName, object key)
    {
      PropertyDescriptor findProperty = null;

      if (!String.IsNullOrEmpty(propertyName))
      {
        Type itemType = typeof(T);
        foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(itemType))
        {
          if (prop.Name == propertyName)
          {
            findProperty = prop;
            break;
          }
        }
      }

      return Find(findProperty, key);

    }

    /// <summary>
    /// Implemented by IList source object.
    /// </summary>
    /// <param name="key">Key value for which to search.</param>
    /// <param name="property">Property to search for the key
    /// value.</param>
    public int Find(PropertyDescriptor property, object key)
    {
      if (_supportsBinding)
        return SortedIndex(_bindingList.Find(property, key));
      else
        return -1;
    }

    /// <summary>
    /// Gets a value indicating whether the view is currently sorted.
    /// </summary>
    public bool IsSorted
    {
      get { return _sorted; }
    }

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
    public event ListChangedEventHandler ListChanged;

    /// <summary>
    /// Raises the <see cref="ListChanged"/> event.
    /// </summary>
    /// <param name="e">Event arguments.</param>
    protected void OnListChanged(ListChangedEventArgs e)
    {
      if (ListChanged != null)
        ListChanged(this, e);
    }

    /// <summary>
    /// Implemented by IList source object.
    /// </summary>
    /// <param name="property">Property for which the
    /// index should be removed.</param>
    public void RemoveIndex(PropertyDescriptor property)
    {
      if (_supportsBinding)
        _bindingList.RemoveIndex(property);
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
    public ListSortDirection SortDirection
    {
      get { return _sortOrder; }
    }

    /// <summary>
    /// Returns the PropertyDescriptor of the current sort.
    /// </summary>
    public PropertyDescriptor SortProperty
    {
      get { return _sortBy; }
    }

    /// <summary>
    /// Returns <see langword="true"/> since this object does raise the
    /// ListChanged event.
    /// </summary>
    public bool SupportsChangeNotification
    {
      get { return true; }
    }

    /// <summary>
    /// Implemented by IList source object.
    /// </summary>
    public bool SupportsSearching
    {
      get
      {
        if (_supportsBinding)
          return _bindingList.SupportsSearching;
        else
          return false;
      }
    }

    /// <summary>
    /// Returns <see langword="true"/>. Sorting is supported.
    /// </summary>
    public bool SupportsSorting
    {
      get { return true; }
    }

    /// <summary>
    /// Implemented by IList source object.
    /// </summary>
    /// <param name="array">Array to receive the data.</param>
    /// <param name="arrayIndex">Starting array index.</param>
    public void CopyTo(T[] array, int arrayIndex)
    {
      int pos = arrayIndex;
      foreach (T child in this)
      {
        array[pos] = child;
        pos++;
      }
    }

    void System.Collections.ICollection.CopyTo(System.Array array, int index)
    {
      T[] tmp = new T[array.Length];
      CopyTo(tmp, index);
      Array.Copy(tmp, 0, array, index, array.Length);
    }

    /// <summary>
    /// Implemented by IList source object.
    /// </summary>
    public int Count
    {
      get { return _list.Count; }
    }

    bool System.Collections.ICollection.IsSynchronized
    {
      get { return false; }
    }

    object System.Collections.ICollection.SyncRoot
    {
      get { return _list; }
    }

    IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    /// <summary>
    /// Implemented by IList source object.
    /// </summary>
    /// <param name="item">Item to add to the list.</param>
    public void Add(T item)
    {
      _list.Add(item);
    }

    int System.Collections.IList.Add(object value)
    {
      Add((T)value);
      return SortedIndex(_list.Count - 1);
    }

    /// <summary>
    /// Implemented by IList source object.
    /// </summary>
    public void Clear()
    {
      _list.Clear();
    }

    /// <summary>
    /// Implemented by IList source object.
    /// </summary>
    /// <param name="item">Item for which to search.</param>
    public bool Contains(T item)
    {
      return _list.Contains(item);
    }

    bool System.Collections.IList.Contains(object value)
    {
      return Contains((T)value);
    }

    /// <summary>
    /// Implemented by IList source object.
    /// </summary>
    /// <param name="item">Item for which to search.</param>
    public int IndexOf(T item)
    {
      return SortedIndex(_list.IndexOf(item));
    }

    int System.Collections.IList.IndexOf(object value)
    {
      return IndexOf((T)value);
    }

    /// <summary>
    /// Implemented by IList source object.
    /// </summary>
    /// <param name="index">Index at
    /// which to insert the item.</param>
    /// <param name="item">Item to insert.</param>
    public void Insert(int index, T item)
    {
      _list.Insert(index, item);
    }

    void System.Collections.IList.Insert(int index, object value)
    {
      Insert(index, (T)value);
    }

    bool System.Collections.IList.IsFixedSize
    {
      get { return false; }
    }

    /// <summary>
    /// Implemented by IList source object.
    /// </summary>
    public bool IsReadOnly
    {
      get { return _list.IsReadOnly; }
    }

    object System.Collections.IList.this[int index]
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
    /// Implemented by IList source object.
    /// </summary>
    /// <param name="item">Item to be removed.</param>
    public bool Remove(T item)
    {
      return _list.Remove(item);
    }

    void System.Collections.IList.Remove(object value)
    {
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
      if (_sorted)
      {
        _initiatedLocally = true;
        int baseIndex = OriginalIndex(index);
        
        // remove the item from the source list
        _list.RemoveAt(baseIndex);

        if (_list.Count != _sortIndex.Count)
        {
          // delete the corresponding value in the sort index
          if (_sortOrder == ListSortDirection.Ascending)
            _sortIndex.RemoveAt(index);
          else
            _sortIndex.RemoveAt(_sortIndex.Count - 1 - index);

          // now fix up all index pointers in the sort index
          foreach (ListItem item in _sortIndex)
            if (item.BaseIndex > baseIndex)
              item.BaseIndex -= 1;
        }

        OnListChanged(new ListChangedEventArgs(
          ListChangedType.ItemDeleted, index));
        _initiatedLocally = false;
      }
      else
        _list.RemoveAt(index);
    }

    /// <summary>
    /// Gets the child item at the specified index in the list,
    /// honoring the sort order of the items.
    /// </summary>
    /// <param name="index">The index of the item in the sorted list.</param>
    public T this[int index]
    {
      get
      {
        if (_sorted)
          return _list[OriginalIndex(index)];
        else
          return _list[index];
      }
      set
      {
        if (_sorted)
          _list[OriginalIndex(index)] = value;
        else
          _list[index] = value;
      }
    }

    #endregion

    #region SourceList

    /// <summary>
    /// Gets the source list over which this
    /// SortedBindingList is a view.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public IList<T> SourceList
    {
      get
      {
        return _list;
      }
    }

    #endregion

    private IList<T> _list;
    private bool _supportsBinding;
    private IBindingList _bindingList;
    private bool _sorted;
    private bool _initiatedLocally;
    private PropertyDescriptor _sortBy;
    private ListSortDirection _sortOrder = 
      ListSortDirection.Ascending;
    private List<ListItem> _sortIndex = 
      new List<ListItem>();


    /// <summary>
    /// Creates a new view based on the provided IList object.
    /// </summary>
    /// <param name="list">The IList (collection) containing the data.</param>
    public SortedBindingList(IList<T> list)
    {
      _list = list;

      if (_list is IBindingList)
      {
        _supportsBinding = true;
        _bindingList = (IBindingList)_list;
        _bindingList.ListChanged += 
          new ListChangedEventHandler(SourceChanged);
      }
    }

    private void SourceChanged(
      object sender, ListChangedEventArgs e)
    {
      if (_sorted)
      {
        switch (e.ListChangedType)
        {
          case ListChangedType.ItemAdded:
            T newItem = _list[e.NewIndex];
            if (e.NewIndex == _list.Count - 1)
            {
              object newKey;
              if (_sortBy != null)
                newKey = _sortBy.GetValue(newItem);
              else
                newKey = newItem;

              if (_sortOrder == ListSortDirection.Ascending)
                _sortIndex.Add(
                  new ListItem(newKey, e.NewIndex));
              else
                _sortIndex.Insert(0,
                  new ListItem(newKey, e.NewIndex));
              if (!_initiatedLocally)
                OnListChanged(
                  new ListChangedEventArgs(
                  ListChangedType.ItemAdded,
                  SortedIndex(e.NewIndex)));
            }
            else
              DoSort();
            break;

          case ListChangedType.ItemChanged:
            // an item changed - just relay the event with
            // a translated index value
            OnListChanged(
              new ListChangedEventArgs(
              ListChangedType.ItemChanged, SortedIndex(e.NewIndex), e.PropertyDescriptor));
            break;

          case ListChangedType.ItemDeleted:
            if (!_initiatedLocally)
              DoSort();
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
      if (_sorted)
      {
        if (_sortOrder == ListSortDirection.Ascending)
          return _sortIndex[sortedIndex].BaseIndex;
        else
          return _sortIndex[_sortIndex.Count - 1 - sortedIndex].BaseIndex;
      }
      else
        return sortedIndex;
    }

    private int SortedIndex(int originalIndex)
    {
      int result = 0;
      if (_sorted)
      {
        for (int index = 0; index < _sortIndex.Count; index++)
        {
          if (_sortIndex[index].BaseIndex == originalIndex)
          {
            result = index;
            break;
          }
        }
        if (_sortOrder == ListSortDirection.Descending)
          result = _sortIndex.Count - 1 - result;
      }
      else
        result = originalIndex;
      return result;
       
    }

    #region ICancelAddNew Members

    void ICancelAddNew.CancelNew(int itemIndex)
    {
      ICancelAddNew can = _list as ICancelAddNew;
      if (can != null)
        can.CancelNew(OriginalIndex(itemIndex));
      else
        _list.RemoveAt(OriginalIndex(itemIndex));
    }

    void ICancelAddNew.EndNew(int itemIndex)
    {
      ICancelAddNew can = _list as ICancelAddNew;
      if (can != null)
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