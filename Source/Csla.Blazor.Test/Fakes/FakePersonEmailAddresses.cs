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
      var emailAddress = DataPortal.CreateChild<FakePersonEmailAddress>();
      this.Add(emailAddress);
      return emailAddress;
    }

  }
}
