using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Data;
using Csla.Server;

namespace InvDal
{
  public class ProductDetail : ObjectFactory
  {
    public InvLib.ProductDetail Fetch(SingleCriteria<InvLib.ProductDetail, int> criteria)
    {
      var item = new InvLib.ProductDetail();
      var p = (from r in MockDb.ProductData where r.Id == criteria.Value select r).Single();
      LoadProperty(item, InvLib.ProductDetail.IdProperty, p.Id);
      LoadProperty(item, InvLib.ProductDetail.NameProperty, p.Name);
      LoadProperty(item, InvLib.ProductDetail.PriceProperty, p.Price);
      LoadProperty(item, InvLib.ProductDetail.CategoryIdProperty, p.CategoryId);
      var totalQoh = (from b in MockDb.BinData
                 where b.ProductId == p.Id
                 select b.Quantity).Sum();
      LoadProperty(item, InvLib.ProductDetail.QohProperty, totalQoh);

      var l = (from r in MockDb.BinData
              join w in MockDb.WarehouseData on r.WarehouseId equals w.Id
              where r.ProductId == p.Id
              orderby w.Name
              select new { BinId = r.Id, WarehouseName = w.Name, Qoh = r.Quantity }).GroupBy(s => s.WarehouseName);

      SetIsReadOnly(item.Locations, false);
      foreach (var bin in l)
      {
        var pli = new InvLib.ProductLocationInfo();
        LoadProperty(pli, InvLib.ProductLocationInfo.WarehouseNameProperty, bin.Key);
        var q = (from i in bin select i.Qoh).Sum();
        LoadProperty(pli, InvLib.ProductLocationInfo.QohProperty, q);
        item.Locations.Add(pli);
      }
      SetIsReadOnly(item.Locations, true);

      return item;
    }
  }
}
