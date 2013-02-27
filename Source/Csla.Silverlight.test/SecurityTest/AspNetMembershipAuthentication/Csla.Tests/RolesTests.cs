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

using System.Configuration.Provider;
using System.Reflection;
using System.Web.Security;
using SilverlightClassLibrary;

using UnitDriven;

namespace Csla.Tests
{
  /// <summary>
  /// All of the Roles Tests utilize Membership API to retreive memberhip/roles 
  /// information from WebServer/DataPortal Server.
  /// We have setup Mock Role and Membership Providers that are providing csla
  /// server components with pre-set responses (user and admin Ids, with "User Role"
  /// and "Admin Role" roles - for details inspect MockRolerProvider, and MockMembershipProvider).
  /// This in turn allows us to test that csla is passing correct information from server to the client.
  /// </summary>

  [TestClass]
  public class RolesTests : MembershipTestBase
  {
    #region DataPortal

    [TestMethod]
    public void DataPortal_AuthenticatedUserLoginBelongsToUserRole()
    {
      var context = GetContext();

      SilverlightPrincipal.Logout();
      SilverlightPrincipal.LoginUsingMembershipProviderDataPortal("user", "1234");

      context.Assert.IsTrue(ApplicationContext.User.IsInRole("User Role"));

      context.Assert.Success();

      context.Complete();
    }

    [TestMethod]
    public void DataPortal_AuthenticatedAdminLoginBelongsToUserAndAdminRole()
    {
      var context = GetContext();

      SilverlightPrincipal.Logout();
      SilverlightPrincipal.LoginUsingMembershipProviderDataPortal("admin", "12345");

      context.Assert.IsTrue(ApplicationContext.User.IsInRole("User Role"));
      context.Assert.IsTrue(ApplicationContext.User.IsInRole("Admin Role"));

      context.Assert.Success();

      context.Complete();
    }

    [TestMethod]
    public void DataPortal_UnAuthenticatedUserLoginDoesNotBelongToEitherRole()
    {
      var context = GetContext();

      SilverlightPrincipal.Logout();
      SilverlightPrincipal.LoginUsingMembershipProviderDataPortal("user", "invalid_password");

      context.Assert.IsFalse(ApplicationContext.User.IsInRole("User Role"));
      context.Assert.IsFalse(ApplicationContext.User.IsInRole("Admin Role"));

      context.Assert.Success();

      context.Complete();
      
    }

    #endregion

    #region WebServer
    [TestMethod]
    public void WebServer_AuthenticatedUserLoginBelongsToUserRole()
    {
      var context = GetContext();

      SilverlightPrincipal.Logout();
      SilverlightPrincipal.LoginUsingMembershipProviderWebServer("user", "1234");

      context.Assert.IsTrue(ApplicationContext.User.IsInRole("User Role"));

      context.Assert.Success();

      context.Complete();
    }

    [TestMethod]
    public void WebServer_AuthenticatedAdminLoginBelongsToUserAndAdminRole()
    {
      var context = GetContext();

      SilverlightPrincipal.Logout();
      SilverlightPrincipal.LoginUsingMembershipProviderWebServer("admin", "12345");

      context.Assert.IsTrue(ApplicationContext.User.IsInRole("User Role"));
      context.Assert.IsTrue(ApplicationContext.User.IsInRole("Admin Role"));

      context.Assert.Success();

      context.Complete();
    }

    [TestMethod]
    public void WebServer_UnAuthenticatedUserLoginDoesNotBelongToEitherRole()
    {
      var context = GetContext();

      SilverlightPrincipal.Logout();
      SilverlightPrincipal.LoginUsingMembershipProviderWebServer("user", "invalid_password");

      context.Assert.IsFalse(ApplicationContext.User.IsInRole("User Role"));
      context.Assert.IsFalse(ApplicationContext.User.IsInRole("Admin Role"));

      context.Assert.Success();

      context.Complete();

    }


    #endregion
  }
}
