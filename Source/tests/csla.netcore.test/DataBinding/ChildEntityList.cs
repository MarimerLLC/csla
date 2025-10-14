﻿//-----------------------------------------------------------------------
// <copyright file="ChildEntityList.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

using System.Data;

namespace Csla.Test.DataBinding
{
  [Serializable]
  public class ChildEntityList : BusinessBindingListBase<ChildEntityList, ChildEntity>
  {
    public ChildEntityList()
    {
      MarkAsChild();
    }

    #region "Criteria"

    [Serializable]
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

    [Fetch]
    private void DataPortal_Fetch(object criteria)
    {
      for (int i = 0; i < 10; i++)
        Add(new ChildEntity(i, "first" + i, "last" + i));
    }
  }
}