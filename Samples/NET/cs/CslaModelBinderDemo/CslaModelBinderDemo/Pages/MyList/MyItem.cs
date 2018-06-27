using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Csla;

namespace WebApplication3.Pages.MyList
{
  [Serializable]
  public class MyItem : BusinessBase<MyItem>
  {
    public static readonly PropertyInfo<int> IdProperty = RegisterProperty<int>(c => c.Id);
    public int Id
    {
      get { return GetProperty(IdProperty); }
      private set { LoadProperty(IdProperty, value); }
    }

    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);
    [Required]
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    public static readonly PropertyInfo<string> CityProperty = RegisterProperty<string>(c => c.City);
    [Required]
    public string City
    {
      get { return GetProperty(CityProperty); }
      set { SetProperty(CityProperty, value); }
    }

    public static readonly PropertyInfo<bool> RemoveProperty = RegisterProperty<bool>(c => c.Remove);
    public bool Remove
    {
      get { return GetProperty(RemoveProperty); }
      set { SetProperty(RemoveProperty, value); }
    }

    private void Child_Fetch(MockDb.DbItem data)
    {
      using (BypassPropertyChecks)
      {
        Id = data.Id;
        Name = data.Name;
        City = data.City;
      }
    }

    [RunLocal]
    protected override void DataPortal_Create()
    {
      base.DataPortal_Create();
    }

    protected override void DataPortal_Insert()
    {
      Child_Insert();
    }

    private void Child_Insert()
    {
      using (BypassPropertyChecks)
      {
        Id = MockDb.Items.Max(r => r.Id) + 1;
        MockDb.Items.Add(new MockDb.DbItem { Id = Id, Name = Name, City = City });
      }
    }

    private void Child_Update()
    {
      using (BypassPropertyChecks)
      {
        var existing = MockDb.Items.Where(i => i.Id == Id).FirstOrDefault();
        if (existing == null)
          throw new InvalidOperationException("No existing item in database {Id}");
        existing.Id = Id;
        existing.Name = Name;
        existing.City = City;
      }
    }

    private void Child_DeleteSelf()
    {
      var existing = MockDb.Items.Where(i => i.Id == Id).FirstOrDefault();
      if (existing == null)
        throw new InvalidOperationException("No existing item in database {Id}");
      MockDb.Items.Remove(existing);
    }
  }
}
