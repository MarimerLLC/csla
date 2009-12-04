using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Server;
using BusinessLibrary;

namespace DataAccess
{
  public class OrderFactory : ObjectFactory
  {
    private static int _lastId;

    public Order Create()
    {
      var obj = (Order)Activator.CreateInstance(typeof(Order), true);
      var id = System.Threading.Interlocked.Decrement(ref _lastId);
      LoadProperty(obj, Order.IdProperty, id);
      MarkNew(obj);
      CheckRules(obj);
      return obj;
    }

    public Order Fetch(SingleCriteria<Order, int> criteria)
    {
      var obj = (Order)Activator.CreateInstance(typeof(Order), true);
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
        MarkOld(obj);
      }
      return obj;
    }

    public void Delete(SingleCriteria<Order, int> criteria)
    {
      // delete data
    }
  }
}
