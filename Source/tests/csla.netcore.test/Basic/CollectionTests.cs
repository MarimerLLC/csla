//-----------------------------------------------------------------------
// <copyright file="CollectionTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

using Csla.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;


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
      TestCollection list = CreateTestCollection();
      list.AddNew();
      list.AddNew();
      TestItem oldItem = CreateTestItem();
      list.Add(oldItem);
      TestItem newItem = CreateTestItem();
      list[2] = newItem;
      Assert.AreEqual(3, list.Count, "List should have 3 items");
      Assert.AreEqual(newItem, list[2], "Last item should be newItem");
      Assert.AreEqual(true, list.ContainsDeleted(oldItem), "Deleted list should have old item");
    }

    [TestMethod]
    public void RootListGetRuleDescriptions()
    {
      RootList list = CreateRootList();
      RootListChild child = list.AddNew();
      string[] rules = child.GetRuleDescriptions();
    }

    private TestCollection CreateTestCollection()
    {
      IDataPortal<TestCollection> dataPortal = _testDIContext.CreateDataPortal<TestCollection>();
      return dataPortal.Create();
    }

    private TestItem CreateTestItem()
    {
      IDataPortal<TestItem> dataPortal = _testDIContext.CreateDataPortal<TestItem>();
      return dataPortal.Create();
    }

    private RootList CreateRootList()
    {
      IDataPortal<RootList> dataPortal = _testDIContext.CreateDataPortal<RootList>();
      return dataPortal.Create();
    }

  }

  [Serializable]
  public class TestCollection : BusinessBindingListBase<TestCollection, TestItem>
  {
    public TestCollection()
    {
      AllowNew = true;
    }

    [Create]
    private void Create()
    {
    }

    [Fetch]
    private void Fetch([Inject] IChildDataPortal<TestItem> dataPortal)
    {
      Add(dataPortal.FetchChild(123));
      Add(dataPortal.FetchChild(2));
      Add(dataPortal.FetchChild(13));
      Add(dataPortal.FetchChild(23));
      Add(dataPortal.FetchChild(3));
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
    private void Create()
    {
    }

    [FetchChild]
    private void Child_Fetch(int id)
    {
      LoadProperty(IdProperty, id);
    }
  }
}