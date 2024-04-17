﻿//-----------------------------------------------------------------------
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
using Microsoft.Extensions.DependencyInjection;
using Csla.Configuration;



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
      IServiceCollection services = new ServiceCollection();
      var options = new CslaOptions(services);

      services.AddCsla(o => o.Binding(bo => bo.PropertyChangedMode = ApplicationContext.PropertyChangedModes.Windows));
      _testDIContext = TestDIContextFactory.CreateDefaultContext();
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
      var dataPortal = _testDIContext.CreateChildDataPortal<SingleRoot>();

      int lc = 0;
      int cc = 0;
      Csla.Core.ChildChangedEventArgs cca = null;

      var root = listDataPortal.Fetch(false);
      root.Add(dataPortal.FetchChild(true));
      root.CollectionChanged += (o, e) =>
      {
        lc++;
      };
      root.ChildChanged += (o, e) =>
      {
        cc++;
        cca = e;
      };
      root[0].Name = "abc";
      Assert.AreEqual(0, lc, "CollectionChanged should not have fired");
      Assert.AreEqual(1, cc, "ChildChanged should have fired");
      Assert.IsTrue(ReferenceEquals(root[0], cca.ChildObject), "Ref should be equal");
    }

    [TestMethod]
    public void SingleList_Serialized()
    {
      var listDataPortal = _testDIContext.CreateDataPortal<SingleList>();
      var dataPortal = _testDIContext.CreateChildDataPortal<SingleRoot>();

      int lc = 0;
      int cc = 0;
      Csla.Core.ChildChangedEventArgs cca = null;

      var root = listDataPortal.Fetch(false);
      root.Add(dataPortal.FetchChild(true));
      root = root.Clone();

      root.CollectionChanged += (o, e) =>
      {
        lc++;
      };
      root.ChildChanged += (o, e) =>
      {
        cc++;
        cca = e;
      };
      root[0].Name = "abc";

      Assert.AreEqual(0, lc, "CollectionChanged should not have fired");
      Assert.AreEqual(1, cc, "ChildChanged should have fired");
      Assert.IsTrue(ReferenceEquals(root[0], cca.ChildObject), "Ref should be equal");
    }

    [TestMethod]
    public void ContainedList()
    {
      IDataPortal<ContainsList> listDataPortal = _testDIContext.CreateDataPortal<ContainsList>();
      var dataPortal = _testDIContext.CreateChildDataPortal<SingleRoot>();

      int lc = 0;
      int rcc = 0;
      int cc = 0;
      Csla.Core.ChildChangedEventArgs cca = null;

      var root = listDataPortal.Fetch();
      root.List.Add(dataPortal.FetchChild(true));
      root.PropertyChanged += (o, e) =>
      {
        Assert.IsTrue(false, "root.PropertyChanged should not fire");
      };
      root.ChildChanged += (o, e) =>
      {
        rcc++;
      };
      root.List.CollectionChanged += (o, e) =>
      {
        lc++;
      };
      root.List.ChildChanged += (o, e) =>
      {
        cc++;
        cca = e;
      };
      root.List[0].Name = "abc";
      Assert.AreEqual(0, lc, "CollectionChanged should not have fired");
      Assert.AreEqual(1, rcc, "root.ChildChanged should have fired");
      Assert.AreEqual(1, cc, "list.ChildChanged should have fired");
      Assert.IsTrue(ReferenceEquals(root.List[0], cca.ChildObject), "Ref should be equal");
    }

    [TestMethod]
    public void ContainedList_Serialized()
    {
      var listDataPortal = _testDIContext.CreateDataPortal<ContainsList>();
      var singleRootPortal = _testDIContext.CreateChildDataPortal<SingleRoot>();
      
      int lc = 0;
      int rcc = 0;
      int cc = 0;
      Csla.Core.ChildChangedEventArgs cca = null;

      var list = listDataPortal.Fetch();
      list.List.Add(singleRootPortal.FetchChild(true));
      list = list.Clone();

      list.PropertyChanged += (o, e) =>
      {
        Assert.IsTrue(false, "root.PropertyChanged should not fire");
      };
      list.ChildChanged += (o, e) =>
      {
        rcc++;
      };
      list.List.CollectionChanged += (o, e) =>
      {
        lc++;
      };
      list.List.ChildChanged += (o, e) =>
      {
        cc++;
        cca = e;
      };
      list.List[0].Name = "abc";
      Assert.AreEqual(0, lc, "CollectionChanged should not have fired");
      Assert.AreEqual(1, rcc, "root.ChildChanged should have fired");
      Assert.AreEqual(1, cc, "list.ChildChanged should have fired");
      Assert.IsTrue(ReferenceEquals(list.List[0], cca.ChildObject), "Ref should be equal");
    }

    [TestMethod]
    public void ListOfLists()
    {
      IDataPortal<ListContainerList> listContainerDataPortal = _testDIContext.CreateDataPortal<ListContainerList>();
      var listDataPortal = _testDIContext.CreateChildDataPortal<ContainsList>();
      var dataPortal = _testDIContext.CreateChildDataPortal<SingleRoot>();
      
      bool rcc = false;
      bool ccc = false;
      bool cc = false;
      Csla.Core.ChildChangedEventArgs cca = null;

      var root = listContainerDataPortal.Fetch();
      var child = listDataPortal.FetchChild(true);
      root.Add(child);
      child.List.Add(dataPortal.FetchChild(true));
      root.ChildChanged += (o, e) =>
      {
        rcc = true;
      };
      root.CollectionChanged += (o, e) =>
        {
          Assert.Fail("root.CollectionChanged should not fire");
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
      child.List.CollectionChanged += (o, e) =>
      {
        lc = true;
      };
      child.List.ChildChanged += (o, e) =>
      {
        cc = true;
        cca = e;
      };
      child.List[0].Name = "abc";
      Assert.IsFalse(lc, "CollectionChanged should not have fired");
      Assert.IsTrue(rcc, "root.ChildChanged should have fired");
      Assert.IsTrue(ccc, "child.ChildChanged should have fired");
      Assert.IsTrue(cc, "list.ChildChanged should have fired");
      Assert.IsTrue(ReferenceEquals(child.List[0], cca.ChildObject), "Ref should be equal");
    }

    [TestMethod]
    public void SimpleMetaState()
    {
      IDataPortal<MetaState> portal = _testDIContext.CreateDataPortal<MetaState>();
      var obj = portal.Create();
      var propertyCount = 0;
      var childCount = 0;
      obj.PropertyChanged += (s, e) => propertyCount++;
      obj.ChildChanged += (s, e) => childCount++;
      obj.Name = "abc";
      Assert.AreEqual(1, propertyCount, "propertyCount");
      Assert.AreEqual(0, childCount, "childCount");
    }
  }

  [Serializable]
  public class MetaState : BusinessBase<MetaState>
  {
    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
    public string Name
    {
      get => GetProperty(NameProperty);
      set => SetProperty(NameProperty, value);
    }

    [Create]
    private void Create() { }
  }
}