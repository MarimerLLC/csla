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
  public class Root : BusinessBase<Root>
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

    public static readonly PropertyInfo<ChildList> ChildrenProperty = RegisterProperty<ChildList>(nameof(Children));
    public ChildList Children
    {
      get { return GetProperty(ChildrenProperty); }
      private set { LoadProperty(ChildrenProperty, value); }
    }

    public void MakeOld()
    {
      MarkOld();
    }

    public static async Task<Root> NewRootAsync()
    {
      return await Csla.DataPortal.CreateAsync<Root>();
    }

    public static async Task<Root> GetRootAsync(int id)
    {
      return await Csla.DataPortal.FetchAsync<Root>(id);
    }

    public static Root NewRoot()
    {
      return Csla.DataPortal.Create<Root>();
    }

    public static Root GetRoot(int id)
    {
      return Csla.DataPortal.Fetch<Root>(id);
    }

    public static void DeleteRoot(int id)
    {
      Csla.DataPortal.Delete<Root>(id);
    }

    [Create]
    protected override void DataPortal_Create()
    {
      Children = Csla.DataPortal.CreateChild<ChildList>();
      base.DataPortal_Create();
    }
    
    [Fetch]
    private void DataPortal_Fetch(int id)
    {
      Children = Csla.DataPortal.CreateChild<ChildList>();
    }

    [Insert]
    protected override void DataPortal_Insert()
    {
      FieldManager.UpdateChildren();
    }

    [Update]
    protected override void DataPortal_Update()
    {
      FieldManager.UpdateChildren();
    }

    [DeleteSelf]
    protected override void DataPortal_DeleteSelf()
    {
      DataPortal_Delete(ReadProperty(IdProperty));
    }

    [Delete]
    private void DataPortal_Delete(int id)
    {
      FieldManager.UpdateChildren();
    }
  }
}
