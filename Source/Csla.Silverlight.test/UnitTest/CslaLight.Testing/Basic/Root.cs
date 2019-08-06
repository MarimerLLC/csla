using System;
using Csla;
using Csla.Serialization;

namespace cslalighttest.Basic
{
  [Serializable]
  public class Root : BusinessBase<Root>
  {
    public static readonly PropertyInfo<string> DataProperty = RegisterProperty<string>(c => c.Data);
    public string Data
    {
      get { return GetProperty(DataProperty); }
      set { SetProperty(DataProperty, value); }
    }

    public void Clean()
    {
      MarkClean();
    }
  }
}
