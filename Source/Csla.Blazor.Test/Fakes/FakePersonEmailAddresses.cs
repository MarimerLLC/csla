using System;
using System.Collections.Generic;
using System.Text;
using Csla.Server;

namespace Csla.Blazor.Test.Fakes
{
  public class FakePersonEmailAddresses : BusinessListBase<FakePersonEmailAddresses, FakePersonEmailAddress>
  {

    public FakePersonEmailAddress NewEmailAddress()
    {
      DataPortal<FakePersonEmailAddress> dataPortal;

      // Create an empty list for holding email addresses
      dataPortal = new DataPortal<FakePersonEmailAddress>(ApplicationContext);

      var emailAddress = dataPortal.CreateChild();
      this.Add(emailAddress);
      return emailAddress;
    }

  }
}
