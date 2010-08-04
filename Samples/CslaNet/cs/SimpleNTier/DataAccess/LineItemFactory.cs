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

    internal void FetchItems(LineItems obj)
    {
      var rlce = obj.RaiseListChangedEvents;
      obj.RaiseListChangedEvents = false;
      obj.Add(Fetch(1, "Line 1"));
      obj.Add(Fetch(2, "Line Test"));
      obj.Add(Fetch(3, "Line 112"));
      obj.Add(Fetch(4, "Line as1"));
      obj.RaiseListChangedEvents = rlce;
    }

    private LineItem Fetch(int id, string name)
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
