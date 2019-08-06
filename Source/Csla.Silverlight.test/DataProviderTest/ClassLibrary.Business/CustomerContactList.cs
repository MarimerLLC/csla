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
  public class CustomerContactList : BusinessListBase<CustomerContactList,CustomerContact>
  {

#if SILVERLIGHT
    public CustomerContactList() { }
#else
    private CustomerContactList() { }
#endif

#if !SILVERLIGHT
    internal static CustomerContactList GetCustomerContactList(int customerID)
    {
      return DataPortal.FetchChild<CustomerContactList>(customerID);
    }

    private void Child_Fetch(int customerID)
    {
      this.RaiseListChangedEvents = false;
      for (int i = 1; i <= customerID; i++)
      {
        Add(CustomerContact.GetCustomerContact(customerID,i,"First Name # " + i.ToString(),"Last Name # " + i.ToString(), new DateTime(1980 + i,1,1)));
      }
      this.RaiseListChangedEvents = true;
    }

#endif

  }
}
