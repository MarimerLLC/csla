using Csla;
using SilverlightClassLibrary;
using UnitDriven;

namespace SilverlightApplication
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
      SilverlightPrincipal.LoginUsingMembershipProviderDatPortal("user", "1234", (o, e2) =>
       {
         context.Assert.IsTrue(ApplicationContext.User.IsInRole("User Role"));

         context.Assert.Success();
       });

      context.Complete();
    }

    [TestMethod]
    public void DataPortal_AuthenticatedAdminLoginBelongsToUserAndAdminRole()
    {
      var context = GetContext();

      SilverlightPrincipal.Logout();
      SilverlightPrincipal.LoginUsingMembershipProviderDatPortal("admin", "12345", (o, e2) =>
       {
         context.Assert.IsTrue(ApplicationContext.User.IsInRole("User Role"));
         context.Assert.IsTrue(ApplicationContext.User.IsInRole("Admin Role"));

         context.Assert.Success();
       });

      context.Complete();
    }

    [TestMethod]
    public void DataPortal_UnAuthenticatedUserLoginDoesNotBelongToEitherRole()
    {
      var context = GetContext();

      SilverlightPrincipal.Logout();
      SilverlightPrincipal.LoginUsingMembershipProviderDatPortal("user", "invalid_password", (o, e2) =>
       {
         context.Assert.IsFalse(ApplicationContext.User.IsInRole("User Role"));
         context.Assert.IsFalse(ApplicationContext.User.IsInRole("Admin Role"));

         context.Assert.Success();
       });

      context.Complete();
    }

    #endregion

    #region WebServer

    [TestMethod]
    public void WebServer_AuthenticatedUserLoginBelongsToUserRole()
    {
      var context = GetContext();

      SilverlightPrincipal.Logout();
      SilverlightPrincipal.LoginUsingMembershipProviderWebServer("user", "1234", (o, e2) =>
                                                                                   {
                                                                                     context.Assert.IsTrue(ApplicationContext.User.IsInRole("User Role"));

                                                                                     context.Assert.Success();
                                                                                   });

      context.Complete();
    }

    [TestMethod]
    public void WebServer_AuthenticatedAdminLoginBelongsToUserAndAdminRole()
    {
      var context = GetContext();

      SilverlightPrincipal.Logout();
      SilverlightPrincipal.LoginUsingMembershipProviderWebServer("admin", "12345", (o, e2) =>
                                                                                     {
                                                                                       context.Assert.IsTrue(ApplicationContext.User.IsInRole("User Role"));
                                                                                       context.Assert.IsTrue(ApplicationContext.User.IsInRole("Admin Role"));

                                                                                       context.Assert.Success();
                                                                                     });

      context.Complete();
    }

    [TestMethod]
    public void WebServer_UnAuthenticatedUserLoginDoesNotBelongToEitherRole()
    {
      var context = GetContext();

      SilverlightPrincipal.Logout();
      SilverlightPrincipal.LoginUsingMembershipProviderWebServer("user", "invalid_password", (o, e2) =>
                                                                                               {
                                                                                                 context.Assert.IsFalse(ApplicationContext.User.IsInRole("User Role"));
                                                                                                 context.Assert.IsFalse(ApplicationContext.User.IsInRole("Admin Role"));

                                                                                                 context.Assert.Success();
                                                                                               });

      context.Complete();
    }

    #endregion

  }
}
