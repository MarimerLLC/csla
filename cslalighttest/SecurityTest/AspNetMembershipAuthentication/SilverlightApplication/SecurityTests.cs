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

    [TestMethod]
    public void ValidMembershipIDandPwd_ResultInSucessfullLogin()
    {
      var context = GetContext();
      
      SLPrincipal.Logout();
      SLPrincipal.Login("TestUser", "1234", "", (o, e2) =>
      {
        var identity = ApplicationContext.User.Identity;
        
        context.Assert.IsNotNull(identity);
        context.Assert.IsTrue(identity.Name == "TestUser");
        context.Assert.IsTrue(identity.IsAuthenticated);
        context.Assert.IsTrue(((SLPrincipal.LoginEventArgs)e2).LoginSucceded);
      });

      context.Complete();
    }
  }
}
