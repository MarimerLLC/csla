//-----------------------------------------------------------------------
// <copyright file="BusinessDocumentBaseTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Tests for BusinessDocumentBase</summary>
//-----------------------------------------------------------------------

using Csla.Serialization;
using Csla.TestHelpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.BusinessDocumentBase
{
  [TestClass]
  public class BusinessDocumentBaseTests
  {
    private static TestDIContext _testDIContext = null!;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
      _testDIContext = TestDIContextFactory.CreateDefaultContext();
    }

    private TestDocument CreateDocument()
    {
      var portal = _testDIContext.CreateDataPortal<TestDocument>();
      return portal.Create();
    }

    private TestDocument FetchDocument(int id)
    {
      var portal = _testDIContext.CreateDataPortal<TestDocument>();
      return portal.Fetch(id);
    }

    private DocumentLineItem CreateLineItem()
    {
      var portal = _testDIContext.CreateChildDataPortal<DocumentLineItem>();
      return portal.CreateChild();
    }

    #region Create and Basic Operations

    [TestMethod]
    public void Create_ShouldCreateEmptyDocument()
    {
      var doc = CreateDocument();
      Assert.IsNotNull(doc);
      Assert.AreEqual(0, doc.Count);
      Assert.IsTrue(doc.IsNew);
    }

    [TestMethod]
    public void Fetch_ShouldLoadDocumentWithProperties()
    {
      var doc = FetchDocument(42);
      Assert.AreEqual("DOC-42", doc.DocumentNumber);
      Assert.AreEqual(DateTime.Today, doc.DocumentDate);
    }

    [TestMethod]
    public void Fetch_ShouldLoadDocumentWithChildren()
    {
      var doc = FetchDocument(1);
      Assert.AreEqual(3, doc.Count);
      Assert.AreEqual("Item 0", doc[0].Description);
      Assert.AreEqual("Item 1", doc[1].Description);
      Assert.AreEqual("Item 2", doc[2].Description);
    }

    #endregion

    #region Collection Operations

    [TestMethod]
    public void Add_ShouldAddChildItem()
    {
      var doc = CreateDocument();
      var item = CreateLineItem();
      item.Description = "Test";

      doc.Add(item);

      Assert.AreEqual(1, doc.Count);
      Assert.AreEqual("Test", doc[0].Description);
    }

    [TestMethod]
    public void Insert_ShouldInsertAtIndex()
    {
      var doc = CreateDocument();
      var item1 = CreateLineItem();
      item1.Description = "First";
      var item2 = CreateLineItem();
      item2.Description = "Second";
      var item3 = CreateLineItem();
      item3.Description = "Inserted";

      doc.Add(item1);
      doc.Add(item2);
      doc.Insert(1, item3);

      Assert.AreEqual(3, doc.Count);
      Assert.AreEqual("First", doc[0].Description);
      Assert.AreEqual("Inserted", doc[1].Description);
      Assert.AreEqual("Second", doc[2].Description);
    }

    [TestMethod]
    public void Remove_ShouldRemoveAndTrackDeleted()
    {
      var doc = FetchDocument(1);
      var removed = doc[1];

      doc.Remove(removed);

      Assert.AreEqual(2, doc.Count);
      Assert.IsTrue(doc.ContainsDeleted(removed));
    }

    [TestMethod]
    public void RemoveAt_ShouldRemoveByIndex()
    {
      var doc = FetchDocument(1);
      Assert.AreEqual(3, doc.Count);

      doc.RemoveAt(0);

      Assert.AreEqual(2, doc.Count);
      Assert.AreEqual("Item 1", doc[0].Description);
    }

    [TestMethod]
    public void Clear_ShouldRemoveAllItems()
    {
      var doc = FetchDocument(1);
      Assert.AreEqual(3, doc.Count);

      doc.Clear();

      Assert.AreEqual(0, doc.Count);
    }

    [TestMethod]
    public void Contains_ShouldFindItem()
    {
      var doc = CreateDocument();
      var item = CreateLineItem();
      doc.Add(item);

      Assert.IsTrue(doc.Contains(item));
    }

    [TestMethod]
    public void IndexOf_ShouldReturnCorrectIndex()
    {
      var doc = CreateDocument();
      var item1 = CreateLineItem();
      var item2 = CreateLineItem();
      doc.Add(item1);
      doc.Add(item2);

      Assert.AreEqual(0, doc.IndexOf(item1));
      Assert.AreEqual(1, doc.IndexOf(item2));
    }

    [TestMethod]
    public void Enumerator_ShouldIterateItems()
    {
      var doc = FetchDocument(1);
      int count = 0;
      foreach (var item in doc)
      {
        Assert.IsNotNull(item);
        count++;
      }
      Assert.AreEqual(3, count);
    }

    [TestMethod]
    public void Indexer_Set_ShouldReplaceItem()
    {
      var doc = FetchDocument(1);
      var newItem = CreateLineItem();
      newItem.Description = "Replacement";

      doc[1] = newItem;

      Assert.AreEqual("Replacement", doc[1].Description);
      Assert.AreEqual(3, doc.Count);
    }

    #endregion

    #region Status Aggregation

    [TestMethod]
    public void IsDirty_ShouldBeFalseWhenFetched()
    {
      var doc = FetchDocument(1);
      Assert.IsFalse(doc.IsDirty);
    }

    [TestMethod]
    public void IsDirty_ShouldBeTrueWhenPropertyChanged()
    {
      var doc = FetchDocument(1);
      doc.DocumentNumber = "CHANGED";
      Assert.IsTrue(doc.IsDirty);
    }

    [TestMethod]
    public void IsDirty_ShouldBeTrueWhenChildChanged()
    {
      var doc = FetchDocument(1);
      doc[0].Description = "Changed";
      Assert.IsTrue(doc.IsDirty);
    }

    [TestMethod]
    public void IsDirty_ShouldBeTrueWhenChildAdded()
    {
      var doc = FetchDocument(1);
      var item = CreateLineItem();
      doc.Add(item);
      Assert.IsTrue(doc.IsDirty);
    }

    [TestMethod]
    public void IsDirty_ShouldBeTrueWhenChildRemoved()
    {
      var doc = FetchDocument(1);
      doc.RemoveAt(0);
      Assert.IsTrue(doc.IsDirty);
    }

    [TestMethod]
    public void IsValid_ShouldBeTrue_WhenAllValid()
    {
      var doc = CreateDocument();
      Assert.IsTrue(doc.IsValid);
    }

    [TestMethod]
    public void IsNew_ShouldBeTrueForNewDocument()
    {
      var doc = CreateDocument();
      Assert.IsTrue(doc.IsNew);
    }

    [TestMethod]
    public void IsNew_ShouldBeFalseForFetchedDocument()
    {
      var doc = FetchDocument(1);
      Assert.IsFalse(doc.IsNew);
    }

    #endregion

    #region Collection Changed Events

    [TestMethod]
    public void Add_ShouldRaiseCollectionChanged()
    {
      var doc = CreateDocument();
      bool changed = false;
      doc.CollectionChanged += (_, _) => changed = true;

      var item = CreateLineItem();
      doc.Add(item);

      Assert.IsTrue(changed);
    }

    [TestMethod]
    public void Remove_ShouldRaiseCollectionChanged()
    {
      var doc = FetchDocument(1);
      bool changed = false;
      doc.CollectionChanged += (_, _) => changed = true;

      doc.RemoveAt(0);

      Assert.IsTrue(changed);
    }

    #endregion

    #region Clone / Serialization

    [TestMethod]
    public void Clone_ShouldPreserveProperties()
    {
      var doc = FetchDocument(1);
      var clone = doc.Clone();

      Assert.AreEqual(doc.DocumentNumber, clone.DocumentNumber);
      Assert.AreEqual(doc.DocumentDate, clone.DocumentDate);
    }

    [TestMethod]
    public void Clone_ShouldPreserveChildren()
    {
      var doc = FetchDocument(1);
      var clone = doc.Clone();

      Assert.AreEqual(doc.Count, clone.Count);
      for (int i = 0; i < doc.Count; i++)
      {
        Assert.AreEqual(doc[i].Description, clone[i].Description);
        Assert.AreEqual(doc[i].Amount, clone[i].Amount);
      }
    }

    [TestMethod]
    public void Clone_ShouldPreserveDeletedList()
    {
      var doc = FetchDocument(1);
      doc.RemoveAt(0);

      var clone = doc.Clone();

      Assert.AreEqual(2, clone.Count);
      Assert.IsTrue(clone.IsDirty);
    }

    #endregion

    #region N-Level Undo

    [TestMethod]
    public void BeginEdit_CancelEdit_ShouldRestoreProperty()
    {
      var doc = FetchDocument(1);
      doc.BeginEdit();
      doc.DocumentNumber = "CHANGED";
      doc.CancelEdit();

      Assert.AreEqual("DOC-1", doc.DocumentNumber);
    }

    [TestMethod]
    public void BeginEdit_CancelEdit_ShouldRestoreAddedChild()
    {
      var doc = FetchDocument(1);
      Assert.AreEqual(3, doc.Count);

      doc.BeginEdit();
      var item = CreateLineItem();
      item.Description = "New";
      doc.Add(item);
      Assert.AreEqual(4, doc.Count);

      doc.CancelEdit();
      Assert.AreEqual(3, doc.Count);
    }

    [TestMethod]
    public void BeginEdit_CancelEdit_ShouldRestoreRemovedChild()
    {
      var doc = FetchDocument(1);
      Assert.AreEqual(3, doc.Count);

      doc.BeginEdit();
      doc.RemoveAt(0);
      Assert.AreEqual(2, doc.Count);

      doc.CancelEdit();
      Assert.AreEqual(3, doc.Count);
    }

    [TestMethod]
    public void BeginEdit_ApplyEdit_ShouldKeepChanges()
    {
      var doc = FetchDocument(1);

      doc.BeginEdit();
      doc.DocumentNumber = "CHANGED";
      var item = CreateLineItem();
      item.Description = "New";
      doc.Add(item);
      doc.ApplyEdit();

      Assert.AreEqual("CHANGED", doc.DocumentNumber);
      Assert.AreEqual(4, doc.Count);
    }

    #endregion

    #region NotUndoable

    [TestMethod]
    public void NotUndoableField_ShouldNotRestoreAfterCancelEdit()
    {
      var doc = FetchDocument(1);
      doc.NotUndoableData = "something";
      doc.DocumentNumber = "data";

      doc.BeginEdit();
      doc.NotUndoableData = "something else";
      doc.DocumentNumber = "new data";
      doc.CancelEdit();

      // [NotUndoable] field is NOT restored on CancelEdit
      Assert.AreEqual("something else", doc.NotUndoableData);
      // Regular property IS restored
      Assert.AreEqual("data", doc.DocumentNumber);
    }

    #endregion

    #region AddNew

    [TestMethod]
    public void AddNew_ShouldAddChildAndRaiseCollectionChanged()
    {
      var doc = CreateDocument();
      bool changed = false;
      doc.CollectionChanged += (_, _) => changed = true;

      var child = doc.AddNew();

      Assert.IsTrue(changed, "CollectionChanged should be raised");
      Assert.AreEqual(1, doc.Count);
      Assert.AreEqual(child, doc[0]);
    }

    [TestMethod]
    public async Task AddNewAsync_ShouldAddChildAndRaiseCollectionChanged()
    {
      var doc = CreateDocument();
      bool changed = false;
      doc.CollectionChanged += (_, _) => changed = true;

      var child = await doc.AddNewAsync();

      Assert.IsTrue(changed, "CollectionChanged should be raised");
      Assert.AreEqual(1, doc.Count);
      Assert.AreEqual(child, doc[0]);
    }

    #endregion

    #region Advanced Undo

    [TestMethod]
    public void AddRemoveAddChild_CancelRestoresOriginal()
    {
      var doc = CreateDocument();
      var item1 = CreateLineItem();
      item1.Description = "1";
      doc.Add(item1);

      doc.BeginEdit();
      doc.Remove(item1);

      var item2 = CreateLineItem();
      item2.Description = "2";
      doc.Add(item2);

      doc.CancelEdit();

      Assert.AreEqual(1, doc.Count);
      Assert.AreEqual("1", doc[0].Description);
    }

    [TestMethod]
    public void ClearItems_TracksDeletedCount()
    {
      var doc = FetchDocument(1);
      Assert.AreEqual(3, doc.Count);

      doc.Clear();

      Assert.AreEqual(0, doc.Count, "Count should be 0");
      Assert.AreEqual(3, doc.DeletedCount, "Deleted count should be 3");
    }

    [TestMethod]
    public void NestedAddApplyEdit_AllChildrenRetained()
    {
      var doc = CreateDocument();

      doc.BeginEdit();
      doc.Add(CreateLineItem());
      doc.BeginEdit();
      doc.Add(CreateLineItem());
      doc.BeginEdit();
      doc.Add(CreateLineItem());
      doc.ApplyEdit();
      doc.ApplyEdit();
      doc.ApplyEdit();

      Assert.AreEqual(3, doc.Count);
    }

    [TestMethod]
    public void NestedAddDeleteApplyEdit_TracksDeletesThroughLevels()
    {
      var doc = CreateDocument();

      doc.BeginEdit();
      doc.Add(CreateLineItem()); // A at level 1
      doc.BeginEdit();
      doc.Add(CreateLineItem()); // B at level 2
      doc.BeginEdit();
      doc.Add(CreateLineItem()); // C at level 3

      var itemC = doc[2];
      Assert.IsTrue(doc.Contains(itemC), "Child should be in collection");

      doc.RemoveAt(0);
      doc.RemoveAt(0);
      doc.RemoveAt(0);

      Assert.IsFalse(doc.Contains(itemC), "Child should not be in collection");
      Assert.IsTrue(doc.ContainsDeleted(itemC), "Deleted child should be in deleted collection");

      doc.ApplyEdit();
      Assert.IsFalse(doc.ContainsDeleted(itemC), "After first ApplyEdit: C added at level 3 should be gone");

      doc.ApplyEdit();
      Assert.IsFalse(doc.ContainsDeleted(itemC), "After second ApplyEdit");

      doc.ApplyEdit();
      Assert.AreEqual(0, doc.Count, "No children should remain");
      Assert.IsFalse(doc.ContainsDeleted(itemC), "After third ApplyEdit");
    }

    #endregion

    #region Equality

    [TestMethod]
    public void BasicEquality_DocumentEquality()
    {
      var doc1 = CreateDocument();
      Assert.IsTrue(doc1.Equals(doc1), "Same instance should be equal");
      Assert.IsTrue(Equals(doc1, doc1), "Same instance equal via static");

      var doc2 = CreateDocument();
      Assert.IsFalse(doc1.Equals(doc2), "Different instances should not be equal");
      Assert.IsFalse(Equals(doc1, doc2), "Different instances not equal via static");

      Assert.IsFalse(doc1.Equals(null), "Should not equal null");
      Assert.IsFalse(Equals(doc1, null), "Should not equal null via static");
      Assert.IsFalse(Equals(null, doc2), "null should not equal doc");
    }

    [TestMethod]
    public void ChildEquality_WithinDocument()
    {
      var doc = CreateDocument();
      var c1 = CreateLineItem(); c1.Description = "abc"; doc.Add(c1);
      var c2 = CreateLineItem(); c2.Description = "xyz"; doc.Add(c2);
      var c3 = CreateLineItem(); c3.Description = "123"; doc.Add(c3);
      doc.Remove(c3);

      Assert.IsTrue(c1.Equals(c1), "Same instance equal");
      Assert.IsTrue(Equals(c1, c1), "Same instance equal via static");

      Assert.IsFalse(c1.Equals(c2), "Different instances not equal");
      Assert.IsFalse(Equals(c1, c2), "Different instances not equal via static");

      Assert.IsFalse(c1.Equals(null), "Not equal to null");
      Assert.IsFalse(Equals(c1, null), "Not equal to null via static");
      Assert.IsFalse(Equals(null, c2), "null not equal to item");

      Assert.IsTrue(doc.Contains(c1), "Doc should contain c1");
      Assert.IsTrue(doc.Contains(c2), "Doc should contain c2");
      Assert.IsFalse(doc.Contains(c3), "Doc should not contain removed c3");
      Assert.IsTrue(doc.ContainsDeleted(c3), "Deleted c3 should be tracked");
    }

    #endregion

    #region DeletedList Advanced

    [TestMethod]
    public void DeletedListClone_PreservesDeletedItems()
    {
      var doc = FetchDocument(1);
      doc.BeginEdit();
      doc.RemoveAt(0);
      doc.RemoveAt(0);
      doc.ApplyEdit();

      var clone = doc.Clone();

      var deletedItems = ((IContainsDeletedList)clone).DeletedList.Cast<DocumentLineItem>().ToList();
      Assert.AreEqual(2, deletedItems.Count, "Clone should have 2 deleted items");
      Assert.AreEqual("Item 0", deletedItems[0].Description);
      Assert.AreEqual("Item 1", deletedItems[1].Description);
      Assert.AreEqual(1, clone.Count);
    }

    [TestMethod]
    public void DeletedListCancelEdit_RestoresDeletedItems()
    {
      var doc = FetchDocument(1);
      doc.BeginEdit();
      doc.RemoveAt(0);
      doc.RemoveAt(0);

      var clone = doc.Clone();

      var deletedInClone = ((IContainsDeletedList)clone).DeletedList.Cast<DocumentLineItem>().ToList();
      Assert.AreEqual(2, deletedInClone.Count, "Clone should see 2 deleted items");
      Assert.AreEqual(1, clone.Count);

      doc.CancelEdit();

      var deletedAfterCancel = ((IContainsDeletedList)doc).DeletedList.Cast<DocumentLineItem>().ToList();
      Assert.AreEqual(0, deletedAfterCancel.Count, "Deleted list should be empty after CancelEdit");
      Assert.AreEqual(3, doc.Count, "All items restored after CancelEdit");
    }

    #endregion

    #region Event Suppression

    [TestMethod]
    public void SuppressEvents_SuppressesCollectionChangedDuringBulkOps()
    {
      var doc = FetchDocument(1);
      bool changed = false;
      doc.CollectionChanged += (_, _) => changed = true;

      var item = CreateLineItem();
      item.Description = "Suppressed";

      Assert.IsTrue(doc.RaiseListChangedEvents);
      using (doc.SuppressListChangedEvents)
      {
        Assert.IsFalse(doc.RaiseListChangedEvents);
        doc.Insert(0, item);
      }

      Assert.IsFalse(changed, "CollectionChanged should not fire during suppression");
      Assert.IsTrue(doc.RaiseListChangedEvents);
      Assert.AreEqual(item, doc[0]);
    }

    #endregion

    #region InsertNonChildFails

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void InsertNonChild_ThrowsInvalidOperationException()
    {
      var doc = CreateDocument();
      var nonChild = new DocumentLineItem(); // not created via child data portal — IsChild = false
      doc.Insert(0, nonChild);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void IndexerSet_NonChild_ThrowsInvalidOperationException()
    {
      var doc = FetchDocument(1);
      var nonChild = new DocumentLineItem(); // not created via child data portal — IsChild = false
      doc[0] = nonChild;
    }

    #endregion

    #region Save Workflow

    [TestMethod]
    public void AcceptChangesAndSaveAfterClone()
    {
      var doc = CreateDocument();
      doc.BeginEdit();
      doc.AddNew();

      doc = doc.Clone();
      doc.ApplyEdit();

      Assert.IsTrue(doc.IsDirty);
      doc = doc.Save();
      Assert.IsFalse(doc.IsDirty);
    }

    [TestMethod]
    public void UndoAcceptAdd_SavesSuccessfully()
    {
      var doc = FetchDocument(1);
      doc.BeginEdit();
      doc.AddNew();
      doc.ApplyEdit();

      Assert.IsTrue(doc.IsDirty);

      doc = doc.Save();
      Assert.IsFalse(doc.IsDirty);
    }

    [TestMethod]
    public void UndoCancelAdd_RemovesAddedChild()
    {
      var doc = FetchDocument(1);
      doc.BeginEdit();
      doc.AddNew();
      Assert.IsTrue(doc.IsDirty);
      Assert.AreEqual(4, doc.Count);

      doc.CancelEdit();

      Assert.IsFalse(doc.IsDirty);
      Assert.AreEqual(3, doc.Count);
    }

    #endregion

    #region Clone Advanced

    [TestMethod]
    public void ClonePreservesChildEditLevel()
    {
      var doc = FetchDocument(1);
      var child = doc[0];
      child.Description = "original";

      doc.BeginEdit();
      child.Description = "modified";

      var docEditLevel = ((Core.IUndoableObject)doc).EditLevel;
      var childEditLevel = ((Core.IUndoableObject)child).EditLevel;
      Assert.AreEqual(1, docEditLevel, "Doc EditLevel should be 1 before clone");
      Assert.AreEqual(1, childEditLevel, "Child EditLevel should be 1 before clone");

      var cloned = doc.Clone();

      Assert.AreEqual(1, ((Core.IUndoableObject)cloned).EditLevel, "Cloned doc EditLevel should be 1");
      Assert.AreEqual(1, ((Core.IUndoableObject)cloned[0]).EditLevel, "Cloned child EditLevel should be 1");
      Assert.AreEqual("modified", cloned[0].Description, "Modified value preserved in clone");
    }

    [TestMethod]
    public void ClonePreservesUndoStateForCancelEdit_Advanced()
    {
      var doc = FetchDocument(1);
      doc[0].Description = "original";

      doc.BeginEdit();
      doc[0].Description = "modified";

      var cloned = doc.Clone();
      cloned.CancelEdit();

      Assert.AreEqual("original", cloned[0].Description, "CancelEdit on clone should restore original");
      Assert.AreEqual(0, ((Core.IUndoableObject)cloned).EditLevel, "EditLevel should be 0 after CancelEdit");
    }

    [TestMethod]
    public void ClonePreservesUndoStateForApplyEdit_Advanced()
    {
      var doc = FetchDocument(1);
      doc[0].Description = "original";

      doc.BeginEdit();
      doc[0].Description = "modified";

      var cloned = doc.Clone();
      cloned.ApplyEdit();

      Assert.AreEqual("modified", cloned[0].Description, "ApplyEdit on clone should keep modified value");
      Assert.AreEqual(0, ((Core.IUndoableObject)cloned).EditLevel, "EditLevel should be 0 after ApplyEdit");

      Assert.IsTrue(cloned.IsDirty);
      cloned = cloned.Save();
      Assert.IsFalse(cloned.IsDirty);
    }

    [TestMethod]
    public void ClonePreservesMultiLevelUndo()
    {
      var doc = FetchDocument(1);
      var child = doc[0];
      child.Description = "level0";

      doc.BeginEdit();
      child.Description = "level1";

      doc.BeginEdit();
      child.Description = "level2";

      Assert.AreEqual(2, ((Core.IUndoableObject)doc).EditLevel, "Doc EditLevel should be 2");
      Assert.AreEqual(2, ((Core.IUndoableObject)child).EditLevel, "Child EditLevel should be 2");

      var cloned = doc.Clone();
      var clonedChild = cloned[0];

      Assert.AreEqual(2, ((Core.IUndoableObject)cloned).EditLevel, "Cloned doc EditLevel should be 2");
      Assert.AreEqual(2, ((Core.IUndoableObject)clonedChild).EditLevel, "Cloned child EditLevel should be 2");
      Assert.AreEqual("level2", clonedChild.Description);

      cloned.CancelEdit();
      Assert.AreEqual("level1", clonedChild.Description, "After first CancelEdit");
      Assert.AreEqual(1, ((Core.IUndoableObject)cloned).EditLevel);

      cloned.CancelEdit();
      Assert.AreEqual("level0", clonedChild.Description, "After second CancelEdit");
      Assert.AreEqual(0, ((Core.IUndoableObject)cloned).EditLevel);
    }

    [TestMethod]
    public void ClonePreservesChildAddedDuringEdit()
    {
      var doc = FetchDocument(1);

      doc.BeginEdit();
      var newChild = doc.AddNew();
      newChild.Description = "new child";

      var cloned = doc.Clone();

      Assert.AreEqual(4, cloned.Count, "Clone should have 4 children");
      Assert.AreEqual("new child", cloned[3].Description);

      cloned.CancelEdit();
      Assert.AreEqual(3, cloned.Count, "Child added during edit should be removed on CancelEdit");
    }

    [TestMethod]
    public void ClonePreservesEditLevelViaBindingSource()
    {
      var doc = FetchDocument(1);
      var child = doc[0];
      child.Description = "original";

      ((System.ComponentModel.IEditableObject)doc).BeginEdit();
      ((System.ComponentModel.IEditableObject)child).BeginEdit();
      child.Description = "modified";

      Assert.AreEqual(1, ((Core.IUndoableObject)doc).EditLevel, "Doc EditLevel should be 1");
      Assert.AreEqual(2, ((Core.IUndoableObject)child).EditLevel, "Child EditLevel should be 2 (both root and direct BeginEdit)");

      var cloned = doc.Clone();

      Assert.AreEqual(1, ((Core.IUndoableObject)cloned).EditLevel, "Cloned doc EditLevel should be 1");
      Assert.AreEqual(2, ((Core.IUndoableObject)cloned[0]).EditLevel, "Cloned child EditLevel should be 2");
      Assert.AreEqual("modified", cloned[0].Description, "Modified value preserved in clone");
    }

    #endregion

    #region Async

    [TestMethod]
    public async Task WaitForIdle_SingleChildWithAsyncRule()
    {
      var doc = CreateDocument();
      var child = doc.AddNew();

      child.AsyncRuleText = "trigger rule";

      await doc.WaitForIdle(TimeSpan.FromSeconds(4));

      Assert.IsFalse(doc.IsBusy);
    }

    [TestMethod]
    public async Task WaitForIdle_MultipleChildrenWithAsyncRules()
    {
      var doc = CreateDocument();
      var child1 = doc.AddNew();
      var child2 = doc.AddNew();

      child1.AsyncRuleText = "trigger rule 1";

      await Task.Delay(TimeSpan.FromMilliseconds(500));

      child2.AsyncRuleText = "trigger rule 2";

      await doc.WaitForIdle(TimeSpan.FromSeconds(4));

      Assert.IsFalse(child1.IsBusy);
      Assert.IsFalse(child2.IsBusy);
      Assert.IsFalse(doc.IsBusy);
    }

    #endregion

    #region Parent References

    [TestMethod]
    public async Task SerializationRoundtrip_PreservesParentOnDeletedItems()
    {
      var doc = FetchDocument(1);
      doc.Clear(); // moves all 3 fetched items to deleted list

      var serializer = _testDIContext.ServiceProvider.GetRequiredService<ISerializationFormatter>();
      var transferred = (TestDocument)serializer.Deserialize(serializer.Serialize(doc));

      var deletedItems = ((IContainsDeletedList)transferred).DeletedList.Cast<DocumentLineItem>().ToList();
      Assert.AreEqual(3, deletedItems.Count, "All 3 items should be in deleted list after roundtrip");

      foreach (var deletedItem in deletedItems)
        Assert.AreSame(transferred, deletedItem.Parent, "Deleted item Parent should be the document after roundtrip");
    }

    #endregion
  }
}
