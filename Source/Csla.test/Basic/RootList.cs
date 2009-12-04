using System;
using System.Collections.Generic;
using System.Text;
using Csla;

namespace Csla.Test.Basic
{
  [Serializable]
  public class RootList : BusinessListBase<RootList, RootListChild>
  {
    public RootList()
    {
      AllowEdit = true;
      AllowNew = true;
      AllowRemove = true;
    }

    protected override object AddNewCore()
    {
      RootListChild child = new RootListChild();
      Add(child);
      return child;
    }
  }

  [Serializable]
  public class RootListChild : BusinessBase<RootListChild>
  {
    int _data;
    public int Data
    {
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
      get
      {
        CanReadProperty(true);
        return _data;
      }
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
      set
      {
        CanWriteProperty(true);
        if (!_data.Equals(value))
        {
          _data = value;
          PropertyHasChanged();
        }
      }
    }

    protected override object GetIdValue()
    {
      return _data;
    }

    public string[] GetRuleDescriptions()
    {
      string[] result = ValidationRules.GetRuleDescriptions();
      if (result == null)
        result = new string[] { };
      return result;
    }

    public RootListChild()
    {
      MarkAsChild();
    }
  }
}
