using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Data;
using Csla.Server;

namespace InvDal
{
  public class CustomerDetail : ObjectFactory
  {
    public InvLib.CustomerDetail Fetch(SingleCriteria<InvLib.CustomerDetail, int> criteria)
    {
      var item = new InvLib.CustomerDetail();
      var p = (from r in MockDb.CustomerData
               where r.Id == criteria.Value
               join l in MockDb.LocationData on r.LocationId equals l.Id
               select new { Id = r.Id, Name = r.Name, Lat = l.Lat, Long = l.Long }).Single();
      LoadProperty(item, InvLib.CustomerDetail.IdProperty, p.Id);
      LoadProperty(item, InvLib.CustomerDetail.NameProperty, p.Name);
      LoadProperty(item, InvLib.CustomerDetail.LocationProperty, p.Lat + "/" + p.Long);


      var orders = from o in MockDb.OrderData
                where o.CustomerId == p.Id
                select o;

      SetIsReadOnly(item.Orders, false);
      foreach (var order in orders)
      {
        var pli = new InvLib.CustomerOrderInfo();
        LoadProperty(pli, InvLib.CustomerOrderInfo.IdProperty, order.Id);
        LoadProperty(pli, InvLib.CustomerOrderInfo.OrderDateProperty, order.OrderDate);
        var amt = (from li in MockDb.OrderLineItemData
                   where li.OrderId == order.Id
                   select li.Price * li.Quantity).Sum();
        LoadProperty(pli, InvLib.CustomerOrderInfo.AmountProperty, amt);
        item.Orders.Add(pli);
      }
      SetIsReadOnly(item.Orders, true);

      return item;
    }
  }
}
