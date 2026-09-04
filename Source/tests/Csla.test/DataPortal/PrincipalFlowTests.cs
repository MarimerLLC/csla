using System.Security.Claims;
using System.Security.Principal;
using Csla.Configuration;
using Csla.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.DataPortal
{
  [TestClass]
  public class PrincipalFlowTests
  {
    [TestMethod]
    [TestCategory("SkipOnCIServer")]
    public void Fetch_FakeRemoteDataPortalWithDefaultFlow_DoesNotFlowPrincipal()
    {
      var principal = new ClaimsPrincipal(new GenericIdentity("rocky", "custom"));
      using var testHost = CslaTestHost.Create(t => t.ConfigureCsla(opts => opts
        .DataPortal(dpo => dpo.AddClientSideDataPortal(dp => dp.UseFakeRemoteDataPortalProxy())))
        .AsPrincipal(principal));

      var dataPortal = testHost.GetDataPortal<PrincipalInfo>();
      var info = dataPortal.Fetch();

      Assert.IsFalse(info.IsAuthenticated);
      Assert.AreEqual(string.Empty, info.Name);
    }

    // TODO: fix test
    [Ignore]
    [TestMethod]
    public void Fetch_FakeRemoteDataPortalWithFlowEnabled_FlowsPrincipal()
    {
      var principal = new ClaimsPrincipal(new GenericIdentity("rocky", "custom"));
      using var testHost = CslaTestHost.Create(t => t.ConfigureCsla(opts => opts.
        DataPortal(dpo => dpo.AddClientSideDataPortal(dp => dp.
          UseFakeRemoteDataPortalProxy())).
        Security(so => so.FlowSecurityPrincipalFromClient = true)).AsPrincipal(principal));

      var dataPortal = testHost.GetDataPortal<PrincipalInfo>();
      var info = dataPortal.Fetch();

      Assert.IsTrue(info.IsAuthenticated);
      Assert.AreEqual(principal.Identity.Name, info.Name);
    }
  }
}
