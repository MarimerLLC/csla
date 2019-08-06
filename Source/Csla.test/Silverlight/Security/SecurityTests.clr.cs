//-----------------------------------------------------------------------
// <copyright file="SecurityTests.clr.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
#if NUNIT
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
using TestSetup = NUnit.Framework.SetUpAttribute;
#elif MSTEST
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

using ApplicationContext = Csla.ApplicationContext;

namespace Csla.Test.Silverlight.Security
{
  [TestClass]
  public partial class SecurityTests
  {
    [TestMethod]
    public void SetMembershipPrincipalWebServer()
    {
      Csla.Testing.Business.Security.SilverlightPrincipal.LoginUsingMembershipProviderWebServer("sergeyb", "pwd");
      Assert.IsNotNull(Csla.ApplicationContext.User);
      Assert.AreEqual(true, Csla.ApplicationContext.User.Identity.IsAuthenticated);
      Assert.AreEqual("sergeyb", Csla.ApplicationContext.User.Identity.Name);
      Assert.AreEqual(true, Csla.ApplicationContext.User.IsInRole("Admin"));

    }

    [TestMethod]
    public void SetMembershipPrincipalDataPortal()
    {
      Csla.Testing.Business.Security.SilverlightPrincipal.LoginUsingMembershipProviderDataPortal("sergeyb", "pwd");
      Assert.IsNotNull(Csla.ApplicationContext.User);
      Assert.AreEqual(true, Csla.ApplicationContext.User.Identity.IsAuthenticated);
      Assert.AreEqual("sergeyb", Csla.ApplicationContext.User.Identity.Name);
      Assert.AreEqual(true, Csla.ApplicationContext.User.IsInRole("Admin"));

    }

    [TestMethod]
    public void SetInvalidMembershipPrincipal()
    {

      Csla.Testing.Business.Security.SilverlightPrincipal.LoginUsingMembershipProviderDataPortal("invalidusername", "pwd");
      Assert.IsNotNull(Csla.ApplicationContext.User);
      Assert.AreEqual(false, Csla.ApplicationContext.User.Identity.IsAuthenticated);
      Assert.AreEqual("", Csla.ApplicationContext.User.Identity.Name);
      Assert.AreEqual(false, Csla.ApplicationContext.User.IsInRole("Admin"));
    }

  }
}