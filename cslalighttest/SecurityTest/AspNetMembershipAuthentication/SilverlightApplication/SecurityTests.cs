using Csla;
using SilverlightClassLibrary;
using UnitDriven;

namespace SilverlightApplication
{
  [TestClass]
  public class SecurityTests : TestBase
  {
    [TestSetup]
    public void Setup()
    {
      DataPortal.ProxyTypeName = "Csla.DataPortalClient.WcfProxy, Csla";
      Csla.DataPortalClient.WcfProxy.DefaultUrl = "http://localhost:3372/WcfPortal.svc";
    }

    #region Authentication

    /// <summary>
    /// Valid User id and password result in user being authenticated against the Membership API provider,
    /// its credentials (Name, IsAuthenticated set to true, adn the list of roles) being passed back to the client.
    /// To configure Membership Provider with Csla all that is required is following entry in "<appSettings>":
    /// "<add key="CslaMembershipProvider" value="[Full Namespace for Provider], [Assembly Name containing Provider ]/>"
    /// For an example please take a look at the Web.Config in this solution.
    /// </summary>
    [TestMethod]
    public void ValidMembershipIdAndPwd_ResultInSucessfullLogin()
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
    public void InvalidMembershipId_ResultsInFailedLogin()
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
    public void InvalidPassword_ResultsInFailedLogin()
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

    #region Authorization

    #endregion

  }
}
