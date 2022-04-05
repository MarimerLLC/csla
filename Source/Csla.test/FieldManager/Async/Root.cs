//-----------------------------------------------------------------------
// <copyright file="Root.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

using System;
using System.Threading.Tasks;

namespace Csla.Test.FieldManager.Async
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

    public async Task FetchChildAsync(IChildDataPortal<Child> childDataPortal)
    {
      SetProperty(ChildProperty, await Child.GetChildAsync(childDataPortal));
    }

    [Create]
    protected async Task CreateAsync([Inject] IChildDataPortal<Child> childDataPortal, [Inject]IChildDataPortal<ChildList> childListDataPortal)
    {
      LoadProperty(ChildProperty, await Child.NewChildAsync(childDataPortal));
      LoadProperty(ChildListProperty, await ChildList.GetListAsync(childListDataPortal));
    }

    [Insert]
    private async Task InsertAsync()
    {
      await FieldManager.UpdateChildrenAsync();
    }

    [Update]
	private async Task UpdateAsync()
    {
      await FieldManager.UpdateChildrenAsync();
    }
  }
}