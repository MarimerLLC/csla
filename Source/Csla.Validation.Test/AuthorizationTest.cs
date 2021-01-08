using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Csla.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Validation.Test
{
  [TestClass]
  public class AuthorizationTest
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
    public void CreateWhenNotIsInRoleThrowsException()
    {
      var project = DataPortal.Create<Project>();

    }

    [TestMethod]
    [ExpectedException(typeof(SecurityException))]
    public void GetWhenNotIsInRoleThrowsException()
    {
      var project = DataPortal.Fetch<Project>(Guid.NewGuid());
    }

    [TestMethod]
    [ExpectedException(typeof(SecurityException))]
    public void DeleteWhenIsInRoleThrowsException()
    {
      DataPortal.Delete<Project>(Guid.NewGuid());
    }

    [TestMethod]
    [TestCategory("SkipWhenLiveUnitTesting")]
    public void CreateWhenIsInRoleReturnsObject()
    {
      Thread.CurrentPrincipal = GetPrincipal("Administrator", "ProjectManager");
      var project = DataPortal.Create<Project>();
      Assert.IsNotNull(project);
    }

    [TestMethod]
    public void GetWhenIsInRoleDeletesObject()
    {
      Thread.CurrentPrincipal = GetPrincipal("Administrator", "ProjectManager");
      var project = DataPortal.Fetch<Project>(Guid.NewGuid()); 
      Assert.IsNotNull(project);
    }
    
    [TestMethod]
    public void DeleteWhenIsInRoleDeletesObject()
    {
      Thread.CurrentPrincipal = GetPrincipal("Administrator", "ProjectManager");
      DataPortal.Delete<Project>(Guid.NewGuid());
      Assert.IsTrue(true);
    }

    [TestMethod]
    [ExpectedException(typeof(SecurityException))]
    public void SetPropertyWhenNotIsInRoleThrowsException()
    {
      Thread.CurrentPrincipal = GetPrincipal("Administrator");
      var project = DataPortal.Fetch<Project>(Guid.NewGuid());
      project.Name = "My name";
      Assert.Inconclusive("should never get here.");
    }

    [TestMethod]
    public void SetPropertyWhenIsInRoleSetsValue()
    {
      Thread.CurrentPrincipal = GetPrincipal("Administrator");
      var project = DataPortal.Fetch<Project>(Guid.NewGuid());
      project.Description = "My description";
      Assert.AreSame("My description", project.Description);
    }
  }
}
