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

    [CreateChild]
    private void Child_Create(int id, string name)
    {
      using (BypassPropertyChecks)
      {
        Id = id;
        Name = name;
      }
    }

    [FetchChild]
    private void Child_Fetch(int id, string name)
    {
      using (BypassPropertyChecks)
      {
        Id = id;
        Name = name;
      }
    }

    [InsertChild]
    private void Child_Insert()
    { }

    [UpdateChild]
    private void Child_Update()
    { }

    [DeleteSelfChild]
    private void Child_DeleteSelf()
    { }
  }
}
