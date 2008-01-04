using System;
using System.Collections.Generic;
using System.Text;
using Csla;

namespace Csla.Test.PropertyGetSet
{
  [Serializable]
  public class ChildList : BusinessListBase<ChildList, EditableGetSet>
  {
    public ChildList()
    { }

    public ChildList(bool isChild)
    {
      MarkAsChild();
    }

    public int EditLevel
    {
      get { return base.EditLevel; }
    }

    internal void Update()
    {
      foreach (var item in this)
        if (item.IsNew)
          item.Insert();
        else
          item.Update();
    }

  }
}
