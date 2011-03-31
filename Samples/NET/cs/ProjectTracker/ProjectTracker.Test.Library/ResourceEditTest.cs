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
  public class ResourceEditTest
  {
    [TestMethod]
    public void GetResource()
    {
      var obj = ResourceEdit.GetResource(1);
      Assert.IsNotNull(obj);
      Assert.AreEqual(1, obj.Id);
    }

    [TestMethod]
    public void GetResourceAsync()
    {
      var sync = new AutoResetEvent(false);
      ResourceEdit.GetResource(1, (o, e) =>
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
      var principal = new System.Security.Principal.GenericPrincipal(
        new System.Security.Principal.GenericIdentity("Test"),
        new string[] { "ProjectManager" });
      Csla.ApplicationContext.User = principal;

      var obj = ResourceEdit.NewResource();
      obj.FirstName = "Rocky";
      obj.LastName = "Lhotka";
      obj = obj.Save();
      Assert.IsTrue(obj.Id > 0);
      Assert.IsFalse(obj.IsNew);
      Assert.IsFalse(obj.IsDirty);
      Assert.IsFalse(obj.IsSavable);
      Assert.IsTrue(ResourceEdit.Exists(obj.Id));

      ResourceEdit.DeleteResource(obj.Id);
    }

    [TestMethod]
    public void UpdateResource()
    {
      var principal = new System.Security.Principal.GenericPrincipal(
        new System.Security.Principal.GenericIdentity("Test"),
        new string[] { "ProjectManager" });
      Csla.ApplicationContext.User = principal;

      var obj = ResourceEdit.NewResource();
      obj.FirstName = "Rocky";
      obj.LastName = "Lhotka";
      obj = obj.Save();

      obj = ResourceEdit.GetResource(obj.Id);
      obj.FirstName = "Jonny";
      obj.LastName = "Bekkum";
      obj = obj.Save();
      Assert.IsFalse(obj.IsNew);
      Assert.IsFalse(obj.IsDirty);
      Assert.IsFalse(obj.IsSavable);

      ResourceEdit.DeleteResource(obj.Id);
    }

    [TestMethod]
    [ExpectedException(typeof(ProjectTracker.Dal.ConcurrencyException))]
    public void UpdateResource_ConcurrencyFail()
    {
      var principal = new System.Security.Principal.GenericPrincipal(
        new System.Security.Principal.GenericIdentity("Test"),
        new string[] { "ProjectManager" });
      Csla.ApplicationContext.User = principal;

      var obj = ResourceEdit.NewResource();
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
      var principal = new System.Security.Principal.GenericPrincipal(
        new System.Security.Principal.GenericIdentity("Test"),
        new string[] { "ProjectManager" });
      Csla.ApplicationContext.User = principal;

      var obj = ResourceEdit.NewResource();
      obj.FirstName = "Rocky";
      obj.LastName = "Lhotka";
      obj = obj.Save();

      ResourceEdit.DeleteResource(obj.Id);

      Assert.IsFalse(ResourceEdit.Exists(obj.Id));
    }

    [TestMethod]
    public void DeferredDeleteResource()
    {
      var principal = new System.Security.Principal.GenericPrincipal(
        new System.Security.Principal.GenericIdentity("Test"),
        new string[] { "ProjectManager" });
      Csla.ApplicationContext.User = principal;

      var obj = ResourceEdit.NewResource();
      obj.FirstName = "Rocky";
      obj.LastName = "Lhotka";
      obj = obj.Save();

      obj.Delete();
      obj = obj.Save();
      Assert.IsTrue(obj.IsNew);
      Assert.IsTrue(obj.IsDirty);
      Assert.IsFalse(ResourceEdit.Exists(obj.Id));
    }
  }
}
