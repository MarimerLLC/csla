using System;
using System.ComponentModel.DataAnnotations;
using Csla;
using Csla.Serialization;

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
    [Required]
    [Display(Name = "Customer name")]
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
        {
          LoadProperty(LineItemsProperty, DataPortal.CreateChild<LineItems>());
          OnPropertyChanged(LineItemsProperty);
        }
        return GetProperty(LineItemsProperty);
      }
    }

    public static void NewOrder(EventHandler<DataPortalResult<Order>> callback)
    {
      DataPortal.BeginCreate<Order>(callback);
    }

    public static void GetOrder(int id, EventHandler<DataPortalResult<Order>> callback)
    {
      DataPortal.BeginFetch<Order>(id, callback);
    }

#if !SILVERLIGHT
    private Order()
    { /* require use of factory methods */ }

    public static Order NewOrder()
    {
      return DataPortal.Create<Order>();
    }

    public static Order GetOrder(int id)
    {
      return DataPortal.Fetch<Order>(id);
    }
#endif
  }
}
