using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectTracker.Library;
using System.Threading;

namespace ProjectTracker.Test.Library
{
  [TestClass]
  public class ProjectGetterTest
  {
    [TestInitialize]
    public void Setup()
    {
      var principal = new System.Security.Principal.GenericPrincipal(
        new System.Security.Principal.GenericIdentity("Test"),
        new string[] { "ProjectManager" });
      Csla.ApplicationContext.User = principal;
    }

    [TestMethod]
    public void NewProjectAsync()
    {
      var sync = new AutoResetEvent(false);
      ProjectGetter.CreateNewProject((o, e) =>
      {
        if (e.Error != null)
          Assert.Fail(e.Error.Message);
        var obj = e.Object.Project;
        Assert.IsNotNull(obj);
        Assert.AreEqual(0, obj.Id);

        var list = e.Object.RoleList;
        Assert.IsNotNull(list);
        Assert.IsTrue(list.Count > 0);
        sync.Set();
      });
      sync.WaitOne(1000);
    }

    [TestMethod]
    public void GetProjectAsync()
    {
      var sync = new AutoResetEvent(false);
      ProjectGetter.GetExistingProject(1, (o, e) =>
      {
        if (e.Error != null)
          Assert.Fail(e.Error.Message);
        var obj = e.Object.Project;
        Assert.IsNotNull(obj);
        Assert.AreEqual(1, obj.Id);

        var list = e.Object.RoleList;
        Assert.IsNotNull(list);
        Assert.IsTrue(list.Count > 0);

        sync.Set();
      });
      sync.WaitOne(1000);
    }
  }
}
