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

    internal void UpdateItems(LineItems lineItems)
    {
      GetDeletedList<LineItem>(lineItems).Clear();
      foreach (var item in lineItems)
        MarkOld(item);
    }
  }
}
