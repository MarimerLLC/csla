using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Validation;
using System.ComponentModel;
using System.Threading;
using Csla.Core;
using Csla.Serialization;

namespace Csla.Testing.Business.BusyStatus
{
  [Serializable]
  public class ItemWithAsynchRuleList : BusinessListBase<ItemWithAsynchRuleList, ItemWithAsynchRule>
  {

    public static ItemWithAsynchRuleList GetListWithItems()
    {
      ItemWithAsynchRuleList returnValue = new ItemWithAsynchRuleList();
      returnValue.Add(ItemWithAsynchRule.GetOneItemForList("1"));
      returnValue.Add(ItemWithAsynchRule.GetOneItemForList("2"));
      return returnValue;
    }
#if SILVERLIGHT
    public void DataPortal_Update(Csla.DataPortalClient.LocalProxy<ItemWithAsynchRuleList>.CompletedHandler handler)
    {
      foreach (var oneItem in this)
      {
        oneItem.DataPortal_Update();
      }
      handler(this, null);
    }
#else
    protected override void DataPortal_Update()
    {
      foreach (var oneItem in this)
      {
        oneItem.DataPortal_Update();
      }
    }
#endif
  }
}
