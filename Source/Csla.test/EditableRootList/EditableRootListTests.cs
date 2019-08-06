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
      _isListSaved = false;
      ERlist list = new ERlist();
      ERitem item = list.AddNew();
      Assert.AreEqual(1, list.Count, "Incorrect count after add");
      Assert.IsTrue(list[0].IsNew, "Object should be new");
      list.Saved += new EventHandler<Csla.Core.SavedEventArgs>(List_Saved);
      list.RemoveAt(0);
      Assert.AreEqual(true, _isListSaved, "List saved event did not fire after save.");
      Assert.AreEqual(0, list.Count, "Incorrect count after remove");
      Assert.IsNull(ApplicationContext.GlobalContext["DP"], "Object should not have done a delete");
      Assert.IsTrue(item.IsNew, "Object should be new after delete");
    }

    [TestMethod]
    public void RemoveOldItem()
    {
      ApplicationContext.GlobalContext.Clear();
      _isListSaved = false;

      ERlist list = new ERlist();
      
      list.Add(ERitem.GetItem("test"));
      ERitem item = list[0];
      item.Saved += new EventHandler<Csla.Core.SavedEventArgs>(item_Saved);
      list.Saved += new EventHandler<Csla.Core.SavedEventArgs>(List_Saved);
      Assert.AreEqual(1, list.Count, "Incorrect count after add");
      Assert.IsFalse(list[0].IsNew, "Object should not be new");

      list.RemoveAt(0);
      Assert.AreEqual(true, _isListSaved, "List saved event did not fire after save.");
      Assert.AreEqual(0, list.Count, "Incorrect count after remove");
      Assert.AreEqual("DeleteSelf", ApplicationContext.GlobalContext["DP"].ToString(), "Object should have deleted itself");
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
      _isListSaved = false;

      ERlist list = new ERlist();
      list.Saved += new EventHandler<Csla.Core.SavedEventArgs>(List_Saved);
      ERitem item = list.AddNew();
      Assert.AreEqual(1, list.Count, "Incorrect count after add");
      
      // simulate grid edit
      System.ComponentModel.IEditableObject obj = (System.ComponentModel.IEditableObject)item;
      obj.BeginEdit();
      list[0].Data = "test";
      obj.EndEdit();
      Assert.AreEqual(true, _isListSaved, "List saved event did not fire after save.");
      Assert.AreEqual("Insert", ApplicationContext.GlobalContext["DP"].ToString(), "Object should have been inserted");
      Assert.IsFalse(list[0].IsNew, "Object should not be new");
    }

    [TestMethod]
    public void UpdateItem()
    {
      _isListSaved = false;

      ERlist list = new ERlist();
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
      Assert.AreEqual("Update", ApplicationContext.GlobalContext["DP"].ToString(), "Object should have been updated");
      Assert.IsFalse(list[0].IsNew, "Object should not be new");
    }

    [TestMethod]
    public void BusyImplemented()
    {
      ERlist list = new ERlist();
      Assert.IsFalse(list.IsBusy);
    }

    [TestMethod]
    public void ErrorRecoveryTest()
    {
      ERlist list = new ERlist();
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