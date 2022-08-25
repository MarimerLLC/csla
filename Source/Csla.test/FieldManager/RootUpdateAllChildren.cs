using System;

namespace Csla.Test.FieldManager
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

    public void FetchChild(IChildDataPortal<Child> childDataPortal)
    {
      SetProperty<Child>(ChildProperty, Child.GetChild(childDataPortal));
    }

    [Create]
    private void Create([Inject]IChildDataPortal<Child> childDataPortal, [Inject]IChildDataPortal<ChildList> childListDataPortal)
    {
      LoadProperty(ChildProperty, Child.NewChild(childDataPortal));
      LoadProperty(ChildListProperty, ChildList.GetList(childListDataPortal));
    }

    [Insert]
    protected void DataPortal_Insert()
    {
      FieldManager.UpdateAllChildren();
    }

    [Update]
		protected void DataPortal_Update()
    {
      FieldManager.UpdateAllChildren();
    }
  }
}