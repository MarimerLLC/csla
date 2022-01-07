//-----------------------------------------------------------------------
// <copyright file="EditableRootListTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using Csla.TestHelpers;

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
    private static TestDIContext _testDIContext;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
      _testDIContext = TestDIContextFactory.CreateDefaultContext();
    }

    [TestMethod]
    public void AddItem()
    {
      IDataPortal<ERlist> dataPortal = _testDIContext.CreateDataPortal<ERlist>();

      ERlist list = dataPortal.Create();
      ERitem item = list.AddNew();
      Assert.AreEqual(1, list.Count, "Count should be 1");
      Assert.IsTrue(list[0].IsNew, "Object should be new");
    }

    [TestMethod]
    public void RemoveNewItem()
    {
      IDataPortal<ERlist> dataPortal = _testDIContext.CreateDataPortal<ERlist>();

      TestResults.Reinitialise();
      _isListSaved = false;
      ERlist list = dataPortal.Create();
      ERitem item = list.AddNew();
      Assert.AreEqual(1, list.Count, "Incorrect count after add");
      Assert.IsTrue(list[0].IsNew, "Object should be new");
      list.Saved += new EventHandler<Csla.Core.SavedEventArgs>(List_Saved);
      list.RemoveAt(0);
      Assert.AreEqual(true, _isListSaved, "List saved event did not fire after save.");
      Assert.AreEqual(0, list.Count, "Incorrect count after remove");
      Assert.AreEqual("", TestResults.GetResult("DP"), "Object should not have done a delete");
      Assert.IsTrue(item.IsNew, "Object should be new after delete");
    }

    [TestMethod]
    public void RemoveOldItem()
    {
      IDataPortal<ERlist> dataPortal = _testDIContext.CreateDataPortal<ERlist>();

      TestResults.Reinitialise();
      _isListSaved = false;

      ERlist list = dataPortal.Create();
      
      list.Add(ERitem.GetItem("test"));
      ERitem item = list[0];
      item.Saved += new EventHandler<Csla.Core.SavedEventArgs>(item_Saved);
      list.Saved += new EventHandler<Csla.Core.SavedEventArgs>(List_Saved);
      Assert.AreEqual(1, list.Count, "Incorrect count after add");
      Assert.IsFalse(list[0].IsNew, "Object should not be new");

      list.RemoveAt(0);
      Assert.AreEqual(true, _isListSaved, "List saved event did not fire after save.");
      Assert.AreEqual(0, list.Count, "Incorrect count after remove");
      Assert.AreEqual("DeleteSelf", TestResults.GetResult("DP"), "Object should have deleted itself");
      Assert.IsTrue(_itemIsNew, "Object should be new after delete");
    }

    private bool _itemIsNew;
    private bool _isListSaved;

    void item_Saved(object sender, Csla.Core.SavedEventArgs e)
    {
      _itemIsNew = ((ERitem)e.NewObject).IsNew;
    }

    void List_Saved(object sender, Csla.Core.SavedEventArgs e)
    {
      _isListSaved = (e.Error==null && e.NewObject != null);
    }

    [TestMethod]
    public void InsertItem()
    {
      IDataPortal<ERlist> dataPortal = _testDIContext.CreateDataPortal<ERlist>();

      _isListSaved = false;

      ERlist list = dataPortal.Create();
      list.Saved += new EventHandler<Csla.Core.SavedEventArgs>(List_Saved);
      ERitem item = list.AddNew();
      Assert.AreEqual(1, list.Count, "Incorrect count after add");
      
      // simulate grid edit
      System.ComponentModel.IEditableObject obj = (System.ComponentModel.IEditableObject)item;
      obj.BeginEdit();
      list[0].Data = "test";
      obj.EndEdit();
      Assert.AreEqual(true, _isListSaved, "List saved event did not fire after save.");
      Assert.AreEqual("Insert", TestResults.GetResult("DP"), "Object should have been inserted");
      Assert.IsFalse(list[0].IsNew, "Object should not be new");
    }

    [TestMethod]
    public void UpdateItem()
    {
      IDataPortal<ERlist> dataPortal = _testDIContext.CreateDataPortal<ERlist>();

      _isListSaved = false;

      ERlist list = dataPortal.Create();
      list.Saved += new EventHandler<Csla.Core.SavedEventArgs>(List_Saved);
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
      Assert.AreEqual(true, _isListSaved, "List saved event did not fire after save.");
      Assert.AreEqual("Update", TestResults.GetResult("DP"), "Object should have been updated");
      Assert.IsFalse(list[0].IsNew, "Object should not be new");
    }

    [TestMethod]
    public void BusyImplemented()
    {
      IDataPortal<ERlist> dataPortal = _testDIContext.CreateDataPortal<ERlist>();

      ERlist list = dataPortal.Create();
      Assert.IsFalse(list.IsBusy);
    }

    [TestMethod]
    public void ErrorRecoveryTest()
    {
      IDataPortal<ERlist> dataPortal = _testDIContext.CreateDataPortal<ERlist>();

      ERlist list = dataPortal.Create();
      bool errorOccurred = false;
      try
      {
        list.SaveItem(10);
      }
      catch
      {
        errorOccurred = true;
      }
      Assert.AreEqual(true, errorOccurred, "An error should have been thrown.");
      Assert.AreEqual(true, list.RaiseListChangedEvents, "RaiseListChangedEvents should have been reset");
    }
  }
}