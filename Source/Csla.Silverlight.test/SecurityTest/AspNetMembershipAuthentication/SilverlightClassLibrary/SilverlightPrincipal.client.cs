using System;
using System.Security.Principal;
using Csla;
using Csla.Security;

namespace SilverlightClassLibrary
{
  public partial class SilverlightPrincipal : Csla.Security.BusinessPrincipalBase
  {

    public static void LoginUsingMembershipProviderDatPortal(string uid, string pwd, EventHandler<DataPortalResult<SilverlightPrincipal>> completed)
    {
      MembershipIdentity.GetMembershipIdentity<MembershipIdentityStub>(
        (o, e) =>
        OnGetIdentityComplete(e, completed), uid, pwd, false);
    }

    public static void LoginUsingMembershipProviderWebServer(string uid, string pwd, EventHandler<DataPortalResult<SilverlightPrincipal>> completed)
    {
      MembershipIdentity.GetMembershipIdentity<MembershipIdentityStub>(
        (o, e) =>
        OnGetIdentityComplete(e, completed), uid, pwd, true);
    }

    private static void OnGetIdentityComplete<T>(DataPortalResult<T> e, EventHandler<DataPortalResult<SilverlightPrincipal>> completed) where T : IIdentity
    {
      if (e.Error == null)
        SetPrincipal(e.Object);
      else
        SetPrincipal(CslaIdentity.UnauthenticatedIdentity());

      if (completed != null)
        completed(Csla.ApplicationContext.User, null);

    }


  }
}