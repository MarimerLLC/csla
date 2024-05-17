//-----------------------------------------------------------------------
// <copyright file="ChildList.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

namespace Csla.Test.FieldManager.Async
{
  [Serializable]
  public class ChildList : BusinessListBase<ChildList, Child>
  {
    public static Task<ChildList> GetListAsync(IChildDataPortal<ChildList> childDataPortal)
    {
      return childDataPortal.FetchChildAsync();
    }

    public ChildList()
    {
    }

    public object MyParent => this.Parent;

    public string Status { get; private set; }

    [FetchChild]
    private async Task Child_FetchAsync()
    {
      await Task.Delay(5);
      Status = "Fetched";
    }

    [UpdateChild]
    protected override async Task Child_UpdateAsync(params object[] p)
    {
      await base.Child_UpdateAsync(p);
      Status = "Updated";
    }
  }
}