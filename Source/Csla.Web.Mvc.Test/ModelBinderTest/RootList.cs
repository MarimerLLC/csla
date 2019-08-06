//-----------------------------------------------------------------------
// <copyright file="RootList.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Web.Mvc.Test.ModelBinderTest
{
  [Serializable()]
  public class RootList : Csla.BusinessListBase<RootList, Child>
  {
    private RootList()
    {}

    public static RootList Get(int itemsCount)
    {
      return DataPortal.Fetch<RootList>(new SingleCriteria<RootList, int>(itemsCount));
    }
    private void DataPortal_Fetch(SingleCriteria<RootList, int> criteria)
    {
      for (int i = 0; i < criteria.Value; i++)
      {
        this.Add(DataPortal.FetchChild<Child>(i));
      }
    }
  }
}