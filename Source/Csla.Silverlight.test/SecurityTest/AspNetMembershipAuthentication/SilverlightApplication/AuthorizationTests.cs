using ClassLibrary;
using Csla;
using SilverlightClassLibrary;
using UnitDriven;

namespace SilverlightApplication
{
  [TestClass]
  public class AuthorizationTests : MembershipTestBase
  {
    [TestMethod]
    public void UnauthorizedUser_CanNotInstaniateObjectWithCreationRules()
    {
      var context = GetContext();

      SilverlightPrincipal.Logout();
      SilverlightPrincipal.LoginUsingMembershipProviderDatPortal("user", "invalid_password", (o, e) => 
        DataPortal.BeginCreate<UserAndAdminCanCreateAndWrite>((o2, e2) =>
        {
          context.Assert.IsTrue(((DataPortalException)e2.Error).ErrorInfo.ExceptionTypeName =="System.Security.SecurityException") ;
          context.Assert.Success();
        }));


      context.Complete();
    }


    [TestMethod]
    public void UnauthorizedUser_CanInstaniateObjectWithoutCreationRules()
    {
      var context = GetContext();

      SilverlightPrincipal.Logout();
      SilverlightPrincipal.LoginUsingMembershipProviderDatPortal("user", "invalid_password", (o, e) => 
        DataPortal.BeginCreate<OnlyAdminCanWrite>((o2,e2)=>
        {
          //OnlyAdminCanWrite does not have Creation Rules set for it - therefore unauthorized user should be able to create it
          context.Assert.IsNotNull(e2.Object);
          context.Assert.Success();          
        }));
      
      context.Complete();
    }

    [TestMethod]
    public void AuthorizedUser_CanInstantiateObjectWithCreationRule()
    {
      var context = GetContext();

      SilverlightPrincipal.Logout();
      SilverlightPrincipal.LoginUsingMembershipProviderDatPortal("user", "1234", (o, e) =>
        DataPortal.BeginCreate<UserAndAdminCanCreateAndWrite>((o2, e2) =>
        {
          context.Assert.IsNotNull(e2.Object);
          context.Assert.Success();
        }));

      context.Complete();
    }

    [TestMethod]
    public void AuthorizedAdmin_CanInstantiateObjectWithCreationRule()
    {
      var context = GetContext();

      SilverlightPrincipal.Logout();
      SilverlightPrincipal.LoginUsingMembershipProviderDatPortal("admin", "12345", (o, e) => 
        DataPortal.BeginCreate<UserAndAdminCanCreateAndWrite>((o2,e2)=>
        {
          context.Assert.IsNotNull(e2.Object);
          context.Assert.Success();         
        }));

      context.Complete();
    }


    [TestMethod]
    public void AuthorizedUser_CanWriteToObject_AuthorizedForUserWrite()
    {
      var context = GetContext();

      SilverlightPrincipal.Logout();
      SilverlightPrincipal.LoginUsingMembershipProviderDatPortal("user", "1234", (o, e) =>
      {
        var item = new UserAndAdminCanCreateAndWrite();

        item.A = "test";//no SecurityException

        context.Assert.Success();
      });

      context.Complete();
    }

    [TestMethod]
    public void AuthorizedAdmin_CanWriteToObject_AuthorizedForUserWrite()
    {
      var context = GetContext();

      SilverlightPrincipal.Logout();
      SilverlightPrincipal.LoginUsingMembershipProviderDatPortal("admin", "12345", (o, e) =>
      {
        var item = new UserAndAdminCanCreateAndWrite();

        item.A = "test";//no SecurityException

        context.Assert.Success();
      });


      context.Complete();
    }

    [TestMethod]
    [ExpectedException(typeof(System.Security.SecurityException))]
    public void AuthorizedUser_CanNotWriteToObject_AuthorizedForAdminOnlyWrite()
    {
      var context = GetContext();

      SilverlightPrincipal.Logout();
      SilverlightPrincipal.LoginUsingMembershipProviderDatPortal("user", "1234", (o, e) =>
      {
        var item = new OnlyAdminCanWrite();

        context.Assert.Try(() =>
        {
          item.A = "test";
        });
        context.Assert.Fail();//assure that exception was thrown for Assert.Try
        context.Assert.Success();
      });

      context.Complete();
    }

    [TestMethod]
    public void AuthorizedAdmin_CanWriteToObject_AuthorizedForAdminOnlyWrite()
    {
      var context = GetContext();

      SilverlightPrincipal.Logout();
      SilverlightPrincipal.LoginUsingMembershipProviderDatPortal("admin", "12345", (o, e) =>
      {
        var item = new OnlyAdminCanWrite();

        item.A = "test";//no SecurityException

        context.Assert.Success();
      });

      context.Complete();
    }


  }
}
