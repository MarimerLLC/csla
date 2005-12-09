using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Reflection;
using System.Collections;

namespace Csla
{

  /// <summary>
  /// Provides a sorted view into an existing IList(Of T).
  /// </summary>
  public class SortedBindingList<T> : 
    IList<T>, IBindingList, IEnumerable<T>
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
          if (Key.Equals(target))
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
      private int index;

      public SortedEnumerator(IList<T> list, List<ListItem> sortIndex, ListSortDirection direction)
      {
        _list = list;
        _sortIndex = sortIndex;
        _sortOrder = direction;
        Reset();
      }

      public T Current
      {
        get { return _list[_sortIndex[index].BaseIndex]; }
      }

      Object System.Collections.IEnumerator.Current
      {
        get { return _list[_sortIndex[index].BaseIndex]; }
      }

      public bool MoveNext()
      {
        if (_sortOrder == ListSortDirection.Ascending)
        {
          if (index < _sortIndex.Count - 1)
          {
            index++;
            return true;
          }
          else
            return false;
        }
        else
        {
          if (index > 0)
          {
            index--;
            return true;
          }
          else
            return false;
        }
      }

      public void Reset()
      {
        if (_sortOrder == ListSortDirection.Ascending)
          index = -1;
        else
          index = _sortIndex.Count;
      }

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

      #region IDisposable Support

      // this code added to correctly implement the disposable pattern.
      public void Dispose()
      {
        // Do not change this code.  Put cleanup code in Dispose(bool disposing) above.
        Dispose(true);
        GC.SuppressFinalize(this);
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
    public void AddIndex(PropertyDescriptor property)
    {
      if (_supportsBinding)
        _bindingList.AddIndex(property);
    }

    /// <summary>
    /// Implemented by IList source object.
    /// </summary>
    object System.ComponentModel.IBindingList.AddNew()
    {
      object result;
      if (_supportsBinding)
      {
        _initiatedLocally = true;
        result = _bindingList.AddNew();
        _initiatedLocally = false;
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
    public int Find(PropertyDescriptor property, object key)
    {
      if (_supportsBinding)
        return _bindingList.Find(property, key);
      else
        return -1;
    }

    /// <summary>
    /// Returns True if the view is currently sorted.
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
    /// is changed to indicate that the view's data has changed.
    /// </remarks>
    public event ListChangedEventHandler ListChanged;

    protected void OnListChanged(ListChangedEventArgs e)
    {
      if (ListChanged != null)
        ListChanged(this, e);
    }

    /// <summary>
    /// Implemented by IList source object.
    /// </summary>
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
    /// Returns True since this object does raise the
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
    /// Returns True. Sorting is supported.
    /// </summary>
    public bool SupportsSorting
    {
      get { return true; }
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
      _list.CopyTo(array, arrayIndex);
    }

    void System.Collections.ICollection.CopyTo(System.Array array, int index)
    {
      CopyTo((T[])array, index);
    }

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

    public void Add(T item)
    {
      _list.Add(item);
    }

    int System.Collections.IList.Add(object value)
    {
      Add((T)value);
      return SortedIndex(_list.Count - 1);
    }

    public void Clear()
    {
      _list.Clear();
    }

    public bool Contains(T item)
    {
      return _list.Contains(item);
    }

    bool System.Collections.IList.Contains(object value)
    {
      return Contains((T)value);
    }

    public int IndexOf(T item)
    {
      return _list.IndexOf(item);
    }

    int System.Collections.IList.IndexOf(object value)
    {
      return IndexOf((T)value);
    }

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

    public bool IsReadOnly
    {
      get { return _list.IsReadOnly; }
    }

    object System.Collections.IList.this[int index]
    {
      get
      {
        if (_sorted)
        {
          if (_sortOrder == ListSortDirection.Ascending)
            return _list[_sortIndex[index].BaseIndex];
          else
            return _list[_sortIndex[_sortIndex.Count - 1 - index].BaseIndex];
        }
        else
          return _list[index];
      }
      set
      {
        if (_sorted)
          if (_sortOrder == ListSortDirection.Ascending)
            _list[_sortIndex[index].BaseIndex] = (T)value;
          else
            _list[_sortIndex[
              _sortIndex.Count - 1 - index].BaseIndex] = (T)value;
        else
          _list[index] = (T)value;
      }
    }

    public bool Remove(T item)
    {
      return _list.Remove(item);
    }

    void System.Collections.IList.Remove(object value)
    {
      Remove((T)value);
    }

    public void RemoveAt(int index)
    {
      if (_sorted)
      {
        _initiatedLocally = true;
        int baseIndex = OriginalIndex(index);
        // remove the item from the source list
        _list.RemoveAt(baseIndex);
        // delete the corresponding value in the sort index
        _sortIndex.RemoveAt(index);
        // now fix up all index pointers in the sort index
        foreach (ListItem item in _sortIndex)
          if (item.BaseIndex > baseIndex)
            item.BaseIndex -= 1;
        OnListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, index));
        _initiatedLocally = false;
      }
      else
        _list.RemoveAt(index);
    }

    public T this[int index]
    {
      get
      {
        if (_sorted)
        {
          if (_sortOrder == ListSortDirection.Ascending)
            return _list[_sortIndex[index].BaseIndex];
          else
            return _list[_sortIndex[_sortIndex.Count - 1 - index].BaseIndex];
        }
        else
          return _list[index];
      }
      set
      {
        if (_sorted)
          _list[OriginalIndex(index)] = (T)value;
        else
          _list[index] = (T)value;
      }
    }

    #endregion

    private IList<T> _list;
    private bool _supportsBinding;
    private IBindingList _bindingList;
    private bool _sorted;
    private bool _initiatedLocally;
    private PropertyDescriptor _sortBy;
    private ListSortDirection _sortOrder = ListSortDirection.Ascending;
    private List<ListItem> _sortIndex = new List<ListItem>();


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
        _bindingList.ListChanged += new ListChangedEventHandler(SourceChanged);
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
            object newKey;
            if (_sorted)
              newKey = _sortBy.GetValue(newItem);
            else
              newKey = newItem;

            if (_sortOrder == ListSortDirection.Ascending)
              _sortIndex.Add(new ListItem(newKey, e.NewIndex));
            else
              _sortIndex.Insert(0, new ListItem(newKey, e.NewIndex));
            if (!_initiatedLocally)
              OnListChanged(
                new ListChangedEventArgs(
                ListChangedType.ItemAdded, SortedIndex(e.NewIndex)));
            break;

          case ListChangedType.ItemChanged:
            // an item changed - just relay the event with
            // a translated index value
            OnListChanged(
              new ListChangedEventArgs(
              ListChangedType.ItemChanged, SortedIndex(e.NewIndex)));
            break;

          case ListChangedType.ItemDeleted:
            if (!_initiatedLocally)
              DoSort();
            break;

          default:
            // for anything other than add, or change
            // just re-sort the list
            DoSort();
            break;
        }
      }
      else
        OnListChanged(e);
    }

    private int OriginalIndex(int sortedIndex)
    {
      if (_sortOrder == ListSortDirection.Descending)
        sortedIndex = _sortIndex.Count - 1 - sortedIndex;
      return _sortIndex[sortedIndex].BaseIndex;
    }

    private int SortedIndex(int originalIndex)
    {
      int result = 0;
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
      return result;
       
    }
  }
}