﻿//-----------------------------------------------------------------------
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
using Csla.TestHelpers;
using FluentAssertions;
using System.Threading.Tasks;
using FluentAssertions.Execution;


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
    private static TestDIContext _testDIContext;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
      _testDIContext = TestDIContextFactory.CreateDefaultContext();
    }

    [TestMethod]
    public void CreateList()
    {
      var obj = CreateRootList();
      Assert.IsNotNull(obj);

      var obj2 = CreateRoot();
      Assert.IsNotNull(obj2.Children);
    }

    [TestMethod]
    public void RootAddNewCore()
    {
      bool changed = false;
      var obj = CreateRootList();
      obj.CollectionChanged += (_, _) =>
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
      var obj = CreateRoot();
      obj.ChildChanged += (_, _) =>
        {
          childChanged = true;
        };
      obj.Children.CollectionChanged += (_, _) =>
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
      var rootList = CreateRootList();
      rootList.BeginEdit();
      var child = rootList.AddNew();

      rootList = rootList.Clone();

      rootList.ApplyEdit();

      Assert.IsTrue(rootList.IsDirty);
      rootList = rootList.Save();
      Assert.IsFalse(rootList.IsDirty);
    }

    [TestMethod]
    public void UndoRootAcceptAdd()
    {
      var obj = CreateRootList();
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
      var obj = CreateRootList();
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
      var obj = CreateRoot();
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
      var obj = CreateRoot();
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
      var obj = CreateChildList();
      obj.CollectionChanged += (_, _) =>
      {
        changed = true;
      };
      var child = CreateChild(); // object is marked as child
      obj.Insert(0, child);
      Assert.IsTrue(changed);
      Assert.AreEqual(child, obj[0]);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))] // thrown by BusinessListBase.InsertItem
    public void InsertNonChildFails()
    {
      bool changed = false;
      var obj = CreateChildList();
      obj.CollectionChanged += (_, _) =>
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
      var obj = CreateChildList();
      obj.CollectionChanged += (_, _) =>
      {
        changed = true;
      };
      var child = CreateChild(); // object is marked as child

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

    [TestMethod]
    public async Task WaitForIdle_WhenAChildIsBusyTheListWillBeNonBusyWhenAllChildsAreNotBusyAnymore() 
    {
      var obj = CreateRootList();
      var child1 = obj.AddNew();

      child1.AsyncRuleText = "Trigger rule";

      await obj.WaitForIdle(TimeSpan.FromSeconds(4));

      obj.IsBusy.Should().BeFalse();
    }

    [TestMethod]
    public async Task WaitForIdle_WhenMultipleChildsAreBusyItShouldOnlyBeIdlingWhenAllChildsAreNotBusyAnymore() 
    {
      var obj = CreateRootList();
      var child1 = obj.AddNew();
      var child2 = obj.AddNew();

      child1.AsyncRuleText = "Trigger rule";

      await Task.Delay(TimeSpan.FromMilliseconds(500));

      child2.AsyncRuleText = "2nd rule triggered";

      await obj.WaitForIdle(TimeSpan.FromSeconds(2));

      using (new AssertionScope()) 
      {
        child2.IsBusy.Should().BeFalse();
        child1.IsBusy.Should().BeFalse();
        obj.IsBusy.Should().BeFalse();
      }
    }

    private Root CreateRoot()
    {
      IDataPortal<Root> dataPortal = _testDIContext.CreateDataPortal<Root>();

      return dataPortal.Create();
    }

    private RootList CreateRootList()
    {
      IDataPortal<RootList> dataPortal = _testDIContext.CreateDataPortal<RootList>();

      return dataPortal.Create();
    }

    private ChildList CreateChildList()
    {
      IChildDataPortal<ChildList> childDataPortal = _testDIContext.CreateChildDataPortal<ChildList>();

      return childDataPortal.CreateChild();
    }

    private Child CreateChild()
    {
      IChildDataPortal<Child> childDataPortal = _testDIContext.CreateChildDataPortal<Child>();

      return childDataPortal.CreateChild();
    }

  }
}