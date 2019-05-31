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
        if (!FieldManager.FieldExists(ChildProperty))
          SetProperty<Child>(ChildProperty, Child.NewChild());
        return GetProperty<Child>(ChildProperty); 
      }
    }

    private static PropertyInfo<ChildList> ChildListProperty = RegisterProperty<ChildList>(typeof(RootUpdateAllChildren), new PropertyInfo<ChildList>("ChildList"));
    public ChildList ChildList
    {
      get
      {
        if (!FieldManager.FieldExists(ChildListProperty))
          SetProperty<ChildList>(ChildListProperty, ChildList.GetList());
        return GetProperty<ChildList>(ChildListProperty);
      }
    }

    public void FetchChild()
    {
      SetProperty<Child>(ChildProperty, Child.GetChild());
    }

    protected override void DataPortal_Insert()
    {
      FieldManager.UpdateAllChildren();
    }

    protected override void DataPortal_Update()
    {
      FieldManager.UpdateAllChildren();
    }
  }
}