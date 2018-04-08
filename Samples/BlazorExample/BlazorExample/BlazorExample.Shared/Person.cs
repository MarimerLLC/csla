using System;
using System.Collections.Generic;
using System.Text;
using Csla;

namespace BlazorExample.Shared
{
  [Serializable]
  public class Person : BusinessBase<Person>
  {
    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    private static int _count;

    private void DataPortal_Fetch(string name)
    {
      using (BypassPropertyChecks)
      {
        _count++;
        Name = name + _count.ToString();
      }
    }
  }
}
