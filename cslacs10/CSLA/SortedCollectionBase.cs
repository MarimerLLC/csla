using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;

/// <summary>
///
/// </summary>
namespace CSLA.Core
{
  /// <summary>
  /// This class implements sorting functionality for collections.
  /// </summary>
  /// <remarks>
  /// <para>
  /// This class inhirits from Core.BindableCollectionBase and adds
  /// sorting capability to collections. BusinessCollectionBase inherits
  /// from this class, and business collections should inherit from
  /// BusinessCollectionBase. Core.SortedCollectionBase is for internal
  /// framework use only.
  /// </para><para>
  /// The Core.BindableCollectionBase class implements the IBindableList
  /// interface. However, it doesn't actually implement sorting. Instead
  /// it delegates the sorting functionality to a set of protected virtual 
  /// methods. This class provides the actual sorting implementation
  /// by overriding those methods.
  /// </para>
  /// </remarks>
  [Serializable()]
  public class SortableCollectionBase : BindableCollectionBase
  {
    [NotUndoable()]
    private bool _isSorted = false;
    [NonSerialized()]
    [NotUndoable()]
    private PropertyDescriptor _sortProperty;
    [NotUndoable()]
    private string _sortPropertyName = string.Empty;
    [NotUndoable()]
    private ListSortDirection _listSortDirection = ListSortDirection.Ascending;
    [NotUndoable()]
    private ArrayList _unsortedList;
    [NotUndoable()]
    private bool _activelySorting = false;

    #region Properties

    /// <summary>
    /// Indicates whether the collection is in the process of
    /// being sorted at this time.
    /// </summary>
    protected bool ActivelySorting
    {
      get
      {
        return _activelySorting;
      }
    }

    /// <summary>
    /// Returns a value indicating whether the collection is currently
    /// sorted.
    /// </summary>
    protected override bool IBindingList_IsSorted
    {
      get
      {
        return _isSorted;
      }
    }

    /// <summary>
    /// Returns the property by which the collection is currently sorted.
    /// </summary>
    /// <remarks>
    /// This method is invoked via the IBindingList interface and is not
    /// intended for use by code in your business class.
    /// </remarks>
    protected override PropertyDescriptor IBindingList_SortProperty
    {
      get
      {
        if(_sortProperty == null && _sortPropertyName.Length > 0)
        {
          try
          {
            // we need to recreate the sortproperty value
            Type childType;
            if(List.Count > 0)
            {
              // get child type from the first element in the collection
              childType = List[0].GetType();
            }
            else
            {
              // get child type from Item property
              try
              {
                Type [] param = {typeof(int)};
                childType = this.GetType().GetProperty("Item", param).PropertyType;
              }
              catch
              {
                childType = typeof(object);
              }
            }

            // now get the property descriptor from the type
            _sortProperty = TypeDescriptor.GetProperties(childType)[_sortPropertyName];
          }
          catch
          {
            // we failed to recreate it - return nothing
            _sortProperty = null;
          }
        }
        return _sortProperty;
      }
    }

    /// <summary>
    /// Returns the current sort direction.
    /// </summary>
    /// <remarks>
    /// This method is invoked via the IBindingList interface and is not
    /// intended for use by code in your business class.
    /// </remarks>
    protected override ListSortDirection IBindingList_SortDirection
    {
      get
      {
        return _listSortDirection;
      }
    }

    #endregion

    #region ApplySort

    /// <summary>
    /// Structure to store temporary data for sorting.
    /// </summary>
    private struct SortData
    {
      private object _key;
      private object _value;

      public SortData(object key, object value)
      {
        _key = key;
        _value = value;
      }

      public object Value
      {
        get
        {
          return _value;
        }
      }

      public object Key
      {
        get
        {
          if(IsNumeric(_key) || _key is string)
            return _key;
          else
            return _key.ToString();
        }
      }
    }

    /// <summary>
    /// Contains code to compare SortData structures
    /// </summary>
    /// <remarks>
    /// This performs a case sensitive comparison. If you want a case insensitive
    /// comparison, change the code to use CaseInsensitiveComparer.Default instead.
    /// </remarks>
    private class SortDataCompare : IComparer
    {
      public int Compare(object x, object y)
      {
        SortData item1 = (SortData)x;
        SortData item2 = (SortData)y;

        return Comparer.Default.Compare(item1.Key, item2.Key);
      }
    }

    /// <summary>
    /// Applies a sort to the collection.
    /// </summary>
    /// <remarks>
    /// This method is invoked via the IBindingList interface and is not
    /// intended for use by code in your business class.
    /// </remarks>
    protected override void IBindingList_ApplySort(System.ComponentModel.PropertyDescriptor property, System.ComponentModel.ListSortDirection direction)
    {
      if(!AllowSort)
        throw new NotSupportedException("Sorting is not supported by this collection.");

      _sortProperty = property;
      _sortPropertyName = _sortProperty.Name;
      _listSortDirection = direction;

      if(!_isSorted && List.Count > 0)
      {
        // this is our first time sorting so
        // make sure to store the original order
        _unsortedList = new ArrayList();
        foreach(object item in List)
          _unsortedList.Add(item);
      }

      if(List.Count > 1)
      {
        try
        {
          _activelySorting = true;

          // copy the key/value pairs into a sorted list
          ArrayList sortList = new ArrayList();
          for(int count = 0; count < List.Count; count++)
            sortList.Add(new SortData(CallByName(List[count], _sortPropertyName, CallType.Get), List[count]));
          sortList.Sort(new SortDataCompare());

          List.Clear();

          if(direction == ListSortDirection.Ascending)
          {
            foreach(SortData item in sortList)
              List.Add(item.Value);
          }
          else // direction = ListSortDirection.Descending
          {
            SortData item;
            for(int count = sortList.Count - 1; count >= 0; count--)
            {
              item = (SortData)sortList[count];
              List.Add(item.Value);
            }
          }

          _isSorted = true;
        }
        catch
        {
          IBindingList_RemoveSort();
        }
        finally
        {
          _activelySorting = false;
        }
      }
      else
        if(List.Count == 1)
        _isSorted = true;
    }

    #endregion

    #region Utils

    private static bool IsNumeric(object value)
    {
      double dbl;
      return double.TryParse(value.ToString(), System.Globalization.NumberStyles.Any, 
        System.Globalization.NumberFormatInfo.InvariantInfo, out dbl);
    }

    private enum CallType
    {
      Get,
      Let,
      Method,
      Set
    }

    private static object CallByName(object target, string methodName, CallType callType, params object [] args)
    {
      switch(callType)
      {
        case CallType.Get:
        {
          PropertyInfo p = target.GetType().GetProperty(methodName);
          return p.GetValue(target, args);
        }
        case CallType.Let:
        case CallType.Set:
        {
          PropertyInfo p = target.GetType().GetProperty(methodName);
          object [] index = null;
          args.CopyTo(index, 1);
          p.SetValue(target, args[0], index);
          return null;
        }
        case CallType.Method:
        {
          MethodInfo m = target.GetType().GetMethod(methodName);
          return m.Invoke(target, args);
        }
      }
      return null;
    }

    #endregion

    #region RemoveSort

    /// <summary>
    /// Removes any sort from the collection.
    /// </summary>
    /// <remarks>
    /// This method is invoked via the IBindingList interface and is not
    /// intended for use by code in your business class.
    /// </remarks>
    protected override void IBindingList_RemoveSort()
    {
      if(!AllowSort)
        throw new NotSupportedException("Sorting is not supported by this collection.");

      if(_isSorted)
      {
        _activelySorting = true;

        //Return the list to its unsorted state
        List.Clear();

        foreach(object item in _unsortedList)
          List.Add(item);

        _unsortedList = null;

        _isSorted = false;
        _sortProperty = null;
        _sortPropertyName = string.Empty;
        _listSortDirection = ListSortDirection.Ascending;
        _activelySorting = false;
      }
    }

    #endregion

    #region Collection events

    /// <summary>
    /// Ensures that any sort is maintained as a new item is inserted.
    /// </summary>
    protected override void OnInsertComplete(int index, object value)
    {
      if(_isSorted && !ActivelySorting)
        _unsortedList.Add(value);
      base.OnInsertComplete(index, value);
    }

    /// <summary>
    /// Ensures that any sort is maintained as the list is cleared.
    /// </summary>
    protected override void OnClearComplete()
    {
      if(_isSorted && !ActivelySorting)
        _unsortedList.Clear();
      base.OnClearComplete();
    }

    /// <summary>
    /// Ensures that any sort is maintained as an item is removed.
    /// </summary>
    protected override void OnRemoveComplete(int index, object value)
    {
      if(_isSorted && !ActivelySorting)
        _unsortedList.Remove(value);
      base.OnRemoveComplete(index, value);
    }

    #endregion

    #region Search/Find

    /// <summary>
    /// Implements search/find functionality for the collection.
    /// </summary>
    protected override int IBindingList_Find(PropertyDescriptor property, object key)
    {
      if(!AllowFind)
        throw new NotSupportedException("Searching is not supported by this collection.");

      object tmp;
      string prop = property.Name;

      for(int index = 0; index < List.Count; index++)
      {
        tmp = CallByName(List[index], prop, CallType.Get);
        if(tmp.Equals(key))
        {
          // we found a match
          return index;
        }
      }

      // we didn't find anything
      return -1;
    }

    #endregion

  }
}
