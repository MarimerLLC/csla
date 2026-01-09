using System;
using Csla;

namespace BusinessLibrary
{
  [Csla.Server.ObjectFactory("DataAccess.OrderFactory, DataAccess", "FetchInfo")]
  [CslaImplementProperties]
  public partial class OrderInfo : ReadOnlyBase<OrderInfo>
  {
    public partial int Id { get; private set; }

    public partial string CustomerName { get; private set; }

    public partial int LineItemCount { get; private set; }
  }
}
