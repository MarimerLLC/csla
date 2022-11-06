using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Csla.Configuration;
using Csla.Security;
using Csla.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.DataPortal
{
  [TestClass]
  public class PrincipalFlowTests
  {
    [TestMethod]
    public void Fetch_FakeRemoteDataPortalWithDefaultFlow_DoesNotFlowPrincipal()
    {
      var principal = new CslaClaimsPrincipal(new GenericIdentity("rocky", "custom"));
      var testDIContext = TestDIContextFactory.CreateContext(opts => opts
        .DataPortal(dp => dp.UseFakeRemoteDataPortalProxy()), 
        principal);

      var dataPortal = testDIContext.CreateDataPortal<PrincipalInfo>();
      var info = dataPortal.Fetch();

      Assert.IsFalse(info.IsAuthenticated);
      Assert.AreEqual(string.Empty, info.Name);
    }

    [TestMethod]
    public void Fetch_FakeRemoteDataPortalWithFlowEnabled_FlowsPrincipal()
    {
      var principal = new CslaClaimsPrincipal(new GenericIdentity("rocky", "custom"));
      var testDIContext = TestDIContextFactory.CreateContext(opts => opts
        .DataPortal(dp => {
          dp.UseFakeRemoteDataPortalProxy();
          dp.EnableSecurityPrincipalFlowFromClient();
        }), 
        principal) ;

      var dataPortal = testDIContext.CreateDataPortal<PrincipalInfo>();
      var info = dataPortal.Fetch();

      Assert.IsTrue(info.IsAuthenticated);
      Assert.AreEqual(principal.Identity.Name, info.Name);
    }
  }
}
