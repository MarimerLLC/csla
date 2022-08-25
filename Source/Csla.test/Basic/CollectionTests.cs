//-----------------------------------------------------------------------
// <copyright file="CollectionTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Diagnostics;
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
  public class CollectionTests
  {
    private static TestDIContext _testDIContext;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
      _testDIContext = TestDIContextFactory.CreateDefaultContext();
    }

    [TestMethod]
    public void SetLast()
    {
      IDataPortal<TestCollection> dataPortal = _testDIContext.CreateDataPortal<TestCollection>();
      IDataPortal<TestItem> childDataPortal = _testDIContext.CreateDataPortal<TestItem>();
      TestCollection list = dataPortal.Create();
      list.Add(childDataPortal.Create());
      list.Add(childDataPortal.Create());
      TestItem oldItem = childDataPortal.Create();
      list.Add(oldItem);
      TestItem newItem = childDataPortal.Create();
      list[2] = newItem;
      Assert.AreEqual(3, list.Count, "List should have 3 items");
      Assert.AreEqual(newItem, list[2], "Last item should be newItem");
      Assert.AreEqual(true, list.ContainsDeleted(oldItem), "Deleted list should have old item");
    }

    [TestMethod]
    public void RootListGetRuleDescriptions()
    {
      RootList list = new RootList();
      RootListChild child = list.AddNew();
      string[] rules = child.GetRuleDescriptions();
    }
  }

  [Serializable]
  public class TestCollection : BusinessBindingListBase<TestCollection, TestItem>
  {
    public TestCollection()
    {
    }

    [Create]
    private void Create()
    {
    }

    private void DataPortal_Fetch([Inject] IChildDataPortal<TestItem> childDataPortal)
    {
      Add(childDataPortal.FetchChild(123));
      Add(childDataPortal.FetchChild(2));
      Add(childDataPortal.FetchChild(13));
      Add(childDataPortal.FetchChild(23));
      Add(childDataPortal.FetchChild(3));
    }
  }

  [Serializable]
  public class TestItem : BusinessBase<TestItem>
  {
    private static PropertyInfo<int> IdProperty = RegisterProperty<int>(c => c.Id);
    public int Id
    {
      get { return GetProperty(IdProperty); }
      set { SetProperty(IdProperty, value); }
    }

    public bool HasParent()
    {
      return (Parent != null);
    }

    public TestItem()
    {
      MarkAsChild();
    }

    [Create]
    [CreateChild]
    protected override void Child_Create()
    { }

    [Fetch]
    [FetchChild]
    private void Child_Fetch(int id)
    {
      LoadProperty(IdProperty, id);
    }
  }
}