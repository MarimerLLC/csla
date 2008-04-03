using System;
using System.Collections.Generic;

namespace Csla.Core
{
  internal class PositionMap<T>
    where T : Core.IEditableBusinessObject
  {
    Dictionary<T, int> _map;
    IList<T> _list;

    public PositionMap(IList<T> list)
    {
      _list = list;
      RebuildMap();
    }

    public void ClearMap()
    {
      _map = new Dictionary<T, int>(_list.Count);
    }

    public void AddToMap(T item)
    {
      if (!_map.ContainsKey(item))
        _map.Add(item, _list.Count - 1);
    }

    public void InsertIntoMap(T item, int position)
    {
      if (position == _list.Count - 1)
      {
        AddToMap(item);
      }
      else
      {
        for (int i = _list.Count - 1; i > position; i--)
          _map[_list[i]]++;
        if (!_map.ContainsKey(item))
          _map.Add(item, position);
      }
    }

    public void RemoveFromMap(T item)
    {
      int oldPosition = PositionOf(item);
      if (oldPosition == -1) return;
      _map.Remove(item);
      for (int i = oldPosition + 1; i < _list.Count; i++)
        _map[_list[i]]--;
    }

    public int PositionOf(T item)
    {
      int result;
      if (_map.TryGetValue(item, out result))
        return result;
      else
        return -1;
    }

    public void RebuildMap()
    {
      ClearMap();
      int i = 0;
      foreach (T item in _list)
      {
        _map.Add(item, i);
        i++;
      }

    }
  }
}
