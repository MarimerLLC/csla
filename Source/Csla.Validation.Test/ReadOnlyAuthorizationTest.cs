using System.Security.Claims;
using System.Threading;
using Csla.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectTracker.Library;

namespace Csla.Validation.Test
{
  [TestClass]
  public class ReadOnlyAuthorizationTest
  {
    private static ClaimsPrincipal GetPrincipal(params string[] roles)
    {
      var identity = new ClaimsIdentity();
      foreach (var item in roles)
        identity.AddClaim(new Claim(ClaimTypes.Role, item));
      return new ClaimsPrincipal(identity);
    }

    [TestInitialize]
    public void Initialize()
    {
      Thread.CurrentPrincipal = GetPrincipal("NoRole");
    }

    [TestCleanup]
    public void Cleanup()
    {
      Thread.CurrentPrincipal = ClaimsPrincipal.Current;
    }

    [TestMethod]
    [ExpectedException(typeof(SecurityException))]
    public void GetWhenNotIsInRoleThrowsException()
    {
      var resources = DataPortal.Fetch<ResourceList>();
    }

    [TestMethod]
    public void GetWhenIsInRoleReturnsList()
    {
      Thread.CurrentPrincipal = GetPrincipal("Administrator");
      var resources = DataPortal.Fetch<ResourceList>();
      Assert.IsTrue(resources.Count > 0);
    }
  }
}
