using System;
using System.Security.Principal;
using Csla.Security;

namespace Csla.Testing.Business.Security
{
  public partial class SilverlightPrincipal
  {
    #region Login Using CSLA

    public static void LoginUsingCSLA(EventHandler<DataPortalResult<SilverlightPrincipal>> completed)
    {
      CslaIdentity.GetCslaIdentity<SilverlightIdentity>(
        (o, e) => 
        OnGetIdentityComplete(e, completed), new Criteria(TEST_UID, TEST_PWD));
    }

    #endregion

    #region Login Using Membership Provider

    public static void LoginUsingMembershipProviderWebServer(EventHandler<DataPortalResult<SilverlightPrincipal>> completed)
    {
      MembershipIdentity.GetMembershipIdentity<SilverlightMembershipIdentity>(
        (o, e) => 
        OnGetIdentityComplete(e, completed), TEST_UID, TEST_PWD, true);
    }

    public static void LoginUsingMembershipProviderDatPortal(EventHandler<DataPortalResult<SilverlightPrincipal>> completed)
    {
      MembershipIdentity.GetMembershipIdentity<SilverlightMembershipIdentity>(
        (o, e) => 
        OnGetIdentityComplete(e, completed), TEST_UID, TEST_PWD, false);
    }

    public static void LoginUsingInvalidMembershipProvider(EventHandler<DataPortalResult<SilverlightPrincipal>> completed)
    {
      MembershipIdentity.GetMembershipIdentity<SilverlightMembershipIdentity>(
        (o, e) => 
        OnGetIdentityComplete(e, completed), "invalidusername", TEST_PWD, true);
    }

    #endregion

    #region Login Using Windwos

    public static void LoginUsingWindows(EventHandler<DataPortalResult<SilverlightPrincipal>> completed)
    {
      SilverlightWindowsIdentity.GetSilverlightWindowsIdentity((o, e) => 
                                                               OnGetIdentityComplete(e, completed));
    }

    #endregion

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