using System;
using System.Collections.Generic;
using System.Text;
using Csla;
using Csla.Serialization;

namespace Csla.Test.PropertyGetSet
{
  [Serializable]
  public class ChildList : BusinessBindingListBase<ChildList, EditableGetSet>
  {
    public ChildList()
    { }

    public ChildList(bool isChild)
    {
      MarkAsChild();
    }

    public new int EditLevel
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
