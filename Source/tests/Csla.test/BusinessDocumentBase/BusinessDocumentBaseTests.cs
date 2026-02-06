//-----------------------------------------------------------------------
// <copyright file="BusinessDocumentBaseTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Tests for BusinessDocumentBase</summary>
//-----------------------------------------------------------------------

using Csla.TestHelpers;
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
  }
}
