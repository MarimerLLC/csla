#if NUNIT
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#elif MSTEST
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

using ClassLibrary;
using SilverlightClassLibrary;

namespace Csla.Tests
{
  [TestClass]
  public class AuthorizationTests : MembershipTestBase
  {
    [TestMethod]
    [ExpectedException(typeof(System.Security.SecurityException))]
    public void UnauthorizedUser_CanNotInstaniateObjectWithCreationRules()
    {
      var context = GetContext();

      SilverlightPrincipal.Logout();
      SilverlightPrincipal.LoginUsingMembershipProviderDataPortal("user", "invalid_password");

      context.Assert.Try(() =>
      {
        var item = DataPortal.Create<UserAndAdminCanCreateAndWrite>(); 
      });
      context.Assert.Fail();
      context.Assert.Success();

      context.Complete();
    }

    [TestMethod]
    public void UnauthorizedUser_CanInstaniateObjectWithoutCreationRules()
    {
      var context = GetContext();

      SilverlightPrincipal.Logout();
      SilverlightPrincipal.LoginUsingMembershipProviderDataPortal("user", "invalid_password");

      //OnlyAdminCanWrite does not have Creation Rules set for it - therefore unauthorized user should be able to create it
      var item = DataPortal.Create<OnlyAdminCanWrite>();

      context.Assert.IsNotNull(item);
      context.Assert.Success();

      context.Complete();
    }

    [TestMethod]
    public void AuthorizedUser_CanInstantiateObjectWithCreationRule()
    {
      var context = GetContext();

      SilverlightPrincipal.Logout();
      SilverlightPrincipal.LoginUsingMembershipProviderDataPortal("user", "1234");

      var item = DataPortal.Create<UserAndAdminCanCreateAndWrite>();

      context.Assert.IsNotNull(item);
      context.Assert.Success();

      context.Complete();      
    }

    [TestMethod]
    public void AuthorizedAdmin_CanInstantiateObjectWithCreationRule()
    {
      var context = GetContext();

      SilverlightPrincipal.Logout();
      SilverlightPrincipal.LoginUsingMembershipProviderDataPortal("admin", "12345");

      var item = DataPortal.Create<UserAndAdminCanCreateAndWrite>();

      context.Assert.IsNotNull(item);
      context.Assert.Success();

      context.Complete();
    }
    [TestMethod]
    public void AuthorizedUser_CanWriteToObject_AuthorizedForUserWrite()
    {
      var context = GetContext();

      SilverlightPrincipal.Logout();
      SilverlightPrincipal.LoginUsingMembershipProviderDataPortal("user", "1234");

      var item = DataPortal.Create<UserAndAdminCanCreateAndWrite>();

      item.A = "test";

      context.Assert.Success();

      context.Complete(); 
    }

    [TestMethod]
    public void AuthorizedAdmin_CanWriteToObject_AuthorizedForUserWrite()
    {
      var context = GetContext();

      SilverlightPrincipal.Logout();
      SilverlightPrincipal.LoginUsingMembershipProviderDataPortal("admin", "12345");

      var item = DataPortal.Create<UserAndAdminCanCreateAndWrite>();

      item.A = "test";

      context.Assert.Success();

      context.Complete();
    }

    [TestMethod]
    [ExpectedException(typeof(System.Security.SecurityException))]
    public void AuthorizedUser_CanNotWriteToObject_AuthorizedForAdminOnlyWrite()
    {
      var context = GetContext();

      SilverlightPrincipal.Logout();
      SilverlightPrincipal.LoginUsingMembershipProviderDataPortal("user", "1234");

      var item = DataPortal.Create<OnlyAdminCanWrite>();

      context.Assert.Try(() =>
      {
        item.A = "test";
      });

      context.Assert.Fail();
      context.Assert.Success();

      context.Complete();

    }

    [TestMethod]
    public void AuthorizedAdmin_CanWriteToObject_AuthorizedForAdminOnlyWrite()
    {
      var context = GetContext();

      SilverlightPrincipal.Logout();
      SilverlightPrincipal.LoginUsingMembershipProviderDataPortal("admin", "12345");

      var item = DataPortal.Create<OnlyAdminCanWrite>();

      item.A = "test";

      context.Assert.Success();

      context.Complete();
    }

  }
}
