using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Csla.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectTracker.Library;

namespace Csla.Validation.Test
{
  [TestClass]
  public class ReadOnlyAuthorizationTest
  {
    [TestInitialize]
    public void Initialize()
    {
      Thread.CurrentPrincipal = new VPrincipal("NoRole");
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
      Thread.CurrentPrincipal = new VPrincipal("Administrator");
      var resources = DataPortal.Fetch<ResourceList>();
      Assert.IsTrue(resources.Count > 0);
    }
  }
}
