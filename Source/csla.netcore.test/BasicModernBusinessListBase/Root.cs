using System.ComponentModel.DataAnnotations;

namespace Csla.Test.BasicModernBusinessListBase
{
  [Serializable]
  public class Root : BusinessListBase<Root, Child>
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

    public void MakeOld()
    {
      MarkOld();
    }

    [Create]
    protected void DataPortal_Create()
    {
      BusinessRules.CheckRules();
    }
    
    [Fetch]
    private void DataPortal_Fetch(int id, [Inject] IChildDataPortal<Child> dataPortal)
    {
      using (LoadListMode)
      {
        Add(dataPortal.FetchChild(id, $"child {id}"));
      }
    }

    [Insert]
    protected void DataPortal_Insert()
    {
      FieldManager.UpdateChildren();
    }

    [Update]
    protected void DataPortal_Update()
    {
      FieldManager.UpdateChildren();
    }

    [DeleteSelf]
    protected void DataPortal_DeleteSelf()
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
