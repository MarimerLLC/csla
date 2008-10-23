using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Serialization;
using System.Runtime.Serialization;
using Csla.DataPortalClient;
using Csla.Core;

namespace Csla.Testing.Business.Security
{
  [Serializable()]
  public class SilverlightMembershipIdentity : Csla.Security.MembershipIdentity
  {

    public SilverlightMembershipIdentity() { }

    protected override void LoadCustomData()
    {
      //This now comes from IdentityFactory - built in
      //Roles = new MobileList<string>(new string[2] { "Admin", "User" });
      //AuthenticationType = "SL";
    }
  }
}
