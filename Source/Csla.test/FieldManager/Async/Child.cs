//-----------------------------------------------------------------------
// <copyright file="Child.cs" company="Marimer LLC">
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
  public class Child : BusinessBase<Child>
  {
    public static async Task<Child> NewChildAsync(IChildDataPortal<Child> childDataPortal)
    {
      return await childDataPortal.CreateChildAsync();
    }

    public static async Task<Child> GetChildAsync(IChildDataPortal<Child> childDataPortal)
    {
      return await childDataPortal.FetchChildAsync();
    }

    public Child()
    {
      MarkAsChild();
    }

    private static PropertyInfo<string> DataProperty = RegisterProperty<string>(typeof(Child), new PropertyInfo<string>("Data"));
    public string Data
    {
      get { return GetProperty<string>(DataProperty); }
      set { SetProperty<string>(DataProperty, value); }
    }

    private static PropertyInfo<string> RootDataProperty = RegisterProperty<string>(typeof(Child), new PropertyInfo<string>("RootData", string.Empty));
    public string RootData
    {
      get { return GetProperty<string>(RootDataProperty); }
      set { SetProperty<string>(RootDataProperty, value); }
    }

    private static PropertyInfo<string> StatusProperty = RegisterProperty<string>(c => c.Status);
    public string Status
    {
      get { return GetProperty(StatusProperty); }
    }

    [CreateChild]
    private async Task CreateAsync()
    {
      await Task.Delay(5);
      LoadProperty(StatusProperty, "Created");
    }

    [FetchChild]
    private async Task FetchAsync()
    {
      await Task.Delay(5);
      LoadProperty(StatusProperty, "Fetched");
    }

    [InsertChild]
    private async Task InsertAsync()
    {
      await Task.Delay(5);
      LoadProperty(StatusProperty, "Inserted");
    }

    [UpdateChild]
    private async Task UpdateAsync()
    {
      await Task.Delay(5);
      LoadProperty(StatusProperty, "Updated");
    }

    [DeleteSelfChild]
    private async Task DeleteSelfAsync()
    {
      await Task.Delay(5);
      LoadProperty(StatusProperty, "Deleted");
    }
  }
}