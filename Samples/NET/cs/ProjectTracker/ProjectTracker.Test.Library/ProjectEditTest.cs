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
    [TestInitialize]
    public void Setup()
    {
      var principal = new System.Security.Principal.GenericPrincipal(
        new System.Security.Principal.GenericIdentity("Test"),
        new string[] { "ProjectManager" });
      Csla.ApplicationContext.User = principal;
    }

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
    [ExpectedException(typeof(ProjectTracker.Dal.ConcurrencyException))]
    public void UpdateProject_ConcurrencyFail()
    {
      var obj = ProjectEdit.NewProject();
      obj.Name = "Test";
      obj.Description = "This is a test";
      obj = obj.Save();

      var obj2 = obj.Clone();

      obj.Name = "Test 2";
      obj = obj.Save();

      obj2.Name = "Test 3";
      try
      {
        obj2 = obj2.Save();
      }
      catch (Csla.DataPortalException ex)
      {
        throw ex.BusinessException;
      }
    }

    [TestMethod]
    public void ImmediateDeleteProject()
    {
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
    public void AddProjectResource()
    {
      var obj = ProjectEdit.NewProject();
      obj.Name = "Test";
      obj.Description = "This is a test";
      obj.Resources.Assign(1);
      obj = obj.Save();

      obj = ProjectEdit.GetProject(obj.Id);
      Assert.IsTrue(obj.Resources.Count > 0);
      Assert.AreEqual(1, obj.Resources[0].ResourceId);

      ProjectEdit.DeleteProject(obj.Id);
    }

    [TestMethod]
    public void UpdateProjectResource()
    {
      var obj = ProjectEdit.NewProject();
      obj.Name = "Test";
      obj.Description = "This is a test";
      obj.Resources.Assign(1);
      obj = obj.Save();

      obj = ProjectEdit.GetProject(obj.Id);
      obj.Resources[0].Role = 2;
      obj = obj.Save();

      obj = ProjectEdit.GetProject(obj.Id);
      Assert.IsTrue(obj.Resources.Count > 0);
      Assert.AreEqual(1, obj.Resources[0].ResourceId);
      Assert.AreEqual(2, obj.Resources[0].Role);

      ProjectEdit.DeleteProject(obj.Id);
    }

    [TestMethod]
    public void RemoveProjectResource()
    {
      var obj = ProjectEdit.NewProject();
      obj.Name = "Test";
      obj.Description = "This is a test";
      obj.Resources.Assign(1);
      obj = obj.Save();

      obj.Resources.Remove(1);
      obj = obj.Save();

      obj = ProjectEdit.GetProject(obj.Id);
      Assert.AreEqual(0, obj.Resources.Count);

      ProjectEdit.DeleteProject(obj.Id);
    }
  }
}
