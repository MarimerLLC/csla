//-----------------------------------------------------------------------
// <copyright file="CustomerContactList.cs" company="Marimer LLC">
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
  public class CustomerContactList : BusinessListBase<CustomerContactList, CustomerContact>
  {

    public Customer MyParent
    {
      get { return (Customer)this.Parent; }
    }

    private CustomerContactList() { }

    private void Child_Fetch(int customerID, [Inject] IChildDataPortal<CustomerContact> childDataPortal)
    {
      this.RaiseListChangedEvents = false;
      for (int i = 1; i <= customerID; i++)
      {
        Add(childDataPortal.FetchChild(customerID, i, "First Name # " + i.ToString(), "Last Name # " + i.ToString(), new DateTime(1980 + i, 1, 1)));
      }
      this.RaiseListChangedEvents = true;
    }
  }
}