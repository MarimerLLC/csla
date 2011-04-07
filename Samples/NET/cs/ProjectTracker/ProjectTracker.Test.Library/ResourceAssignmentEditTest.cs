using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectTracker.Library;

namespace ProjectTracker.Test.Library
{
  [TestClass]
  public class ResourceAssignmentEditTest
  {
    [TestMethod]
    public void RoleName()
    {
      var resource = ResourceEdit.GetResource(1);
      var assignment = resource.Assignments[0];
      var role = RoleList.GetList().Where(r => r.Key == assignment.Role).First();
      Assert.AreEqual(role.Value, assignment.RoleName);
    }
  }
}
