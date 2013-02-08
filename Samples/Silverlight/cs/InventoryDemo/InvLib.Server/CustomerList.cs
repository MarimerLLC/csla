using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Serialization;

namespace InvLib
{
  [Serializable]
  public partial class CustomerList : ReadOnlyListBase<CustomerList, CustomerInfo>
  {
    #region Factory Methods

    public static void GetCustomerList(EventHandler<DataPortalResult<CustomerList>> callback)
    {
      var dp = new DataPortal<CustomerList>();
      dp.FetchCompleted += callback;
      dp.BeginFetch();
    }

    #endregion
  }
}
