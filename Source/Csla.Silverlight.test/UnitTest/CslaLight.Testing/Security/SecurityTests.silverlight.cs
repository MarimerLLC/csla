//-----------------------------------------------------------------------
// <copyright file="SecurityTests.silverlight.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using Csla;
using Csla.DataPortalClient;
using Csla.Testing.Business.Security;
using cslalighttest.Properties;
using UnitDriven;

namespace cslalighttest.Security
{
  //[TestClass]
  public partial class SecurityTests 
  {
    [TestMethod]
    public void SetMembershipPrincipalWebServer()
    {

      UnitTestContext context = GetContext();
      DataPortal.ProxyTypeName = "Csla.DataPortalClient.WcfProxy, Csla";
      WcfProxy.DefaultUrl = Resources.RemotePortalUrl;

      SilverlightPrincipal.LoginUsingMembershipProviderWebServer(
        (o, e) =>
        {
          context.Assert.IsNotNull(ApplicationContext.User);
          context.Assert.IsTrue(ApplicationContext.User.Identity.IsAuthenticated);
          context.Assert.AreEqual("sergeyb", ApplicationContext.User.Identity.Name);
          context.Assert.IsTrue(ApplicationContext.User.IsInRole("Admin"));
          context.Assert.Success();
        });
      context.Complete();
    }

    [TestMethod]
    public void SetMembershipPrincipalDataPortal()
    {

      UnitTestContext context = GetContext();
      DataPortal.ProxyTypeName = "Csla.DataPortalClient.WcfProxy, Csla";
      WcfProxy.DefaultUrl = Resources.RemotePortalUrl;

      SilverlightPrincipal.LoginUsingMembershipProviderDatPortal(
        (o, e) =>
        {
          context.Assert.IsNotNull(ApplicationContext.User);
          context.Assert.IsTrue(ApplicationContext.User.Identity.IsAuthenticated);
          context.Assert.AreEqual("sergeyb", ApplicationContext.User.Identity.Name);
          context.Assert.IsTrue(ApplicationContext.User.IsInRole("Admin"));
          context.Assert.Success();
        });
      context.Complete();
    }

    [TestMethod]
    public void SetInvalidMembershipPrincipal()
    {

      UnitTestContext context = GetContext();
      DataPortal.ProxyTypeName = "Csla.DataPortalClient.WcfProxy, Csla";
      WcfProxy.DefaultUrl = Resources.RemotePortalUrl;

      SilverlightPrincipal.LoginUsingInvalidMembershipProvider(
        (o, e) =>
        {
          context.Assert.IsNotNull(ApplicationContext.User);
          context.Assert.AreEqual(false, ApplicationContext.User.Identity.IsAuthenticated);
          context.Assert.AreEqual("", ApplicationContext.User.Identity.Name);
          context.Assert.AreEqual(false, ApplicationContext.User.IsInRole("Admin"));
          context.Assert.Success();
        });
      context.Complete();
    }


    //[TestMethod]
    //public void SetWindowsPrincipal()
    //{
    //  UnitTestContext context = GetContext();


    //  Csla.DataPortal.ProxyTypeName = "Csla.DataPortalClient.WcfProxy, Csla";
    //  Csla.DataPortalClient.WcfProxy.DefaultUrl = "http://localhost//SLTestWeb//WcfPortal.svc";
    //    //cslalighttest.Properties.Resources.RemotePortalUrl;

    //  Csla.Testing.Business.Security.SilverlightPrincipal.LoginUsingWindows(
    //     (o, e) =>
    //     {
    //       context.Assert.IsNotNull(ApplicationContext.User);
    //       context.Assert.AreEqual(true, ApplicationContext.User.Identity.IsAuthenticated);
    //       context.Assert.AreEqual(true, ApplicationContext.User.Identity.Name.Length>0);
    //       context.Assert.AreEqual(true, ApplicationContext.User.IsInRole("Everyone"));
    //       context.Assert.Success();
    //     });
    //  context.Complete();
    //}

  }
}