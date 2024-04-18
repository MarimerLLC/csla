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

    [ClassInitialize]
    public static void ClassInitialize(TestContext testContext)
    {
      IServiceCollection services = new ServiceCollection();

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
      root.PropertyChanged += (_, _) =>
        {
          pc = true;
        };
      root.ChildChanged += (_, _) =>
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
      root.PropertyChanged += (_, _) =>
      {
        pc = true;
      };
      root.ChildChanged += (_, _) =>
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
      root.PropertyChanged += (_, _) =>
      {
        pc = true;
      };
      root.ChildChanged += (_, _) =>
      {
        cc = true;
      };
      root.Child.PropertyChanged += (_, _) =>
      {
        cpc = true;
      };
      root.Child.ChildChanged += (_, e) =>
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
      root.CollectionChanged += (_, _) =>
      {
        lc++;
      };
      root.ChildChanged += (_, e) =>
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

      root.CollectionChanged += (_, _) =>
      {
        lc++;
      };
      root.ChildChanged += (_, e) =>
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
      root.PropertyChanged += (_, _) =>
      {
        Assert.IsTrue(false, "root.PropertyChanged should not fire");
      };
      root.ChildChanged += (_, _) =>
      {
        rcc++;
      };
      root.List.CollectionChanged += (_, _) =>
      {
        lc++;
      };
      root.List.ChildChanged += (_, e) =>
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

      list.PropertyChanged += (_, _) =>
      {
        Assert.IsTrue(false, "root.PropertyChanged should not fire");
      };
      list.ChildChanged += (_, _) =>
      {
        rcc++;
      };
      list.List.CollectionChanged += (_, _) =>
      {
        lc++;
      };
      list.List.ChildChanged += (_, e) =>
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
      root.ChildChanged += (_, _) =>
      {
        rcc = true;
      };
      root.CollectionChanged += (_, _) =>
        {
          Assert.Fail("root.CollectionChanged should not fire");
        };
      child.ChildChanged += (_, _) =>
      {
        ccc = true;
      };
      child.PropertyChanged += (_, _) =>
      {
        Assert.IsTrue(false, "child.PropertyChanged should not fire");
      };
      bool lc = false;
      child.List.CollectionChanged += (_, _) =>
      {
        lc = true;
      };
      child.List.ChildChanged += (_, e) =>
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
      obj.PropertyChanged += (_, _) => propertyCount++;
      obj.ChildChanged += (_, _) => childCount++;
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