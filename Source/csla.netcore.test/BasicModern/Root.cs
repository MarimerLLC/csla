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
    public static readonly PropertyInfo<int> IdProperty = RegisterProperty<int>(c => c.Id);
    public int Id
    {
      get { return GetProperty(IdProperty); }
      set { SetProperty(IdProperty, value); }
    }

    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);
    [Required]
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    public static readonly PropertyInfo<ChildList> ChildrenProperty = RegisterProperty<ChildList>(c => c.Children);
    public ChildList Children
    {
      get { return GetProperty(ChildrenProperty); }
      private set { LoadProperty(ChildrenProperty, value); }
    }

    public void MakeOld()
    {
      MarkOld();
    }

    public static void NewRoot(EventHandler<DataPortalResult<Root>> callback)
    {
      Csla.DataPortal.BeginCreate<Root>(callback);
    }

    public static void GetRoot(int id, EventHandler<DataPortalResult<Root>> callback)
    {
      Csla.DataPortal.BeginFetch<Root>(id, callback);
    }

#if !SILVERLIGHT
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

    protected override void DataPortal_Create()
    {
      Children = Csla.DataPortal.CreateChild<ChildList>();
      base.DataPortal_Create();
    }
    
    private void DataPortal_Fetch(int id)
    {
      // TODO: load values into object
      Children = Csla.DataPortal.CreateChild<ChildList>();
    }

    protected override void DataPortal_Insert()
    {
      // TODO: insert object's data
      FieldManager.UpdateChildren();
    }

    protected override void DataPortal_Update()
    {
      // TODO: update object's data
      FieldManager.UpdateChildren();
    }

    protected override void DataPortal_DeleteSelf()
    {
      DataPortal_Delete(ReadProperty(IdProperty));
    }

    private void DataPortal_Delete(int id)
    {
      // TODO: delete object's data
      FieldManager.UpdateChildren();
    }

#endif
  }
}
