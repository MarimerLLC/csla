//-----------------------------------------------------------------------
// <copyright file="ChildEntityList.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Csla.Test.DataBinding
{
  [Serializable()]
  public class ChildEntityList : BusinessBindingListBase<ChildEntityList, ChildEntity>
  {
    public ChildEntityList()
    {
      this.MarkAsChild();
    }

    #region "factory methods"

    public static ChildEntityList NewChildEntityList()
    {
      return new ChildEntityList();
    }

    #endregion

    #region "Criteria"

    [Serializable()]
    private class Criteria
    {
      //no criteria for this list
    }

    #endregion

    internal void update(IDbTransaction tr)
    {
      foreach (ChildEntity child in this)
      {
        //child.Update(tr);
      }
    }

    public static ChildEntityList GetList()
    {
      return Csla.DataPortal.Fetch<ChildEntityList>(new Criteria());
    }

    [Fetch]
    protected override void DataPortal_Fetch(object criteria)
    {
      for (int i = 0; i < 10; i++)
        Add(new ChildEntity(i, "first" + i, "last" + i));
    }
  }
}