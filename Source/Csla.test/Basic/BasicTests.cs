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
            Csla.ApplicationContext.GlobalContext.Clear();
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
            Assert.AreEqual("Fetched", Csla.ApplicationContext.GlobalContext["NameValueListObj"]);

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
            Csla.ApplicationContext.GlobalContext.Clear();
            CommandObject obj = new CommandObject();
            Assert.AreEqual("Executed", obj.ExecuteServerCode().AProperty);
        }

        [TestMethod]
        public void CreateGenRoot()
        {
            Csla.ApplicationContext.GlobalContext.Clear();
            GenRoot root;
            root = GenRoot.NewRoot();
            Assert.IsNotNull(root);
            Assert.AreEqual("<new>", root.Data);
            Assert.AreEqual("Created", Csla.ApplicationContext.GlobalContext["GenRoot"]);
            Assert.AreEqual(true, root.IsNew);
            Assert.AreEqual(false, root.IsDeleted);
            Assert.AreEqual(true, root.IsDirty);
        }

        [TestMethod]
        public void InheritanceUndo()
        {
          Csla.ApplicationContext.GlobalContext.Clear();
          GenRoot root;
          root = GenRoot.NewRoot();
          root.BeginEdit();
          root.Data = "abc";
          root.CancelEdit();

          Csla.ApplicationContext.GlobalContext.Clear();
          root = GenRoot.NewRoot();
          root.BeginEdit();
          root.Data = "abc";
          root.ApplyEdit();
        }

        [TestMethod]
        public void CreateRoot()
        {
            Csla.ApplicationContext.GlobalContext.Clear();
            Root root;
            root = Csla.Test.Basic.Root.NewRoot();
            Assert.IsNotNull(root);
            Assert.AreEqual("<new>", root.Data);
            Assert.AreEqual("Created", Csla.ApplicationContext.GlobalContext["Root"]);
            Assert.AreEqual(true, root.IsNew);
            Assert.AreEqual(false, root.IsDeleted);
            Assert.AreEqual(true, root.IsDirty);
        }

        [TestMethod]
        public void AddChild()
        {
            Csla.ApplicationContext.GlobalContext.Clear();
            Root root = Csla.Test.Basic.Root.NewRoot();
            root.Children.Add("1");
            Assert.AreEqual(1, root.Children.Count);
            Assert.AreEqual("1", root.Children[0].Data);
        }

        [TestMethod]
        public void AddRemoveChild()
        {
            Csla.ApplicationContext.GlobalContext.Clear();
            Root root = Csla.Test.Basic.Root.NewRoot();
            root.Children.Add("1");
            root.Children.Remove(root.Children[0]);
            Assert.AreEqual(0, root.Children.Count);
        }

        [TestMethod]
        public void AddRemoveAddChild()
        {
            Csla.ApplicationContext.GlobalContext.Clear();
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
            Csla.ApplicationContext.GlobalContext.Clear();
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
            Csla.ApplicationContext.GlobalContext.Clear();
            Root root = Csla.Test.Basic.Root.NewRoot();
            root.Children.Add("1");
            Child child = root.Children[0];
            child.GrandChildren.Add("1");
            child.GrandChildren.Remove(child.GrandChildren[0]);
            Assert.AreEqual(0, child.GrandChildren.Count);
        }

        ///<remarks>"the non-generic method AreEqual cannot be used with type arguments" - though
        ///it is used with type arguments in BasicTests.vb
        ///</remarks>
        //[TestMethod]
        //public void CloneGraph()
        //{
        //    Csla.ApplicationContext.GlobalContext.Clear();
        //    Root root = Csla.Test.Basic.Root.NewRoot();
        //    FormSimulator form = new FormSimulator(root);
        //    SerializableListener listener = new SerializableListener(root);
        //    root.Children.Add("1");
        //    Child child = root.Children[0];
        //    Child.GrandChildren.Add("1");
        //    Assert.AreEqual<int>(1, child.GrandChildren.Count);
        //    Assert.AreEqual<string>("1", child.GrandChildren[0].Data);

        //    Root clone = ((Root)(root.Clone()));
        //    child = clone.Children[0];
        //    Assert.AreEqual<int>(1, child.GrandChildren.Count);
        //    Assert.AreEqual<string>("1", child.GrandChildren[0].Data);

        //    Assert.AreEqual<string>("root Deserialized", ((string)(Csla.ApplicationContext.GlobalContext["Deserialized"])));
        //    Assert.AreEqual<string>("GC Deserialized", ((string)(Csla.ApplicationContext.GlobalContext["GCDeserialized"])));
        //}

        [TestMethod]
        public void ClearChildList()
        {
            Csla.ApplicationContext.GlobalContext.Clear();
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
            Csla.ApplicationContext.GlobalContext.Clear();
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
            Csla.ApplicationContext.GlobalContext.Clear();
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
            Csla.ApplicationContext.GlobalContext.Clear();
            Root r1 = Root.NewRoot();
            r1.Data = "abc";
            Assert.AreEqual(true, r1.Equals(r1), "objects should be equal on instance compare");
            Assert.AreEqual(true, Equals(r1, r1), "objects should be equal on static compare");

            Csla.ApplicationContext.GlobalContext.Clear();
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
            Csla.ApplicationContext.GlobalContext.Clear();
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
          Csla.ApplicationContext.GlobalContext.Clear();
          Root root = Csla.Test.Basic.Root.NewRoot();
          root.Children.Add("1");
          root.Children.Add("2");
          root.Children.Add("3");
          root.BeginEdit();
          root.Children.Remove(root.Children[0]);
          root.Children.Remove(root.Children[0]);
          root.ApplyEdit();

          Root copy = root.Clone();

          var deleted = (List<Child>)(root.Children.GetType().GetProperty("DeletedList", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.FlattenHierarchy | System.Reflection.BindingFlags.IgnoreCase).GetValue(copy.Children, null));

          Assert.AreEqual(2, deleted.Count);
          Assert.AreEqual("1", deleted[0].Data);
          Assert.AreEqual("2", deleted[1].Data);
          Assert.AreEqual(1, root.Children.Count);
        }

        [TestMethod]
        public void DeletedListTestWithCancel()
        {
          Csla.ApplicationContext.GlobalContext.Clear();
          Root root = Csla.Test.Basic.Root.NewRoot();
          root.Children.Add("1");
          root.Children.Add("2");
          root.Children.Add("3");
          root.BeginEdit();
          root.Children.Remove(root.Children[0]);
          root.Children.Remove(root.Children[0]);

          Root copy = root.Clone();

          List<Child> deleted = (List<Child>)(root.Children.GetType().GetProperty("DeletedList", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.FlattenHierarchy | System.Reflection.BindingFlags.IgnoreCase).GetValue(copy.Children, null));

          Assert.AreEqual(2, deleted.Count);
          Assert.AreEqual("1", deleted[0].Data);
          Assert.AreEqual("2", deleted[1].Data);
          Assert.AreEqual(1, root.Children.Count);

          root.CancelEdit();

          deleted = (List<Child>)(root.Children.GetType().GetProperty("DeletedList", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.FlattenHierarchy | System.Reflection.BindingFlags.IgnoreCase).GetValue(root.Children, null));

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

        [TestCleanup]
        public void ClearContextsAfterEachTest() {
          Csla.ApplicationContext.GlobalContext.Clear();
        }
      }

    public class FormSimulator
    {
        private Core.BusinessBase _obj;

        public FormSimulator(Core.BusinessBase obj)
        {
            this._obj.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(obj_IsDirtyChanged);
            this._obj = obj;
        }

        private void obj_IsDirtyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) 
        {}
    }

    [Serializable()]
    public class SerializableListener
    {
        private Core.BusinessBase _obj;

        public SerializableListener(Core.BusinessBase obj)
        {
            this._obj.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(obj_IsDirtyChanged);
            this._obj = obj;
        }

        public void obj_IsDirtyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        { }
    }

}