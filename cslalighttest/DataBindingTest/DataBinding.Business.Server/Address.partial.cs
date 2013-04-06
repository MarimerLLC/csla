using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;

namespace DataBinding.Business
{
  public partial class Address
  {
    private Address() { }

    #region Server Factories

    public static Address Load(int id, int customerId, string street1, string street2, string city, string state, string zip)
    {
      return DataPortal.FetchChild<Address>(id, customerId, street1, street2, city, state, zip);
    }

    #endregion

    #region Data Access

    private void Child_Fetch(int id, int customerId, string street1, string street2, string city, string state, string zip)
    {
      LoadProperty<int>(IdProperty, id);
      LoadProperty<int>(CustomerIdProperty, customerId);
      LoadProperty<string>(Street1Property, street1);
      LoadProperty<string>(Street2Property, street2);
      LoadProperty<string>(CityProperty, city);
      LoadProperty<string>(StateProperty, state);
      LoadProperty<string>(ZipProperty, zip);
      ValidationRules.CheckRules();
    }

    private void Child_Update()
    {
      // simulating update...
    }

    #endregion
  }
}
