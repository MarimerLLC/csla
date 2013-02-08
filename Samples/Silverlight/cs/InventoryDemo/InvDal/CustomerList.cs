using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Server;
using Csla.Data;

namespace InvDal
{
  public class CustomerList : ObjectFactory
  {
    public InvLib.CustomerList Fetch()
    {
      var item = new InvLib.CustomerList();
      base.SetIsReadOnly(item, false);
      var wd = from r in MockDb.CustomerData
               join l in MockDb.LocationData on r.LocationId equals l.Id
               select new { Id = r.Id, Name = r.Name, Lat = l.Lat, Long = l.Long };
      foreach (var p in wd)
      {
        var oc = (from o in MockDb.OrderData
                  where o.CustomerId == p.Id
                  select o).Count();
        item.Add(MakeChild(p.Id, p.Name, p.Lat, p.Long, oc));
      }
      base.SetIsReadOnly(item, true);
      return item;
    }

    private InvLib.CustomerInfo MakeChild(int id, string name, int lat, int lng, int orderCount)
    {
      var child = new InvLib.CustomerInfo();
      LoadProperty(child, InvLib.CustomerInfo.IdProperty, id);
      LoadProperty(child, InvLib.CustomerInfo.NameProperty, name);
      LoadProperty(child, InvLib.CustomerInfo.LocationProperty, lat + "/" + lng);
      LoadProperty(child, InvLib.CustomerInfo.OrderCountProperty, orderCount);
      return child;
    }
  }
}
