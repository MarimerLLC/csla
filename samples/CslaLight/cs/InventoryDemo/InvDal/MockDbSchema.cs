using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InvDal
{
  public class ProductData
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public float Price { get; set; }
    public int CategoryId { get; set; } // foreign key
  }

  public class ProductCategoryData
  {
    public int Id { get; set; }
    public string Name { get; set; }
  }

  public class WarehouseData
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public int LocationId { get; set; } // foreign key
  }

  public class CustomerData
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public int LocationId { get; set; } // foreign key
  }

  public class LocationData
  {
    public int Id { get; set; }
    public int Lat { get; set; }
    public int Long { get; set; }
  }

  public class BinData
  {
    public int WarehouseId { get; set; } // foreign key
    public int Id { get; set; }
    public int ProductId { get; set; } // foreign key
    public int Quantity { get; set; }
  }

  public class OrderData
  {
    public int Id { get; set; }
    public int CustomerId { get; set; } // foreign key
    public DateTime OrderDate { get; set; }
    public DateTime? ShipDate { get; set; }
  }

  public class OrderLineItemData
  {
    public int OrderId { get; set; } // foreign key
    public int ProductId { get; set; } // foreign key
    public int Quantity { get; set; }
    public float Price { get; set; }
  }
}
