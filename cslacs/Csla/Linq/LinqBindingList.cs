using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Reflection;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using Csla.Core;
using Csla.Properties;

namespace Csla
{
  /// <summary>
  /// Provides a filtered view into an existing IList(Of T).
  /// </summary>
  /// <typeparam name="T">The type of the objects contained
  /// in the original list.</typeparam>
  public class LinqBindingList<T> :
    IList<T>, IBindingList, IEnumerable<T>,
    ICancelAddNew, IQueryable<T>,IOrderedQueryable<T>
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

    #region Filter/Unfilter
    /// <summary>
    /// Applies a filter from the original Linq query to the view 
    /// </summary>
    public void ApplyFilter()
    {
      BuildFilterIndex();
      OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, 0));
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
      foreach (ListItem listItem in _filterIndex)
      {
        array[pos] = _list[listItem.BaseIndex];
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
        return _filterIndex.Count;
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
      _list.Clear();
    }

    /// <summary>
    /// Determines whether the specified
    /// item is contained in the list.
    /// </summary>
    /// <param name="item">Item to find.</param>
    /// <returns><see langword="true"/> if the item is
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
    /// <returns><see langword="true"/> if the 
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
      _list.RemoveAt(OriginalIndex(index));
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
        int src = OriginalIndex(index);
        return _list[src];
      }
      set
      {
        _list[OriginalIndex(index)] = value;
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
    private PropertyDescriptor _filterBy;
    private Expression _sortExpression;
    private List<ListItem> _filterIndex = 
      new List<ListItem>();

    /// <summary>
    /// Creates a new view based on the provided IList object.
    /// </summary>
    /// <param name="list">The IList (collection) containing the data.</param>
    public LinqBindingList(IList<T> list)
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

    Expression _expression;
    IQueryProvider _queryProvider;

    internal LinqBindingList(IList<T> list, IQueryProvider queryProvider, Expression expression)
      : this(list)
    {
      SetFilterByExpression(expression);
      _queryProvider = queryProvider;
      if (expression == null)
        _expression = Expression.Constant(this);
      else
        _expression = expression;
    }

    private static Expression<Func<T, bool>> SelectAll()
    {
      return (T) => true;
    }

    private void SetFilterByExpression(Expression expression)
    {
      InnermostWhereFinder whereFinder = new InnermostWhereFinder();
      MethodCallExpression whereExpression = whereFinder.GetInnermostWhere(expression);
      if (whereExpression == null) return;
      if (whereExpression.Arguments.Count < 2) return;
      if (whereExpression.Arguments[1] == null) return;
      Expression<Func<T, bool>> whereBody = (Expression<Func<T, bool>>)((UnaryExpression)(whereExpression.Arguments[1])).Operand;
      if (whereBody.Body is BinaryExpression)
      {
        BinaryExpression binExp = (BinaryExpression)whereBody.Body;
        if (binExp.Left.NodeType == ExpressionType.MemberAccess && (binExp.Left as MemberExpression).Member.MemberType == MemberTypes.Property)
          _filterBy = TypeDescriptor.GetProperties(typeof(T))[(binExp.Left as MemberExpression).Member.Name];
      }
    }

    private bool ItemShouldBeInList(T item)
    {
      InnermostWhereFinder whereFinder = new InnermostWhereFinder();
      MethodCallExpression whereExpression = whereFinder.GetInnermostWhere(_expression);
      if (whereExpression == null) return false;
      if (whereExpression.Arguments.Count < 2) return false;
      if (whereExpression.Arguments[1] == null) return false;
      Expression<Func<T, bool>> whereBody = (Expression<Func<T, bool>>)((UnaryExpression)(whereExpression.Arguments[1])).Operand;

      var searchable = _list as Linq.IIndexSearchable<T>;
      if (searchable == null)
        return false;
      else
        return searchable.SearchByExpression(whereBody).Contains(item);
    }
    
    private void SourceChanged(
      object sender, ListChangedEventArgs e)
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

          //check to see if it is in the filter
          if (ItemShouldBeInList(newItem))
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
            if (ItemShouldBeInList(newItem))
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
          BuildFilterIndex();
          //DoFilter();
          OnListChanged(
            new ListChangedEventArgs(
            ListChangedType.Reset, 0));
          break;
      }
    }

    private int OriginalIndex(int filteredIndex)
    {
      return _filterIndex[filteredIndex].BaseIndex;
    }

    private int FilteredIndex(int originalIndex)
    {
      int result = -1;
      for (int index = 0; index < _filterIndex.Count; index++)
      {
        if (_filterIndex[index].BaseIndex == originalIndex)
        {
          result = index;
          break;
        }
      }
      return result;
    }

    #region ICancelAddNew Members

    void ICancelAddNew.CancelNew(int itemIndex)
    {
      ICancelAddNew can = _list as ICancelAddNew;
      if (can != null)
        can.CancelNew(itemIndex);
      else
        _list.RemoveAt(itemIndex);
    }

    void ICancelAddNew.EndNew(int itemIndex)
    {
      ICancelAddNew can = _list as ICancelAddNew;
      if (can != null)
        can.EndNew(itemIndex);
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

    internal void SortByExpression(Expression expression)
    {
      _sortExpression = expression;
    }
    
    internal void BuildFilterIndex()
    {
      // Find the call to Where() and get the lambda expression predicate.
      InnermostWhereFinder whereFinder = new InnermostWhereFinder();
      MethodCallExpression whereExpression = whereFinder.GetInnermostWhere(_expression);
      Expression<Func<T, bool>> whereBody = GetWhereBodyFromExpression(whereExpression);

      _filterIndex.Clear();
      if (_list is Linq.IIndexSearchable<T>)
        //before we can start, we do have to go through the whole thing once to make our filterindex.  
        foreach (T item in (_list as Linq.IIndexSearchable<T>).SearchByExpression(whereBody))
        {
          if (_filterBy != null)
          {
            object tmp = _filterBy.GetValue(item);
            _filterIndex.Add(new ListItem(tmp, ((IPositionMappable<T>)_list).PositionOf(item)));
          }
          else
          {
            _filterIndex.Add(new ListItem(item.GetHashCode(), ((IPositionMappable<T>)_list).PositionOf(item)));
          }

        }
    }

    private static Expression<Func<T, bool>> GetWhereBodyFromExpression(MethodCallExpression whereExpression)
    {
      Expression<Func<T, bool>> whereBody;
      if (whereExpression == null) whereBody = SelectAll();
      else if (whereExpression.Arguments.Count < 2) whereBody = SelectAll();
      else if (whereExpression.Arguments[1] == null) whereBody = SelectAll();
      else whereBody = (Expression<Func<T, bool>>)((UnaryExpression)(whereExpression.Arguments[1])).Operand;
      return whereBody;
    }

    internal List<T> ToList<TResult>()
    {
      var newList = new List<T>(this.Count);
      foreach (T item in this)
        newList.Add(item);
      return newList;
    }

    #region IEnumerable<T> Members

    /// <summary>
    /// Gets an enumerator object.
    /// </summary>
    /// <returns></returns>
    public IEnumerator<T> GetEnumerator()
    {
      if (_list is Linq.IIndexSearchable<T> && _list is Core.IPositionMappable<T>)
      {
        // Find the call to Where() and get the lambda expression predicate.
        InnermostWhereFinder whereFinder = new InnermostWhereFinder();
        MethodCallExpression whereExpression = whereFinder.GetInnermostWhere(_expression);
        Expression<Func<T, bool>> whereBody = GetWhereBodyFromExpression(whereExpression);

        var subset = (_list as Linq.IIndexSearchable<T>).SearchByExpression(whereBody);
        if (_sortExpression != null && _sortExpression is MethodCallExpression)
          subset = BuildSortedSubset(subset, (MethodCallExpression) _sortExpression);
        foreach (T item in subset)
          yield return item;
      }
      else
      {
        foreach (T item in _list)
          yield return item;
      }
    }

    private static IEnumerable<T> BuildSortedSubset(IEnumerable<T> subset, MethodCallExpression sortExpression)
    {
      //get the lambda buried in the second argument
      var lambda = (LambdaExpression)((UnaryExpression)sortExpression.Arguments[1]).Operand;
      //get the generic arguments of the lambda
      var genArgs = lambda.Type.GetGenericArguments();
      //get the sort method out via reflection
      var sortMethod = typeof(Queryable).GetMethods().Where(
        m =>
          m.Name == sortExpression.Method.Name &&
          m.GetParameters().Count() == 2
          ).First();
      //make a generic method using the generic arguments
      var genericSortMethod = sortMethod.MakeGenericMethod(genArgs);
      //replace the subset with the sorted subset
      subset = (IEnumerable<T>)genericSortMethod.Invoke(null, new object[] { subset.AsQueryable<T>(), lambda });
      return subset;
    }

    #endregion

    #region IQueryable Members

    Type IQueryable.ElementType
    {
      get { return typeof(T); }
    }

    Expression IQueryable.Expression
    {
      get { return _expression; }
    }

    IQueryProvider IQueryable.Provider
    {
      get { return _queryProvider; }
    }

    #endregion

  }
}
