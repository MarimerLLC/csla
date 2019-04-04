//-----------------------------------------------------------------------
// <copyright file="ChildChangedTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
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
    private ApplicationContext.PropertyChangedModes _mode;

    [TestInitialize]
    public void Initialize()
    {
      _mode = Csla.ApplicationContext.PropertyChangedMode;
      Csla.ApplicationContext.PropertyChangedMode = ApplicationContext.PropertyChangedModes.Windows;
    }

    [TestCleanup]
    public void Cleanup()
    {
      Csla.ApplicationContext.PropertyChangedMode = _mode;
    }

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
      System.ComponentModel.PropertyDescriptor lcp = null;
      root.ListChanged += (o, e) =>
      {
        lc++;
        lcp = e.PropertyDescriptor;
      };
      root.ChildChanged += (o, e) =>
      {
        cc++;
        cca = e;
      };
      root[0].Name = "abc";
      Assert.AreEqual(1, lc, "ListChanged should have fired");
      Assert.IsNotNull(lcp, "PropertyDescriptor should be provided");
      Assert.AreEqual("Name", lcp.Name, "PropertyDescriptor.Name should be Name");
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

      System.ComponentModel.PropertyDescriptor lcp = null;
      root.ListChanged += (o, e) =>
      {
        lc++;
        lcp = e.PropertyDescriptor;
      };
      root.ChildChanged += (o, e) =>
      {
        cc++;
        cca = e;
      };
      root[0].Name = "abc";
      Assert.AreEqual(1, lc, "ListChanged should have fired");
      Assert.IsNotNull(lcp, "PropertyDescriptor should be provided");
      Assert.AreEqual("Name", lcp.Name, "PropertyDescriptor.Name should be Name");
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
      System.ComponentModel.PropertyDescriptor lcp = null;
      root.List.ListChanged += (o, e) =>
      {
        lc++;
        lcp = e.PropertyDescriptor;
      };
      root.List.ChildChanged += (o, e) =>
      {
        cc++;
        cca = e;
      };
      root.List[0].Name = "abc";
      Assert.AreEqual(1, lc, "ListChanged should have fired");
      Assert.IsNotNull(lcp, "PropertyDescriptor should be provided");
      Assert.AreEqual("Name", lcp.Name, "PropertyDescriptor.Name should be Name");
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
      System.ComponentModel.PropertyDescriptor lcp = null;
      root.List.ListChanged += (o, e) =>
      {
        lc++;
        lcp = e.PropertyDescriptor;
      };
      root.List.ChildChanged += (o, e) =>
      {
        cc++;
        cca = e;
      };
      root.List[0].Name = "abc";
      Assert.AreEqual(1, lc, "ListChanged should have fired");
      Assert.IsNotNull(lcp, "PropertyDescriptor should be provided");
      Assert.AreEqual("Name", lcp.Name, "PropertyDescriptor.Name should be Name");
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
      System.ComponentModel.PropertyDescriptor lcp = null;
      root.ListChanged += (o, e) =>
        {
          Assert.Fail("root.ListChanged should not fire");
        };
      child.ChildChanged += (o, e) =>
      {
        ccc = true;
      };
      child.PropertyChanged += (o, e) =>
      {
        Assert.IsTrue(false, "child.PropertyChanged should not fire");
      };
      bool lc = false;
      child.List.ListChanged += (o, e) =>
      {
        lc = true;
        lcp = e.PropertyDescriptor;
      };
      child.List.ChildChanged += (o, e) =>
      {
        cc = true;
        cca = e;
      };
      child.List[0].Name = "abc";
      Assert.IsTrue(lc, "ListChanged should have fired");
      Assert.IsNotNull(lcp, "PropertyDescriptor should be provided");
      Assert.AreEqual("Name", lcp.Name, "PropertyDescriptor.Name should be Name");
			Assert.IsTrue(rcc, "root.ChildChanged should have fired");
      Assert.IsTrue(ccc, "child.ChildChanged should have fired");
      Assert.IsTrue(cc, "list.ChildChanged should have fired");
      Assert.IsTrue(ReferenceEquals(child.List[0], cca.ChildObject), "Ref should be equal");
    }
  }
}