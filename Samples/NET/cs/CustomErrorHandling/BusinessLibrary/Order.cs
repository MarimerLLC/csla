using System;
using Csla;

namespace BusinessLibrary
{
  [Serializable]
  [Csla.Server.ObjectFactory("DataAccess.OrderFactory, DataAccess")]
  public class Order : BusinessBase<Order>
  {
    public static readonly PropertyInfo<int> IdProperty = RegisterProperty(new PropertyInfo<int>("Id", "Id"));
    public int Id
    {
      get { return GetProperty(IdProperty); }
      private set { SetProperty(IdProperty, value); }
    }

    public static readonly PropertyInfo<string> CustomerNameProperty = RegisterProperty(new PropertyInfo<string>("CustomerName", "Customer name"));
    public string CustomerName
    {
      get { return GetProperty(CustomerNameProperty); }
      set { SetProperty(CustomerNameProperty, value); }
    }

    public static readonly PropertyInfo<LineItems> LineItemsProperty = RegisterProperty(new PropertyInfo<LineItems>("LineItems", "Line items"));
    public LineItems LineItems
    {
      get
      {
        if (!FieldManager.FieldExists(LineItemsProperty))
          LoadProperty(LineItemsProperty, LineItems.NewList());
        return GetProperty(LineItemsProperty);
      }
    }

    protected override void AddBusinessRules()
    {
      BusinessRules.AddRule(new Csla.Rules.CommonRules.Required(CustomerNameProperty));
    }

    public Order()
    { /* require use of factory methods */ }

    public static Order NewOrder()
    {
      return DataPortal.Create<Order>();
    }

    public static Order GetOrder(int id)
    {
      return DataPortal.Fetch<Order>(id);
    }

    public new Order Save()
    {
      return base.Save();
    }

    public static void Delete(int id)
    {
      DataPortal.Delete<Order>(new SingleCriteria<Order, int>(id));
    }
  }
}
