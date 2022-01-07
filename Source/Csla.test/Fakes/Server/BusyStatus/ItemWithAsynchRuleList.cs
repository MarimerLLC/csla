//-----------------------------------------------------------------------
// <copyright file="ItemWithAsynchRuleList.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Rules;
using System.ComponentModel;
using System.Threading;
using Csla.Core;
using Csla.Serialization;

namespace Csla.Testing.Business.BusyStatus
{
  [Serializable]
  public class ItemWithAsynchRuleList : BusinessListBase<ItemWithAsynchRuleList, ItemWithAsynchRule>
  {

    public static ItemWithAsynchRuleList GetListWithItems(IDataPortal<ItemWithAsynchRuleList> dataPortal)
    {
      return dataPortal.Fetch();
    }

    [Fetch]
    private void Fetch([Inject] IChildDataPortal<ItemWithAsynchRule> childDataPortal)
    {
      Add(ItemWithAsynchRule.GetOneItemForList(childDataPortal, "1"));
      Add(ItemWithAsynchRule.GetOneItemForList(childDataPortal, "2"));
    }

    [Update]
	protected void DataPortal_Update()
    {
      foreach (var oneItem in this)
      {
        oneItem.Update();
      }
    }
  }
}