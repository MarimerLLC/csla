//-----------------------------------------------------------------------
// <copyright file="CustomerList.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Security;
using Csla.Core;
using Csla.Serialization;

namespace cslalighttest.CslaDataProvider
{
  [Serializable]
  public class CustomerList : BusinessListBase<CustomerList, Customer>
  {
#if SILVERLIGHT
    public CustomerList() { }

    protected override void AddNewCore()
    {
      Customer newItem = Customer.NewCustomer();
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

    protected override void DataPortal_Create()
    {

    }
        
    protected override void DataPortal_Update()
    {
      if (this.Items[0].ThrowException)
        throw new Exception();
      Csla.ApplicationContext.GlobalContext["CustomerUpdate"] = "Updating Customer List";
    }

#else
    public static void GetCustomerList(EventHandler<DataPortalResult<CustomerList>> handler)
    {
      DataPortal<CustomerList> dp = new DataPortal<CustomerList>();
      dp.FetchCompleted += handler;
      dp.BeginFetch();
    }

    public override void BeginSave(EventHandler<SavedEventArgs> handler, object userState)
    {
      base.BeginSave(handler, userState);
    }
#endif
  }
}