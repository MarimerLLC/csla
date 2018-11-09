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
      Thread.CurrentPrincipal = new VPrincipal("Administrator", "ProjectManager");
      var project = DataPortal.Create<Project>();
      Assert.IsNotNull(project);
    }

    [TestMethod]
    public void GetWhenIsInRoleDeletesObject()
    {
      Thread.CurrentPrincipal = new VPrincipal("Administrator", "ProjectManager");
      var project = DataPortal.Fetch<Project>(Guid.NewGuid()); 
      Assert.IsNotNull(project);
    }
    
    [TestMethod]
    public void DeleteWhenIsInRoleDeletesObject()
    {
      Thread.CurrentPrincipal = new VPrincipal("Administrator", "ProjectManager");
      DataPortal.Delete<Project>(Guid.NewGuid());
      Assert.IsTrue(true);
    }

    [TestMethod]
    [ExpectedException(typeof(SecurityException))]
    public void SetPropertyWhenNotIsInRoleThrowsException()
    {
      Thread.CurrentPrincipal = new VPrincipal("Administrator");
      var project = DataPortal.Fetch<Project>(Guid.NewGuid());
      project.Name = "My name";
      Assert.Inconclusive("should never get here.");
    }

    [TestMethod]
    public void SetPropertyWhenIsInRoleSetsValue()
    {
      Thread.CurrentPrincipal = new VPrincipal("Administrator");
      var project = DataPortal.Fetch<Project>(Guid.NewGuid());
      project.Description = "My description";
      Assert.AreSame("My description", project.Description);
    }
  }
}
