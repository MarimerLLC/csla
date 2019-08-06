//-----------------------------------------------------------------------
// <copyright file="SecurityTests.client.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using Csla.DataPortalClient;
using Csla.Testing.Business.Security;
using cslalighttest.Properties;
using UnitDriven;

namespace Csla.Test.Silverlight.Security
{
  //[TestClass]
  public partial class SecurityTests
  {

    [TestMethod]
    public void SetCSLAPrincipalLocal()
    {
      var context = GetContext();
      Csla.DataPortal.ProxyTypeName = "Local";

      SilverlightPrincipal.LoginUsingCSLA(
        (o, e) =>
        {
          context.Assert.IsNotNull(Csla.ApplicationContext.User);
          context.Assert.AreEqual(true, Csla.ApplicationContext.User.Identity.IsAuthenticated);
          context.Assert.AreEqual("SilverlightIdentity", Csla.ApplicationContext.User.Identity.Name);
          context.Assert.AreEqual(true, Csla.ApplicationContext.User.IsInRole(AdminRoleName));
          context.Assert.Success();
        });
      context.Complete();
    }

    [TestMethod]
    public void SetCSLAPrincipalRemote()
    {
      var context = GetContext();
      Csla.DataPortal.ProxyTypeName = WcfProxyTypeName;
      WcfProxy.DefaultUrl = Resources.RemotePortalUrl;

      SilverlightPrincipal.LoginUsingCSLA(
        (o, e) =>
        {
          context.Assert.IsNotNull(Csla.ApplicationContext.User);
          context.Assert.AreEqual(true, Csla.ApplicationContext.User.Identity.IsAuthenticated);
          context.Assert.AreEqual("SilverlightIdentity", Csla.ApplicationContext.User.Identity.Name);
          context.Assert.AreEqual(true, Csla.ApplicationContext.User.IsInRole(AdminRoleName));
          context.Assert.Success();
        });
      context.Complete();
    }

    [TestMethod]
    public void SetMembershipPrincipalWebServer()
    {

      var context = GetContext();
      Csla.DataPortal.ProxyTypeName = WcfProxyTypeName;
      WcfProxy.DefaultUrl = Resources.RemotePortalUrl;

      SilverlightPrincipal.LoginUsingMembershipProviderWebServer(
        (o, e) =>
        {
          context.Assert.IsNotNull(Csla.ApplicationContext.User);
          context.Assert.IsTrue(Csla.ApplicationContext.User.Identity.IsAuthenticated);
          context.Assert.AreEqual(SilverlightPrincipal.VALID_TEST_UID, Csla.ApplicationContext.User.Identity.Name);
          context.Assert.IsTrue(Csla.ApplicationContext.User.IsInRole("User Role"));
          context.Assert.Success();
        });
      context.Complete();
    }

    //[TestMethod]
    //public void SetMembershipPrincipalDataPortal()
    //{

    //  var context = GetContext();
    //  Csla.DataPortal.ProxyTypeName = WcfProxyTypeName;
    //  WcfProxy.DefaultUrl = Resources.RemotePortalUrl;

    //  SilverlightPrincipal.LoginUsingMembershipProviderDatPortal(
    //    (o, e) =>
    //    {
    //      context.Assert.IsNotNull(Csla.ApplicationContext.User);
    //      context.Assert.IsTrue(Csla.ApplicationContext.User.Identity.IsAuthenticated);
    //      context.Assert.AreEqual(SilverlightPrincipal.VALID_TEST_UID, Csla.ApplicationContext.User.Identity.Name);
    //      context.Assert.IsTrue(Csla.ApplicationContext.User.IsInRole("User Role"));
    //      context.Assert.Success();
    //    });
    //  context.Complete();
    //}

    [TestMethod]
    public void SetInvalidMembershipPrincipal()
    {

      var context = GetContext();
      Csla.DataPortal.ProxyTypeName = WcfProxyTypeName;
      WcfProxy.DefaultUrl = Resources.RemotePortalUrl;

      SilverlightPrincipal.LoginUsingInvalidMembershipProvider(
        (o, e) =>
        {
          context.Assert.IsNotNull(Csla.ApplicationContext.User);
          context.Assert.AreEqual(false, Csla.ApplicationContext.User.Identity.IsAuthenticated);
          context.Assert.AreEqual("", Csla.ApplicationContext.User.Identity.Name);
          context.Assert.AreEqual(false, Csla.ApplicationContext.User.IsInRole(AdminRoleName));
          context.Assert.Success();
        });
      context.Complete();
    }
  }
}