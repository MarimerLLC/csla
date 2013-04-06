using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;

namespace DataBinding.Business
{
  public partial class AddressList
  {    
    private AddressList() { }

    private void DataPortal_Fetch(SingleCriteria<AddressList, int> criteria)
    {
      Fetch(criteria.Value);
    }

    private void Fetch(int customerId)
    {
      RaiseListChangedEvents = false;

      Address a1 = Address.Load(1, 1, "123 Elm st.", null, "Springfield", "IL", "55123");
      Address a2 = Address.Load(2, 1, "1 Summer Cottage Ln.", null, "Nowhere", "FL", "12345");
      Address a3 = Address.Load(3, 2, "456 Main St.", "apt 1", "Gotham", "DC", "90832");

      Address[] addresses = new Address[] { a1, a2, a3 };

      var found = from a in addresses
                  where a.CustomerId == customerId
                  select a;

      AddRange(found.ToArray());

      RaiseListChangedEvents = true;
    }
  }
}
