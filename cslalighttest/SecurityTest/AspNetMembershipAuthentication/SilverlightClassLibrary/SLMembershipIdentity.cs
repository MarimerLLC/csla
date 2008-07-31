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
    protected override void LoadCustomData()
    {
      base.LoadCustomData();
    }

    public void SetRoles(string roles)
    {
      if (IsAuthenticated)
      {
        base.Roles = new MobileList<string>(roles.Split(';'));
      }
    }
  }
}
