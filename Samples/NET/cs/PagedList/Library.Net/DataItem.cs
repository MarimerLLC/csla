using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Serialization;

namespace Library
{
  [Serializable]
  public class DataItem : BusinessBase<DataItem>
  {
    public static PropertyInfo<int> IdProperty = RegisterProperty<int>(c => c.Id);
    public int Id
    {
      get { return GetProperty(IdProperty); }
      set { SetProperty(IdProperty, value); }
    }

    public static PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    private void Child_Fetch(int id, string name)
    {
      using (BypassPropertyChecks)
      {
        Id = id;
        Name = name;
      }
    }
  }
}
