//-----------------------------------------------------------------------
// <copyright file="ChildList.cs" company="Marimer LLC">
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
  public class ChildList : BusinessListBase<ChildList, Child>
  {
    public static async Task<ChildList> GetListAsync(IChildDataPortal<ChildList> childDataPortal)
    {
      return await childDataPortal.FetchChildAsync();
    }

    public ChildList()
    {
    }

    public object MyParent
    {
      get { return this.Parent; }
    }

    private string _status;
    public string Status
    {
      get { return _status; }
    }

    [FetchChild]
    private async Task Child_FetchAsync()
    {
      await Task.Delay(5);
      _status = "Fetched";
    }

    [UpdateChild]
    protected override async Task Child_UpdateAsync(params object[] p)
    {
      await base.Child_UpdateAsync(p);
      _status = "Updated";
    }
  }
}