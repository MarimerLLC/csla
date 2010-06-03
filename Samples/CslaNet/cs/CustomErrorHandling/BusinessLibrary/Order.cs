using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;

namespace BusinessLibrary
{
  [Serializable]
  [Csla.Server.ObjectFactory("DataAccess.OrderFactory, DataAccess")]
  public class Order : BusinessBase<Order>
  {
    public static PropertyInfo<int> IdProperty = RegisterProperty(new PropertyInfo<int>("Id", "Id"));
    public int Id
    {
      get { return GetProperty(IdProperty); }
      private set { SetProperty(IdProperty, value); }
    }

    public static PropertyInfo<string> CustomerNameProperty = RegisterProperty(new PropertyInfo<string>("CustomerName", "Customer name"));
    public string CustomerName
    {
      get { return GetProperty(CustomerNameProperty); }
      set { SetProperty(CustomerNameProperty, value); }
    }

    public static PropertyInfo<LineItems> LineItemsProperty = RegisterProperty(new PropertyInfo<LineItems>("LineItems", "Line items"));
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

    private Order()
    { /* require use of factory methods */ }

    public static Order NewOrder()
    {
      return DataPortal.Create<Order>();
    }

    public override Order Save()
    {
      return base.Save();
    }

    public static void Delete(int id)
    {
        DataPortal.Delete<Order>(new SingleCriteria<Order, int> (id));
    }
  }
}
