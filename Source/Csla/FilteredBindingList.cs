//-----------------------------------------------------------------------
// <copyright file="FilteredBindingList.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Provides a filtered view into an existing IList(Of T).</summary>
//-----------------------------------------------------------------------
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections;
using Csla.Properties;

namespace Csla
{
  /// <summary>
  /// Provides a filtered view into an existing IList(Of T).
  /// </summary>
  /// <typeparam name="T">The type of the objects contained
  /// in the original list.</typeparam>
  public class FilteredBindingList<T> :
    IList<T>, IBindingList, IEnumerable<T>,
    ICancelAddNew
  {

    #region ListItem class

    private class ListItem 
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

      public override string ToString()
      {
        return Key.ToString();
      }

    }

    #endregion

    #region Filtered enumerator

    private class FilteredEnumerator : IEnumerator<T>
    {
      private IList<T> _list;
      private List<ListItem> _filterIndex;
      private int _index;

      public FilteredEnumerator(
        IList<T> list, 
        List<ListItem> filterIndex)
      {
        _list = list;
        _filterIndex = filterIndex;
        Reset();
      }

      public T Current
      {
        get { return _list[_filterIndex[_index].BaseIndex]; }
      }

      Object System.Collections.IEnumerator.Current
      {
        get { return _list[_filterIndex[_index].BaseIndex]; }
      }

      public bool MoveNext()
      {
        if (_index < _filterIndex.Count - 1)
        {
          _index++;
          return true;
        }
        else
          return false;
      }

      public void Reset()
      {
        _index = -1;
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

      ~FilteredEnumerator()
      {
        Dispose(false);
      }

      #endregion

    }

    #endregion

    #region Filter/Unfilter

    private void DoFilter()
    {
      int index = 0;
      _filterIndex.Clear();

      if (_provider == null)
        _provider = DefaultFilter.Filter;

      if (_filterBy == null)
      {
        foreach (T obj in _list)
        {
          if (_provider.Invoke(obj, _filter))
            _filterIndex.Add(new ListItem(obj, index));
          index++;
        }
      }
      else
      {
        foreach (T obj in _list)
        {
          object tmp = _filterBy.GetValue(obj);
          if (_provider.Invoke(tmp, _filter))
            _filterIndex.Add(new ListItem(tmp, index));
          index++;
        }
      }

      _filtered = true;

      OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, 0));

    }

    private void UnDoFilter()
    {
      _filterIndex.Clear();
      _filterBy = null;
      _filter = null;
      _filtered = false;

      OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, 0));

    }

    #endregion

    #region IEnumerable<T>

    /// <summary>
    /// Gets an enumerator object.
    /// </summary>
    /// <returns></returns>
    public IEnumerator<T> GetEnumerator()
    {
      if (_filtered)
        return new FilteredEnumerator(_list, _filterIndex);
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
      T result;
      if (_supportsBinding)
        result = (T)_bindingList.AddNew();
      else
        result = default(T);

      //_newItem = (T)result;
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
    /// Sorts the list if the original list
    /// supports sorting.
    /// </summary>
    /// <param name="property">Property on which to sort.</param>
    /// <param name="direction">Direction of the sort.</param>
    public void ApplySort(
      PropertyDescriptor property, ListSortDirection direction)
    {
      if (SupportsSorting)
        _bindingList.ApplySort(property, direction);
      else
        throw new NotSupportedException(Resources.SortingNotSupported);
    }

    /// <summary>
    /// Sorts the list if the original list
    /// supports sorting.
    /// </summary>
    /// <param name="propertyName">PropertyName on which to sort.</param>
    /// <param name="direction">Direction of the sort.</param>
    public void ApplySort(
      string propertyName, ListSortDirection direction)
    {
      if (SupportsSorting)
      {
        var property = GetPropertyDescriptor(propertyName);
        _bindingList.ApplySort(property, direction);
      }
      else
        throw new NotSupportedException(Resources.SortingNotSupported);
    }

    /// <summary>
    /// Gets the property descriptor.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns>PropertyDescriptor</returns>
    private static PropertyDescriptor GetPropertyDescriptor(string propertyName)
    {
      PropertyDescriptor property = null;

      if (!String.IsNullOrEmpty(propertyName))
      {
        Type itemType = typeof(T);
        foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(itemType))
        {
          if (prop.Name == propertyName)
          {
            property = prop;
            break;
          }
        }

        // throw exception if propertyDescriptor could not be found
        if (property == null)
          throw new ArgumentException(string.Format(Resources.SortedBindingListPropertyNameNotFound, propertyName), propertyName);
      }

      return property;
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
        return FilteredIndex(_bindingList.Find(property, key));
      else
        return -1;
    }

    /// <summary>
    /// Returns True if the view is currently sorted.
    /// </summary>
    public bool IsSorted
    {
      get
      {
        if (SupportsSorting)
          return _bindingList.IsSorted;
        else
          return false;
      }
    }

    /// <summary>
    /// Raised to indicate that the list's data has changed.
    /// </summary>
    /// <remarks>
    /// This event is raised if the underling IList object's data changes
    /// (assuming the underling IList also implements the IBindingList
    /// interface). It is also raised if the filter
    /// is changed to indicate that the view's data has changed.
    /// </remarks>
    public event ListChangedEventHandler ListChanged;

    /// <summary>
    /// Raises the ListChanged event.
    /// </summary>
    /// <param name="e">Parameter for the event.</param>
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
      if (SupportsSorting)
        _bindingList.RemoveSort();
      else
        throw new NotSupportedException(Resources.SortingNotSupported);
    }

    /// <summary>
    /// Returns the direction of the current sort.
    /// </summary>
    public ListSortDirection SortDirection
    {
      get 
      {
        if (SupportsSorting)
          return _bindingList.SortDirection;
        else
          return ListSortDirection.Ascending; 
      }
    }

    /// <summary>
    /// Returns the PropertyDescriptor of the current sort.
    /// </summary>
    public PropertyDescriptor SortProperty
    {
      get 
      {
        if (SupportsSorting)
          return _bindingList.SortProperty;
        else
          return null; 
      }
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
      get 
      {
        if (_supportsBinding)
          return _bindingList.SupportsSorting;
        else
          return false; 
      }
    }

    /// <summary>
    /// Copies the contents of the list to
    /// an array.
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
    /// Gets the number of items in the list.
    /// </summary>
    public int Count
    {
      get 
      {
        if (_filtered)
          return _filterIndex.Count;
        else
          return _list.Count; 
      }
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
    /// Adds an item to the list.
    /// </summary>
    /// <param name="item">Item to be added.</param>
    public void Add(T item)
    {
      _list.Add(item);
    }

    int System.Collections.IList.Add(object value)
    {
      Add((T)value);
      int index = FilteredIndex(_list.Count - 1);
      if (index > -1)
        return index;
      else
        return 0;
    }

    /// <summary>
    /// Clears the list.
    /// </summary>
    public void Clear()
    {
      if (_filtered)
        for (int index = Count - 1; index >= 0; index--)
          RemoveAt(index);
      else
        _list.Clear();
    }

    /// <summary>
    /// Determines whether the specified
    /// item is contained in the list.
    /// </summary>
    /// <param name="item">Item to find.</param>
    /// <returns>true if the item is
    /// contained in the list.</returns>
    public bool Contains(T item)
    {
      return _list.Contains(item);
    }

    bool System.Collections.IList.Contains(object value)
    {
      return Contains((T)value);
    }

    /// <summary>
    /// Gets the 0-based index of an
    /// item in the list.
    /// </summary>
    /// <param name="item">The item to find.</param>
    /// <returns>0-based index of the item
    /// in the list.</returns>
    public int IndexOf(T item)
    {
      return FilteredIndex(_list.IndexOf(item));
    }

    int System.Collections.IList.IndexOf(object value)
    {
      return IndexOf((T)value);
    }

    /// <summary>
    /// Inserts an item into the list.
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
    /// Gets a value indicating whether the list
    /// is read-only.
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
    /// Removes an item from the list.
    /// </summary>
    /// <param name="item">Item to remove.</param>
    /// <returns>true if the 
    /// remove succeeds.</returns>
    public bool Remove(T item)
    {
      return _list.Remove(item);
    }

    void System.Collections.IList.Remove(object value)
    {
      Remove((T)value);
    }

    /// <summary>
    /// Removes an item from the list.
    /// </summary>
    /// <param name="index">Index of item
    /// to be removed.</param>
    public void RemoveAt(int index)
    {
      if (_filtered)
      {
        _list.RemoveAt(OriginalIndex(index));
      }
      else
        _list.RemoveAt(index);
    }

    /// <summary>
    /// Gets or sets the item at 
    /// the specified index.
    /// </summary>
    /// <param name="index">Index of the item.</param>
    /// <returns>Item at the specified index.</returns>
    public T this[int index]
    {
      get
      {
        if (_filtered)
        {
          int src = OriginalIndex(index);
          return _list[src];
        }
        else
          return _list[index];
      }
      set
      {
        if (_filtered)
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
    private bool _filtered;
    private PropertyDescriptor _filterBy;
    private object _filter;
    FilterProvider _provider = null;
    private List<ListItem> _filterIndex = 
      new List<ListItem>();

    /// <summary>
    /// Creates a new view based on the provided IList object.
    /// </summary>
    /// <param name="list">The IList (collection) containing the data.</param>
    public FilteredBindingList(IList<T> list)
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

    /// <summary>
    /// Creates a new view based on the provided IList object.
    /// </summary>
    /// <param name="list">The IList (collection) containing the data.</param>
    /// <param name="filterProvider">
    /// Delegate pointer to a method that implements the filter behavior.
    /// </param>
    public FilteredBindingList(IList<T> list, FilterProvider filterProvider) : this(list)
    {
      _provider = filterProvider;
    }

    /// <summary>
    /// Gets or sets the filter provider method.
    /// </summary>
    /// <value>
    /// Delegate pointer to a method that implements the filter behavior.
    /// </value>
    /// <returns>
    /// Delegate pointer to a method that implements the filter behavior.
    /// </returns>
    /// <remarks>
    /// If this value is set to Nothing (null in C#) then the default
    /// filter provider, <see cref="DefaultFilter" /> will be used.
    /// </remarks>
    public FilterProvider FilterProvider
    {
      get
      {
        return _provider;
      }
      set
      {
        _provider = value;
      }
    }

    /// <summary>
    /// The property on which the items will be filtered.
    /// </summary>
    /// <value>A descriptor for the property on which
    /// the items in the collection will be filtered.</value>
    /// <returns></returns>
    /// <remarks></remarks>
    public PropertyDescriptor FilterProperty
    {
      get { return _filterBy; }
    }

    /// <summary>
    /// Returns True if the view is currently filtered.
    /// </summary>
    public bool IsFiltered
    {
      get { return _filtered; }
    }

    /// <summary>
    /// Applies a filter to the view using the
    /// most recently used property name and
    /// filter provider.
    /// </summary>
    public void ApplyFilter()
    {
      if (_filterBy == null || _filter == null)
        throw new ArgumentNullException(Resources.FilterRequiredException);
      DoFilter();
    }

    /// <summary>
    /// Applies a filter to the view.
    /// </summary>
    /// <param name="propertyName">The text name of the property on which to filter.</param>
    /// <param name="filter">The filter criteria.</param>
    public void ApplyFilter(string propertyName, object filter)
    {
      _filterBy = null;
      _filter = filter;

      if (!String.IsNullOrEmpty(propertyName))
      {
        Type itemType = typeof(T);
        foreach (PropertyDescriptor prop in
          TypeDescriptor.GetProperties(itemType))
        {
          if (prop.Name == propertyName)
          {
            _filterBy = prop;
            break;
          }
        }
      }

      ApplyFilter(_filterBy, filter);

    }

    /// <summary>
    /// Applies a filter to the view.
    /// </summary>
    /// <param name="property">A PropertyDescriptor for the property on which to filter.</param>
    /// <param name="filter">The filter criteria.</param>
    public void ApplyFilter(
      PropertyDescriptor property, object filter)
    {
      _filterBy = property;
      _filter = filter;
      DoFilter();
    }

    /// <summary>
    /// Removes the filter from the list,
    /// so the view reflects the state of
    /// the original list.
    /// </summary>
    public void RemoveFilter()
    {
      UnDoFilter();
    }

    private void SourceChanged(
      object sender, ListChangedEventArgs e)
    {
      if (_filtered)
      {
        int listIndex;
        int filteredIndex = -1;
        T newItem;
        object newKey;
        switch (e.ListChangedType)
        {
          case ListChangedType.ItemAdded:
            listIndex = e.NewIndex;
            // add new value to index
            newItem = _list[listIndex];
            if (_filterBy != null)
              newKey = _filterBy.GetValue(newItem);
            else
              newKey = newItem;
            _filterIndex.Add(
              new ListItem(newKey, listIndex));
            filteredIndex = _filterIndex.Count - 1;
            // raise event 
            OnListChanged(
              new ListChangedEventArgs(
              e.ListChangedType, filteredIndex));
            break;

          case ListChangedType.ItemChanged:
            listIndex = e.NewIndex;
            // update index value
            filteredIndex = FilteredIndex(listIndex);
            if (filteredIndex != -1)
            {
              newItem = _list[listIndex];
              if (_filterBy != null)
                newKey = _filterBy.GetValue(newItem);
              else
                newKey = newItem;
              _filterIndex[filteredIndex] =
                new ListItem(newKey, listIndex);
            }
            // raise event if appropriate
            if (filteredIndex > -1)
              OnListChanged(
                new ListChangedEventArgs(
                e.ListChangedType, filteredIndex, e.PropertyDescriptor));
            break;

          case ListChangedType.ItemDeleted:
            listIndex = e.NewIndex;
            // delete corresponding item from index
            // (if any)
            filteredIndex = FilteredIndex(listIndex);
            if (filteredIndex != -1)
              _filterIndex.RemoveAt(filteredIndex);
            // adjust index xref values
            foreach (ListItem item in _filterIndex)
              if (item.BaseIndex > e.NewIndex)
                item.BaseIndex--;
            // raise event if appropriate
            if (filteredIndex > -1)
              OnListChanged(
                new ListChangedEventArgs(
                e.ListChangedType, filteredIndex));
            break;

          case ListChangedType.PropertyDescriptorAdded:
          case ListChangedType.PropertyDescriptorChanged:
          case ListChangedType.PropertyDescriptorDeleted:
            OnListChanged(e);
            break;

          default:
            DoFilter();
            OnListChanged(
              new ListChangedEventArgs(
              ListChangedType.Reset, 0));
            break;
        }
      }
      else
        OnListChanged(e);
    }

    private int OriginalIndex(int filteredIndex)
    {
      if (_filtered)
        return _filterIndex[filteredIndex].BaseIndex;
      else
        return filteredIndex;
    }

    private int FilteredIndex(int originalIndex)
    {
      int result = -1;
      if (_filtered)
      {
        for (int index = 0; index < _filterIndex.Count; index++)
        {
          if (_filterIndex[index].BaseIndex == originalIndex)
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

    //private T _newItem;

    //void ICancelAddNew.CancelNew(int itemIndex)
    //{
    //  if (_newItem != null)
    //    Remove(_newItem);
    //}

    //void ICancelAddNew.EndNew(int itemIndex)
    //{
    //  // do nothing
    //}

    void ICancelAddNew.CancelNew(int itemIndex)
    {
      if (itemIndex <= -1) return;

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
