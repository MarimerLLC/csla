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
using Csla.TestHelpers;

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
        private static TestDIContext _testDIContext;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
          _testDIContext = TestDIContextFactory.CreateDefaultContext();
        }

        [TestMethod]
        public void TestNotUndoableField()
        {
            IDataPortal<DataBinding.ParentEntity> dataPortal = _testDIContext.CreateDataPortal<DataBinding.ParentEntity>();

            TestResults.Reinitialise();
            Csla.Test.DataBinding.ParentEntity p = DataBinding.ParentEntity.NewParentEntity(dataPortal);

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
              //Assert.AreEqual("Fetched", TestResults.GetResult("ReadOnlyList"));
        }

        [TestMethod]
        public void TestNameValueList()
        {
            IDataPortal<NameValueListObj> dataPortal = _testDIContext.CreateDataPortal<NameValueListObj>();

            NameValueListObj nvList = dataPortal.Fetch();
            // TODO: Fix test
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
            IDataPortal<CommandObject> dataPortal = _testDIContext.CreateDataPortal<CommandObject>();

            TestResults.Reinitialise();
            CommandObject obj = new CommandObject();
            obj = dataPortal.Execute(obj);
            Assert.AreEqual("Executed", obj.AProperty);
        }

        [TestMethod]
        public void CreateGenRoot()
        {
            TestResults.Reinitialise();
            GenRoot root;
            root = NewGenRoot();
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
          root = NewGenRoot();
          root.BeginEdit();
          root.Data = "abc";
          root.CancelEdit();

          TestResults.Reinitialise();
          root = NewGenRoot();
          root.BeginEdit();
          root.Data = "abc";
          root.ApplyEdit();
        }

        [TestMethod]
        public void CreateRoot()
        {
            TestResults.Reinitialise();
            Root root;
            root = NewRoot();
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
            Root root = NewRoot();
            root.Children.Add("1");
            Assert.AreEqual(1, root.Children.Count);
            Assert.AreEqual("1", root.Children[0].Data);
        }

        [TestMethod]
        public void AddRemoveChild()
        {
            TestResults.Reinitialise();
            Root root = NewRoot();
            root.Children.Add("1");
            root.Children.Remove(root.Children[0]);
            Assert.AreEqual(0, root.Children.Count);
        }

        [TestMethod]
        public void AddRemoveAddChild()
        {
            TestResults.Reinitialise();
            Root root = NewRoot();
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
            Root root = NewRoot();
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
            Root root = NewRoot();
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
        //    TestResults.Reinitialise();
        //    Root root = NewRoot();
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

        //    Assert.AreEqual<string>("root Deserialized", ((string)(TestResults.GetResult("Deserialized"))));
        //    Assert.AreEqual<string>("GC Deserialized", ((string)(TestResults.GetResult("GCDeserialized"))));
        //}

        [TestMethod]
        public void ClearChildList()
        {
            TestResults.Reinitialise();
            Root root = NewRoot();
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
            Root root = NewRoot();
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
            Root root = NewRoot();
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
            Root r1 = NewRoot();
            r1.Data = "abc";
            Assert.AreEqual(true, r1.Equals(r1), "objects should be equal on instance compare");
            Assert.AreEqual(true, Equals(r1, r1), "objects should be equal on static compare");

            TestResults.Reinitialise();
            Root r2 = NewRoot();
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
            Root root = NewRoot();
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
          Root root = NewRoot();
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
          TestResults.Reinitialise();
          Root root = NewRoot();
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
          TestResults.Reinitialise();
        }

        private Root NewRoot()
        {
          IDataPortal<Root> dataPortal = _testDIContext.CreateDataPortal<Root>();
          return dataPortal.Create(new Root.Criteria());
        }

        private GenRoot NewGenRoot()
        {
          IDataPortal<GenRoot> dataPortal = _testDIContext.CreateDataPortal<GenRoot>();
          return dataPortal.Create(new GenRoot.Criteria());
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