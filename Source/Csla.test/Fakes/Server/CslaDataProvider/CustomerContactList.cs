//-----------------------------------------------------------------------
// <copyright file="CustomerContactList.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

using Csla;

namespace cslalighttest.CslaDataProvider
{
  [Serializable]
  public class CustomerContactList : BusinessListBase<CustomerContactList, CustomerContact>
  {

    public Customer MyParent => (Customer)this.Parent;

    private CustomerContactList() { }

    private void Child_Fetch(int customerID, [Inject] IChildDataPortal<CustomerContact> childDataPortal)
    {
      this.RaiseListChangedEvents = false;
      for (int i = 1; i <= customerID; i++)
      {
        Add(childDataPortal.FetchChild(customerID, i, $"First Name # {i}", $"Last Name # {i}", new DateTime(1980 + i, 1, 1)));
      }
      this.RaiseListChangedEvents = true;
    }
  }
}