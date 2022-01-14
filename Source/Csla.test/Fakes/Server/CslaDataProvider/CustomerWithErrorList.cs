//-----------------------------------------------------------------------
// <copyright file="CustomerWithErrorList.cs" company="Marimer LLC">
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
  public class CustomerWithErrorList : BusinessListBase<CustomerWithErrorList, CustomerWithError>
  {
    private CustomerWithErrorList() { }

    protected void DataPortal_Fetch()
    {
      int maxCustomerWithError = (new Random()).Next(3, 10);
      for (int i = 1; i < maxCustomerWithError; i++)
      {
        Add(CustomerWithError.GetCustomerWithError(i));
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
      TestResults.Add("CustomerWithErrorUpdate", "Updating CustomerWithError List");
    }

  }
}