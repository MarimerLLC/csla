//-----------------------------------------------------------------------
// <copyright file="Root.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

using System;

namespace Csla.Test.FieldManager
{
  [Serializable]
  public class Root : BusinessBase<Root>
  {
    private static PropertyInfo<string> DataProperty = RegisterProperty<string>(typeof(Root), new PropertyInfo<string>("Data"));
    public string Data
    {
      get { return GetProperty<string>(DataProperty); }
      set { SetProperty<string>(DataProperty, value); }
    }

    private static PropertyInfo<Child> ChildProperty = RegisterProperty<Child>(typeof(Root), new PropertyInfo<Child>("Child"));
    public Child Child
    {
      get 
      {
        return GetProperty<Child>(ChildProperty); 
      }
    }

    private static PropertyInfo<ChildList> ChildListProperty = RegisterProperty<ChildList>(typeof(Root), new PropertyInfo<ChildList>("ChildList"));
    public ChildList ChildList
    {
      get
      {
        return GetProperty<ChildList>(ChildListProperty);
      }
    }

    public void FetchChild(IChildDataPortal<Child> childDataPortal)
    {
      SetProperty(ChildProperty, Child.GetChild(childDataPortal));
    }

    [Create]
    protected void Create([Inject] IChildDataPortal<Child> childDataPortal, [Inject]IChildDataPortal<ChildList> childListDataPortal)
    {
      LoadProperty(ChildProperty, Child.NewChild(childDataPortal));
      LoadProperty(ChildListProperty, ChildList.GetList(childListDataPortal));
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
  }
}