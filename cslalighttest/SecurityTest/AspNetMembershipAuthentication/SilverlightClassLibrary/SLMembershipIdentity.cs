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
      if (IsAuthenticated)
      {
        AddRoles(new MobileList<string>(new string[2] { "Admin", "User" }));
      }
    }

    internal bool InRole(string role)
    {
      return base.IsInRole(role);
    }
  }
}
