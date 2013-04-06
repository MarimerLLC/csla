using System;
using System.Collections.Generic;
using System.Text;
using UnitDriven;

#if NUNIT
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
using TestSetup = NUnit.Framework.SetUpAttribute;
#elif MSTEST
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace Csla.Test.ChildChanged
{
  [TestClass]
  public class ChildChangedTests
  {
    [TestMethod]
    public void SingleRoot()
    {
      bool pc = false;
      bool cc = false;

      var root = new SingleRoot();
      root.PropertyChanged += (o, e) =>
        {
          pc = true;
        };
      root.ChildChanged += (o, e) =>
      {
        cc = true;
      };
      root.Name = "abc";
      Assert.IsTrue(pc, "PropertyChanged should have fired");
      Assert.IsFalse(cc, "ChildChanged should not have fired");
    }

    [TestMethod]
    public void SingleChild()
    {
      bool pc = false;
      bool cc = false;

      var root = new SingleChild();
      root.PropertyChanged += (o, e) =>
      {
        pc = true;
      };
      root.ChildChanged += (o, e) =>
      {
        cc = true;
      };
      root.Child.Name = "abc";
      Assert.IsFalse(pc, "PropertyChanged should not have fired");
      Assert.IsTrue(cc, "ChildChanged should have fired");
    }

    [TestMethod]
    public void GrandChild()
    {
      bool pc = false;
      bool cc = false;
      bool cpc = false;
      bool ccc = false;
      Csla.Core.ChildChangedEventArgs cca = null;

      var root = new Grandchild();
      root.PropertyChanged += (o, e) =>
      {
        pc = true;
      };
      root.ChildChanged += (o, e) =>
      {
        cc = true;
      };
      root.Child.PropertyChanged += (o, e) =>
      {
        cpc = true;
      };
      root.Child.ChildChanged += (o, e) =>
      {
        ccc = true;
        cca = e;
      };
      root.Child.Child.Name = "abc";
      Assert.IsFalse(cpc, "C PropertyChanged should not have fired");
      Assert.IsTrue(ccc, "C ChildChanged should have fired");
      Assert.IsFalse(pc, "PropertyChanged should not have fired");
      Assert.IsTrue(cc, "ChildChanged should have fired");
      Assert.IsTrue(ReferenceEquals(root.Child.Child, cca.ChildObject), "Ref should be equal");
    }

    [TestMethod]
    public void SingleList()
    {
      int lc = 0;
      int cc = 0;
      Csla.Core.ChildChangedEventArgs cca = null;

      var root = new SingleList();
      root.Add(new SingleRoot(true));
#if !SILVERLIGHT
      System.ComponentModel.PropertyDescriptor lcp = null;
      root.ListChanged += (o, e) =>
      {
        lc++;
        lcp = e.PropertyDescriptor;
      };
#else
      root.CollectionChanged += (o, e) =>
      {
        lc++;
      };
#endif
      root.ChildChanged += (o, e) =>
      {
        cc++;
        cca = e;
      };
      root[0].Name = "abc";
#if !SILVERLIGHT
      Assert.AreEqual(1, lc, "ListChanged should have fired");
      Assert.IsNotNull(lcp, "PropertyDescriptor should be provided");
      Assert.AreEqual("Name", lcp.Name, "PropertyDescriptor.Name should be Name");
#endif
      Assert.AreEqual(1, cc, "ChildChanged should have fired");
      Assert.IsTrue(ReferenceEquals(root[0], cca.ChildObject), "Ref should be equal");
    }

    [TestMethod]
    public void SingleList_Serialized()
    {
      int lc = 0;
      int cc = 0;
      Csla.Core.ChildChangedEventArgs cca = null;

      var root = new SingleList();
      root.Add(new SingleRoot(true));
      root = root.Clone();

#if !SILVERLIGHT
      System.ComponentModel.PropertyDescriptor lcp = null;
      root.ListChanged += (o, e) =>
      {
        lc++;
        lcp = e.PropertyDescriptor;
      };
#else
      root.CollectionChanged += (o, e) =>
      {
        lc++;
      };
#endif
      root.ChildChanged += (o, e) =>
      {
        cc++;
        cca = e;
      };
      root[0].Name = "abc";
#if !SILVERLIGHT
      Assert.AreEqual(1, lc, "ListChanged should have fired");
      Assert.IsNotNull(lcp, "PropertyDescriptor should be provided");
      Assert.AreEqual("Name", lcp.Name, "PropertyDescriptor.Name should be Name");
#endif
      Assert.AreEqual(1, cc, "ChildChanged should have fired");
      Assert.IsTrue(ReferenceEquals(root[0], cca.ChildObject), "Ref should be equal");
    }

    [TestMethod]
    public void ContainedList()
    {
      int lc = 0;
      int rcc = 0;
      int cc = 0;
      Csla.Core.ChildChangedEventArgs cca = null;

      var root = new ContainsList();
      root.List.Add(new SingleRoot(true));
      root.PropertyChanged += (o, e) =>
      {
        Assert.IsTrue(false, "root.PropertyChanged should not fire");
      };
      root.ChildChanged += (o, e) =>
      {
        rcc++;
      };
#if !SILVERLIGHT
      System.ComponentModel.PropertyDescriptor lcp = null;
      root.List.ListChanged += (o, e) =>
      {
        lc++;
        lcp = e.PropertyDescriptor;
      };
#else
      root.List.CollectionChanged += (o, e) =>
      {
        lc++;
      };
#endif
      root.List.ChildChanged += (o, e) =>
      {
        cc++;
        cca = e;
      };
      root.List[0].Name = "abc";
#if !SILVERLIGHT
      Assert.AreEqual(1, lc, "ListChanged should have fired");
      Assert.IsNotNull(lcp, "PropertyDescriptor should be provided");
      Assert.AreEqual("Name", lcp.Name, "PropertyDescriptor.Name should be Name");
#endif
      Assert.AreEqual(1, rcc, "root.ChildChanged should have fired");
      Assert.AreEqual(1, cc, "list.ChildChanged should have fired");
      Assert.IsTrue(ReferenceEquals(root.List[0], cca.ChildObject), "Ref should be equal");
    }

    [TestMethod]
    public void ContainedList_Serialized()
    {
      int lc = 0;
      int rcc = 0;
      int cc = 0;
      Csla.Core.ChildChangedEventArgs cca = null;

      var root = new ContainsList();
      root.List.Add(new SingleRoot(true));
      root = root.Clone();

      root.PropertyChanged += (o, e) =>
      {
        Assert.IsTrue(false, "root.PropertyChanged should not fire");
      };
      root.ChildChanged += (o, e) =>
      {
        rcc++;
      };
#if !SILVERLIGHT
      System.ComponentModel.PropertyDescriptor lcp = null;
      root.List.ListChanged += (o, e) =>
      {
        lc++;
        lcp = e.PropertyDescriptor;
      };
#else
      root.List.CollectionChanged += (o, e) =>
      {
        lc++;
      };
#endif
      root.List.ChildChanged += (o, e) =>
      {
        cc++;
        cca = e;
      };
      root.List[0].Name = "abc";
#if !SILVERLIGHT
      Assert.AreEqual(1, lc, "ListChanged should have fired");
      Assert.IsNotNull(lcp, "PropertyDescriptor should be provided");
      Assert.AreEqual("Name", lcp.Name, "PropertyDescriptor.Name should be Name");
#endif
      Assert.AreEqual(1, rcc, "root.ChildChanged should have fired");
      Assert.AreEqual(1, cc, "list.ChildChanged should have fired");
      Assert.IsTrue(ReferenceEquals(root.List[0], cca.ChildObject), "Ref should be equal");
    }

    [TestMethod]
    public void ListOfLists()
    {
      bool rcc = false;
      bool ccc = false;
      bool cc = false;
      Csla.Core.ChildChangedEventArgs cca = null;

      var root = new ListContainerList();
      var child = new ContainsList(true);
      root.Add(child);
      child.List.Add(new SingleRoot(true));
      root.ChildChanged += (o, e) =>
      {
        rcc = true;
      };
#if !SILVERLIGHT
      System.ComponentModel.PropertyDescriptor lcp = null;
      root.ListChanged += (o, e) =>
        {
          Assert.Fail("root.ListChanged should not fire");
        };
#else
      root.CollectionChanged += (o, e) =>
      {
        Assert.IsTrue(false, "root.ListChanged should not fire");
      };
#endif
      child.ChildChanged += (o, e) =>
      {
        ccc = true;
      };
      child.PropertyChanged += (o, e) =>
      {
        Assert.IsTrue(false, "child.PropertyChanged should not fire");
      };
#if !SILVERLIGHT
      bool lc = false;
      child.List.ListChanged += (o, e) =>
      {
        lc = true;
        lcp = e.PropertyDescriptor;
      };
#endif
      child.List.ChildChanged += (o, e) =>
      {
        cc = true;
        cca = e;
      };
      child.List[0].Name = "abc";
#if !SILVERLIGHT
      Assert.IsTrue(lc, "ListChanged should have fired");
      Assert.IsNotNull(lcp, "PropertyDescriptor should be provided");
      Assert.AreEqual("Name", lcp.Name, "PropertyDescriptor.Name should be Name");
#endif
      Assert.IsTrue(rcc, "root.ChildChanged should have fired");
      Assert.IsTrue(ccc, "child.ChildChanged should have fired");
      Assert.IsTrue(cc, "list.ChildChanged should have fired");
      Assert.IsTrue(ReferenceEquals(child.List[0], cca.ChildObject), "Ref should be equal");
    }
  }
}
