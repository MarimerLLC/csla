using System;
using System.Collections.Generic;
using System.Globalization;

namespace InvDal
{
  public static class MockDb
  {
    public static List<ProductData> ProductData = new List<ProductData>();
    public static List<ProductCategoryData> ProductCategoryData = new List<ProductCategoryData>();
    public static List<WarehouseData> WarehouseData = new List<WarehouseData>();
    public static List<CustomerData> CustomerData = new List<CustomerData>();
    public static List<LocationData> LocationData = new List<LocationData>();
    public static List<BinData> BinData = new List<BinData>();
    public static List<OrderData> OrderData = new List<OrderData>();
    public static List<OrderLineItemData> OrderLineItemData = new List<OrderLineItemData>();

    static MockDb()
    {
      LoadProductCategories();
      LoadLocations();
      LoadWarehouses();
      LoadCustomers();
      LoadProducts();
      LoadBins();
      LoadOrders();
      LoadOrderLineItems();
    }

    private static void LoadProductCategories()
    {
      ProductCategoryData.Add(new ProductCategoryData { Id = 1, Name = "Food" });
      ProductCategoryData.Add(new ProductCategoryData { Id = 2, Name = "Drink" });
      ProductCategoryData.Add(new ProductCategoryData { Id = 3, Name = "Clothing" });
      ProductCategoryData.Add(new ProductCategoryData { Id = 4, Name = "Other" });
    }

    private static void LoadProducts()
    {
      ProductData.Add(new ProductData { Id = 1, Name = "Candy", Price = 0.75f, CategoryId = 1 });
      ProductData.Add(new ProductData { Id = 2, Name = "Soda (20oz)", Price = 1.95f, CategoryId = 2 });
      ProductData.Add(new ProductData { Id = 4, Name = "Soda (16oz)", Price = 1.25f, CategoryId = 2 });
      ProductData.Add(new ProductData { Id = 5, Name = "Bread", Price = 4.25f, CategoryId = 1 });
      ProductData.Add(new ProductData { Id = 3, Name = "Butter", Price = 2.5f, CategoryId = 1 });
      ProductData.Add(new ProductData { Id = 10, Name = "Jelly", Price = 3.75f, CategoryId = 1 });
      ProductData.Add(new ProductData { Id = 12, Name = "Sweater", Price = 39.95f, CategoryId = 3 });

      for (int i = 0; i < 500; i++)
      {
        ProductData.Add(new ProductData { Id = i + 20, Name = "Product " + i.ToString(), Price = 3.75f, CategoryId = 4 });
      }
    }

    private static void LoadLocations()
    {
      LocationData.Add(new LocationData { Id = 1, Lat = 100, Long = 100 });
      LocationData.Add(new LocationData { Id = 2, Lat = 10, Long = 1 });
      LocationData.Add(new LocationData { Id = 3, Lat = 200, Long = 150 });
      LocationData.Add(new LocationData { Id = 4, Lat = 50, Long = 175 });
      LocationData.Add(new LocationData { Id = 5, Lat = 25, Long = 185 });
      LocationData.Add(new LocationData { Id = 6, Lat = 125, Long = 80 });
      LocationData.Add(new LocationData { Id = 7, Lat = 210, Long = 110 });
      LocationData.Add(new LocationData { Id = 8, Lat = 500, Long = 250 });
    }

    private static void LoadWarehouses()
    {
      WarehouseData.Add(new WarehouseData { Id = 1, Name = "Warehouse 1", LocationId = 1 });
      WarehouseData.Add(new WarehouseData { Id = 2, Name = "Warehouse 2", LocationId = 3 });
      WarehouseData.Add(new WarehouseData { Id = 3, Name = "Warehouse 3", LocationId = 5 });
      WarehouseData.Add(new WarehouseData { Id = 4, Name = "Warehouse 4", LocationId = 7 });
    }

    private static void LoadCustomers()
    {
      CustomerData.Add(new CustomerData { Id = 1, Name = "Customer 1", LocationId = 2 });
      CustomerData.Add(new CustomerData { Id = 2, Name = "Customer 2", LocationId = 4 });
      CustomerData.Add(new CustomerData { Id = 3, Name = "Customer 3", LocationId = 6 });
      CustomerData.Add(new CustomerData { Id = 4, Name = "Customer 4", LocationId = 8 });
    }

    private static void LoadBins()
    {
      BinData.Add(new BinData { WarehouseId = 1, Id = 1, ProductId = 1, Quantity = 100 });
      BinData.Add(new BinData { WarehouseId = 2, Id = 1, ProductId = 2, Quantity = 50 });
      BinData.Add(new BinData { WarehouseId = 2, Id = 2, ProductId = 1, Quantity = 25 });
      BinData.Add(new BinData { WarehouseId = 4, Id = 1, ProductId = 3, Quantity = 120 });
      BinData.Add(new BinData { WarehouseId = 4, Id = 2, ProductId = 4, Quantity = 40 });
      BinData.Add(new BinData { WarehouseId = 3, Id = 1, ProductId = 3, Quantity = 10 });
      BinData.Add(new BinData { WarehouseId = 1, Id = 3, ProductId = 5, Quantity = 1 });
      BinData.Add(new BinData { WarehouseId = 3, Id = 4, ProductId = 1, Quantity = 133 });
      BinData.Add(new BinData { WarehouseId = 1, Id = 9, ProductId = 1, Quantity = 75 });
    }

    private static void LoadOrders()
    {
      var culture = CultureInfo.GetCultureInfo("en-US");
      OrderData.Add(new OrderData { Id = 1, CustomerId = 1, OrderDate = DateTime.Parse("10/5/08", culture) });
      OrderData.Add(new OrderData { Id = 2, CustomerId = 2, OrderDate = DateTime.Parse("10/15/08", culture) });
      OrderData.Add(new OrderData { Id = 3, CustomerId = 3, OrderDate = DateTime.Parse("10/25/08", culture) });
      OrderData.Add(new OrderData { Id = 4, CustomerId = 4, OrderDate = DateTime.Parse("11/5/08", culture) });
      OrderData.Add(new OrderData { Id = 5, CustomerId = 1, OrderDate = DateTime.Parse("10/1/08", culture), ShipDate = DateTime.Parse("1/12/09", culture) });
      OrderData.Add(new OrderData { Id = 6, CustomerId = 2, OrderDate = DateTime.Parse("9/2/08", culture) });
      OrderData.Add(new OrderData { Id = 13, CustomerId = 3, OrderDate = DateTime.Parse("12/6/08", culture) });
      OrderData.Add(new OrderData { Id = 14, CustomerId = 4, OrderDate = DateTime.Parse("11/13/08", culture) });
      OrderData.Add(new OrderData { Id = 11, CustomerId = 1, OrderDate = DateTime.Parse("12/20/08", culture) });
    }

    private static void LoadOrderLineItems()
    {
      OrderLineItemData.Add(new OrderLineItemData { OrderId = 1, ProductId = 1, Quantity = 10, Price = 0.4f });
      OrderLineItemData.Add(new OrderLineItemData { OrderId = 5, ProductId = 1, Quantity = 1, Price = 1.4f });
      OrderLineItemData.Add(new OrderLineItemData { OrderId = 5, ProductId = 2, Quantity = 13, Price = 1.4f });
      OrderLineItemData.Add(new OrderLineItemData { OrderId = 5, ProductId = 3, Quantity = 9, Price = 2.54f });
    }
  }
}
