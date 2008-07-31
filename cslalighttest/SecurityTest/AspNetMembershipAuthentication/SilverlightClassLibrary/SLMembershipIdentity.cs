using System;
using System.Net;
using Csla.Security;
using Csla.Serialization;
using Csla.Core;

namespace SilverlightClassLibrary
{
  [Serializable()]
  public class SLMembershipIdentity:MembershipIdentity
  {
    public void SetRoles(string roles)
    {
      if (IsAuthenticated)
      {
        base.Roles = new MobileList<string>(roles.Split(';'));
      }
    }
  }
}
