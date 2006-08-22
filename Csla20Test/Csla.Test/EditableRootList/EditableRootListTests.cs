using System;
using System.Collections.Generic;
using System.Text;

#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#endif 

namespace Csla.Test.EditableRootList
{
  [TestClass]
  public class EditableRootListTests
  {
    [TestMethod]
    public void AddItem()
    {
      ERlist list = new ERlist();
      ERitem item = list.AddNew();
      Assert.AreEqual(1, list.Count, "Count should be 1");
      Assert.IsTrue(list[0].IsNew, "Object should be new");
    }

    [TestMethod]
    public void RemoveNewItem()
    {
      ApplicationContext.GlobalContext.Clear();

      ERlist list = new ERlist();
      ERitem item = list.AddNew();
      Assert.AreEqual(1, list.Count, "Incorrect count after add");
      Assert.IsTrue(list[0].IsNew, "Object should be new");

      list.RemoveAt(0);
      
      Assert.AreEqual(0, list.Count, "Incorrect count after remove");
      Assert.IsNull(ApplicationContext.GlobalContext["DP"], "Object should not have done a delete");
      Assert.IsTrue(item.IsNew, "Object should be new after delete");
    }

    [TestMethod]
    public void RemoveOldItem()
    {
      ApplicationContext.GlobalContext.Clear();

      ERlist list = new ERlist();
      list.Add(ERitem.GetItem("test"));
      ERitem item = list[0];
      Assert.AreEqual(1, list.Count, "Incorrect count after add");
      Assert.IsFalse(list[0].IsNew, "Object should not be new");

      list.RemoveAt(0);

      Assert.AreEqual(0, list.Count, "Incorrect count after remove");
      Assert.AreEqual("DeleteSelf", ApplicationContext.GlobalContext["DP"].ToString(), "Object should have deleted itself");
      Assert.IsTrue(item.IsNew, "Object should be new after delete");
    }

    [TestMethod]
    public void InsertItem()
    {
      ERlist list = new ERlist();
      ERitem item = list.AddNew();
      Assert.AreEqual(1, list.Count, "Incorrect count after add");
      
      // simulate grid edit
      System.ComponentModel.IEditableObject obj = (System.ComponentModel.IEditableObject)item;
      obj.BeginEdit();
      item.Data = "test";
      obj.EndEdit();

      Assert.AreEqual("Insert", ApplicationContext.GlobalContext["DP"].ToString(), "Object should have been inserted");
      Assert.IsFalse(item.IsNew, "Object should not be new");
    }

    [TestMethod]
    public void UpdateItem()
    {
      ERlist list = new ERlist();
      list.Add(ERitem.GetItem("test"));
      ERitem item = list[0];
      Assert.AreEqual(1, list.Count, "Incorrect count after add");
      Assert.IsFalse(list[0].IsNew, "Object should not be new");

      // simulate grid edit
      System.ComponentModel.IEditableObject obj = (System.ComponentModel.IEditableObject)item;
      obj.BeginEdit();
      item.Data = "new data";
      Assert.IsFalse(list[0].IsNew, "Object should not be new");
      Assert.IsFalse(list[0].IsDeleted, "Object should not be deleted");
      Assert.IsTrue(list[0].IsDirty, "Object should be dirty");
      obj.EndEdit();

      Assert.AreEqual("Update", ApplicationContext.GlobalContext["DP"].ToString(), "Object should have been updated");
      Assert.IsFalse(item.IsNew, "Object should not be new");
    }
  }
}
