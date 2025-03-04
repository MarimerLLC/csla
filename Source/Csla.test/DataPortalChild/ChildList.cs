//-----------------------------------------------------------------------
// <copyright file="ChildList.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

namespace Csla.Test.DataPortalChild
{
  [Serializable]
  public class ChildList : BusinessBindingListBase<ChildList, Child>
  {
    public ChildList()
    {
      MarkAsChild();
    }

    public object MyParent
    {
      get { return Parent; }
    }

    public string Status { get; private set; } = "Random";

    public string[] CreateValues { get; private set; }

    [CreateChild]
    private async Task CreateChild(string createValues, [Inject]IChildDataPortal<Child> childPortal)
    {
      CreateValues = [createValues];
      for (int i = 0; i < 2; i++)
      {
        var child = await childPortal.CreateChildAsync(createValues);
        Add(child);
      }
    }

    [FetchChild]
    protected void Child_Fetch()
    {
      Status = "Fetched";
    }

    [UpdateChild]
    protected override void Child_Update(params object[] p)
    {
      base.Child_Update(p);
      Status = "Updated";
    }
  }
}