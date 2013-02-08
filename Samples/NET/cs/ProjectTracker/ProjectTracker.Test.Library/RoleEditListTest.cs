using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectTracker.Library.Admin;

namespace ProjectTracker.Test.Library
{
  [TestClass]
  public class RoleEditListTest
  {
    [TestInitialize]
    public void Setup()
    {
      var principal = new System.Security.Principal.GenericPrincipal(
        new System.Security.Principal.GenericIdentity("Test"),
        new string[] { "Administrator" });
      Csla.ApplicationContext.User = principal;
    }

    [TestMethod]
    public void GetList()
    {
      var obj = RoleEditList.GetRoles();
      Assert.IsTrue(obj.Count > 0);
    }

    [TestMethod]
    public void AddRole()
    {
      var obj = RoleEditList.GetRoles();

      var item = obj.AddNew();
      Assert.IsTrue(item.IsNew);
      item.Name = "Test";

      obj = obj.Save();

      item = (from r in obj
              where r.Name == "Test"
              select r).First();
      Assert.IsTrue(item.Id > 0);
      Assert.IsFalse(item.IsNew);
      Assert.IsFalse(item.IsDirty);

      obj.Remove(item);
      obj = obj.Save();
    }

    [TestMethod]
    public void UpdateRole()
    {
      var obj = RoleEditList.GetRoles();

      var item = obj.AddNew();
      Assert.IsTrue(item.IsNew);
      item.Name = "Unit Test Item";
      obj = obj.Save();
      obj = RoleEditList.GetRoles();

      item = (from r in obj
              where r.Name == "Unit Test Item"
              select r).First();
      item.Name = "Unit Test Item Update";
      var id = item.Id;
      Assert.IsTrue(item.IsDirty);
      obj = obj.Save();

      item = (from r in obj
              where r.Id == id
              select r).First();
      Assert.AreEqual("Unit Test Item Update", item.Name);
      Assert.IsFalse(item.IsNew);
      Assert.IsFalse(item.IsDirty);

      obj.Remove(item);
      obj = obj.Save();
    }

    [TestMethod]
    public void DeleteRole()
    {
      var obj = RoleEditList.GetRoles();

      var item = obj.AddNew();
      Assert.IsTrue(item.IsNew);
      item.Name = "Unit Test Item";
      obj = obj.Save();

      item = (from r in obj
              where r.Name == "Unit Test Item"
              select r).First();
      var id = item.Id;
      
      obj.Remove(item);
      obj = obj.Save();

      Assert.AreEqual(0, obj.Where(r => r.Id == id).Count());
    }
  }
}
