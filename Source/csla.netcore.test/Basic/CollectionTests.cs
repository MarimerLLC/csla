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
    [TestMethod]
    public void SetLast()
    {
      TestCollection list = new TestCollection();
      list.Add(new TestItem());
      list.Add(new TestItem());
      TestItem oldItem = new TestItem();
      list.Add(oldItem);
      TestItem newItem = new TestItem();
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
      AllowNew = true;
    }

    protected override object AddNewCore()
    {
      var item = Csla.DataPortal.CreateChild<TestItem>();
      Add(item);
      return item;
    }

    public static TestCollection GetList()
    {
      return Csla.DataPortal.Fetch<TestCollection>();
    }

    [Fetch]
    private void DataPortal_Fetch()
    {
      Add(Csla.DataPortal.FetchChild<TestItem>(123));
      Add(Csla.DataPortal.FetchChild<TestItem>(2));
      Add(Csla.DataPortal.FetchChild<TestItem>(13));
      Add(Csla.DataPortal.FetchChild<TestItem>(23));
      Add(Csla.DataPortal.FetchChild<TestItem>(3));
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

    [CreateChild]
    protected override void Child_Create()
    { }

    [FetchChild]
    private void Child_Fetch(int id)
    {
      LoadProperty(IdProperty, id);
    }
  }
}