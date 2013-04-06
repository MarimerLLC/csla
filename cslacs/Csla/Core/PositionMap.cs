using System;
using System.Collections.Generic;
using System.Linq;

namespace Csla.Core
{
  internal class PositionMap<T>
    where T : Core.IEditableBusinessObject
  {
    Dictionary<T, int> _map;
    //on rare occasions, we get duplicates.  Store those here
    Dictionary<T, int> _duplicatePositions = new Dictionary<T,int>(); //T is the item, the int is the position count (if there is a dupe)

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

    public void AddToDuplicates(T item)
    {
      if (!_duplicatePositions.ContainsKey(item)) //if there is not a duplicate entry already
        _duplicatePositions.Add(item, 1); //number refers to # of dups
      else
        _duplicatePositions[item]++;
    }
    
    public void AddToMap(T item)
    {
      if (!_map.ContainsKey(item))
        _map.Add(item, _list.Count - 1);
      else //its a duplicate - handle here
      {
        var duplicateCount = _list.Count(checkItem => ReferenceEquals(item, checkItem));
        if (duplicateCount > 1) AddToDuplicates(item);
      }

    }

    public void InsertIntoMap(T item, int position)
    {
      if (position == _list.Count - 1)
      {
        AddToMap(item);
      }
      else
      {
        if (!_map.ContainsKey(item))
        {
          for (int i = _list.Count - 1; i > position; i--)
            _map[_list[i]]++;
          _map.Add(item, position);
        }
        else AddToDuplicates(item);
      }
    }

    public void RemoveFromMap(T item)
    {
      if (_duplicatePositions.ContainsKey(item))
      {
        if (_duplicatePositions[item] == 1)
          _duplicatePositions.Remove(item);
        else
          _duplicatePositions[item]--;
        return;
      }
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
