//-----------------------------------------------------------------------
// <copyright file="ChildCollectionMergeTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Tests merging child collections held by a parent object</summary>
//-----------------------------------------------------------------------

using Csla.Core;
using Csla.Testing;
using Csla.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.GraphMerge
{
  [TestClass]
  public class ChildCollectionMergeTests
  {
    private static CslaTestHost _testHost;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
      _testHost = CslaTestHost.Create();
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
      _testHost?.Dispose();
    }

    [TestMethod]
    public void MergeGraph_MergesChildListAndChildBindingList()
    {
      var (target, source) = CreateTargetAndSource();

      new GraphMerger(_testHost.ApplicationContext).MergeGraph(target, source);

      AssertMerged(target, source);
    }

    [TestMethod]
    public async Task MergeGraphAsync_MergesChildListAndChildBindingList()
    {
      var (target, source) = CreateTargetAndSource();

      await new GraphMerger(_testHost.ApplicationContext).MergeGraphAsync(target, source);

      AssertMerged(target, source);
    }

    private static (MergeParent Target, MergeParent Source) CreateTargetAndSource()
    {
      var target = _testHost.GetDataPortal<MergeParent>().Create();
      target.Items.AddNew().Name = "1";
      target.Items.AddNew().Name = "12";
      target.BindingItems.AddNew().Name = "a";
      target.BindingItems.AddNew().Name = "ab";
      target.MockUpdated();

      var source = target.Clone();
      source.Items[0].Name = "2";
      source.Items.RemoveAt(1);
      source.Items.AddNew().Name = "42";
      source.BindingItems[0].Name = "b";
      source.BindingItems.RemoveAt(1);
      source.BindingItems.AddNew().Name = "xyz";
      source.MockUpdated();

      return (target, source);
    }

    private static void AssertMerged(MergeParent target, MergeParent source)
    {
      Assert.AreEqual(2, target.Items.Count, "Items count");
      Assert.AreEqual("2", target.Items[0].Name, "Items[0] name");
      Assert.AreEqual("42", target.Items[1].Name, "Items[1] name");
      Assert.IsFalse(ReferenceEquals(target.Items[0], source.Items[0]), "Items[0] ref");
      Assert.IsTrue(ReferenceEquals(target.Items[1], source.Items[1]), "Items[1] ref");

      Assert.AreEqual(2, target.BindingItems.Count, "BindingItems count");
      Assert.AreEqual("b", target.BindingItems[0].Name, "BindingItems[0] name");
      Assert.AreEqual("xyz", target.BindingItems[1].Name, "BindingItems[1] name");
      Assert.IsFalse(ReferenceEquals(target.BindingItems[0], source.BindingItems[0]), "BindingItems[0] ref");
      Assert.IsTrue(ReferenceEquals(target.BindingItems[1], source.BindingItems[1]), "BindingItems[1] ref");
    }
  }

  [Serializable]
  public class MergeParent : BusinessBase<MergeParent>
  {
    public static readonly PropertyInfo<MergeItemList> ItemsProperty = RegisterProperty<MergeItemList>(c => c.Items);
    public MergeItemList Items
    {
      get => GetProperty(ItemsProperty);
      private set => LoadProperty(ItemsProperty, value);
    }

    public static readonly PropertyInfo<MergeItemBindingList> BindingItemsProperty = RegisterProperty<MergeItemBindingList>(c => c.BindingItems);
    public MergeItemBindingList BindingItems
    {
      get => GetProperty(BindingItemsProperty);
      private set => LoadProperty(BindingItemsProperty, value);
    }

    public void MockUpdated()
    {
      MarkOld();
      Items.MockUpdated();
      BindingItems.MockUpdated();
    }

    [Create]
    private void Create([Inject] IChildDataPortal<MergeItemList> listPortal, [Inject] IChildDataPortal<MergeItemBindingList> bindingListPortal)
    {
      Items = listPortal.CreateChild();
      BindingItems = bindingListPortal.CreateChild();
    }
  }

  [Serializable]
  public class MergeItem : BusinessBase<MergeItem>
  {
    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);
    public string Name
    {
      get => GetProperty(NameProperty);
      set => SetProperty(NameProperty, value);
    }

    public void MockUpdated() => MarkOld();

    [CreateChild]
    private void CreateChild()
    { }
  }

  [Serializable]
  public class MergeItemList : BusinessListBase<MergeItemList, MergeItem>
  {
    public void MockUpdated()
    {
      foreach (var item in this)
        item.MockUpdated();
      DeletedList.Clear();
    }

    [CreateChild]
    private void CreateChild()
    { }
  }

  [Serializable]
  public class MergeItemBindingList : BusinessBindingListBase<MergeItemBindingList, MergeItem>
  {
    public void MockUpdated()
    {
      foreach (var item in this)
        item.MockUpdated();
      DeletedList.Clear();
    }

    [CreateChild]
    private void CreateChild()
    { }
  }
}
