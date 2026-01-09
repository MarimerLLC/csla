using System;
using Csla;

namespace BusinessLibrary
{
  [Csla.Server.ObjectFactory("DataAccess.OrderFactory, DataAccess", "FetchList")]
  public class OrderList : ReadOnlyListBase<OrderList, OrderInfo>
  {
  }
}
