using System;
using System.Threading.Tasks;

namespace Csla.Test.FieldManager.Async
{
  [Serializable]
  public class RootUpdateAllChildren : BusinessBase<RootUpdateAllChildren>
  {
    private static PropertyInfo<string> DataProperty = RegisterProperty<string>(typeof(RootUpdateAllChildren), new PropertyInfo<string>("Data"));
    public string Data
    {
      get { return GetProperty<string>(DataProperty); }
      set { SetProperty<string>(DataProperty, value); }
    }

    private static PropertyInfo<Child> ChildProperty = RegisterProperty<Child>(typeof(RootUpdateAllChildren), new PropertyInfo<Child>("Child"));
    public Child Child
    {
      get 
      {
        return GetProperty<Child>(ChildProperty); 
      }
    }

    private static PropertyInfo<ChildList> ChildListProperty = RegisterProperty<ChildList>(typeof(RootUpdateAllChildren), new PropertyInfo<ChildList>("ChildList"));
    public ChildList ChildList
    {
      get
      {
        return GetProperty<ChildList>(ChildListProperty);
      }
    }

    public async Task FetchChildAsync(IChildDataPortal<Child> childDataPortal)
    {
      SetProperty<Child>(ChildProperty, await Child.GetChildAsync(childDataPortal));
    }

    [Create]
    private async Task CreateAsync([Inject]IChildDataPortal<Child> childDataPortal, [Inject]IChildDataPortal<ChildList> childListDataPortal)
    {
      LoadProperty(ChildProperty, await Child.NewChildAsync(childDataPortal));
      LoadProperty(ChildListProperty, await ChildList.GetListAsync(childListDataPortal));
    }

    [Insert]
    protected async Task InsertAsync()
    {
      await FieldManager.UpdateAllChildrenAsync();
    }

    [Update]
	protected async Task UpdateAsync()
    {
      await FieldManager.UpdateAllChildrenAsync();
    }
  }
}