//-----------------------------------------------------------------------
// <copyright file="CustomerList.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
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
using Csla.Test;

namespace cslalighttest.CslaDataProvider
{
  [Serializable]
  public class CustomerList : BusinessListBase<CustomerList, Customer>
  {
    private CustomerList() { }

    protected void DataPortal_Fetch([Inject] IDataPortal<Customer> customerDataPortal)
    {
      int maxCustomer = (new Random()).Next(3, 10);
      for (int i = 1; i < maxCustomer; i++)
      {
        Add(customerDataPortal.Fetch(i));
      }
    }

    [Create]
    protected void DataPortal_Create()
    {

    }
        
    [Update]
		protected void DataPortal_Update()
    {
      if (this.Items[0].ThrowException)
        throw new Exception();
      TestResults.Add("CustomerUpdate", "Updating Customer List");
    }
  }
}