using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;

namespace WinSortFilter
{
  [Serializable]
  public class Data : BusinessBase<Data>
  {
    public Data()
    {
      MarkAsChild();
    }

    private static PropertyInfo<int> IdProperty = RegisterProperty<int>(c => c.Id);
    public int Id
    {
      get { return GetProperty(IdProperty); }
      set { SetProperty(IdProperty, value); }
    }

    private static PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }
  }
}
