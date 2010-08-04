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
  public class OrderFactory : ObjectFactory
  {
    private static int _lastId;

    public Order Create()
    {
      var obj = (Order)MethodCaller.CreateInstance(typeof(Order));
      var id = System.Threading.Interlocked.Decrement(ref _lastId);
      LoadProperty(obj, Order.IdProperty, id);
      MarkNew(obj);
      CheckRules(obj);
      return obj;
    }

    public Order Fetch(int id)
    {
      var obj = (Order)MethodCaller.CreateInstance(typeof(Order));
      using (BypassPropertyChecks(obj))
      {
        LoadProperty(obj, Order.IdProperty, id);
        LoadProperty(obj, Order.CustomerNameProperty, "Test name");
        var lif = new LineItemFactory();
        lif.FetchItems(obj.LineItems);
      }
      MarkOld(obj);
      return obj;
    }

    public Order Update(Order obj)
    {
      if (obj.IsDeleted)
      {
        if (!obj.IsNew)
        {
          // delete data
          return Create();
        }
        MarkNew(obj);
      }
      else
      {
        if (obj.IsNew)
        {
          // insert data
          LoadProperty(obj, Order.IdProperty, System.Math.Abs(obj.Id));
        }
        else
        {
          // update data
        }
        var lif = new LineItemFactory();
        lif.UpdateItems(obj.LineItems);
        MarkOld(obj);
      }
      return obj;
    }

    public void Delete(int id)
    {
      // delete data
    }
  }
}
