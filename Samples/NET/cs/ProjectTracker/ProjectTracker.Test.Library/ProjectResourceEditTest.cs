using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectTracker.Library;

namespace ProjectTracker.Test.Library
{
  [TestClass]
  public class ProjectResourceEditTest
  {
    [TestMethod]
    public void RoleName()
    {
      var project = ProjectEdit.GetProject(1);
      var resource = project.Resources[0];
      var role = RoleList.GetList().Where(r => r.Key == resource.Role).First();
      Assert.AreEqual(role.Value, resource.RoleName);
    }
  }
}
