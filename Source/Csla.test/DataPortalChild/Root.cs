//-----------------------------------------------------------------------
// <copyright file="Root.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using Csla;

namespace Csla.Test.DataPortalChild
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
        return GetProperty(ChildProperty); 
      }
    }

    private static PropertyInfo<ChildList> ChildListProperty = RegisterProperty<ChildList>(typeof(Root), new PropertyInfo<ChildList>("ChildList"));
    public ChildList ChildList
    {
      get
      {
        return GetProperty(ChildListProperty);
      }
    }

    public void FetchChild(IChildDataPortal<Child> childDataPortal)
    {
      SetProperty(ChildProperty, childDataPortal.FetchChild());
    }

    [Create]
    protected void DataPortal_Create([Inject] IChildDataPortal<Child> childDataPortal, [Inject] IChildDataPortal<ChildList> childListDataPortal)
    {
      LoadProperty(ChildProperty, childDataPortal.CreateChild());
      LoadProperty(ChildListProperty, childListDataPortal.CreateChild());
    }

    [Insert]
    protected void DataPortal_Insert()
    {
      FieldManager.UpdateChildren(this);
    }

    [Update]
		protected void DataPortal_Update()
    {
      FieldManager.UpdateChildren(this);
    }
  }
}