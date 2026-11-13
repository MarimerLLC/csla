using System;
using Csla;

namespace WinSortFilter
{
  [Serializable]
  public partial class DataList : BusinessBindingListBase<DataList, Data>
  {
    [Create]
    private void Create()
    { }
  }
}
