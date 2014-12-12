using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Reflection;
using Csla.Server;
using BusinessLibrary;

namespace DataAccess
{
  public class LineItemFactory : ObjectFactory
  {
    public LineItem Create()
    {
      var obj = (LineItem)MethodCaller.CreateInstance(typeof(LineItem));
      MarkAsChild(obj);
      MarkNew(obj);
      CheckRules(obj);
      return obj;
    }

    internal LineItems FetchItems(int orderId)
    {
      var obj = (LineItems)MethodCaller.CreateInstance(typeof(LineItems));
      var rlce = obj.RaiseListChangedEvents;
      obj.RaiseListChangedEvents = false;

      obj.AddRange(from r in MockDb.LineItems
                   where r.OrderId == orderId
                   select GetLineItem(r.Id, r.Name));

      obj.RaiseListChangedEvents = rlce;
      return obj;
    }

    private LineItem GetLineItem(int id, string name)
    {
      var obj = (LineItem)MethodCaller.CreateInstance(typeof(LineItem));
      MarkAsChild(obj);
      using (BypassPropertyChecks(obj))
      {
        LoadProperty(obj, LineItem.IdProperty, id);
        LoadProperty(obj, LineItem.NameProperty, name);
      }
      MarkOld(obj);
      return obj;
    }

    internal void UpdateItems(Order order, LineItems lineItems)
    {
      var delList = GetDeletedList<LineItem>(lineItems);
      foreach (var item in delList)
        MockDb.LineItems.Remove(MockDb.LineItems.Where(r => r.Id == item.Id).First());
      delList.Clear();

      foreach (var item in lineItems)
      {
        if (item.IsNew)
        {
          var data = new LineItemData { OrderId = order.Id, Id = item.Id, Name = item.Name };
          MockDb.LineItems.Add(data);
        }
        else
        {
          var data = MockDb.LineItems.Where(r => r.Id == item.Id).First();
          data.Name = item.Name;
        }
        MarkOld(item);
      }
    }
  }
}
