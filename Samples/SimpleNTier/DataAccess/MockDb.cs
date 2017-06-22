using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess
{
  public static class MockDb
  {
    public static List<OrderData> Orders;
    public static List<LineItemData> LineItems;

    static MockDb()
    {
      Orders = new List<OrderData> 
      { 
        new OrderData { Id = 441, CustomerName = "Fred" },
        new OrderData { Id = 231, CustomerName = "Mary" },
        new OrderData { Id = 123, CustomerName = "Melissa" },
        new OrderData { Id = 230, CustomerName = "Anthony" }
      };
      LineItems = new List<LineItemData> 
      { 
        new LineItemData { OrderId = 441, Id = 1, Name = "Product 1" },
        new LineItemData { OrderId = 441, Id = 2, Name = "Product A" },
        new LineItemData { OrderId = 441, Id = 3, Name = "Product 42" },
        new LineItemData { OrderId = 123, Id = 1, Name = "Product 1" },
        new LineItemData { OrderId = 123, Id = 2, Name = "Product 44" },
        new LineItemData { OrderId = 123, Id = 3, Name = "Product A21" }
      };
    }
  }

  public class OrderData
  {
    public int Id { get; set; }
    public string CustomerName { get; set; }
  }

  public class LineItemData
  {
    public int OrderId { get; set; }
    public int Id { get; set; }
    public string Name { get; set; }
  }
}
