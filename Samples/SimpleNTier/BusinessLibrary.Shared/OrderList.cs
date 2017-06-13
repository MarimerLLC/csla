using System;
using System.Threading.Tasks;
using Csla;

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
  }
}
