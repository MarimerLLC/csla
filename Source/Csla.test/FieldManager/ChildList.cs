//-----------------------------------------------------------------------
// <copyright file="ChildList.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

namespace Csla.Test.FieldManager
{
  [Serializable]
  public class ChildList : BusinessBindingListBase<ChildList, Child>
  {
    public static ChildList GetList(IChildDataPortal<ChildList> childDataPortal)
    {
      return childDataPortal.FetchChild();
    }

    public ChildList()
    {
      MarkAsChild();
    }

    public object MyParent
    {
      get { return this.Parent; }
    }

    public string Status { get; private set; }

    [FetchChild]
    protected void Child_Fetch()
    {
      Status = "Fetched";
    }

    protected override void Child_Update(params object[] p)
    {
      base.Child_Update();
      Status = "Updated";
    }
  }
}