using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Serialization;

namespace InvLib
{
  [Serializable]
  public partial class CustomerDetail : ReadOnlyBase<CustomerDetail>
  {
    public static PropertyInfo<int> IdProperty = RegisterProperty<int>(c => c.Id);
    public int Id
    {
      get { return GetProperty(IdProperty); }
    }

    public static PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);
    public string Name
    {
      get { return GetProperty(NameProperty); }
    }

    public static PropertyInfo<string> LocationProperty = RegisterProperty<string>(c => c.Location);
    public string Location
    {
      get { return GetProperty(LocationProperty); }
    }

    public static PropertyInfo<CustomerOrders> OrdersProperty = RegisterProperty<CustomerOrders>(c => c.Orders);
    public CustomerOrders Orders
    {
      get
      {
        if (!FieldManager.FieldExists(OrdersProperty))
          LoadProperty(OrdersProperty, new CustomerOrders());
        return GetProperty(OrdersProperty);
      }
    }

    #region Factory Methods

    public static void GetCustomerDetail(int id, EventHandler<DataPortalResult<CustomerDetail>> callback)
    {
      var dp = new DataPortal<CustomerDetail>();
      dp.FetchCompleted += callback;
      dp.BeginFetch(new SingleCriteria<CustomerDetail, int>(id));
    }

    #endregion
  }

  [Serializable]
  public class CustomerOrders : ReadOnlyListBase<CustomerOrders, CustomerOrderInfo>
  {
  }

  [Serializable]
  public class CustomerOrderInfo : ReadOnlyBase<CustomerOrderInfo>
  {
    public static PropertyInfo<int> IdProperty = RegisterProperty<int>(c => c.Id);
    public int Id
    {
      get { return GetProperty(IdProperty); }
    }

    public static PropertyInfo<DateTime?> OrderDateProperty = RegisterProperty<DateTime?>(c => c.OrderDate);
    public DateTime? OrderDate
    {
      get { return GetProperty(OrderDateProperty); }
    }

    public static PropertyInfo<float> AmountProperty = RegisterProperty<float>(c => c.Amount);
    public float Amount
    {
      get { return GetProperty(AmountProperty); }
    }
  }
}
