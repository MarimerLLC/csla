using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Csla.Test.Basic
{
    [TestFixture]
    public class BasicTests
    {
        [Test]
        public void CreateGenRoot()
        {
            Csla.ApplicationContext.GlobalContext.Clear();
            GenRoot root;
            root = GenRoot.NewRoot();
            Assert.IsNotNull(root);
            Assert.AreEqual("<new>", root.Data);
            Assert.AreEqual("Created", Csla.ApplicationContext.GlobalContext["Root"]);
            Assert.AreEqual(true, root.IsNew);
            Assert.AreEqual(false, root.IsDeleted);
            Assert.AreEqual(true, root.IsDirty);
        }

        [Test]
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

        [Test]
        public void AddChild()
        {
            Csla.ApplicationContext.GlobalContext.Clear();
            Root root = Csla.Test.Basic.Root.NewRoot();
            root.Children.Add("1");
            Assert.AreEqual(1, root.Children.Count);
            Assert.AreEqual("1", root.Children[0].Data);
        }

        [Test]
        public void AddRemoveChild()
        {
            Csla.ApplicationContext.GlobalContext.Clear();
            Root root = Csla.Test.Basic.Root.NewRoot();
            root.Children.Add("1");
            root.Children.Remove(root.Children[0]);
            Assert.AreEqual(0, root.Children.Count);
        }

        [Test]
        public void AddGrandChild()
        { }


    }
}
