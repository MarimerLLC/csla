//-----------------------------------------------------------------------
// <copyright file="CustomerWithErrorList.cs" company="Marimer LLC">
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
  public class CustomerWithErrorList : BusinessListBase<CustomerWithErrorList, CustomerWithError>
  {
#if SILVERLIGHT
    public CustomerWithErrorList() { }

    protected override void AddNewCore()
    {
      CustomerWithError newItem = CustomerWithError.NewCustomerWithError();
      this.Add(newItem);
    }

#else
    private CustomerWithErrorList() { }
#endif


#if !SILVERLIGHT

    protected void DataPortal_Fetch()
    {
      int maxCustomerWithError = (new Random()).Next(3, 10);
      for (int i = 1; i < maxCustomerWithError; i++)
      {
        Add(CustomerWithError.GetCustomerWithError(i));
      }
    }

    protected override void DataPortal_Create()
    {

    }
        
    protected override void DataPortal_Update()
    {
      if (this.Items[0].ThrowException)
        throw new Exception();
      Csla.ApplicationContext.GlobalContext["CustomerWithErrorUpdate"] = "Updating CustomerWithError List";
    }

#else
    public static void GetCustomerWithErrorList(EventHandler<DataPortalResult<CustomerWithErrorList>> handler)
    {
      DataPortal<CustomerWithErrorList> dp = new DataPortal<CustomerWithErrorList>();
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