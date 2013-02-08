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
  /// All of the Authentication Tests utilize Membership API to retreive memberhip/roles 
  /// information from WebServer/DataPortal Server.
  /// We have setup Mock Role and Membership Providers that are providing csla
  /// server components with pre-set responses (user and admin Ids, with "User Role"
  /// and "Admin Role" roles - for details inspect MockRolerProvider, and MockMembershipProvider).
  /// This in turn allows us to test that csla is passing correct information from server to the client.
  /// </summary>

  [TestClass]
  public class AuthenticationTests : MembershipTestBase
  {
    #region DataPortal

    /// <summary>
    /// Valid User id and password result in user being authenticated against the Membership API provider,
    /// its credentials (Name, IsAuthenticated set to true, adn the list of roles) being passed back to the client.
    /// </summary>
    [Test]
    public void DataPortal_ValidMembershipIdAndPwd_ResultInSucessfullLogin()
    {
      var context = GetContext();

      SilverlightPrincipal.Logout();
      SilverlightPrincipal.LoginUsingMembershipProviderDataPortal("user", "1234");
      
      var identity = ApplicationContext.User.Identity;

      context.Assert.IsNotNull(identity);
      context.Assert.IsTrue(identity.Name == "user");
      context.Assert.IsTrue(identity.IsAuthenticated);
      context.Assert.IsTrue(ApplicationContext.User.IsInRole("User Role"));

      context.Assert.Success();

      context.Complete();
    }

    [Test]
    public void DataPortal_InvalidMembershipId_ResultsInFailedLogin()
    {
      var context = GetContext();

      SilverlightPrincipal.Logout();
      SilverlightPrincipal.LoginUsingMembershipProviderDataPortal("invalid", "1234");
      var identity = ApplicationContext.User.Identity;

      context.Assert.IsNotNull(identity);
      context.Assert.IsTrue(identity.Name == "");
      context.Assert.IsFalse(identity.IsAuthenticated);
      context.Assert.IsFalse(ApplicationContext.User.IsInRole("User Role"));

      context.Assert.Success();

      context.Complete();
    }

    [Test]
    public void DataPortal_InvalidPassword_ResultsInFailedLogin()
    {
      var context = GetContext();

      SilverlightPrincipal.Logout();
      SilverlightPrincipal.LoginUsingMembershipProviderDataPortal("user", "invalid");
      
      var identity = ApplicationContext.User.Identity;

      context.Assert.IsNotNull(identity);
      context.Assert.IsTrue(identity.Name == "");
      context.Assert.IsFalse(identity.IsAuthenticated);
      context.Assert.IsFalse(ApplicationContext.User.IsInRole("User Role"));

      context.Assert.Success();

      context.Complete();
    }

    #endregion

    #region WebServer

    [Test]
    public void WebServer_ValidMembershipIdAndPwd_ResultInSucessfullLogin()
    {
      var context = GetContext();

      SilverlightPrincipal.Logout();
      SilverlightPrincipal.LoginUsingMembershipProviderWebServer("user", "1234");

      var identity = ApplicationContext.User.Identity;

      context.Assert.IsNotNull(identity);
      context.Assert.IsTrue(identity.Name == "user");
      context.Assert.IsTrue(identity.IsAuthenticated);
      context.Assert.IsTrue(ApplicationContext.User.IsInRole("User Role"));

      context.Assert.Success();

      context.Complete();
    }

    [Test]
    public void WebServer_InvalidMembershipId_ResultsInFailedLogin()
    {
      var context = GetContext();

      SilverlightPrincipal.Logout();
      SilverlightPrincipal.LoginUsingMembershipProviderWebServer("invalid", "1234");
      var identity = ApplicationContext.User.Identity;

      context.Assert.IsNotNull(identity);
      context.Assert.IsTrue(identity.Name == "");
      context.Assert.IsFalse(identity.IsAuthenticated);
      context.Assert.IsFalse(ApplicationContext.User.IsInRole("User Role"));

      context.Assert.Success();

      context.Complete();
    }

    [Test]
    public void WebServer_InvalidPassword_ResultsInFailedLogin()
    {
      var context = GetContext();

      SilverlightPrincipal.Logout();
      SilverlightPrincipal.LoginUsingMembershipProviderWebServer("user", "invalid");

      var identity = ApplicationContext.User.Identity;

      context.Assert.IsNotNull(identity);
      context.Assert.IsTrue(identity.Name == "");
      context.Assert.IsFalse(identity.IsAuthenticated);
      context.Assert.IsFalse(ApplicationContext.User.IsInRole("User Role"));

      context.Assert.Success();

      context.Complete();
    }

    #endregion
    
  }
}
