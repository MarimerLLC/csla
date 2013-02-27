using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Security;
using Csla.Core;
using Csla.Serialization;

namespace ClassLibrary.Business
{
  [Serializable]
  public class CustomerList : BusinessListBase<CustomerList, Customer>
  {
#if SILVERLIGHT
    public CustomerList() { }

    protected override void AddNewCore()
    {
      Customer newItem = Customer.NewChildCustomer();
      this.Add(newItem);
    }

#else
    private CustomerList() { }
#endif


#if !SILVERLIGHT

    protected void DataPortal_Fetch()
    {
      int maxCustomer = (new Random()).Next(3, 10);
      for (int i = 1; i < maxCustomer; i++)
      {
        Add(Customer.GetCustomer(i));
      }
    }

    protected void DataPortal_Create()
    {

    }
        
    protected override void DataPortal_Update()
    {
      Csla.ApplicationContext.GlobalContext["CustomerUpdate"] = "Updating Customer List";
      foreach (var oneCustomer in DeletedList)
      {
        oneCustomer.DataPortal_DeleteSelf();
      }
      DeletedList.Clear();
      foreach (var oneCustomer in this)
      {
        oneCustomer.DataPortal_Update();
      }
    }

#else
    public static void GetCustomerList(EventHandler<DataPortalResult<CustomerList>> handler)
    {
      DataPortal<CustomerList> dp = new DataPortal<CustomerList>();
      dp.FetchCompleted += handler;
      dp.BeginFetch();
    }
#endif
  }
}
