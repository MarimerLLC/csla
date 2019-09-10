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
    public static readonly PropertyInfo<int> IdProperty = RegisterProperty<int>(c => c.Id);
    public int Id
    {
      get { return GetProperty(IdProperty); }
      set { SetProperty(IdProperty, value); }
    }

    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    protected override void Child_Create()
    {
      base.DataPortal_Create();
      MarkAsChild();
    }
  }
}
