//-----------------------------------------------------------------------
// <copyright file="ChildList.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

using System;

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

    private string _status;
    public string Status
    {
      get { return _status; }
    }

    [FetchChild]
    protected void Child_Fetch()
    {
      _status = "Fetched";
    }

    protected override void Child_Update(params object[] p)
    {
      base.Child_Update();
      _status = "Updated";
    }
  }
}