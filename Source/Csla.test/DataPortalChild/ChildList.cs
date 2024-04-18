//-----------------------------------------------------------------------
// <copyright file="ChildList.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;

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
      get { return this.Parent; }
    }

    public string Status { get; private set; }

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