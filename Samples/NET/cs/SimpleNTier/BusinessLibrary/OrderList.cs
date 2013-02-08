using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Csla;
using Csla.Serialization;

namespace BusinessLibrary
{
  [Serializable]
  [Csla.Server.ObjectFactory("DataAccess.OrderFactory, DataAccess", "FetchList")]
  public class OrderList : ReadOnlyListBase<OrderList, OrderInfo>
  {
    public async static Task<OrderList> GetListAsync()
    {
      return await DataPortal.FetchAsync<OrderList>();
    }

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
