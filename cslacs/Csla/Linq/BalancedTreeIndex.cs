using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;
using Csla.Properties;

namespace Csla.Linq
{
  internal class BalancedTreeIndex<T> : IRangeTestableIndex<T>
  {
    private string _indexField = "";
    private object _index;
    private int _countCache = 0;

    PropertyInfo _theProp = null;
    IndexableAttribute _indexAttribute = null;
    private bool _loaded = false;

    private BalancedTreeIndex() {}

    public BalancedTreeIndex(string indexField, IndexableAttribute indexAttribute)
    {
      _indexField = indexField;
      _theProp = typeof(T).GetProperty(_indexField);
      _indexAttribute = indexAttribute;
      var propType = _theProp.PropertyType;
      var propInfo = typeof(T).GetProperty(indexField);

      var classTypeWrapper = Type.GetType("Csla.Linq.RedBlackTreeWrapper`2");
      var typeParamsWrapper = new Type[] { propType, typeof(T) };
      var constructedTypeWrapper = classTypeWrapper.MakeGenericType(typeParamsWrapper);
      var ctorParamsWrapper = new object[] { propInfo };
      _index = Activator.CreateInstance(constructedTypeWrapper, ctorParamsWrapper);


      if (_indexAttribute.IndexMode == IndexModeEnum.IndexModeAlways)
        _loaded = true;
    }

    private void LoadOnDemandIndex()
    {
      if (!_loaded && _indexAttribute.IndexMode != IndexModeEnum.IndexModeNever)
        ((IIndex<T>)this).LoadComplete();
    }

    #region IIndex<T> Members

    public System.Reflection.PropertyInfo IndexField
    {
      get { return _theProp; }
    }

    public IEnumerable<T> WhereEqual(T item)
    {

      LoadOnDemandIndex();
      var pivotVal = _theProp.GetValue(item, null);
      var balancedIndex = (IBalancedSearch<T>)_index;
      return balancedIndex.ItemsEqualTo(pivotVal);

    }

    public IEnumerable<T> WhereEqual(object pivotVal, Func<T, bool> expr)
    {
      var balancedIndex = (IBalancedSearch<T>)_index;
      return balancedIndex.ItemsEqualTo(pivotVal);
    }

    private void RemoveByReference(T item)
    {
      foreach (T itemToCheck in this)
        if (ReferenceEquals(itemToCheck, item))
          (this as ICollection<T>).Remove(item);
    }

    public void ReIndex(T item)
    {
      bool wasRemoved = (this as System.Collections.Generic.ICollection<T>).Remove(item);
      if (!wasRemoved) RemoveByReference(item);
      (this as ICollection<T>).Add(item);
    }

    public bool Loaded
    {
      get
      {
        return _loaded;
      }
    }

    public void InvalidateIndex()
    {
      if (_indexAttribute.IndexMode != IndexModeEnum.IndexModeNever)
        _loaded = false;
    }

    public void LoadComplete()
    {
      if (_indexAttribute.IndexMode != IndexModeEnum.IndexModeNever)
        _loaded = true;
    }

    public IndexModeEnum IndexMode
    {
      get
      {
        return _indexAttribute.IndexMode;
      }
      set
      {
        _indexAttribute.IndexMode = value;
      }
    }

    #endregion

    #region ICollection<T> Members

    private void DoAdd(T item)
    {
      if (_theProp != null)
      {
        ((ICollection<T>)_index).Add(item);
        _countCache++;
      }
    }

    public void Add(T item)
    {
      DoAdd(item);
    }

    public void Clear()
    {
      ((ICollection<T>)_index).Clear();
    }

    public bool Contains(T item)
    {
      return ((ICollection<T>)_index).Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
      if (object.ReferenceEquals(array, null))
      {
        throw new ArgumentNullException(
            Resources.NullArrayReference,
            "array"
            );
      }

      if (arrayIndex < 0)
      {
        throw new ArgumentOutOfRangeException(
            Resources.IndexIsOutOfRange,
            "index"
            );
      }

      if (array.Rank > 1)
      {
        throw new ArgumentException(
            Resources.ArrayIsMultiDimensional,
            "array"
            );
      }

      foreach (object o in this)
      {
        array.SetValue(o, arrayIndex);
        arrayIndex++;
      }
    }

    public int Count
    {
      get { return _countCache; }
    }

    public bool IsReadOnly
    {
      get { return false; }
    }

    public bool Remove(T item)
    {
      var removed = ((ICollection<T>)_index).Remove(item);
      if (removed)
        _countCache--;
      return removed;
    }

    #endregion

    #region IEnumerable<T> Members

    public IEnumerator<T> GetEnumerator()
    {
      foreach (var item in (ICollection<T>)_index)
        yield return item;
    }

    #endregion

    #region IEnumerable Members

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }

    #endregion


    #region IRangeTestableIndex<T> Members

    public IEnumerable<T> WhereLessThan(object pivotVal)
    {
      var balancedIndex = (IBalancedSearch<T>)_index;
      return balancedIndex.ItemsLessThan(pivotVal);
    }

    public IEnumerable<T> WhereGreaterThan(object pivotVal)
    {
      var balancedIndex = (IBalancedSearch<T>)_index;
      return balancedIndex.ItemsGreaterThan(pivotVal);
    }

    public IEnumerable<T> WhereLessThanOrEqualTo(object pivotVal)
    {
      var balancedIndex = (IBalancedSearch<T>)_index;
      return balancedIndex.ItemsLessThanOrEqualTo(pivotVal);
    }

    public IEnumerable<T> WhereGreaterThanOrEqualTo(object pivotVal)
    {
      var balancedIndex = (IBalancedSearch<T>)_index;
      return balancedIndex.ItemsGreaterThanOrEqualTo(pivotVal);
    }

    #endregion
  }
}
