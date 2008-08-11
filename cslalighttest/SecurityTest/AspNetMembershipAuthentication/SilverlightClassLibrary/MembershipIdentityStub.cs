using System;
//using System.Web.Security;
using Csla.Security;
using Csla.Core;

using Csla.Serialization;

namespace SilverlightClassLibrary
{
  [Serializable]
  public class MembershipIdentityStub : MembershipIdentity
  {
    public MembershipIdentityStub(){}

    #if !SILVERLIGHT

    protected override void LoadCustomData()
    {
      base.LoadCustomData();

      Roles = new MobileList<string>(
          System.Web.Security.Roles.GetRolesForUser(Name));
    }
    #endif
  }
}
