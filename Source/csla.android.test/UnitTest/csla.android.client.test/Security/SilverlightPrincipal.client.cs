//-----------------------------------------------------------------------
// <copyright file="SilverlightPrincipal.client.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
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
        OnGetIdentityComplete(e, completed), new Criteria(VALID_TEST_UID, VALID_TEST_PWD));
    }

    #endregion

    #region Login Using Membership Provider

    public static void LoginUsingMembershipProviderWebServer(EventHandler<DataPortalResult<SilverlightPrincipal>> completed)
    {
      MembershipIdentity.GetMembershipIdentity<SilverlightMembershipIdentity>(VALID_TEST_UID, VALID_TEST_PWD,
        (o, e) =>
        OnGetIdentityComplete(e, completed));
    }

    public static void LoginUsingInvalidMembershipProvider(EventHandler<DataPortalResult<SilverlightPrincipal>> completed)
    {
      MembershipIdentity.GetMembershipIdentity<SilverlightMembershipIdentity>("invalidusername", VALID_TEST_PWD,
        (o, e) =>
        OnGetIdentityComplete(e, completed));
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
        SetPrincipal(e.Object);
      else
        SetPrincipal(CslaIdentity.UnauthenticatedIdentity());

      if (completed != null)
        completed(Csla.ApplicationContext.User, null);
    }

  }
}