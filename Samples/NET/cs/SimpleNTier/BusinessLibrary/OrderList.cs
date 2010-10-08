using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Serialization;

namespace BusinessLibrary
{
  [Serializable]
  [Csla.Server.ObjectFactory("DataAccess.OrderFactory, DataAccess", "FetchList")]
  public class OrderList : ReadOnlyListBase<OrderList, OrderInfo>
  {
    public static void GetList(EventHandler<DataPortalResult<OrderList>> callback)
    {
      DataPortal.BeginFetch<OrderList>(callback);
    }

#if ! SILVERLIGHT
    public static OrderList GetList()
    {
      return DataPortal.Fetch<OrderList>();
    }
#endif
  }
}
