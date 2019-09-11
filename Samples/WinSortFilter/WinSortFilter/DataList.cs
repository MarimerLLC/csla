using System;
using Csla;

namespace WinSortFilter
{
  [Serializable]
  public class DataList : BusinessBindingListBase<DataList, Data>
  {
    [Create]
    private void Create()
    { }
  }
}
