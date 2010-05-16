using System;
using System.Collections.Generic;
using System.Text;
using Csla;

namespace BLBTest
{
  [Serializable]
  public class DataList : BusinessBindingListBase<DataList, DataEdit>
  {
    public DataList()
    {
      AllowEdit = true;
      AllowNew = true;
      AllowRemove = true;
    }

    protected override object AddNewCore()
    {
      DataEdit item = new DataEdit();
      Add(item);
      return item;
    }
  }
}
