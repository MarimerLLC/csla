using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Principal;
using Csla.Security;

namespace MembershipTest
{
  [Serializable]
  public class MyPrincipal : BusinessPrincipalBase
  {
    protected MyPrincipal(IIdentity identity)
      : base(identity)
    { }

    public static MyPrincipal Login(
      string username, string password)
    {
      var identity =
        MembershipIdentity.GetMembershipIdentity
        <MembershipIdentity>(username, password, true);
      return new MyPrincipal(identity);
    }
  }
}
