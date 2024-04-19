﻿//-----------------------------------------------------------------------
// <copyright file="LinqObservableCollectionTest.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

using System.Collections.ObjectModel;
using Csla.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.Linq
{
  /// <summary>
  ///This is a test class for LinqObservableCollectionTest and is intended
  ///to contain all LinqObservableCollectionTest Unit Tests
  ///</summary>
  [TestClass]
  public class LinqObservableCollectionTest
  {
    private static TestDIContext _testDIContext;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
      _testDIContext = TestDIContextFactory.CreateDefaultContext();
    }

    [TestInitialize]
    public void Initialize()
    {
      TestResults.Reinitialise();
    }

#if !WINDOWS_PHONE
    [TestMethod]
    public void Blb2Loc_WhereOnly()
    {
      IDataPortal<TestList> dataPortal = _testDIContext.CreateDataPortal<TestList>();

      var source = dataPortal.Create();
      var synced = source.ToSyncList(c => c.Id > 100);
      Assert.AreEqual(3, synced.Count);
    }
#endif

    [TestMethod]
    public void Blb2Loc()
    {
      IDataPortal<TestList> dataPortal = _testDIContext.CreateDataPortal<TestList>();

      var source = dataPortal.Create();
      var synced = source.ToSyncList(from r in source
                  where r.Id > 100
                  select r);
      Assert.AreEqual(3, synced.Count);
    }

    [TestMethod]
    public void Blb2Loc_Ordered()
    {
      IDataPortal<TestList> dataPortal = _testDIContext.CreateDataPortal<TestList>();

      var source = dataPortal.Create();
      var synced = source.ToSyncList(from r in source
                                     orderby r.Name
                                     select r);
      Assert.AreEqual(453, synced[0].Id);
      Assert.AreEqual(123, synced[3].Id);
    }

    [TestMethod]
    public void Blb2Loc_ResultToSync()
    {
      IDataPortal<TestList> dataPortal = _testDIContext.CreateDataPortal<TestList>();

      var source = dataPortal.Create();
      var synced = (from r in source
                     where r.Id > 100
                     select r).ToSyncList(source);
      Assert.AreEqual(3, synced.Count);
    }

    [TestMethod]
    public void Blb2Loc_Add()
    {
      IDataPortal<TestList> dataPortal = _testDIContext.CreateDataPortal<TestList>();
      IChildDataPortal<TestItem> childDataPortal = _testDIContext.CreateChildDataPortal<TestItem>();

      var source = dataPortal.Create();
      var query = from r in source
                  where r.Id > 100
                  select r;
      var synced = source.ToSyncList(query);

      var newItem = childDataPortal.FetchChild(432, "New item");
      synced.Add(newItem);

      Assert.AreEqual(4, synced.Count, "synced should have item");
      Assert.AreEqual(5, source.Count, "source should have item");
      Assert.IsTrue(synced.Contains(newItem), "synced should contain new item");
      Assert.IsTrue(source.Contains(newItem), "source should contain new item");
    }

    [TestMethod]
    public void Blb2Loc_Remove()
    {
      IDataPortal<TestList> dataPortal = _testDIContext.CreateDataPortal<TestList>();

      var source = dataPortal.Create();
      var query = from r in source
                  where r.Id > 100
                  select r;
      var synced = source.ToSyncList(query);

      var oldItem = synced[0];
      synced.RemoveAt(0);

      Assert.AreEqual(2, synced.Count, "synced count wrong");
      Assert.AreEqual(3, source.Count, "source count wrong");
      Assert.IsFalse(synced.Contains(oldItem), "synced should not contain item");
      Assert.IsFalse(source.Contains(oldItem), "source should not contain item");
    }

    [TestMethod]
    public void Blb2Loc_RemoveOriginal()
    {
      IDataPortal<TestList> dataPortal = _testDIContext.CreateDataPortal<TestList>();

      var source = dataPortal.Create();
      var query = from r in source
                  where r.Id > 100
                  select r;
      var synced = source.ToSyncList(query);
      int count = 0;
      foreach (var item in synced)
        count++;
      Assert.AreEqual(3, count, "Calculated count wrong (br)");
      Assert.AreEqual(3, synced.Count, "Synced count wrong (br)");
      Assert.AreEqual(4, source.Count, "source count wrong (br)");
      Assert.AreEqual(0, synced.Count(_ => _.Id == 12), "synced contains 12");

      source.RemoveAt(3);

      count = 0;
      foreach (var item in synced)
        count++;

      Assert.AreEqual(3, count, "Calculated count wrong");
      Assert.AreEqual(3, synced.Count, "Synced count wrong");
      Assert.AreEqual(3, source.Count, "source count wrong");
      Assert.AreEqual(0, synced.Count(_ => _.Id == 12), "synced contains 12");
    }

    [TestMethod]
    public void Create()
    {
      var source = new ObservableCollection<string>() { "a", "b", "c" };
      var query = from r in source
                  where r == "b"
                  select r;
      var obj = new LinqObservableCollection<string>(source, query);
      Assert.IsNotNull(obj);
      Assert.AreEqual(1, obj.Count);
      Assert.AreEqual("b", obj[0]);
    }

    [TestMethod]
    public void AddItem()
    {
      var source = new ObservableCollection<string>() { "a", "b", "c" };
      var query = from r in source
                  where r == "b"
                  select r;
      var obj = new LinqObservableCollection<string>(source, query);

      bool sourceChanged = false;
      bool objChanged = false;
      source.CollectionChanged += (_, _) =>
        {
          sourceChanged = true;
        };
      obj.CollectionChanged += (_, _) =>
        {
          objChanged = true;
        };
      source.Add("z");
      Assert.IsTrue(sourceChanged, "SourceChanged");
      Assert.IsTrue(objChanged, "objChanged");
      Assert.AreEqual(4, source.Count);
      Assert.AreEqual(2, obj.Count);
      Assert.AreEqual("z", source[3]);
      Assert.AreEqual("z", obj[1]);

      sourceChanged = false;
      objChanged = false;
      obj.Add("x");
      Assert.IsTrue(sourceChanged, "SourceChanged");
      Assert.IsTrue(objChanged, "objChanged");
      Assert.AreEqual(5, source.Count);
      Assert.AreEqual(3, obj.Count);
      Assert.AreEqual("x", source[4]);
      Assert.AreEqual("x", obj[2]);
    }

    [TestMethod]
    public void InsertItem()
    {
      var source = new ObservableCollection<string>() { "a", "b", "c" };
      var query = from r in source
                  where r == "b"
                  select r;
      var obj = new LinqObservableCollection<string>(source, query);

      bool sourceChanged = false;
      bool objChanged = false;
      source.CollectionChanged += (_, _) =>
      {
        sourceChanged = true;
      };
      obj.CollectionChanged += (_, _) =>
      {
        objChanged = true;
      };
      source.Insert(1, "z");
      Assert.IsTrue(sourceChanged, "SourceChanged");
      Assert.IsTrue(objChanged, "objChanged");
      Assert.AreEqual(4, source.Count);
      Assert.AreEqual(2, obj.Count);
      Assert.AreEqual("z", source[1]);
      Assert.AreEqual("z", obj[1]);

      sourceChanged = false;
      objChanged = false;
      obj.Insert(1, "x");
      Assert.IsTrue(sourceChanged, "SourceChanged");
      Assert.IsTrue(objChanged, "objChanged");
      Assert.AreEqual(5, source.Count);
      Assert.AreEqual(3, obj.Count);
      Assert.AreEqual("x", source[1]);
      Assert.AreEqual("x", obj[1]);
    }

    [TestMethod]
    public void RemoveAt()
    {
      var source = new ObservableCollection<string>() { "a", "b", "c", "bb" };
      var query = from r in source
                  where r[0] == 'b'
                  select r;
      var obj = new LinqObservableCollection<string>(source, query);

      bool sourceChanged = false;
      bool objChanged = false;
      source.CollectionChanged += (_, _) =>
      {
        sourceChanged = true;
      };
      obj.CollectionChanged += (_, _) =>
      {
        objChanged = true;
      };
      source.RemoveAt(1);
      Assert.IsTrue(sourceChanged, "SourceChanged");
      Assert.IsTrue(objChanged, "objChanged");
      Assert.AreEqual(3, source.Count);
      Assert.AreEqual(1, obj.Count);
      Assert.AreEqual("c", source[1]);
      Assert.AreEqual("bb", obj[0]);

      sourceChanged = false;
      objChanged = false;
      obj.RemoveAt(0);
      Assert.IsTrue(sourceChanged, "SourceChanged");
      Assert.IsTrue(objChanged, "objChanged");
      Assert.AreEqual(2, source.Count);
      Assert.AreEqual(0, obj.Count);
      Assert.AreEqual("a", source[0]);
      Assert.AreEqual("c", source[1]);
    }

    [TestMethod]
    public void RemoveItem()
    {
      var source = new ObservableCollection<string>() { "a", "b", "c", "bb" };
      var query = from r in source
                  where r[0] == 'b'
                  select r;
      var obj = new LinqObservableCollection<string>(source, query);

      bool sourceChanged = false;
      bool objChanged = false;
      source.CollectionChanged += (_, _) =>
      {
        sourceChanged = true;
      };
      obj.CollectionChanged += (_, _) =>
      {
        objChanged = true;
      };
      source.Remove("b");
      Assert.IsTrue(sourceChanged, "SourceChanged");
      Assert.IsTrue(objChanged, "objChanged");
      Assert.AreEqual(3, source.Count);
      Assert.AreEqual(1, obj.Count);
      Assert.AreEqual("c", source[1]);
      Assert.AreEqual("bb", obj[0]);

      sourceChanged = false;
      objChanged = false;
      obj.Remove("bb");
      Assert.IsTrue(sourceChanged, "SourceChanged");
      Assert.IsTrue(objChanged, "objChanged");
      Assert.AreEqual(2, source.Count);
      Assert.AreEqual(0, obj.Count);
      Assert.AreEqual("a", source[0]);
      Assert.AreEqual("c", source[1]);
    }

    [TestMethod]
    public void ReplaceItem()
    {
      var source = new ObservableCollection<string>() { "a", "b", "c" };
      var query = from r in source
                  where r == "b"
                  select r;
      var obj = new LinqObservableCollection<string>(source, query);

      bool sourceChanged = false;
      bool objChanged = false;
      source.CollectionChanged += (_, _) =>
      {
        sourceChanged = true;
      };
      obj.CollectionChanged += (_, _) =>
      {
        objChanged = true;
      };
      source[0] = "z";
      Assert.IsTrue(sourceChanged, "SourceChanged");
      Assert.IsFalse(objChanged, "objChanged");
      Assert.AreEqual(3, source.Count);
      Assert.AreEqual(1, obj.Count);
      Assert.AreEqual("z", source[0]);

      source[1] = "x";
      Assert.IsTrue(sourceChanged, "SourceChanged");
      Assert.IsTrue(objChanged, "objChanged");
      Assert.AreEqual(3, source.Count);
      Assert.AreEqual(1, obj.Count);
      Assert.AreEqual("x", source[1]);
      Assert.AreEqual("x", obj[0]);

      obj[0] = "r";
      Assert.IsTrue(sourceChanged, "SourceChanged");
      Assert.IsTrue(objChanged, "objChanged");
      Assert.AreEqual(3, source.Count);
      Assert.AreEqual(1, obj.Count);
      Assert.AreEqual("r", source[1]);
      Assert.AreEqual("r", obj[0]);
    }

    [TestMethod]
    public void MoveItem()
    {
      var source = new ObservableCollection<string>() { "a", "b", "c" };
      var query = from r in source
                  where r == "b"
                  select r;
      var obj = new LinqObservableCollection<string>(source, query);

      bool sourceChanged = false;
      bool objChanged = false;
      source.CollectionChanged += (_, _) =>
      {
        sourceChanged = true;
      };
      obj.CollectionChanged += (_, _) =>
      {
        objChanged = true;
      };
      source.Move(1, 2);
      Assert.IsTrue(sourceChanged, "SourceChanged");
      Assert.IsFalse(objChanged, "objChanged");
      Assert.AreEqual(3, source.Count);
      Assert.AreEqual(1, obj.Count);
      Assert.AreEqual("b", obj[0]);
      Assert.AreEqual("b", source[2]);
      Assert.AreEqual("c", source[1]);
    }

    [TestMethod]
    public void ClearSource()
    {
      var source = new ObservableCollection<string>() { "a", "b", "c" };
      var query = from r in source
                  where r == "b"
                  select r;
      var obj = new LinqObservableCollection<string>(source, query);

      bool sourceChanged = false;
      bool objChanged = false;
      source.CollectionChanged += (_, _) =>
      {
        sourceChanged = true;
      };
      obj.CollectionChanged += (_, _) =>
      {
        objChanged = true;
      };
      source.Clear();
      Assert.IsTrue(sourceChanged, "SourceChanged");
      Assert.IsTrue(objChanged, "objChanged");
      Assert.AreEqual(0, source.Count);
      Assert.AreEqual(0, obj.Count);
    }

    [TestMethod]
    public void ClearView()
    {
      var source = new ObservableCollection<string>() { "a", "b", "c" };
      var query = from r in source
                  where r == "b"
                  select r;
      var obj = new LinqObservableCollection<string>(source, query);

      bool sourceChanged = false;
      bool objChanged = false;
      source.CollectionChanged += (_, _) =>
      {
        sourceChanged = true;
      };
      obj.CollectionChanged += (_, _) =>
      {
        objChanged = true;
      };
      obj.Clear();
      Assert.IsTrue(sourceChanged, "SourceChanged");
      Assert.IsTrue(objChanged, "objChanged");
      Assert.AreEqual(2, source.Count);
      Assert.AreEqual(0, obj.Count);
      Assert.AreEqual("a", source[0]);
      Assert.AreEqual("c", source[1]);
    }
  }

  [Serializable]
  public class TestList : BusinessListBase<TestList, TestItem>
  {
    public TestList()
    {
    }

    public static TestList NewTestList(IDataPortal<TestList> dataPortal)
    {
      return dataPortal.Create();
    }

    [Create]
    private void Create([Inject] IChildDataPortal<TestItem> childDataPortal)
    {
      Add(childDataPortal.FetchChild(123, "Zebra has stripes"));
      Add(childDataPortal.FetchChild(2233, "Software is neat"));
      Add(childDataPortal.FetchChild(453, "Run, the sky is falling"));
      Add(childDataPortal.FetchChild(12, "What is new?"));
    }
  }

  [Serializable]
  public class TestItem : BusinessBase<TestItem>
  {
    public static PropertyInfo<int> IdProperty = RegisterProperty<int>(c => c.Id);
    public int Id
    {
      get { return GetProperty(IdProperty); }
      set { SetProperty(IdProperty, value); }
    }

    public static PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    private void Child_Fetch(int id, string name)
    {
      using (BypassPropertyChecks)
      {
        Id = id;
        Name = name;
      }
    }
  }
}