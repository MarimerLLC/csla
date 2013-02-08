using Csla;
using SilverlightClassLibrary;
using UnitDriven;

namespace SilverlightApplication
{
  public abstract class MembershipTestBase : TestBase
  {
    //[TestSetup]
    public void Setup()
    {
      DataPortal.ProxyTypeName = "Csla.DataPortalClient.WcfProxy, Csla";
      Csla.DataPortalClient.WcfProxy.DefaultUrl = "http://localhost:3372/WcfPortal.svc";
    }
  }
}
