//-----------------------------------------------------------------------
// <copyright file="BasicTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#endif 

namespace Csla.Test.Basic
{
  [TestClass]
  public class BasicTests
  {
    [TestMethod]
    public void TestNotUndoableField()
    {
#pragma warning disable CS0618 // Type or member is obsolete
      Csla.ApplicationContext.GlobalContext.Clear();
#pragma warning restore CS0618 // Type or member is obsolete
      Csla.Test.DataBinding.ParentEntity p = Csla.Test.DataBinding.ParentEntity.NewParentEntity();

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
      //ReadOnlyList list = ReadOnlyList.GetReadOnlyList();
      // Assert.AreEqual("Fetched", Csla.ApplicationContext.GlobalContext["ReadOnlyList"]);
    }

    [TestMethod]
    public void TestNameValueList()
    {
      NameValueListObj nvList = NameValueListObj.GetNameValueListObj();
#pragma warning disable CS0618 // Type or member is obsolete
      Assert.AreEqual("Fetched", Csla.ApplicationContext.GlobalContext["NameValueListObj"]);
#pragma warning restore CS0618 // Type or member is obsolete

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
#pragma warning disable CS0618 // Type or member is obsolete
      Csla.ApplicationContext.GlobalContext.Clear();
#pragma warning restore CS0618 // Type or member is obsolete
      CommandObject obj = new CommandObject();
      Assert.AreEqual("Executed", obj.ExecuteServerCode().AProperty);
    }

    [TestMethod]
    public void CreateGenRoot()
    {
#pragma warning disable CS0618 // Type or member is obsolete
      Csla.ApplicationContext.GlobalContext.Clear();
#pragma warning restore CS0618 // Type or member is obsolete
      GenRoot root;
      root = GenRoot.NewRoot();
      Assert.IsNotNull(root);
      Assert.AreEqual("<new>", root.Data);
#pragma warning disable CS0618 // Type or member is obsolete
      Assert.AreEqual("Created", Csla.ApplicationContext.GlobalContext["GenRoot"]);
#pragma warning restore CS0618 // Type or member is obsolete
      Assert.AreEqual(true, root.IsNew);
      Assert.AreEqual(false, root.IsDeleted);
      Assert.AreEqual(true, root.IsDirty);
    }

    [TestMethod]
    public void InheritanceUndo()
    {
#pragma warning disable CS0618 // Type or member is obsolete
      Csla.ApplicationContext.GlobalContext.Clear();
#pragma warning restore CS0618 // Type or member is obsolete
      GenRoot root;
      root = GenRoot.NewRoot();
      root.BeginEdit();
      root.Data = "abc";
      root.CancelEdit();

#pragma warning disable CS0618 // Type or member is obsolete
      Csla.ApplicationContext.GlobalContext.Clear();
#pragma warning restore CS0618 // Type or member is obsolete
      root = GenRoot.NewRoot();
      root.BeginEdit();
      root.Data = "abc";
      root.ApplyEdit();
    }

    [TestMethod]
    public void CreateRoot()
    {
#pragma warning disable CS0618 // Type or member is obsolete
      Csla.ApplicationContext.GlobalContext.Clear();
#pragma warning restore CS0618 // Type or member is obsolete
      Root root;
      root = Csla.Test.Basic.Root.NewRoot();
      Assert.IsNotNull(root);
      Assert.AreEqual("<new>", root.Data);
#pragma warning disable CS0618 // Type or member is obsolete
      Assert.AreEqual("Created", Csla.ApplicationContext.GlobalContext["Root"]);
#pragma warning restore CS0618 // Type or member is obsolete
      Assert.AreEqual(true, root.IsNew);
      Assert.AreEqual(false, root.IsDeleted);
      Assert.AreEqual(true, root.IsDirty);
    }

    [TestMethod]
    public void AddChild()
    {
#pragma warning disable CS0618 // Type or member is obsolete
      Csla.ApplicationContext.GlobalContext.Clear();
#pragma warning restore CS0618 // Type or member is obsolete
      Root root = Csla.Test.Basic.Root.NewRoot();
      root.Children.Add("1");
      Assert.AreEqual(1, root.Children.Count);
      Assert.AreEqual("1", root.Children[0].Data);
    }

    [TestMethod]
    public void AddRemoveChild()
    {
#pragma warning disable CS0618 // Type or member is obsolete
      Csla.ApplicationContext.GlobalContext.Clear();
#pragma warning restore CS0618 // Type or member is obsolete
      Root root = Csla.Test.Basic.Root.NewRoot();
      root.Children.Add("1");
      root.Children.Remove(root.Children[0]);
      Assert.AreEqual(0, root.Children.Count);
    }

    [TestMethod]
    public void AddRemoveAddChild()
    {
#pragma warning disable CS0618 // Type or member is obsolete
      Csla.ApplicationContext.GlobalContext.Clear();
#pragma warning restore CS0618 // Type or member is obsolete
      Root root = Csla.Test.Basic.Root.NewRoot();
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
#pragma warning disable CS0618 // Type or member is obsolete
      Csla.ApplicationContext.GlobalContext.Clear();
#pragma warning restore CS0618 // Type or member is obsolete
      Root root = Csla.Test.Basic.Root.NewRoot();
      root.Children.Add("1");
      Child child = root.Children[0];
      child.GrandChildren.Add("1");
      Assert.AreEqual(1, child.GrandChildren.Count);
      Assert.AreEqual("1", child.GrandChildren[0].Data);
    }

    [TestMethod]
    public void AddRemoveGrandChild()
    {
#pragma warning disable CS0618 // Type or member is obsolete
      Csla.ApplicationContext.GlobalContext.Clear();
#pragma warning restore CS0618 // Type or member is obsolete
      Root root = Csla.Test.Basic.Root.NewRoot();
      root.Children.Add("1");
      Child child = root.Children[0];
      child.GrandChildren.Add("1");
      child.GrandChildren.Remove(child.GrandChildren[0]);
      Assert.AreEqual(0, child.GrandChildren.Count);
    }

    [TestMethod]
    public void ClearChildList()
    {
#pragma warning disable CS0618 // Type or member is obsolete
      Csla.ApplicationContext.GlobalContext.Clear();
#pragma warning restore CS0618 // Type or member is obsolete
      Root root = Csla.Test.Basic.Root.NewRoot();
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
#pragma warning disable CS0618 // Type or member is obsolete
      Csla.ApplicationContext.GlobalContext.Clear();
#pragma warning restore CS0618 // Type or member is obsolete
      Root root = Csla.Test.Basic.Root.NewRoot();
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
#pragma warning disable CS0618 // Type or member is obsolete
      Csla.ApplicationContext.GlobalContext.Clear();
#pragma warning restore CS0618 // Type or member is obsolete
      Root root = Csla.Test.Basic.Root.NewRoot();
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
#pragma warning disable CS0618 // Type or member is obsolete
      Csla.ApplicationContext.GlobalContext.Clear();
#pragma warning restore CS0618 // Type or member is obsolete
      Root r1 = Root.NewRoot();
      r1.Data = "abc";
      Assert.AreEqual(true, r1.Equals(r1), "objects should be equal on instance compare");
      Assert.AreEqual(true, Equals(r1, r1), "objects should be equal on static compare");

#pragma warning disable CS0618 // Type or member is obsolete
      Csla.ApplicationContext.GlobalContext.Clear();
#pragma warning restore CS0618 // Type or member is obsolete
      Root r2 = Root.NewRoot();
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
#pragma warning disable CS0618 // Type or member is obsolete
      Csla.ApplicationContext.GlobalContext.Clear();
#pragma warning restore CS0618 // Type or member is obsolete
      Root root = Root.NewRoot();
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
#pragma warning disable CS0618 // Type or member is obsolete
      Csla.ApplicationContext.GlobalContext.Clear();
#pragma warning restore CS0618 // Type or member is obsolete
      Root root = Csla.Test.Basic.Root.NewRoot();
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
#pragma warning disable CS0618 // Type or member is obsolete
      Csla.ApplicationContext.GlobalContext.Clear();
#pragma warning restore CS0618 // Type or member is obsolete
      Root root = Csla.Test.Basic.Root.NewRoot();
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
      bool changed = false;
      var obj = new RootList();
      obj.ListChanged += (o, e) =>
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
      var list = await Csla.DataPortal.CreateAsync<RootList>();
      list.BeginEdit();
      list.AddNew();
      var clone = (RootList)((ICloneable)list).Clone();
      clone.ApplyEdit();
    }

    [TestMethod]
    public async Task ChildEditLevelDeleteClone()
    {
      var list = await Csla.DataPortal.CreateAsync<RootList>();
      list.BeginEdit();
      list.AddNew();
      list.RemoveAt(0);
      var clone = (RootList)((ICloneable)list).Clone();
      clone.ApplyEdit();
    }

    [TestMethod]
    public async Task UndoStateStack()
    {
      var obj = await Csla.DataPortal.CreateAsync<Root>(new Root.Criteria(""));
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
#pragma warning disable CS0618 // Type or member is obsolete
      Csla.ApplicationContext.GlobalContext.Clear();
#pragma warning restore CS0618 // Type or member is obsolete
    }
  }

  public class FormSimulator
  {
    private readonly Core.BusinessBase _obj;

    public FormSimulator(Core.BusinessBase obj)
    {
      this._obj.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Obj_IsDirtyChanged);
      this._obj = obj;
    }

    private void Obj_IsDirtyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    { }
  }

  [Serializable()]
  public class SerializableListener
  {
    private readonly Core.BusinessBase _obj;

    public SerializableListener(Core.BusinessBase obj)
    {
      this._obj.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Obj_IsDirtyChanged);
      this._obj = obj;
    }

    public void Obj_IsDirtyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    { }
  }
}