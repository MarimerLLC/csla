using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Csla;
using Csla.Serialization;

namespace Csla.Test.BasicModern
{
  [Serializable]
  public class Child : BusinessBase<Child>
  {
    public static readonly PropertyInfo<int> IdProperty = RegisterProperty<int>(nameof(Id));
    public int Id
    {
      get { return GetProperty(IdProperty); }
      set { SetProperty(IdProperty, value); }
    }

    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
    [Required]
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    public void Child_Create(int id, string name)
    {
      using (BypassPropertyChecks)
      {
        Id = id;
        Name = name;
      }
    }

    public void Child_Fetch(int id, string name)
    {
      using (BypassPropertyChecks)
      {
        Id = id;
        Name = name;
      }
    }

    public void Child_Insert()
    { }

    public void Child_Update()
    { }

    public void Child_DeleteSelf()
    { }
  }
}
