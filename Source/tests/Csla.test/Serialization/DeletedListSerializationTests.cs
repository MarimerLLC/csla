//-----------------------------------------------------------------------
// <copyright file="DeletedListSerializationTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Tests for serialization of the deleted list in editable list types.</summary>
//-----------------------------------------------------------------------

using Csla.Core;
using Csla.Serialization;
using Csla.Serialization.Mobile;
using Csla.Test.BusinessDocumentBase;
using Csla.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.Serialization
{
  [TestClass]
  public class DeletedListSerializationTests
  {
    private static CslaTestHost _testHost;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
      _testHost = CslaTestHost.Create();
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
      _testHost?.Dispose();
    }

    [TestMethod]
    public void BusinessListBase_EmptyDeletedList_IsNotSerialized()
    {
      var list = _testHost.GetDataPortal<DeletedListTestList>().Fetch(3);
      // IsDirty enumerates the deleted list, which creates it
      Assert.IsFalse(list.IsDirty);

      Assert.IsFalse(HasChild(list, "_deletedList"));
    }

    [TestMethod]
    public void BusinessListBase_NonEmptyDeletedList_RoundTrips()
    {
      var list = _testHost.GetDataPortal<DeletedListTestList>().Fetch(3);
      list.RemoveAt(0);

      Assert.IsTrue(HasChild(list, "_deletedList"));
      var clone = Clone(list);

      Assert.AreEqual(2, clone.Count);
      Assert.AreEqual(1, DeletedCount(clone));
      Assert.IsTrue(clone.IsDirty);
    }

    [TestMethod]
    public void BusinessListBase_CloneWithoutDeletedList_CanStillDelete()
    {
      var list = _testHost.GetDataPortal<DeletedListTestList>().Fetch(3);
      var clone = Clone(list);

      Assert.AreEqual(0, DeletedCount(clone));
      Assert.IsFalse(clone.IsDirty);

      var removed = clone[0];
      clone.RemoveAt(0);
      Assert.IsTrue(clone.ContainsDeleted(removed));
      Assert.IsTrue(clone.IsDirty);
    }

    [TestMethod]
    public void BusinessBindingListBase_EmptyDeletedList_IsNotSerialized()
    {
      var list = _testHost.GetDataPortal<DeletedListTestBindingList>().Fetch(3);
      Assert.IsFalse(list.IsDirty);

      Assert.IsFalse(HasChild(list, "_deletedList"));
    }

    [TestMethod]
    public void BusinessBindingListBase_NonEmptyDeletedList_RoundTrips()
    {
      var list = _testHost.GetDataPortal<DeletedListTestBindingList>().Fetch(3);
      list.RemoveAt(0);

      Assert.IsTrue(HasChild(list, "_deletedList"));
      var clone = Clone(list);

      Assert.AreEqual(2, clone.Count);
      Assert.AreEqual(1, DeletedCount(clone));
      Assert.IsTrue(clone.IsDirty);
    }

    [TestMethod]
    public void BusinessBindingListBase_CloneWithoutDeletedList_CanStillDelete()
    {
      var list = _testHost.GetDataPortal<DeletedListTestBindingList>().Fetch(3);
      var clone = Clone(list);

      Assert.AreEqual(0, DeletedCount(clone));
      Assert.IsFalse(clone.IsDirty);

      var removed = clone[0];
      clone.RemoveAt(0);
      Assert.IsTrue(clone.ContainsDeleted(removed));
      Assert.IsTrue(clone.IsDirty);
    }

    [TestMethod]
    public void BusinessDocumentBase_EmptyDeletedList_IsNotSerialized()
    {
      var doc = _testHost.GetDataPortal<TestDocument>().Fetch(1);
      Assert.IsFalse(doc.IsDirty);

      Assert.IsFalse(HasChild(doc, "_bdb_deletedList"));
    }

    [TestMethod]
    public void BusinessDocumentBase_NonEmptyDeletedList_RoundTrips()
    {
      var doc = _testHost.GetDataPortal<TestDocument>().Fetch(1);
      doc.RemoveAt(0);

      Assert.IsTrue(HasChild(doc, "_bdb_deletedList"));
      var clone = Clone(doc);

      Assert.AreEqual(2, clone.Count);
      Assert.AreEqual(1, DeletedCount(clone));
      Assert.IsTrue(clone.IsDirty);
    }

    [TestMethod]
    public void BusinessDocumentBase_CloneWithoutDeletedList_CanStillDelete()
    {
      var doc = _testHost.GetDataPortal<TestDocument>().Fetch(1);
      var clone = Clone(doc);

      Assert.AreEqual(0, DeletedCount(clone));
      Assert.IsFalse(clone.IsDirty);

      var removed = clone[0];
      clone.RemoveAt(0);
      Assert.IsTrue(clone.ContainsDeleted(removed));
      Assert.IsTrue(clone.IsDirty);
    }

    private static bool HasChild(object graph, string name)
    {
      var formatter = (MobileFormatter)_testHost.Services.GetRequiredService<ISerializationFormatter>();
      return formatter.SerializeToDTO(graph).Any(i => i.Children.ContainsKey(name));
    }

    private static T Clone<T>(T graph)
    {
      var formatter = _testHost.Services.GetRequiredService<ISerializationFormatter>();
      using var buffer = new MemoryStream();
      formatter.Serialize(buffer, graph);
      buffer.Position = 0;
      return (T)formatter.Deserialize(buffer);
    }

    private static int DeletedCount(IContainsDeletedList list) => list.DeletedList.Count();
  }

  [Serializable]
  public class DeletedListTestList : BusinessListBase<DeletedListTestList, DeletedListTestChild>
  {
    [Fetch]
    private void Fetch(int count, [Inject] IChildDataPortal<DeletedListTestChild> childPortal)
    {
      using (LoadListMode)
      {
        for (int i = 0; i < count; i++)
          Add(childPortal.FetchChild(i));
      }
    }
  }

  [Serializable]
  public class DeletedListTestBindingList : BusinessBindingListBase<DeletedListTestBindingList, DeletedListTestChild>
  {
    [Fetch]
    private void Fetch(int count, [Inject] IChildDataPortal<DeletedListTestChild> childPortal)
    {
      using (LoadListMode)
      {
        for (int i = 0; i < count; i++)
          Add(childPortal.FetchChild(i));
      }
    }
  }

  [Serializable]
  public class DeletedListTestChild : BusinessBase<DeletedListTestChild>
  {
    public static readonly PropertyInfo<int> IdProperty = RegisterProperty<int>(nameof(Id));
    public int Id
    {
      get => GetProperty(IdProperty);
      set => SetProperty(IdProperty, value);
    }

    [FetchChild]
    private void Fetch(int id)
    {
      LoadProperty(IdProperty, id);
    }
  }
}
