//-----------------------------------------------------------------------
// <copyright file="BusinessListBaseTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

using Csla.Serialization;
using Csla.TestHelpers;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
    
    [TestMethod]
    public async Task Parent_LocationTransferWithDeletedItemsMustSetParentOnDeletedItems()
    {
      var root = CreateRoot();

      for (int i = 0; i < 5; i++)
      {
        root.Children.AddNew();
      }

      root = await root.SaveAsync();
      root.Children.Clear();

      var transferredGraph = SimulateLocationTransfer(root);

      using (new AssertionScope())
      {
        transferredGraph.Children.Parent.Should().BeSameAs(transferredGraph);
        transferredGraph.Children.DeletedItems.Should().AllSatisfy(c => c.Parent.Should().BeSameAs(transferredGraph.Children));
      }


      static Root SimulateLocationTransfer(Root original)
      {
        var serializer = _testDIContext.ServiceProvider.GetRequiredService<ISerializationFormatter>();
        return (Root)serializer.Deserialize(serializer.Serialize(original));
      }
    }

    [TestMethod]
    public void ClonePreservesChildEditLevel()
    {
      // Arrange: Create a root with children and begin editing
      var root = CreateRoot();
      var child = root.Children.AddNew();
      child.Data = "original";

      root.BeginEdit();

      // Make changes after BeginEdit
      child.Data = "modified";

      // Verify edit levels before clone
      var rootEditLevel = ((Core.IUndoableObject)root).EditLevel;
      var childEditLevel = ((Core.IUndoableObject)child).EditLevel;
      var childListEditLevel = ((Core.IUndoableObject)root.Children).EditLevel;

      Assert.AreEqual(1, rootEditLevel, "Root EditLevel should be 1 before clone");
      Assert.AreEqual(1, childEditLevel, "Child EditLevel should be 1 before clone");
      Assert.AreEqual(1, childListEditLevel, "ChildList EditLevel should be 1 before clone");

      // Act: Clone the object graph
      var clonedRoot = root.Clone();

      // Assert: Verify edit levels are preserved after clone
      var clonedRootEditLevel = ((Core.IUndoableObject)clonedRoot).EditLevel;
      var clonedChildEditLevel = ((Core.IUndoableObject)clonedRoot.Children[0]).EditLevel;
      var clonedChildListEditLevel = ((Core.IUndoableObject)clonedRoot.Children).EditLevel;

      Assert.AreEqual(1, clonedRootEditLevel, "Cloned Root EditLevel should be 1");
      Assert.AreEqual(1, clonedChildEditLevel, "Cloned Child EditLevel should be 1");
      Assert.AreEqual(1, clonedChildListEditLevel, "Cloned ChildList EditLevel should be 1");

      // Verify the modified data is preserved
      Assert.AreEqual("modified", clonedRoot.Children[0].Data, "Modified data should be preserved in clone");
    }

    [TestMethod]
    public void ClonePreservesUndoStateForCancelEdit()
    {
      // Arrange: Create a root with children and begin editing
      var root = CreateRoot();
      var child = root.Children.AddNew();
      child.Data = "original";

      root.BeginEdit();

      // Make changes after BeginEdit
      child.Data = "modified";

      // Act: Clone the object graph
      var clonedRoot = root.Clone();

      // Cancel edit on the clone - this should restore the original value
      clonedRoot.CancelEdit();

      // Assert: The cloned child should revert to original value
      Assert.AreEqual("original", clonedRoot.Children[0].Data, "CancelEdit on clone should restore original value");
      Assert.AreEqual(0, ((Core.IUndoableObject)clonedRoot).EditLevel, "Root EditLevel should be 0 after CancelEdit");
    }

    [TestMethod]
    public void ClonePreservesUndoStateForApplyEdit()
    {
      // Arrange: Create a root with children and begin editing
      var root = CreateRoot();
      var child = root.Children.AddNew();
      child.Data = "original";

      root.BeginEdit();

      // Make changes after BeginEdit
      child.Data = "modified";

      // Act: Clone the object graph
      var clonedRoot = root.Clone();

      // Apply edit on the clone
      clonedRoot.ApplyEdit();

      // Assert: The cloned child should keep the modified value
      Assert.AreEqual("modified", clonedRoot.Children[0].Data, "ApplyEdit on clone should keep modified value");
      Assert.AreEqual(0, ((Core.IUndoableObject)clonedRoot).EditLevel, "Root EditLevel should be 0 after ApplyEdit");

      // Should be able to save without errors
      Assert.IsTrue(clonedRoot.IsDirty, "Clone should be dirty");
      clonedRoot = clonedRoot.Save();
      Assert.IsFalse(clonedRoot.IsDirty, "Clone should not be dirty after save");
    }

    [TestMethod]
    public void ClonePreservesMultiLevelUndo()
    {
      // Arrange: Create a root with children
      var root = CreateRoot();
      var child = root.Children.AddNew();
      child.Data = "level0";

      // First level of editing
      root.BeginEdit();
      child.Data = "level1";

      // Second level of editing
      root.BeginEdit();
      child.Data = "level2";

      // Verify edit level is 2 before clone
      Assert.AreEqual(2, ((Core.IUndoableObject)root).EditLevel, "Root EditLevel should be 2");
      Assert.AreEqual(2, ((Core.IUndoableObject)child).EditLevel, "Child EditLevel should be 2");

      // Act: Clone the object graph
      var clonedRoot = root.Clone();
      var clonedChild = clonedRoot.Children[0];

      // Assert: Edit levels preserved
      Assert.AreEqual(2, ((Core.IUndoableObject)clonedRoot).EditLevel, "Cloned Root EditLevel should be 2");
      Assert.AreEqual(2, ((Core.IUndoableObject)clonedChild).EditLevel, "Cloned Child EditLevel should be 2");

      // Verify multi-level undo works
      Assert.AreEqual("level2", clonedChild.Data, "Should be at level2");

      clonedRoot.CancelEdit();
      Assert.AreEqual("level1", clonedChild.Data, "After first CancelEdit should be at level1");
      Assert.AreEqual(1, ((Core.IUndoableObject)clonedRoot).EditLevel, "EditLevel should be 1");

      clonedRoot.CancelEdit();
      Assert.AreEqual("level0", clonedChild.Data, "After second CancelEdit should be at level0");
      Assert.AreEqual(0, ((Core.IUndoableObject)clonedRoot).EditLevel, "EditLevel should be 0");
    }

    [TestMethod]
    public void ClonePreservesChildAddedDuringEdit()
    {
      // Arrange: Create a root and begin editing before adding child
      var root = CreateRoot();

      root.BeginEdit();

      // Add child after BeginEdit - child should have EditLevelAdded = 1
      var child = root.Children.AddNew();
      child.Data = "new child";

      // Act: Clone the object graph
      var clonedRoot = root.Clone();

      // Assert: Clone should have the child
      Assert.AreEqual(1, clonedRoot.Children.Count, "Clone should have 1 child");
      Assert.AreEqual("new child", clonedRoot.Children[0].Data, "Clone should have child data");

      // Cancel edit should remove the child (it was added after BeginEdit)
      clonedRoot.CancelEdit();
      Assert.AreEqual(0, clonedRoot.Children.Count, "After CancelEdit, child added during edit should be removed");
    }

    [TestMethod]
    public void ClonePreservesChildEditLevelWhenUsingBindingSource()
    {
      // Arrange: Create a root with children and begin editing
      var root = CreateRoot();
      var child = root.Children.AddNew();
      child.Data = "original";

      ((System.ComponentModel.IEditableObject)root).BeginEdit();
      ((System.ComponentModel.IEditableObject)child).BeginEdit();

      // Make changes after BeginEdit
      child.Data = "modified";

      // Verify edit levels before clone
      var rootEditLevel = ((Core.IUndoableObject)root).EditLevel;
      var childEditLevel = ((Core.IUndoableObject)child).EditLevel;
      var childListEditLevel = ((Core.IUndoableObject)root.Children).EditLevel;

      Assert.AreEqual(1, rootEditLevel, "Root EditLevel should be 1 before clone");
      Assert.AreEqual(1, childEditLevel, "Child EditLevel should be 1 before clone");
      Assert.AreEqual(0, childListEditLevel, "ChildList EditLevel should be 0 before clone");

      // Act: Clone the object graph
      var clonedRoot = root.Clone();

      // Assert: Verify edit levels are preserved after clone
      var clonedRootEditLevel = ((Core.IUndoableObject)clonedRoot).EditLevel;
      var clonedChildEditLevel = ((Core.IUndoableObject)clonedRoot.Children[0]).EditLevel;
      var clonedChildListEditLevel = ((Core.IUndoableObject)clonedRoot.Children).EditLevel;

      Assert.AreEqual(1, clonedRootEditLevel, "Cloned Root EditLevel should be 1");
      Assert.AreEqual(1, clonedChildEditLevel, "Cloned Child EditLevel should be 1");
      Assert.AreEqual(0, clonedChildListEditLevel, "Cloned ChildList EditLevel should be 0");

      // Verify the modified data is preserved
      Assert.AreEqual("modified", clonedRoot.Children[0].Data, "Modified data should be preserved in clone");
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