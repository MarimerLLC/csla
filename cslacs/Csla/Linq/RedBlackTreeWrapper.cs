using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Csla.C5;

namespace Csla.Linq
{
  internal class RedBlackTreeWrapper<K, T> : IBalancedSearch<T>, System.Collections.Generic.ICollection<T>
    where K : IComparable 
  {
    private Csla.C5.TreeDictionary<K, List<T>> _internalWrapper;
    private PropertyInfo _propInfo = null;

    private RedBlackTreeWrapper() { }

    public RedBlackTreeWrapper(PropertyInfo propInfo)
    {
      _propInfo = propInfo;
      _internalWrapper = new TreeDictionary<K, List<T>>();
    }

    #region IBalancedSearch<T> Members

    public IEnumerable<T> ItemsLessThan(object pivot)
    {
      foreach (var pair in _internalWrapper.RangeTo((K)pivot))
        foreach (var item in pair.Value)
          yield return item;
    }

    public IEnumerable<T> ItemsGreaterThan(object pivot)
    {
      var comparer = System.Collections.Generic.Comparer<K>.Default;
      foreach (var pair in _internalWrapper.RangeFrom((K)pivot))
        if (comparer.Compare((K)pivot,pair.Key) != 0)
          foreach (var item in pair.Value)
            yield return item;
    }

    public IEnumerable<T> ItemsLessThanOrEqualTo(object pivot)
    {
      foreach (var item in ItemsLessThan(pivot))
        yield return item;
      foreach (var item in ItemsEqualTo(pivot))
        yield return item;
    }

    public IEnumerable<T> ItemsGreaterThanOrEqualTo(object pivot)
    {
      foreach (var item in ItemsGreaterThan(pivot))
        yield return item;
      foreach (var item in ItemsEqualTo(pivot))
        yield return item;
    }

    public IEnumerable<T> ItemsEqualTo(object pivot)
    {
      List<T> result = null;
      _internalWrapper.Find((K)pivot, out result);
      if (result != null)
        foreach (T item in result)
          yield return item;
    }

    #endregion

    private K GetKeyVal(T obj)
    {
      return (K) _propInfo.GetValue(obj, null);
    }

    #region ICollection<T> Members

    public void Add(T item)
    {
      if (!_internalWrapper.Contains(GetKeyVal(item)))
        _internalWrapper.Add(GetKeyVal(item), new List<T>());
      _internalWrapper[GetKeyVal(item)].Add(item);
        
    }

    public void Clear()
    {
      _internalWrapper.Clear();
    }

    public bool Contains(T item)
    {
      return _internalWrapper.Contains(GetKeyVal(item));
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
      throw new NotImplementedException();
    }

    public int Count
    {
      get { return _internalWrapper.Count; }
    }

    public bool IsReadOnly
    {
      get { return false; }
    }

    public bool Remove(T item)
    {
      if (_internalWrapper.Contains(GetKeyVal(item)))
        return _internalWrapper[GetKeyVal(item)].Remove(item);
      else
        return false;
    }

    #endregion

    #region IEnumerable<T> Members

    public IEnumerator<T> GetEnumerator()
    {
      foreach (var pair in _internalWrapper)
        foreach (var item in pair.Value)
          yield return item;
    }

    #endregion

    #region IEnumerable Members

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      foreach (var pair in _internalWrapper)
        foreach (var item in pair.Value)
          yield return item;
    }

    #endregion
  }
}
