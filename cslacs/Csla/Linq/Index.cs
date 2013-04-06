using System;
using System.Collections.Generic;
using System.Reflection;
using Csla.Properties;

namespace Csla.Linq
{
  internal class Index<T> : IIndex<T>
  {
    //implements a hashtable with chaining for cases
    //   where we have a collision on hash code

    private string _indexField = "";
    private Dictionary<int, List<T>> _index = new Dictionary<int, List<T>>();
    private int _countCache = 0;

    PropertyInfo _theProp = null;
    IndexableAttribute _indexAttribute = null;
    private bool _loaded = false;
    
    private Index() {}

    public Index(string indexField, IndexableAttribute indexAttribute)
    {
      _indexField = indexField;
      _theProp = typeof(T).GetProperty(_indexField);
      _indexAttribute = indexAttribute;
      if (_indexAttribute.IndexMode == IndexModeEnum.IndexModeAlways)
        _loaded = true;
    }

    private void LoadOnDemandIndex()
    {
      if (!_loaded && _indexAttribute.IndexMode != IndexModeEnum.IndexModeNever)
        ((IIndex<T>)this).LoadComplete();
    }

    #region IIndex<T> Members

    PropertyInfo IIndex<T>.IndexField
    {
      get { return _theProp; }
    }

    IEnumerable<T> IIndex<T>.WhereEqual(T item)
    {
      int hashCode = item.GetHashCode();
      IComparable propertyValue = _theProp.GetValue(item,null) as IComparable;
      LoadOnDemandIndex();
      if (_index.ContainsKey(hashCode))
      {
        foreach (T itemFromIndex in _index[hashCode])
        {
          IComparable propertyValueFromIndex 
            = _theProp.GetValue(itemFromIndex, null) as IComparable;
          if (propertyValue.Equals(propertyValueFromIndex))
            yield return itemFromIndex;
        }
      }         
    }

    IEnumerable<T> IIndex<T>.WhereEqual(object pivotVal, Func<T, bool> expr)
    {
      var hashCode = pivotVal.GetHashCode();
      LoadOnDemandIndex();
      if (_index.ContainsKey(hashCode))
        foreach (T item in _index[hashCode])
          if (expr(item))
            yield return item;
    }


    #endregion

    private void DoAdd(T item)
    {
      if (_theProp != null)
      {
        object value = _theProp.GetValue(item, null);
        if (value != null)
        {
          int hashCode = value.GetHashCode();
          if (_index.ContainsKey(hashCode))
            _index[hashCode].Add(item);
          else
          {
            List<T> newList = new List<T>(1);
            newList.Add(item);
            _index.Add(hashCode, newList);
          }
          _countCache++;
        }
      }
    }

    #region ICollection<T> Members

    void ICollection<T>.Add(T item)
    {
      DoAdd(item);
    }

    void ICollection<T>.Clear()
    {
      _index.Clear();
    }

    bool ICollection<T>.Contains(T item)
    {
      int hashCode = _theProp.GetValue(item, null).GetHashCode();
      if (_index.ContainsKey(hashCode))
        return _index[hashCode].Contains(item);
      else
        return false;
    }

    void ICollection<T>.CopyTo(T[] array, int arrayIndex)
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

    int ICollection<T>.Count
    {
      get { return _countCache; }
    }

    bool ICollection<T>.IsReadOnly
    {
      get { return false; }
    }

    bool ICollection<T>.Remove(T item)
    {
      int hashCode = _theProp.GetValue(item, null).GetHashCode();
      if (_index.ContainsKey(hashCode))
      {
        if (_index[hashCode].Contains(item))
        {
          _index[hashCode].Remove(item);
          _countCache--;
          return true;
        }
        else return false;
      }
      else
        return false;
    }

    void IIndex<T>.ReIndex(T item)
    {
      bool wasRemoved = (this as ICollection<T>).Remove(item);
      if (!wasRemoved) RemoveByReference(item);
      (this as ICollection<T>).Add(item);
    }

    #endregion

    #region IEnumerable<T> Members

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
      foreach (List<T> list in _index.Values)
          foreach (T item in list)
              yield return item;
    }

    #endregion

    #region IEnumerable Members

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      foreach (List<T> list in _index.Values)
          foreach (T item in list)
              yield return item;
    }

    #endregion



    private void RemoveByReference(T item)
    {
      foreach (T itemToCheck in this)
          if (ReferenceEquals(itemToCheck, item))
              (this as ICollection<T>).Remove(item);
    }

    #region IIndex<T> Members


    bool IIndex<T>.Loaded
    {
      get
      {
        return _loaded;
      }
    }

    void IIndex<T>.InvalidateIndex()
    {
      if (_indexAttribute.IndexMode != IndexModeEnum.IndexModeNever)
        _loaded = false;
    }

    void IIndex<T>.LoadComplete()
    {
      if (_indexAttribute.IndexMode != IndexModeEnum.IndexModeNever)
        _loaded = true;
    }
    IndexModeEnum IIndex<T>.IndexMode
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
  }
}
