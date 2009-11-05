using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Test.EditableRootList
{
  public class ERlist : Csla.EditableRootListBase<ERitem>
  {
    public ERlist()
    {
      AllowEdit = true;
      AllowNew = true;
      AllowRemove = true;
    }

    protected override object AddNewCore()
    {
      ERitem item = ERitem.NewItem();
      this.Add(item);
      return item;
    }
  }
}
