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
          //ND correct way to run the assert below would be the assert in commented line above and we will switch to it once the e.Error.InnerException serialization problem is fixed
          //context.Assert.IsTrue(e2.Error.InnerException is System.Security.SecurityException);
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
        DataPortal.BeginCreate<UserAndAdminCanCreateAndWrite>((o2,e2) =>
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
         DataPortal.BeginCreate<UserAndAdminCanCreateAndWrite>((o2, e2) =>
         {
           var item = e2.Object;

           item.A = "test";

           context.Assert.Success();
         }));

      context.Complete();
    }

    [TestMethod]
    public void AuthorizedAdmin_CanWriteToObject_AuthorizedForUserWrite()
    {
      var context = GetContext();

      SilverlightPrincipal.Logout();
      SilverlightPrincipal.LoginUsingMembershipProviderDatPortal("admin", "12345", (o, e) =>
        DataPortal.BeginCreate<UserAndAdminCanCreateAndWrite>((o2, e2) =>
        {
          var item = e2.Object;

          item.A = "test";

          context.Assert.Success();
        }));


      context.Complete();
    }

    [TestMethod]
    [ExpectedException(typeof(System.Security.SecurityException))]
    public void AuthorizedUser_CanNotWriteToObject_AuthorizedForAdminOnlyWrite()
    {
      var context = GetContext();

      SilverlightPrincipal.Logout();
      
      SilverlightPrincipal.LoginUsingMembershipProviderDatPortal("user", "1234", (o, e) =>
         DataPortal.BeginCreate<OnlyAdminCanWrite>((o2, e2) =>
         {
           var item = e2.Object;

           context.Assert.Try(() =>
            {
              item.A = "test";
            });
           context.Assert.Fail();//assure that exception was thrown for Assert.Try
           context.Assert.Success();
         }));

      context.Complete();
    }

    [TestMethod]
    public void AuthorizedAdmin_CanWriteToObject_AuthorizedForAdminOnlyWrite()
    {
      var context = GetContext();

      SilverlightPrincipal.Logout();

      //OnlyAdminCanWrite item = new 
      SilverlightPrincipal.LoginUsingMembershipProviderDatPortal("admin", "12345", (o, e) =>
        DataPortal.BeginCreate<OnlyAdminCanWrite>((o2, e2) =>
        {
          var item = e2.Object;

          item.A = "test";

          context.Assert.Success();
        }));

      context.Complete();
    }


  }
}
