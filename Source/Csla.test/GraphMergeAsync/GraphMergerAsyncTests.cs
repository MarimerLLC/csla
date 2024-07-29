//-----------------------------------------------------------------------
// <copyright file="GraphMergerAsyncTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

using Csla.Core;
using Csla.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Csla.Test.GraphMergeAsync
{
  [TestClass]
  public class GraphMergerAsyncTests
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

    [TestMethod]
    public async Task MergeInsert()
    {
      ApplicationContext applicationContext = _testDIContext.CreateTestApplicationContext();
      IDataPortal<Foo> dataPortal = _testDIContext.CreateDataPortal<Foo>();

      var obj = dataPortal.Create();
      obj.Name = "1";
      var cloned = obj.Clone();
      cloned.Name = "2";
      cloned.MockUpdated();

      var changed = false;
      obj.PropertyChanged += (_, _) => { changed = true; };

      var merger = new GraphMerger(applicationContext);
      await merger.MergeGraphAsync(obj, cloned);
      Assert.AreEqual(cloned.Name, obj.Name);
      Assert.AreEqual(cloned.IsDirty, obj.IsDirty);
      Assert.AreEqual(cloned.IsNew, obj.IsNew);
      Assert.IsFalse(obj.IsNew);
      Assert.IsFalse(obj.IsDirty);
      Assert.IsFalse(obj.IsValid);
      Assert.IsTrue(changed);
    }

    [TestMethod]
    public async Task MergeUpdate()
    {
      ApplicationContext applicationContext = _testDIContext.CreateTestApplicationContext();
      IDataPortal<Foo> dataPortal = _testDIContext.CreateDataPortal<Foo>();

      var obj = dataPortal.Create();
      obj.Name = "1";
      obj.MockUpdated();
      var cloned = obj.Clone();
      cloned.Name = "2";
      cloned.MockUpdated();

      var changed = false;
      obj.PropertyChanged += (_, _) => { changed = true; };

      var merger = new GraphMerger(applicationContext);
      await merger.MergeGraphAsync(obj, cloned);
      Assert.AreEqual(cloned.Name, obj.Name);
      Assert.AreEqual(cloned.IsDirty, obj.IsDirty);
      Assert.AreEqual(cloned.IsNew, obj.IsNew);
      Assert.IsFalse(obj.IsDirty);
      Assert.IsFalse(obj.IsValid);
      Assert.IsTrue(changed);
    }

    [TestMethod]
    public async Task MergeRuleUnbroken()
    {
      ApplicationContext applicationContext = _testDIContext.CreateTestApplicationContext();
      IDataPortal<Foo> dataPortal = _testDIContext.CreateDataPortal<Foo>();

      var obj = dataPortal.Create();
      obj.Name = "2";
      obj.MockUpdated();
      var cloned = obj.Clone();
      cloned.Name = "1";
      cloned.MockUpdated();

      var changed = false;
      obj.PropertyChanged += (_, _) => { changed = true; };

      var merger = new GraphMerger(applicationContext);
      await merger.MergeGraphAsync(obj, cloned);
      Assert.AreEqual(cloned.Name, obj.Name);
      Assert.AreEqual(cloned.IsDirty, obj.IsDirty);
      Assert.AreEqual(cloned.IsNew, obj.IsNew);
      Assert.IsFalse(obj.IsDirty);
      Assert.IsTrue(obj.IsValid);
      Assert.IsTrue(changed);
    }

    [TestMethod]
    public async Task MergeDelete()
    {
      ApplicationContext applicationContext = _testDIContext.CreateTestApplicationContext();
      IDataPortal<Foo> dataPortal = _testDIContext.CreateDataPortal<Foo>();

      var obj = dataPortal.Create();
      obj.Name = "1";
      obj.MockUpdated(); // make it an old object
      obj.MarkForDelete(); // mark for deletion
      var cloned = obj.Clone();
      cloned.Name = "2";
      cloned.MockDeleted(); // mock DP delete result

      var changed = false;
      obj.PropertyChanged += (_, _) => { changed = true; };

      var merger = new GraphMerger(applicationContext);
      await merger.MergeGraphAsync(obj, cloned);
      Assert.AreEqual(cloned.Name, obj.Name);
      Assert.IsTrue(obj.IsNew, "obj.IsTrue");
      Assert.IsFalse(obj.IsDeleted, "obj.IsDeleted");
      Assert.AreEqual(cloned.IsDeleted, obj.IsDeleted, "IsDeleted equal");
      Assert.AreEqual(cloned.IsDirty, obj.IsDirty);
      Assert.AreEqual(cloned.IsNew, obj.IsNew);
      Assert.IsFalse(obj.IsValid);
      Assert.IsTrue(changed);
    }

    [TestMethod]
    public async Task MergeChildInsert()
    {
      ApplicationContext applicationContext = _testDIContext.CreateTestApplicationContext();
      IDataPortal<Foo> dataPortal = _testDIContext.CreateDataPortal<Foo>();

      var obj = dataPortal.Create();
      obj.Name = "1";
      obj.AddChild(dataPortal);
      obj.Child.Name = "42";
      var cloned = obj.Clone();
      cloned.Name = "2";
      cloned.Child.Name = "43";
      cloned.MockUpdated();

      var changed = false;
      obj.PropertyChanged += (_, _) => { changed = true; };

      var merger = new GraphMerger(applicationContext);
      await merger.MergeGraphAsync(obj, cloned);
      Assert.IsFalse(ReferenceEquals(obj.Child, cloned.Child), "ref");
      Assert.AreEqual(cloned.Child.Name, obj.Child.Name, "name");
      Assert.AreEqual(cloned.Child.IsDirty, obj.Child.IsDirty, "isdirty");
      Assert.AreEqual(cloned.Child.IsNew, obj.Child.IsNew, "isnew");
      Assert.IsFalse(obj.Child.IsNew, "isnew false");
      Assert.IsFalse(obj.Child.IsDirty, "isdirty false");
      Assert.IsTrue(obj.Child.IsValid, "isvalid true");
      Assert.IsTrue(changed, "changed");
    }

    [TestMethod]
    public async Task MergeChildUpdate()
    {
      ApplicationContext applicationContext = _testDIContext.CreateTestApplicationContext();
      IDataPortal<Foo> dataPortal = _testDIContext.CreateDataPortal<Foo>();

      var obj = dataPortal.Create();
      obj.Name = "1";
      obj.AddChild(dataPortal);
      obj.Child.Name = "42";
      obj.MockUpdated();
      var cloned = obj.Clone();
      cloned.Name = "2";
      cloned.Child.Name = "2";
      cloned.MockUpdated();

      var changed = false;
      obj.PropertyChanged += (_, _) => { changed = true; };

      var merger = new GraphMerger(applicationContext);
      await merger.MergeGraphAsync(obj, cloned);
      Assert.IsFalse(ReferenceEquals(obj.Child, cloned.Child), "ref");
      Assert.AreEqual(cloned.Child.Name, obj.Child.Name, "name");
      Assert.AreEqual(cloned.Child.IsDirty, obj.Child.IsDirty, "isdirty");
      Assert.AreEqual(cloned.Child.IsNew, obj.Child.IsNew, "isnew");
      Assert.IsFalse(obj.Child.IsNew, "isnew false");
      Assert.IsFalse(obj.Child.IsDirty, "isdirty false");
      Assert.IsFalse(obj.Child.IsValid, "isvalid false");
      Assert.IsTrue(changed, "changed");
    }

    [TestMethod]
    public async Task MergeNewChildUpdate()
    {
      ApplicationContext applicationContext = _testDIContext.CreateTestApplicationContext();
      IDataPortal<Foo> dataPortal = _testDIContext.CreateDataPortal<Foo>();

      var obj = dataPortal.Create();
      obj.Name = "1";
      obj.MockUpdated();
      var cloned = obj.Clone();
      cloned.AddChild(dataPortal);
      cloned.Child.Name = "42";
      cloned.MockUpdated();

      await new GraphMerger(applicationContext).MergeGraphAsync(obj, cloned);

      Assert.IsTrue(ReferenceEquals(obj.Child, cloned.Child), "ref");
      Assert.IsTrue(ReferenceEquals(obj, obj.Child.Parent));
    }

    [TestMethod]
    public async Task MergeChildDelete()
    {
      ApplicationContext applicationContext = _testDIContext.CreateTestApplicationContext();
      IDataPortal<Foo> dataPortal = _testDIContext.CreateDataPortal<Foo>();

      var obj = dataPortal.Create();
      obj.Name = "1";
      obj.AddChild(dataPortal);
      obj.Child.Name = "42";
      obj.MockUpdated();
      obj.MarkForDelete();
      var cloned = obj.Clone();
      cloned.Name = "2";
      cloned.Child.Name = "43";
      cloned.MockDeleted();

      var changed = false;
      obj.PropertyChanged += (_, _) => { changed = true; };

      var merger = new GraphMerger(applicationContext);
      await merger.MergeGraphAsync(obj, cloned);
      Assert.IsFalse(ReferenceEquals(obj.Child, cloned.Child), "ref");
      Assert.AreEqual(cloned.Child.Name, obj.Child.Name, "name");
      Assert.AreEqual(cloned.Child.IsDirty, obj.Child.IsDirty, "isdirty");
      Assert.AreEqual(cloned.Child.IsNew, obj.Child.IsNew, "isnew");
      Assert.IsTrue(obj.Child.IsNew, "isnew true");
      Assert.IsTrue(obj.Child.IsDirty, "isdirty true");
      Assert.IsTrue(obj.Child.IsValid, "isvalid true");
      Assert.IsTrue(changed, "changed");
    }

    [TestMethod]

    public async Task MergeList()
    {
      ApplicationContext applicationContext = _testDIContext.CreateTestApplicationContext();
      IDataPortal<FooList> dataPortal = _testDIContext.CreateDataPortal<FooList>();

      var obj = dataPortal.Create();
      obj.AddNew().Name = "existing";
      obj[0].MockUpdated();
      obj.AddNew().Name = "to be deleted";
      obj.Remove(obj[1]);
      obj.AddNew().Name = "new";
      obj.AddNew().Name = "remove in clone";
      Assert.AreEqual(3, obj.Count, "preclone count");
      var cloned = obj.Clone();
      Assert.AreEqual(3, cloned.Count, "postclone count");
      cloned.Remove(cloned.Where(_ => _.Name == "remove in clone").First());
      Assert.AreEqual(2, cloned.Count, "postclone count after removed obj");
      Assert.AreEqual(cloned[0].Name, obj[0].Name, "postclone [0]");
      Assert.AreEqual(cloned[1].Name, obj[1].Name, "postclone [1]");
      cloned[0].Name = "existing (cloned)";
      cloned[1].Name = "new (cloned)";
      cloned.AddNew().Name = "new in clone";
      cloned.MockUpdated();

      var merger = new GraphMerger(applicationContext);
      await merger.MergeBusinessListGraphAsync<FooList, Foo>(obj, cloned);
      Assert.AreEqual(cloned.Count, obj.Count, "count");
      Assert.AreEqual(3, obj.Count, "count 3");
      Assert.AreEqual(cloned[0].Name, obj[0].Name, "[0]");
      Assert.AreEqual(cloned[1].Name, obj[1].Name, "[1]");
      Assert.AreEqual(cloned[2].Name, obj[2].Name, "[2]");
      Assert.AreEqual(cloned.IsDirty, obj.IsDirty);
      Assert.IsFalse(obj.IsDirty);
    }

    [TestMethod]
    public async Task MergeListNewChild()
    {
      ApplicationContext applicationContext = _testDIContext.CreateTestApplicationContext();
      IDataPortal<FooList> dataPortal = _testDIContext.CreateDataPortal<FooList>();

      var obj = dataPortal.Create();
      var original = obj;
      var newChild = obj.AddNew();
      newChild.Name = "new";

      var bo = (IBusinessObject)newChild;
      Assert.IsTrue(bo.Identity >= 0, "bo needs identity");

      var saved = obj.Save();
      Assert.AreEqual(((IBusinessObject)newChild).Identity, ((IBusinessObject)saved[0]).Identity, "identity should survive save");

      Assert.IsTrue(!ReferenceEquals(obj[0], saved[0]), "saved object is not original");

      await new GraphMerger(applicationContext).MergeBusinessListGraphAsync<FooList, Foo>(obj, saved);

      Assert.AreEqual(((IBusinessObject)newChild).Identity, ((IBusinessObject)obj[0]).Identity);
      Assert.AreEqual(((IBusinessObject)newChild).Identity, ((IBusinessObject)saved[0]).Identity);
      Assert.AreEqual(((IBusinessObject)obj[0]).Identity, ((IBusinessObject)saved[0]).Identity);

      Assert.AreEqual(obj[0].IsNew, saved[0].IsNew);

      Assert.IsFalse(ReferenceEquals(original, saved));
      Assert.IsTrue(ReferenceEquals(original, obj));

      Assert.IsTrue(ReferenceEquals(original[0], obj[0]));
      Assert.IsTrue(ReferenceEquals(newChild, obj[0]));

      obj[0].Name = "changed";
      Assert.AreEqual(original[0].Name, obj[0].Name);
    }
  }
}
