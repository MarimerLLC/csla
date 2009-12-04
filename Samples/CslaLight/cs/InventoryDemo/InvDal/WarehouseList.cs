using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Server;

namespace InvDal
{
  public class WarehouseList : ObjectFactory
  {
    public InvLib.WarehouseList Fetch()
    {
      var item = new InvLib.WarehouseList();
      base.SetIsReadOnly(item, false);
      var wd = from r in MockDb.WarehouseData
               join l in MockDb.LocationData on r.LocationId equals l.Id
               select new { Id = r.Id, Name = r.Name, Lat = l.Lat, Long = l.Long };
      foreach (var p in wd)
      {
        item.Add(MakeChild(p.Id, p.Name, p.Lat, p.Long));
      }
      base.SetIsReadOnly(item, true);
      return item;
    }

    private InvLib.WarehouseInfo MakeChild(int id, string name, int lat, int lng)
    {
      var child = new InvLib.WarehouseInfo();
      LoadProperty(child, InvLib.WarehouseInfo.IdProperty, id);
      LoadProperty(child, InvLib.WarehouseInfo.NameProperty, name);
      LoadProperty(child, InvLib.WarehouseInfo.LocationProperty, lat + "/" + lng);
      return child;
    }
  }
}
