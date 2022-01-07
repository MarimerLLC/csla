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
using Csla.TestHelpers;

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
    private static TestDIContext _testDIContext;
    private ApplicationContext.PropertyChangedModes _mode;

    [ClassInitialize]
    public static void ClassInitialize(TestContext testContext)
    {
      _testDIContext = TestDIContextFactory.CreateDefaultContext();
    }

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
      IDataPortal<SingleRoot> dataPortal = _testDIContext.CreateDataPortal<SingleRoot>();
      
      bool pc = false;
      bool cc = false;

      var root = dataPortal.Fetch(false);
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
      IDataPortal<SingleChild> dataPortal = _testDIContext.CreateDataPortal<SingleChild>();

      bool pc = false;
      bool cc = false;

      var root = dataPortal.Fetch(false);
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
      IDataPortal<Grandchild> dataPortal = _testDIContext.CreateDataPortal<Grandchild>();

      bool pc = false;
      bool cc = false;
      bool cpc = false;
      bool ccc = false;
      Csla.Core.ChildChangedEventArgs cca = null;

      var root = dataPortal.Fetch();
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
      IDataPortal<SingleList> listDataPortal = _testDIContext.CreateDataPortal<SingleList>();
      IDataPortal<SingleRoot> dataPortal = _testDIContext.CreateDataPortal<SingleRoot>();

      int lc = 0;
      int cc = 0;
      Csla.Core.ChildChangedEventArgs cca = null;

      var root = listDataPortal.Fetch(false);
      root.Add(dataPortal.Fetch(true));
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
      IDataPortal<SingleList> listDataPortal = _testDIContext.CreateDataPortal<SingleList>();
      IDataPortal<SingleRoot> dataPortal = _testDIContext.CreateDataPortal<SingleRoot>();

      int lc = 0;
      int cc = 0;
      Csla.Core.ChildChangedEventArgs cca = null;

      var root = listDataPortal.Fetch(false);
      root.Add(dataPortal.Fetch(true));
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
      IDataPortal<ContainsList> listDataPortal = _testDIContext.CreateDataPortal<ContainsList>();
      IDataPortal<SingleRoot> dataPortal = _testDIContext.CreateDataPortal<SingleRoot>();

      int lc = 0;
      int rcc = 0;
      int cc = 0;
      Csla.Core.ChildChangedEventArgs cca = null;

      var root = listDataPortal.Fetch();
      root.List.Add(dataPortal.Fetch(true));
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
      IDataPortal<ContainsList> listDataPortal = _testDIContext.CreateDataPortal<ContainsList>();
      IDataPortal<SingleRoot> dataPortal = _testDIContext.CreateDataPortal<SingleRoot>();
      
      int lc = 0;
      int rcc = 0;
      int cc = 0;
      Csla.Core.ChildChangedEventArgs cca = null;

      var root = listDataPortal.Fetch();
      root.List.Add(dataPortal.Fetch(true));
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
      IDataPortal<ListContainerList> listContainerDataPortal = _testDIContext.CreateDataPortal<ListContainerList>();
      IDataPortal<ContainsList> listDataPortal = _testDIContext.CreateDataPortal<ContainsList>();
      IDataPortal<SingleRoot> dataPortal = _testDIContext.CreateDataPortal<SingleRoot>();
      
      bool rcc = false;
      bool ccc = false;
      bool cc = false;
      Csla.Core.ChildChangedEventArgs cca = null;

      var root = listContainerDataPortal.Fetch();
      var child = listDataPortal.Fetch(true);
      root.Add(child);
      child.List.Add(dataPortal.Fetch(true));
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