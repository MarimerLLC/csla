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

    public static ChildList NewObject(IDataPortal<ChildList> dataPortal, bool isChild)
    {
      return dataPortal.Create(isChild);
    }

    public new int EditLevel
    {
      get { return base.EditLevel; }
    }

    internal void Update()
    {
      foreach (var item in this)
        if (item.IsNew)
          item.Insert();
        else
          item.Update();
    }

    [Create]
    private void Create(bool isChild)
    {
      if (isChild)
        MarkAsChild();

    }

  }
}