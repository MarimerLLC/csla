using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Serialization;

namespace DataBinding.Business
{
  [Serializable]
  public partial class CustomerList : BusinessListBase<CustomerList, Customer>
  {
    private CustomerList() { }

    public static void FetchByName(string name, EventHandler<DataPortalResult<CustomerList>> completed)
    {
      DataPortal<CustomerList> dp = new DataPortal<CustomerList>();
      dp.FetchCompleted += completed;
      dp.BeginFetch(new SingleCriteria<CustomerList, string>(name));
    }
  }
}
