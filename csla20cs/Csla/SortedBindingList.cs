using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace Csla
{
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
  public class SortedBindingList<T> : BindingList<T>
  {
    // Inspired by code from Michael Weinhardt
    // http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dnforms/html/winforms02182005.asp

    private IBindingList _source;

    public SortedBindingList()
    {
      _source.ListChanged += new ListChangedEventHandler(_source_ListChanged);
    }

    public SortedBindingList(IBindingList sourceList)
    {
      _source = sourceList;
      _source.ListChanged += new ListChangedEventHandler(_source_ListChanged);
      // copy references into this for sorting
      // without impacting the original list
      foreach (T item in _source)
        this.Add(item);
    }

    #region Sorting

    private bool _isSorted;
    private PropertyDescriptor _property;
    private ListSortDirection _direction;

    protected override void ApplySortCore(PropertyDescriptor prop, ListSortDirection direction)
    {
      List<T> items = (List<T>)this.Items;

      if (items != null)
      {
        PropertyComparer pc = new PropertyComparer(prop, direction);
        items.Sort(pc);
        _property = prop;
        _direction = direction;
        _isSorted = true;
      }
      else
        _isSorted = false;

      OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
    }

    protected override void RemoveSortCore()
    {
      _isSorted = false;
      if (_source != null)
      {
        this.Clear();
        foreach (T item in _source)
          this.Add(item);
      }
    }

    protected override bool SupportsSortingCore
    {
      get { return true; }
    }

    protected override bool IsSortedCore
    {
      get { return _isSorted; }
    }

    protected override ListSortDirection SortDirectionCore
    {
      get { return _direction; }
    }

    protected override PropertyDescriptor SortPropertyCore
    {
      get { return _property; }
    }

    #endregion

    #region PropertyComparer class

    public class PropertyComparer : System.Collections.Generic.IComparer<T>
    {
      private PropertyDescriptor _property;
      private ListSortDirection _direction;

      public PropertyComparer(PropertyDescriptor property, ListSortDirection direction)
      {
        _property = property;
        _direction = direction;
      }

      public int Compare(T val1, T val2)
      {
        object prop1 = Utilities.CallByName(val1, _property.Name, CallType.Get);
        object prop2 = Utilities.CallByName(val2, _property.Name, CallType.Get);

        if (_direction == ListSortDirection.Ascending)
          return CompareValues(prop1, prop2);
        else
          return CompareValues(prop1, prop2) * -1;
      }

      private int CompareValues(object val1, object val2)
      {
        int result;
        if (val1 is IComparable)
          result = ((IComparable)val1).CompareTo(val2);
        else if (val1.Equals(val2))
          result = 0;
        else
          result = val1.ToString().CompareTo(val2.ToString());
        return result;
      }

    }

    #endregion

    private void _source_ListChanged(object sender, ListChangedEventArgs e)
    {
      switch (e.ListChangedType)
      {
        case ListChangedType.ItemAdded:
          this.Add((T)_source[e.NewIndex]);
          this.ApplySortCore(this.SortPropertyCore, this.SortDirectionCore);
          break;

        case ListChangedType.ItemChanged:
          this.ApplySortCore(this.SortPropertyCore, this.SortDirectionCore);
          break;

        case ListChangedType.Reset:
          this.Clear();
          foreach (T item in _source)
            this.Add(item);
          break;
      }
    }
  }
}