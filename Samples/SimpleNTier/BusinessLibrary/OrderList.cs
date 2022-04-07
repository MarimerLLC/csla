using System;
using Csla;

namespace BusinessLibrary
{
  [Serializable]
  [Csla.Server.ObjectFactory("DataAccess.OrderFactory, DataAccess", "FetchList")]
  public class OrderList : ReadOnlyListBase<OrderList, OrderInfo>
  {
  }
}
