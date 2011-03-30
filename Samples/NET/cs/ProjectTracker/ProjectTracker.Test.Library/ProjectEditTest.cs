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
  public class ProjectEditTest
  {
    [TestMethod]
    public void GetProject()
    {
      var obj = ProjectEdit.GetProject(1);
      Assert.IsNotNull(obj);
      Assert.AreEqual(1, obj.Id);
    }

    [TestMethod]
    public void GetProjectAsync()
    {
      var sync = new AutoResetEvent(false);
      ProjectEdit.GetProject(1, (o, e) =>
        {
          if (e.Error != null)
            Assert.Fail(e.Error.Message);
          var obj = e.Object;
          Assert.IsNotNull(obj);
          Assert.AreEqual(1, obj.Id);
          sync.Set();
        });
      sync.WaitOne(1000);
    }

    [TestMethod]
    public void InsertProject()
    {
      var principal = new System.Security.Principal.GenericPrincipal(
        new System.Security.Principal.GenericIdentity("Test"),
        new string[] { "ProjectManager" });
      Csla.ApplicationContext.User = principal;

      var obj = ProjectEdit.NewProject();
      obj.Name = "Test";
      obj.Description = "This is a test";
      obj = obj.Save();
      Assert.IsTrue(obj.Id > 0);
      Assert.IsFalse(obj.IsNew);
      Assert.IsFalse(obj.IsDirty);
      Assert.IsFalse(obj.IsSavable);
      Assert.IsTrue(ProjectEdit.Exists(obj.Id));

      ProjectEdit.DeleteProject(obj.Id);
    }

    [TestMethod]
    public void UpdateProject()
    {
      var principal = new System.Security.Principal.GenericPrincipal(
        new System.Security.Principal.GenericIdentity("Test"),
        new string[] { "ProjectManager" });
      Csla.ApplicationContext.User = principal;

      var obj = ProjectEdit.NewProject();
      obj.Name = "Test";
      obj.Description = "This is a test";
      obj = obj.Save();

      obj = ProjectEdit.GetProject(obj.Id);
      obj.Name = "Test 2";
      obj.Description = "More testing";
      obj.Started = DateTime.Today.ToShortDateString();
      obj.Ended = DateTime.Today.ToShortDateString();
      obj = obj.Save();
      Assert.IsFalse(obj.IsNew);
      Assert.IsFalse(obj.IsDirty);
      Assert.IsFalse(obj.IsSavable);

      ProjectEdit.DeleteProject(obj.Id);
    }

    [TestMethod]
    public void ImmediateDeleteProject()
    {
      var principal = new System.Security.Principal.GenericPrincipal(
        new System.Security.Principal.GenericIdentity("Test"),
        new string[] { "ProjectManager" });
      Csla.ApplicationContext.User = principal;

      var obj = ProjectEdit.NewProject();
      obj.Name = "Test";
      obj.Description = "This is a test";
      obj = obj.Save();

      ProjectEdit.DeleteProject(obj.Id);

      Assert.IsFalse(ProjectEdit.Exists(obj.Id));
    }

    [TestMethod]
    public void DeferredDeleteProject()
    {
      var principal = new System.Security.Principal.GenericPrincipal(
        new System.Security.Principal.GenericIdentity("Test"),
        new string[] { "ProjectManager" });
      Csla.ApplicationContext.User = principal;

      var obj = ProjectEdit.NewProject();
      obj.Name = "Test";
      obj.Description = "This is a test";
      obj = obj.Save();

      obj.Delete();
      obj = obj.Save();
      Assert.IsTrue(obj.IsNew);
      Assert.IsTrue(obj.IsDirty);
      Assert.IsFalse(ProjectEdit.Exists(obj.Id));
    }

    [TestMethod]
    [Ignore]
    public void StartEndCompare()
    {
      var principal = new System.Security.Principal.GenericPrincipal(
        new System.Security.Principal.GenericIdentity("Test"),
        new string[] { "ProjectManager" });
      Csla.ApplicationContext.User = principal;

      var obj = ProjectEdit.NewProject();
      obj.Name = "Test";
      obj.Description = "Testing";
      Assert.IsTrue(obj.IsValid);

      obj.Started = DateTime.Today.Add(new TimeSpan(10, 0, 0, 0)).ToShortDateString();
      Assert.IsTrue(obj.IsValid);

      obj.Started = string.Empty;
      Assert.IsTrue(obj.IsValid);

      obj.Ended = DateTime.Today.ToShortDateString();
      Assert.IsFalse(obj.IsValid);

      obj.Started = DateTime.Today.Subtract(new TimeSpan(10, 0, 0, 0)).ToShortDateString();
      Assert.IsTrue(obj.IsValid);
    }
  }
}
