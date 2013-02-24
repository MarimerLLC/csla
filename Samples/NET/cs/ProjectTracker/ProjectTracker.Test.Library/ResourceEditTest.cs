﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectTracker.Library;
using System.Threading;

namespace ProjectTracker.Test.Library
{
  [TestClass]
  public class ResourceEditTest
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
    public void GetResourceEdit()
    {
      var obj = ResourceEdit.GetResourceEdit(1);
      Assert.IsNotNull(obj);
      Assert.AreEqual(1, obj.Id);
    }

    [TestMethod]
    public void GetResourceAsync()
    {
      var sync = new AutoResetEvent(false);
      ResourceEdit.GetResourceEdit(1, (o, e) =>
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
    public void InsertResource()
    {
      var obj = ResourceEdit.NewResourceEdit();
      obj.FirstName = "Rocky";
      obj.LastName = "Lhotka";
      obj = obj.Save();
      Assert.IsTrue(obj.Id > 0);
      Assert.IsFalse(obj.IsNew);
      Assert.IsFalse(obj.IsDirty);
      Assert.IsFalse(obj.IsSavable);
      Assert.IsTrue(ResourceEdit.Exists(obj.Id));

      ResourceEdit.DeleteResourceEdit(obj.Id);
    }

    [TestMethod]
    public void UpdateResource()
    {
      var obj = ResourceEdit.NewResourceEdit();
      obj.FirstName = "Rocky";
      obj.LastName = "Lhotka";
      obj = obj.Save();

      obj = ResourceEdit.GetResourceEdit(obj.Id);
      obj.FirstName = "Jonny";
      obj.LastName = "Bekkum";
      obj = obj.Save();
      Assert.IsFalse(obj.IsNew);
      Assert.IsFalse(obj.IsDirty);
      Assert.IsFalse(obj.IsSavable);

      ResourceEdit.DeleteResourceEdit(obj.Id);
    }

    [TestMethod]
    [ExpectedException(typeof(ProjectTracker.Dal.ConcurrencyException))]
    public void UpdateResource_ConcurrencyFail()
    {
      var obj = ResourceEdit.NewResourceEdit();
      obj.FirstName = "Rocky";
      obj.LastName = "Lhotka";
      obj = obj.Save();

      var obj2 = obj.Clone();

      obj.FirstName = "Jonny";
      obj = obj.Save();

      obj2.FirstName = "Kevin";
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
    public void ImmediateDeleteResource()
    {
      var obj = ResourceEdit.NewResourceEdit();
      obj.FirstName = "Rocky";
      obj.LastName = "Lhotka";
      obj = obj.Save();

      ResourceEdit.DeleteResourceEdit(obj.Id);

      Assert.IsFalse(ResourceEdit.Exists(obj.Id));
    }

    [TestMethod]
    public void DeferredDeleteResource()
    {
      var obj = ResourceEdit.NewResourceEdit();
      obj.FirstName = "Rocky";
      obj.LastName = "Lhotka";
      obj = obj.Save();

      obj.Delete();
      obj = obj.Save();
      Assert.IsTrue(obj.IsNew);
      Assert.IsTrue(obj.IsDirty);
      Assert.IsFalse(ResourceEdit.Exists(obj.Id));
    }

    [TestMethod]
    public void AddResourceAssignment()
    {
      var obj = ResourceEdit.NewResourceEdit();
      obj.FirstName = "Rocky";
      obj.LastName = "Lhotka";
      obj.Assignments.AssignTo(1);
      obj = obj.Save();

      obj = ResourceEdit.GetResourceEdit(obj.Id);
      Assert.IsTrue(obj.Assignments.Count > 0);
      Assert.AreEqual(1, obj.Assignments[0].ProjectId);

      ProjectEdit.DeleteProject(obj.Id);
    }

    [TestMethod]
    public void UpdateResourceAssignment()
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
    public void RemoveResourceAssignment()
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
