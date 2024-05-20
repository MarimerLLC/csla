//-----------------------------------------------------------------------
// <copyright file="BasicTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

using Csla.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Csla.Test.Basic
{
  [TestClass]
  public class BasicTests
  {
    private static TestDIContext _testDIContext;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
      _testDIContext = TestDIContextFactory.CreateDefaultContext();
    }

    [TestMethod]
    public void TestNotUndoableField()
    {
      TestResults.Reinitialise();

      DataBinding.ParentEntity p = CreateParentEntityInstance();

      p.NotUndoable = "something";
      p.Data = "data";
      p.BeginEdit();
      p.NotUndoable = "something else";
      p.Data = "new data";
      p.CancelEdit();
      //NotUndoable property points to a private field marked with [NotUndoable()]
      //so its state is never copied when BeginEdit() is called
      Assert.AreEqual("something else", p.NotUndoable);
      //the Data property points to a field that is undoable, so it reverts
      Assert.AreEqual("data", p.Data);
    }

    [TestMethod]
    public void TestReadOnlyList()
    {
      TestResults.Reinitialise();

      //ReadOnlyList list = ReadOnlyList.GetReadOnlyList();
      //Assert.AreEqual("Fetched", TestResults.GetResult("ReadOnlyList"));
    }

    [TestMethod]
    public void TestNameValueList()
    {
      TestResults.Reinitialise();

      NameValueListObj nvList = GetNameValueListObjInstance();
      Assert.AreEqual("Fetched", TestResults.GetResult("NameValueListObj"));

      Assert.AreEqual("element_1", nvList[1].Value);

      //won't work, because IsReadOnly is set to true after object is populated in the for
      //loop in DataPortal_Fetch
      //NameValueListObj.NameValuePair newPair = new NameValueListObj.NameValuePair(45, "something");

      //nvList.Add(newPair);

      //Assert.AreEqual("something", nvList[45].Value);
    }

    [TestMethod]
    public void TestCommandBase()
    {
      TestResults.Reinitialise();

      IDataPortal<CommandObject> dataPortal = _testDIContext.CreateDataPortal<CommandObject>();
      CommandObject obj = dataPortal.Create();
      obj = dataPortal.Execute(obj);
      Assert.AreEqual("Executed", obj.AProperty);
    }

    [TestMethod]
    public void CreateGenRoot()
    {
      TestResults.Reinitialise();

      GenRoot root;
      root = CreateGenRootInstance();
      Assert.IsNotNull(root);
      Assert.AreEqual("<new>", root.Data);
      Assert.AreEqual("Created", TestResults.GetResult("GenRoot"));
      Assert.AreEqual(true, root.IsNew);
      Assert.AreEqual(false, root.IsDeleted);
      Assert.AreEqual(true, root.IsDirty);
    }

    [TestMethod]
    public void InheritanceUndo()
    {
      TestResults.Reinitialise();

      GenRoot root;
      root = CreateGenRootInstance();
      root.BeginEdit();
      root.Data = "abc";
      root.CancelEdit();

      TestResults.Reinitialise();
      root = CreateGenRootInstance();
      root.BeginEdit();
      root.Data = "abc";
      root.ApplyEdit();
    }

    [TestMethod]
    public void CreateRoot()
    {
      TestResults.Reinitialise();

      Root root;
      root = CreateRootInstance();
      Assert.IsNotNull(root);
      Assert.AreEqual("<new>", root.Data);
      Assert.AreEqual("Created", TestResults.GetResult("Root"));
      Assert.AreEqual(true, root.IsNew);
      Assert.AreEqual(false, root.IsDeleted);
      Assert.AreEqual(true, root.IsDirty);
    }

    [TestMethod]
    public void AddChild()
    {
      TestResults.Reinitialise();

      Root root = CreateRootInstance();
      root.Children.Add("1");
      Assert.AreEqual(1, root.Children.Count);
      Assert.AreEqual("1", root.Children[0].Data);
    }

    [TestMethod]
    public void AddRemoveChild()
    {
      TestResults.Reinitialise();

      Root root = CreateRootInstance();
      root.Children.Add("1");
      root.Children.Remove(root.Children[0]);
      Assert.AreEqual(0, root.Children.Count);
    }

    [TestMethod]
    public void AddRemoveAddChild()
    {
      TestResults.Reinitialise();

      Root root = CreateRootInstance();
      root.Children.Add("1");
      root.BeginEdit();
      root.Children.Remove(root.Children[0]);

      root.Children.Add("2");
      root.CancelEdit();

      Assert.AreEqual(1, root.Children.Count);
      Assert.AreEqual("1", root.Children[0].Data);
    }

    [TestMethod]
    public void AddGrandChild()
    {
      TestResults.Reinitialise();

      Root root = CreateRootInstance();
      root.Children.Add("1");
      Child child = root.Children[0];
      child.GrandChildren.Add("1");
      Assert.AreEqual(1, child.GrandChildren.Count);
      Assert.AreEqual("1", child.GrandChildren[0].Data);
    }

    [TestMethod]
    public void AddRemoveGrandChild()
    {
      TestResults.Reinitialise();

      Root root = CreateRootInstance();
      root.Children.Add("1");
      Child child = root.Children[0];
      child.GrandChildren.Add("1");
      child.GrandChildren.Remove(child.GrandChildren[0]);
      Assert.AreEqual(0, child.GrandChildren.Count);
    }

    [TestMethod]
    public void ClearChildList()
    {
      TestResults.Reinitialise();

      Root root = CreateRootInstance();
      root.Children.Add("A");
      root.Children.Add("B");
      root.Children.Add("C");
      root.Children.Clear();
      Assert.AreEqual(0, root.Children.Count, "Count should be 0");
      Assert.AreEqual(3, root.Children.DeletedCount, "Deleted count should be 3");
    }

    [TestMethod]
    public void NestedAddAcceptchild()
    {
      TestResults.Reinitialise();

      Root root = CreateRootInstance();
      root.BeginEdit();
      root.Children.Add("A");
      root.BeginEdit();
      root.Children.Add("B");
      root.BeginEdit();
      root.Children.Add("C");
      root.ApplyEdit();
      root.ApplyEdit();
      root.ApplyEdit();
      Assert.AreEqual(3, root.Children.Count);
    }

    [TestMethod]
    public void NestedAddDeleteAcceptChild()
    {
      TestResults.Reinitialise();

      Root root = CreateRootInstance();
      root.BeginEdit();
      root.Children.Add("A");
      root.BeginEdit();
      root.Children.Add("B");
      root.BeginEdit();
      root.Children.Add("C");
      Child childC = root.Children[2];
      Assert.AreEqual(true, root.Children.Contains(childC), "Child should be in collection");
      root.Children.Remove(root.Children[0]);
      root.Children.Remove(root.Children[0]);
      root.Children.Remove(root.Children[0]);
      Assert.AreEqual(false, root.Children.Contains(childC), "Child should not be in collection");
      Assert.AreEqual(true, root.Children.ContainsDeleted(childC), "Deleted child should be in deleted collection");
      root.ApplyEdit();
      Assert.AreEqual(false, root.Children.ContainsDeleted(childC), "Deleted child should not be in deleted collection after first applyedit");
      root.ApplyEdit();
      Assert.AreEqual(false, root.Children.ContainsDeleted(childC), "Deleted child should not be in deleted collection after ApplyEdit");
      root.ApplyEdit();
      Assert.AreEqual(0, root.Children.Count, "No children should remain");
      Assert.AreEqual(false, root.Children.ContainsDeleted(childC), "Deleted child should not be in deleted collection after third applyedit");
    }

    [TestMethod]
    public void BasicEquality()
    {
      TestResults.Reinitialise();

      Root r1 = CreateRootInstance();
      r1.Data = "abc";
      Assert.AreEqual(true, r1.Equals(r1), "objects should be equal on instance compare");
      Assert.AreEqual(true, Equals(r1, r1), "objects should be equal on static compare");

      TestResults.Reinitialise();
      Root r2 = CreateRootInstance();
      r2.Data = "xyz";
      Assert.AreEqual(false, r1.Equals(r2), "objects should not be equal");
      Assert.AreEqual(false, Equals(r1, r2), "objects should not be equal");

      Assert.AreEqual(false, r1.Equals(null), "Objects should not be equal");
      Assert.AreEqual(false, Equals(r1, null), "Objects should not be equal");
      Assert.AreEqual(false, Equals(null, r2), "Objects should not be equal");
    }

    [TestMethod]
    public void ChildEquality()
    {
      TestResults.Reinitialise();

      Root root = CreateRootInstance();
      root.Children.Add("abc");
      root.Children.Add("xyz");
      root.Children.Add("123");
      Child c1 = root.Children[0];
      Child c2 = root.Children[1];
      Child c3 = root.Children[2];
      root.Children.Remove(c3);

      Assert.AreEqual(true, c1.Equals(c1), "objects should be equal");
      Assert.AreEqual(true, Equals(c1, c1), "objects should be equal");

      Assert.AreEqual(false, c1.Equals(c2), "objects should not be equal");
      Assert.AreEqual(false, Equals(c1, c2), "objects should not be equal");

      Assert.AreEqual(false, c1.Equals(null), "objects should not be equal");
      Assert.AreEqual(false, Equals(c1, null), "objects should not be equal");
      Assert.AreEqual(false, Equals(null, c2), "objects should not be equal");

      Assert.AreEqual(true, root.Children.Contains(c1), "Collection should contain c1");
      Assert.AreEqual(true, root.Children.Contains(c2), "collection should contain c2");
      Assert.AreEqual(false, root.Children.Contains(c3), "collection should not contain c3");
      Assert.AreEqual(true, root.Children.ContainsDeleted(c3), "Deleted collection should contain c3");
    }

    [TestMethod]
    public void DeletedListTest()
    {
      TestResults.Reinitialise();

      Root root = CreateRootInstance();
      root.Children.Add("1");
      root.Children.Add("2");
      root.Children.Add("3");
      root.BeginEdit();
      root.Children.Remove(root.Children[0]);
      root.Children.Remove(root.Children[0]);
      root.ApplyEdit();

      Root copy = root.Clone();

      var deleted = copy.Children.GetDeletedList();

      Assert.AreEqual(2, deleted.Count);
      Assert.AreEqual("1", deleted[0].Data);
      Assert.AreEqual("2", deleted[1].Data);
      Assert.AreEqual(1, root.Children.Count);
    }

    [TestMethod]
    public void DeletedListTestWithCancel()
    {
      TestResults.Reinitialise();

      Root root = CreateRootInstance();
      root.Children.Add("1");
      root.Children.Add("2");
      root.Children.Add("3");
      root.BeginEdit();
      root.Children.Remove(root.Children[0]);
      root.Children.Remove(root.Children[0]);

      Root copy = root.Clone();

      var deleted = copy.Children.GetDeletedList();

      Assert.AreEqual(2, deleted.Count);
      Assert.AreEqual("1", deleted[0].Data);
      Assert.AreEqual("2", deleted[1].Data);
      Assert.AreEqual(1, root.Children.Count);

      root.CancelEdit();

      deleted = root.Children.GetDeletedList();

      Assert.AreEqual(0, deleted.Count);
      Assert.AreEqual(3, root.Children.Count);

    }

    [TestMethod]
    public void SuppressListChangedEventsDoNotRaiseCollectionChanged()
    {
      TestResults.Reinitialise();
      
      bool changed = false;
      var obj = new RootList();
      obj.ListChanged += (_, _) =>
      {
        changed = true;
      };
      var child = new RootListChild(); // object is marked as child

      Assert.IsTrue(obj.RaiseListChangedEvents);
      using (obj.SuppressListChangedEvents)
      {
        Assert.IsFalse(obj.RaiseListChangedEvents);

        obj.Add(child);
      }
      Assert.IsFalse(changed, "Should not raise ListChanged event");
      Assert.IsTrue(obj.RaiseListChangedEvents);
      Assert.AreEqual(child, obj[0]);
    }

    [TestMethod]
    public async Task ChildEditLevelClone()
    {
      TestResults.Reinitialise();

      var list = await CreateRootListInstanceAsync();
      list.BeginEdit();
      list.AddNew();
      var clone = (RootList)((ICloneable)list).Clone();
      clone.ApplyEdit();
    }

    [TestMethod]
    public async Task ChildEditLevelDeleteClone()
    {
      TestResults.Reinitialise();
      
      var list = await CreateRootListInstanceAsync();
      list.BeginEdit();
      list.AddNew();
      list.RemoveAt(0);
      var clone = (RootList)((ICloneable)list).Clone();
      clone.ApplyEdit();
    }

    [TestMethod]
    public async Task UndoStateStack()
    {
      TestResults.Reinitialise();

      var obj = await CreateRootInstanceAsync(new Root.Criteria(""));
      Assert.AreEqual("", obj.Data);
      obj.BeginEdit();
      obj.Data = "1";
      obj.BeginEdit();
      obj.Data = "2";
      Assert.AreEqual("2", obj.Data);
      obj.CancelEdit();
      Assert.AreEqual("1", obj.Data);
      obj.BeginEdit();
      obj.Data = "2";
      Assert.AreEqual(2, obj.GetEditLevel());
      var clone = obj.Clone();
      Assert.AreEqual(2, clone.GetEditLevel());
      Assert.AreEqual("2", clone.Data);
      clone.CancelEdit();
      Assert.AreEqual("1", clone.Data);
      clone.CancelEdit();
      Assert.AreEqual("", clone.Data);
    }

    [TestCleanup]
    public void ClearContextsAfterEachTest()
    {
      TestResults.Reinitialise();
    }

    private Root CreateRootInstance()
    {
      IDataPortal<Root> dataPortal = _testDIContext.CreateDataPortal<Root>();
      return dataPortal.Create(new Root.Criteria());
    }

    private async Task<Root> CreateRootInstanceAsync()
    {
      IDataPortal<Root> dataPortal = _testDIContext.CreateDataPortal<Root>();
      return await dataPortal.CreateAsync(new Root.Criteria());
    }

    private async Task<Root> CreateRootInstanceAsync(Root.Criteria criteria)
    {
      IDataPortal<Root> dataPortal = _testDIContext.CreateDataPortal<Root>();
      return await dataPortal.CreateAsync(criteria);
    }

    private async Task<RootList> CreateRootListInstanceAsync()
    {
      IDataPortal<RootList> dataPortal = _testDIContext.CreateDataPortal<RootList>();
      return await dataPortal.CreateAsync();
    }

    private GenRoot CreateGenRootInstance()
    {
      IDataPortal<GenRoot> dataPortal = _testDIContext.CreateDataPortal<GenRoot>();
      return dataPortal.Create(new GenRootBase.Criteria());
    }

    private DataBinding.ParentEntity CreateParentEntityInstance()
    {
      IDataPortal<DataBinding.ParentEntity> dataPortal = _testDIContext.CreateDataPortal<DataBinding.ParentEntity>();
      return dataPortal.Create();
    }

    private NameValueListObj GetNameValueListObjInstance()
    {
      IDataPortal<NameValueListObj> dataPortal = _testDIContext.CreateDataPortal<NameValueListObj>();
      return dataPortal.Fetch();
    }

  }

  public class FormSimulator
  {
    private readonly Core.BusinessBase _obj;

    public FormSimulator(Core.BusinessBase obj)
    {
      _obj.PropertyChanged += Obj_IsDirtyChanged;
      _obj = obj;
    }

    private void Obj_IsDirtyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    { }
  }

  [Serializable]
  public class SerializableListener
  {
    private readonly Core.BusinessBase _obj;

    public SerializableListener(Core.BusinessBase obj)
    {
      _obj.PropertyChanged += Obj_IsDirtyChanged;
      _obj = obj;
    }

    public void Obj_IsDirtyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    { }
  }
}