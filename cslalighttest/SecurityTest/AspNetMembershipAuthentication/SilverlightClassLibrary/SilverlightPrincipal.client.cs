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


    private static void OnGetIdentityComplete<T>(DataPortalResult<T> e, EventHandler<DataPortalResult<SilverlightPrincipal>> completed) where T : IIdentity
    {
      if (e.Error == null)
        OnLoggedIn(e, completed);
      else
        OnLoginFailed(completed);
    }


    private static void OnLoggedIn<T>(DataPortalResult<T> e, EventHandler<DataPortalResult<SilverlightPrincipal>> completed) where T : IIdentity
    {
      SetPrincipal(e.Object);
      if (completed != null)
        completed(Csla.ApplicationContext.User, null);
    }

    private static void OnLoginFailed(EventHandler<DataPortalResult<SilverlightPrincipal>> completed)
    {
      SetPrincipal(CslaIdentity.UnauthenticatedIdentity());
      if (completed != null)
        completed(Csla.ApplicationContext.User, null);
    }    
  }
}