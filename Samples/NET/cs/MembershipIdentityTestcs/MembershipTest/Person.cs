using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Csla;

namespace MembershipTest
{
  [Serializable]
  public class Person : BusinessBase<Person>
  {
    private static PropertyInfo<int> IdProperty = RegisterProperty(new PropertyInfo<int>("Id", "Id"));
    public int Id
    {
      get { return GetProperty(IdProperty); }
      set { SetProperty(IdProperty, value); }
    }

    private static PropertyInfo<string> NameProperty = RegisterProperty(new PropertyInfo<string>("Name", "Name"));
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }
  }
}
