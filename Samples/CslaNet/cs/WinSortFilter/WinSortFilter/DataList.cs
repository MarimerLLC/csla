using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Csla;

namespace WinSortFilter
{
  [Serializable]
  public class DataList : BusinessBindingListBase<DataList, Data>
  {
    public DataList()
    {

    }

    protected override object AddNewCore()
    {
      var data = new Data();
      Add(data);
      return data;
    }
  }
}
