using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Serialization;

namespace BusinessLibrary
{
  [Serializable]
  [Csla.Server.ObjectFactory("DataAccess.OrderFactory, DataAccess", "FetchInfo")]
  public class OrderInfo : ReadOnlyBase<OrderInfo>
  {
    public static readonly PropertyInfo<int> IdProperty = RegisterProperty<int>(c => c.Id);
    public int Id
    {
      get { return GetProperty(IdProperty); }
      private set { LoadProperty(IdProperty, value); }
    }

    public static readonly PropertyInfo<string> CustomerNameProperty = RegisterProperty<string>(c => c.CustomerName);
    public string CustomerName
    {
      get { return GetProperty(CustomerNameProperty); }
      private set { LoadProperty(CustomerNameProperty, value); }
    }

    public static readonly PropertyInfo<int> LineItemCountProperty = RegisterProperty<int>(c => c.LineItemCount);
    public int LineItemCount
    {
      get { return GetProperty(LineItemCountProperty); }
      private set { LoadProperty(LineItemCountProperty, value); }
    }

    public static void GetOrderInfo(int id, EventHandler<DataPortalResult<OrderInfo>> callback)
    {
      DataPortal.BeginFetch<OrderInfo>(id, callback);
    }

#if !SILVERLIGHT
    public static OrderInfo GetOrderInfo(int id)
    {
      return DataPortal.Fetch<OrderInfo>(id);
    }
#endif
  }
}
