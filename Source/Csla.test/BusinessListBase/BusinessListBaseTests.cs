//-----------------------------------------------------------------------
// <copyright file="BusinessListBaseTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnitDriven;


#if NUNIT
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#elif MSTEST
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif 

namespace Csla.Test.BusinessListBase
{
  [TestClass]
  public class BusinessListBaseTests
  {
    [TestMethod]
    
    public void CreateList()
    {
      var obj = Csla.DataPortal.Create<RootList>();
      Assert.IsNotNull(obj);

      var obj2 = Csla.DataPortal.Create<Root>();
      Assert.IsNotNull(obj2.Children);
    }

    [TestMethod]
    public void RootAddNewCore()
    {
      bool changed = false;
      var obj = Csla.DataPortal.Create<RootList>();
      obj.CollectionChanged += (o, e) =>
        {
          changed = true;
        };
      var child = obj.AddNew();
      Assert.IsTrue(changed);
      Assert.AreEqual(child, obj[0]);
    }

    [TestMethod]
    
    public void ChildAddNewCore()
    {
      bool childChanged = false;
      bool changed = false;
      var obj = Csla.DataPortal.Create<Root>();
      obj.ChildChanged += (o, e) =>
        {
          childChanged = true;
        };
      obj.Children.CollectionChanged += (o, e) =>
      {
        changed = true;
      };
      var child = obj.Children.AddNew();
      Assert.IsTrue(childChanged, "ChildChanged should be true");
      Assert.IsTrue(changed, "Collection changed should be true");
      Assert.AreEqual(child, obj.Children[0]);
    }

    [TestMethod]
    public void AcceptChangesAndSaveAfterCloneUsingMobileFormatter()
    {
      var oldSetting = Configuration.ConfigurationManager.AppSettings["CslaSerializationFormatter"];
      try
      {
        Configuration.ConfigurationManager.AppSettings.Set("CslaSerializationFormatter", "MobileFormatter");

        var rootList = Csla.DataPortal.Create<RootList>();
        rootList.BeginEdit();
        var child = rootList.AddNew();

        rootList = rootList.Clone();

        rootList.ApplyEdit();

        Assert.IsTrue(rootList.IsDirty);
        rootList = rootList.Save();
        Assert.IsFalse(rootList.IsDirty);
      }
      finally
      {
        Configuration.ConfigurationManager.AppSettings.Set("CslaSerializationFormatter", oldSetting);
      }
    }

    [TestMethod]
    public void UndoRootAcceptAdd()
    {
      var obj = Csla.DataPortal.Create<RootList>();
      obj.BeginEdit();
      obj.AddNew();
      obj.ApplyEdit();

      Assert.IsTrue(obj.IsDirty);
      
      obj = obj.Save();
      Assert.IsFalse(obj.IsDirty);
    }

    [TestMethod]
    public void UndoRootCancelAdd()
    {
      var obj = Csla.DataPortal.Create<RootList>();
      obj.BeginEdit();
      obj.AddNew();
      Assert.IsTrue(obj.IsDirty);
      Assert.AreEqual(1, obj.Count);

      obj.CancelEdit();

      Assert.IsFalse(obj.IsDirty);
      Assert.AreEqual(0, obj.Count);
    }

    [TestMethod]
    public void UndoChildAcceptAdd()
    {
      var obj = Csla.DataPortal.Create<Root>();
      obj.BeginEdit();
      obj.Children.AddNew();
      obj.ApplyEdit();

      Assert.IsTrue(obj.Children.IsDirty);

      obj = obj.Save();
      Assert.IsFalse(obj.Children.IsDirty);
    }

    [TestMethod]
    public void UndoChildCancelAdd()
    {
      var obj = Csla.DataPortal.Create<Root>();
      obj.BeginEdit();
      obj.Children.AddNew();
      Assert.IsTrue(obj.Children.IsDirty);
      Assert.AreEqual(1, obj.Children.Count);

      obj.CancelEdit();

      Assert.IsFalse(obj.Children.IsDirty);
      Assert.AreEqual(0, obj.Children.Count);
    }

    [TestMethod]
    public void InsertChild()
    {

      bool changed = false;
      var obj = Csla.DataPortal.CreateChild<ChildList>();
      obj.CollectionChanged += (o, e) =>
      {
        changed = true;
      };
      var child = Csla.DataPortal.CreateChild<Child>(); // object is marked as child
      obj.Insert(0, child);
      Assert.IsTrue(changed);
      Assert.AreEqual(child, obj[0]);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))] // thrown by BusinessListBase.InsertItem
    public void InsertNonChildFails()
    {
      bool changed = false;
      var obj = Csla.DataPortal.CreateChild<ChildList>();
      obj.CollectionChanged += (o, e) =>
      {
        changed = true;
      };
      var nonChild = new Child(); // object is not marked as child
      obj.Insert(0, nonChild);
      Assert.IsTrue(changed);
      Assert.AreEqual(nonChild, obj[0]);
    }

    [TestMethod]
    public void SuppressListChangedEventsDoNotRaiseCollectionChanged()
    {

      bool changed = false;
      var obj = Csla.DataPortal.CreateChild<ChildList>();
      obj.CollectionChanged += (o, e) =>
      {
        changed = true;
      };
      var child = Csla.DataPortal.CreateChild<Child>(); // object is marked as child

      Assert.IsTrue(obj.RaiseListChangedEvents);
      using (obj.SuppressListChangedEvents)
      {
        Assert.IsFalse(obj.RaiseListChangedEvents);

        obj.Insert(0, child);
      }
      Assert.IsFalse(changed, "Should not raise ListChanged event");
      Assert.IsTrue(obj.RaiseListChangedEvents);
      Assert.AreEqual(child, obj[0]);
    }
  }
}