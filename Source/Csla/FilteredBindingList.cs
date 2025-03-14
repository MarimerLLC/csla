//-----------------------------------------------------------------------
// <copyright file="FilteredBindingList.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Provides a filtered view into an existing IList(Of T).</summary>
//-----------------------------------------------------------------------

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Collections;
using Csla.Properties;

namespace Csla
{
  /// <summary>
  /// Provides a filtered view into an existing IList(Of T).
  /// </summary>
  /// <typeparam name="T">The type of the objects contained
  /// in the original list.</typeparam>
  public class FilteredBindingList<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]T> :
    IList<T>, IBindingList, IEnumerable<T>,
    ICancelAddNew
    where T: notnull
  {

    #region ListItem class

    private class ListItem 
    {
      public object Key { get; }

      public int BaseIndex { get; set; }

      public ListItem(object key, int baseIndex)
      {
        Key = key;
        BaseIndex = baseIndex;
      }

      public override string ToString()
      {
        return Key.ToString()!;
      }

    }

    #endregion

    #region Filtered enumerator

    private class FilteredEnumerator : IEnumerator<T>
    {
      private readonly IList<T> _list;
      private readonly List<ListItem> _filterIndex;
      private int _index;

      public FilteredEnumerator(IList<T> list, List<ListItem> filterIndex)
      {
        _list = list;
        _filterIndex = filterIndex;
        Reset();
      }

      public T Current => _list[_filterIndex[_index].BaseIndex];

      object IEnumerator.Current => _list[_filterIndex[_index].BaseIndex];

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

      public void Dispose()
      {
      }

      #endregion
    }

    #endregion

    #region Filter/Unfilter

    private void DoFilter()
    {
      int index = 0;
      _filterIndex.Clear();

      if (FilterProvider == null)
        FilterProvider = DefaultFilter.Filter;

      foreach (T obj in SourceList)
      {
        object objToFilter = obj;
        if (FilterProperty is not null)
        {
          var tmp = FilterProperty.GetValue(obj);
          if (tmp is null)
            continue;

          objToFilter = tmp;
        }

        if (FilterProvider.Invoke(objToFilter, _filter))
          _filterIndex.Add(new ListItem(objToFilter, index));
        index++;
      }

      IsFiltered = true;

      OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, 0));
    }

    private void UnDoFilter()
    {
      _filterIndex.Clear();
      FilterProperty = null;
      _filter = null;
      IsFiltered = false;

      OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, 0));
    }

    #endregion

    #region IEnumerable<T>

    /// <summary>
    /// Gets an enumerator object.
    /// </summary>
    public IEnumerator<T> GetEnumerator()
    {
      if (IsFiltered)
        return new FilteredEnumerator(SourceList, _filterIndex);
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
      T? result;
      if (SupportsBinding)
        result = (T?)_bindingList!.AddNew();
      else
        result = default(T);

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
    /// Sorts the list if the original list
    /// supports sorting.
    /// </summary>
    /// <param name="property">Property on which to sort.</param>
    /// <param name="direction">Direction of the sort.</param>
    /// <exception cref="ArgumentNullException"><paramref name="property"/> is <see langword="null"/>.</exception>
    public void ApplySort(PropertyDescriptor property, ListSortDirection direction)
    {
      if (!SupportsSorting)
        throw new NotSupportedException(Resources.SortingNotSupported);
      
      if (property is null)
        throw new ArgumentNullException(nameof(property));
      
      _bindingList!.ApplySort(property, direction);
    }

    /// <summary>
    /// Sorts the list if the original list
    /// supports sorting.
    /// </summary>
    /// <param name="propertyName">PropertyName on which to sort.</param>
    /// <param name="direction">Direction of the sort.</param>
    /// <exception cref="ArgumentException"><paramref name="propertyName"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    public void ApplySort(string propertyName, ListSortDirection direction)
    {
      if (!SupportsSorting)
        throw new NotSupportedException(Resources.SortingNotSupported);

      if (string.IsNullOrWhiteSpace(propertyName))
        throw new ArgumentException(string.Format(Resources.StringNotNullOrWhiteSpaceException, nameof(propertyName)), nameof(propertyName));

      var property = GetPropertyDescriptor(propertyName);
      _bindingList!.ApplySort(property, direction);
    }

    /// <summary>
    /// Gets the property descriptor.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns>PropertyDescriptor</returns>
    private static PropertyDescriptor GetPropertyDescriptor(string propertyName)
    {
      Type itemType = typeof(T);
      foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(itemType))
      {
        if (prop.Name == propertyName)
          return prop;
      }

      // throw exception if propertyDescriptor could not be found
      throw new ArgumentException(string.Format(Resources.SortedBindingListPropertyNameNotFound, propertyName), propertyName);
    }
    
    /// <summary>
    /// Finds an item in the view
    /// </summary>
    /// <param name="propertyName">Name of the property to search</param>
    /// <param name="key">Value to find</param>
    public int Find(string propertyName, object key)
    {
      if (string.IsNullOrWhiteSpace(propertyName))
        throw new ArgumentException(string.Format(Resources.StringNotNullOrWhiteSpaceException, nameof(propertyName)), nameof(propertyName));

      var findProperty = GetPropertyDescriptor(propertyName);
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
      if (property is null)
        throw new ArgumentNullException(nameof(property));

      if (SupportsBinding)
        return FilteredIndex(_bindingList!.Find(property, key));
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
          return _bindingList!.IsSorted;
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
    public event ListChangedEventHandler? ListChanged;

    /// <summary>
    /// Raises the ListChanged event.
    /// </summary>
    /// <param name="e">Parameter for the event.</param>
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
      if (SupportsSorting)
        _bindingList!.RemoveSort();
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
          return _bindingList!.SortDirection;
        else
          return ListSortDirection.Ascending; 
      }
    }

    /// <summary>
    /// Returns the PropertyDescriptor of the current sort.
    /// </summary>
    public PropertyDescriptor? SortProperty
    {
      get 
      {
        if (SupportsSorting)
          return _bindingList!.SortProperty;
        else
          return null; 
      }
    }

    /// <summary>
    /// Returns True since this object does raise the
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
    /// Returns True. Sorting is supported.
    /// </summary>
    public bool SupportsSorting
    {
      get 
      {
        if (SupportsBinding)
          return _bindingList!.SupportsSorting;
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

    /// <inheritdoc />
    void ICollection.CopyTo(Array array, int index)
    {
      if (array is null)
        throw new ArgumentNullException(nameof(array));

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
        if (IsFiltered)
          return _filterIndex.Count;
        else
          return SourceList.Count; 
      }
    }

    bool ICollection.IsSynchronized => false;

    object ICollection.SyncRoot => SourceList;

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    /// <summary>
    /// Adds an item to the list.
    /// </summary>
    /// <param name="item">Item to be added.</param>
    /// <exception cref="ArgumentNullException"><paramref name="item"/> is <see langword="null"/>.</exception>
    public void Add(T item)
    {
      if (item is null)
        throw new ArgumentNullException(nameof(item));

      SourceList.Add(item);
    }

    int IList.Add(object? value)
    {
      if (value is null)
        throw new ArgumentNullException(nameof(value));

      Add((T)value);
      int index = FilteredIndex(SourceList.Count - 1);
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
      if (IsFiltered)
        for (int index = Count - 1; index >= 0; index--)
          RemoveAt(index);
      else
        SourceList.Clear();
    }

    /// <summary>
    /// Determines whether the specified
    /// item is contained in the list.
    /// </summary>
    /// <param name="item">Item to find.</param>
    /// <returns>true if the item is
    /// contained in the list.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="item"/> is <see langword="null"/>.</exception>
    public bool Contains(T item)
    {
      if(item is null) 
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
    /// Gets the 0-based index of an
    /// item in the list.
    /// </summary>
    /// <param name="item">The item to find.</param>
    /// <returns>0-based index of the item
    /// in the list.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="item"/> is <see langword="null"/>.</exception>
    public int IndexOf(T item)
    {
      if (item is null)
        throw new ArgumentNullException(nameof(item));

      return FilteredIndex(SourceList.IndexOf(item));
    }

    int IList.IndexOf(object? value)
    {
      if (value is null)
        throw new ArgumentNullException(nameof(value));

      return IndexOf((T)value);
    }

    /// <summary>
    /// Inserts an item into the list.
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
    /// Gets a value indicating whether the list
    /// is read-only.
    /// </summary>
    public bool IsReadOnly => SourceList.IsReadOnly;

    object? IList.this[int index]
    {
      get => this[index];
      set => this[index] = (T)(value ?? throw new ArgumentNullException(nameof(value)));
    }

    /// <summary>
    /// Removes an item from the list.
    /// </summary>
    /// <param name="item">Item to remove.</param>
    /// <returns>true if the 
    /// remove succeeds.</returns>
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
    /// Removes an item from the list.
    /// </summary>
    /// <param name="index">Index of item
    /// to be removed.</param>
    public void RemoveAt(int index)
    {
      if (IsFiltered)
      {
        SourceList.RemoveAt(OriginalIndex(index));
      }
      else
        SourceList.RemoveAt(index);
    }

    /// <summary>
    /// Gets or sets the item at 
    /// the specified index.
    /// </summary>
    /// <param name="index">Index of the item.</param>
    /// <returns>Item at the specified index.</returns>
    /// <exception cref="ArgumentNullException">value is <see langword="null"/>.</exception>
    public T this[int index]
    {
      get
      {
        if (IsFiltered)
        {
          int src = OriginalIndex(index);
          return SourceList[src];
        }
        else
          return SourceList[index];
      }
      set
      {
        if(value is null) 
          throw new ArgumentNullException(nameof(value));

        if (IsFiltered)
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
    private bool SupportsBinding { get; set; }
    private IBindingList? _bindingList;
    private object? _filter;

    private readonly List<ListItem> _filterIndex = [];

    /// <summary>
    /// Creates a new view based on the provided IList object.
    /// </summary>
    /// <param name="list">The IList (collection) containing the data.</param>
    /// <exception cref="ArgumentNullException"><paramref name="list"/> is <see langword="null"/>.</exception>
    public FilteredBindingList(IList<T> list)
    {
      SourceList = list ?? throw new ArgumentNullException(nameof(list));

      if (SourceList is IBindingList sourceList)
      {
        SupportsBinding = true;
        _bindingList = sourceList;
        _bindingList!.ListChanged += SourceChanged;
      }
    }

    /// <summary>
    /// Creates a new view based on the provided IList object.
    /// </summary>
    /// <param name="list">The IList (collection) containing the data.</param>
    /// <param name="filterProvider">
    /// Delegate pointer to a method that implements the filter behavior.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="list"/> is <see langword="null"/>.</exception>
    public FilteredBindingList(IList<T> list, FilterProvider? filterProvider) : this(list)
    {
      FilterProvider = filterProvider;
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
    public FilterProvider? FilterProvider { get; set; } = null;

    /// <summary>
    /// The property on which the items will be filtered.
    /// </summary>
    /// <value>A descriptor for the property on which
    /// the items in the collection will be filtered.</value>
    public PropertyDescriptor? FilterProperty { get; private set; }

    /// <summary>
    /// Returns True if the view is currently filtered.
    /// </summary>
    public bool IsFiltered { get; private set; }

    /// <summary>
    /// Applies a filter to the view using the
    /// most recently used property name and
    /// filter provider.
    /// </summary>
    /// <exception cref="ArgumentNullException"><see cref="FilterProperty"/> or filter <see langword="null"/>.</exception>
    public void ApplyFilter()
    {
      if (FilterProperty == null || _filter == null)
        throw new ArgumentNullException(Resources.FilterRequiredException);
      DoFilter();
    }

    /// <summary>
    /// Applies a filter to the view.
    /// </summary>
    /// <param name="propertyName">The text name of the property on which to filter.</param>
    /// <param name="filter">The filter criteria.</param>
    /// <exception cref="ArgumentNullException"><paramref name="filter"/> is <see langword="null"/>.</exception>
    public void ApplyFilter(string? propertyName, object filter)
    {
      FilterProperty = null;
      _filter = filter ?? throw new ArgumentNullException(nameof(filter));

      if (!string.IsNullOrEmpty(propertyName))
      {
        Type itemType = typeof(T);
        foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(itemType))
        {
          if (prop.Name == propertyName)
          {
            FilterProperty = prop;
            break;
          }
        }
      }

      ApplyFilter(FilterProperty, filter);

    }

    /// <summary>
    /// Applies a filter to the view.
    /// </summary>
    /// <param name="property">A PropertyDescriptor for the property on which to filter.</param>
    /// <param name="filter">The filter criteria.</param>
    public void ApplyFilter(PropertyDescriptor? property, object filter)
    {
      FilterProperty = property;
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

    private void SourceChanged(object? sender, ListChangedEventArgs e)
    {
      if (IsFiltered)
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
            newItem = SourceList[listIndex];
            if (FilterProperty != null)
              newKey = FilterProperty.GetValue(newItem) ?? throw new InvalidOperationException();
            else
              newKey = newItem;
            _filterIndex.Add(new ListItem(newKey, listIndex));
            filteredIndex = _filterIndex.Count - 1;
            // raise event 
            OnListChanged(new ListChangedEventArgs(e.ListChangedType, filteredIndex));
            break;

          case ListChangedType.ItemChanged:
            listIndex = e.NewIndex;
            // update index value
            filteredIndex = FilteredIndex(listIndex);
            if (filteredIndex != -1)
            {
              newItem = SourceList[listIndex];
              if (FilterProperty != null)
                newKey = FilterProperty.GetValue(newItem) ?? throw new InvalidOperationException();
              else
                newKey = newItem;
              _filterIndex[filteredIndex] = new ListItem(newKey, listIndex);
            }
            // raise event if appropriate
            if (filteredIndex > -1)
              OnListChanged(new ListChangedEventArgs(e.ListChangedType, filteredIndex, e.PropertyDescriptor));
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
      if (IsFiltered)
        return _filterIndex[filteredIndex].BaseIndex;
      else
        return filteredIndex;
    }

    private int FilteredIndex(int originalIndex)
    {
      int result = -1;
      if (IsFiltered)
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
