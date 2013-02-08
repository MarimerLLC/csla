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
    public Order Create()
    {
      var obj = (Order)MethodCaller.CreateInstance(typeof(Order));
      LoadProperty(obj, Order.IdProperty, -1);
      MarkNew(obj);
      CheckRules(obj);
      return obj;
    }

    public Order Fetch(int id)
    {
      var obj = (Order)MethodCaller.CreateInstance(typeof(Order));
      var item = (from r in MockDb.Orders
                  where r.Id == id
                  select r).First();
      using (BypassPropertyChecks(obj))
      {
        LoadProperty(obj, Order.IdProperty, item.Id);
        LoadProperty(obj, Order.CustomerNameProperty, item.CustomerName);
        var lif = new LineItemFactory();
        LoadProperty(obj, Order.LineItemsProperty, lif.FetchItems(id));
      }
      MarkOld(obj);
      return obj;
    }

    public OrderList FetchList()
    {
      var obj = (OrderList)MethodCaller.CreateInstance(typeof(OrderList));
      obj.RaiseListChangedEvents = false;

      obj.AddRange(from r in MockDb.Orders
                   select GetOrderInfo(r.Id, r.CustomerName));

      obj.RaiseListChangedEvents = true;
      return obj;
    }

    private OrderInfo GetOrderInfo(int id, string customerName)
    {
      var obj = (OrderInfo)MethodCaller.CreateInstance(typeof(OrderInfo));
      LoadProperty(obj, OrderInfo.IdProperty, id);
      LoadProperty(obj, OrderInfo.CustomerNameProperty, customerName);
      return obj;
    }

    public OrderInfo FetchInfo(int id)
    {
      var obj = (OrderInfo)MethodCaller.CreateInstance(typeof(OrderInfo));
      var item = (from r in MockDb.Orders
                  where r.Id == id
                  select r).First();
      LoadProperty(obj, OrderInfo.IdProperty, item.Id);
      LoadProperty(obj, OrderInfo.CustomerNameProperty, item.CustomerName);
      var itemCount = MockDb.LineItems.Where(r => r.OrderId == item.Id).Count();
      LoadProperty(obj, OrderInfo.LineItemCountProperty, itemCount);
      return obj;
    }

    public Order Update(Order obj)
    {
      if (obj.IsDeleted)
      {
        if (!obj.IsNew)
        {
          // delete data
          Delete(obj.Id);
          return Create();
        }
        MarkNew(obj);
      }
      else
      {
        if (obj.IsNew)
        {
          // insert data
          var id = MockDb.Orders.Max(r => r.Id) + 1;
          LoadProperty(obj, Order.IdProperty, id);
          MockDb.Orders.Add(new OrderData { Id = id, CustomerName = obj.CustomerName });
        }
        else
        {
          // update data
          var item = MockDb.Orders.Where(r => r.Id == obj.Id).First();
          item.CustomerName = obj.CustomerName;
        }
        var lif = new LineItemFactory();
        lif.UpdateItems(obj, obj.LineItems);
        MarkOld(obj);
      }
      return obj;
    }

    public void Delete(int id)
    {
      // delete data
      var lineItems = from r in MockDb.LineItems
                      where r.OrderId == id
                      select r;
      foreach (var item in lineItems.ToList())
        MockDb.LineItems.Remove(item);
      MockDb.Orders.Remove((MockDb.Orders.Where(r => r.Id == id).First()));
    }
  }
}
