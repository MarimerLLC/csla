using System;
using Csla;

namespace BusinessLibrary
{
  [Serializable]
  [Csla.Server.ObjectFactory("DataAccess.OrderFactory, DataAccess")]
  public class Order : BusinessBase<Order>
  {
    public static PropertyInfo<int> IdProperty = RegisterProperty<int>(p => p.Id);
    public int Id
    {
      get { return GetProperty(IdProperty); }
      private set { SetProperty(IdProperty, value); }
    }

    public static PropertyInfo<string> CustomerNameProperty = RegisterProperty<string>(p => p.CustomerName);
    public string CustomerName
    {
      get { return GetProperty(CustomerNameProperty); }
      set { SetProperty(CustomerNameProperty, value); }
    }

    public static PropertyInfo<LineItems> LineItemsProperty = RegisterProperty<LineItems>(p => p.LineItems, RelationshipTypes.LazyLoad);
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
  }
}
