//-----------------------------------------------------------------------
// <copyright file="ChildList.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using Csla;
using Csla.Serialization;

namespace Csla.Test.PropertyGetSet
{
  [Serializable]
  public class ChildList : BusinessListBase<ChildList, EditableGetSet>
  {
    public ChildList()
    { }

    public static ChildList NewObject(IDataPortal<ChildList> dataPortal)
    {
      return dataPortal.Create();
    }

    public static ChildList NewChildObject(IChildDataPortal<ChildList> dataPortal)
    {
      return dataPortal.CreateChild();
    }

    public static ChildList GetChildObject(IChildDataPortal<ChildList> dataPortal)
    {
      return dataPortal.FetchChild();
    }

    public new int EditLevel
    {
      get { return base.EditLevel; }
    }

    [Create]
    [CreateChild]
    private void Create()
    {
    }

    [Fetch]
    [FetchChild]
    private void Fetch()
    {
    }
  }
}