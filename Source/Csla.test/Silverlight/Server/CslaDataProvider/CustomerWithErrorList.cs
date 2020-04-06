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

    protected override void DataPortal_Create()
    {

    }
        
    protected override void DataPortal_Update()
    {
      if (this.Items[0].ThrowException)
        throw new Exception();
#pragma warning disable CS0618 // Type or member is obsolete
      Csla.ApplicationContext.GlobalContext["CustomerWithErrorUpdate"] = "Updating CustomerWithError List";
#pragma warning restore CS0618 // Type or member is obsolete
    }

  }
}