using Csla;
using SilverlightClassLibrary;
using UnitDriven;

namespace SilverlightApplication
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
    /// To configure Membership Provider with Csla all that is required is following entry in "<appSettings>":
    /// "<add key="CslaMembershipProvider" value="[Full Namespace for Provider], [Assembly Name containing Provider ]/>"
    /// For an example please take a look at the Web.Config in this solution.
    /// </summary>
    [TestMethod]
    public void DataPortal_ValidMembershipIdAndPwd_ResultInSucessfullLogin()
    {
      var context = GetContext();
      
      SilverlightPrincipal.Logout();
      SilverlightPrincipal.LoginUsingMembershipProviderDatPortal("user", "1234", (o, e2) =>
      {
       var identity = ApplicationContext.User.Identity;

       context.Assert.IsNotNull(identity);
       context.Assert.IsTrue(identity.Name == "user");
       context.Assert.IsTrue(identity.IsAuthenticated);
       context.Assert.IsTrue(ApplicationContext.User.IsInRole("User Role"));

       context.Assert.Success();
      });

      context.Complete();
    }

    [TestMethod]
    public void DataPortal_InvalidMembershipId_ResultsInFailedLogin()
    {
      var context = GetContext();

      SilverlightPrincipal.Logout();
      SilverlightPrincipal.LoginUsingMembershipProviderDatPortal("invalid", "1234", (o, e2) =>
      {
        var identity = ApplicationContext.User.Identity;

        context.Assert.IsNotNull(identity);
        context.Assert.IsTrue(identity.Name == "");
        context.Assert.IsFalse(identity.IsAuthenticated);
        context.Assert.IsFalse(ApplicationContext.User.IsInRole("User Role"));

        context.Assert.Success();
      });

      context.Complete();
    }

    [TestMethod]
    public void DataPortal_InvalidPassword_ResultsInFailedLogin()
    {
      var context = GetContext();

      SilverlightPrincipal.Logout();
      SilverlightPrincipal.LoginUsingMembershipProviderDatPortal("user", "invalid", (o, e2) =>
      {
        var identity = ApplicationContext.User.Identity;

        context.Assert.IsNotNull(identity);
        context.Assert.IsTrue(identity.Name == "");
        context.Assert.IsFalse(identity.IsAuthenticated);
        context.Assert.IsFalse(ApplicationContext.User.IsInRole("User Role"));

        context.Assert.Success();
      });

      context.Complete();
    }

    #endregion

    #region WebServer

    [TestMethod]
    public void WebServer_ValidMembershipIdAndPwd_ResultInSucessfullLogin()
    {
      var context = GetContext();

      SilverlightPrincipal.Logout();
      SilverlightPrincipal.LoginUsingMembershipProviderWebServer("user", "1234", (o, e2) =>
       {
         var identity = ApplicationContext.User.Identity;

         context.Assert.IsNotNull(identity);
         context.Assert.IsTrue(identity.Name == "user");
         context.Assert.IsTrue(identity.IsAuthenticated);
         context.Assert.IsTrue(ApplicationContext.User.IsInRole("User Role"));

         context.Assert.Success();
       });

      context.Complete();
    }

    [TestMethod]
    public void WebServer_InvalidMembershipId_ResultsInFailedLogin()
    {
      var context = GetContext();

      SilverlightPrincipal.Logout();
      SilverlightPrincipal.LoginUsingMembershipProviderWebServer("invalid", "1234", (o, e2) =>
      {
        var identity = ApplicationContext.User.Identity;

        context.Assert.IsNotNull(identity);
        context.Assert.IsTrue(identity.Name == "");
        context.Assert.IsFalse(identity.IsAuthenticated);
        context.Assert.IsFalse(ApplicationContext.User.IsInRole("User Role"));

        context.Assert.Success();
      });

      context.Complete();
    }

    [TestMethod]
    public void WebServer_InvalidPassword_ResultsInFailedLogin()
    {
      var context = GetContext();

      SilverlightPrincipal.Logout();
      SilverlightPrincipal.LoginUsingMembershipProviderWebServer("user", "invalid", (o, e2) =>
      {
        var identity = ApplicationContext.User.Identity;

        context.Assert.IsNotNull(identity);
        context.Assert.IsTrue(identity.Name == "");
        context.Assert.IsFalse(identity.IsAuthenticated);
        context.Assert.IsFalse(ApplicationContext.User.IsInRole("User Role"));

        context.Assert.Success();
      });

      context.Complete();
    }

    #endregion

  }
}
