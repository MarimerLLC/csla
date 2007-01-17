using System;
using System.Collections.Generic;

namespace Csla.Validation
{
  internal class RulesList
  {
    private List<IRuleMethod> _list = new List<IRuleMethod>();
    private bool _sorted;
    private List<string> _dependantProperties;

    public void Add(IRuleMethod item)
    {
      _list.Add(item);
      _sorted = false;
    }

    public List<IRuleMethod> GetList(bool applySort)
    {
      if (applySort && !_sorted)
      {
        lock (_list)
        {
          if (applySort && !_sorted)
          {
            _list.Sort();
            _sorted = true;
          }
        }
      }
      return _list;
    }

    public List<string> GetDependancyList(bool create)
    {
      if (_dependantProperties == null && create)
        _dependantProperties = new List<string>();
      return _dependantProperties;
    }
  }
}
