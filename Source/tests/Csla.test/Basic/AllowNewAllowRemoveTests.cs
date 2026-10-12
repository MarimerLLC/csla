//-----------------------------------------------------------------------
// <copyright file="AllowNewAllowRemoveTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Tests enforcement of AllowNew and AllowRemove on editable lists</summary>
//-----------------------------------------------------------------------

using Csla.Core;
using Csla.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.Basic
{
  [TestClass]
  public class AllowNewAllowRemoveTests
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

    private static AllowItem NewChild() => _testHost.GetChildDataPortal<AllowItem>().CreateChild();

    private static AllowList FetchList(bool allowNew, bool allowRemove) =>
      _testHost.GetDataPortal<AllowList>().Fetch(allowNew, allowRemove);

    private static AllowBindingList FetchBindingList(bool allowNew, bool allowRemove) =>
      _testHost.GetDataPortal<AllowBindingList>().Fetch(allowNew, allowRemove);

    #region BusinessListBase

    [TestMethod]
    public void BusinessList_DefaultsAllowAddAndRemove()
    {
      var list = _testHost.GetDataPortal<AllowList>().Create();

      list.AllowNew.Should().BeTrue();
      list.AllowRemove.Should().BeTrue();

      list.AddNew();
      list.Add(NewChild());
      list.Count.Should().Be(2);

      list.RemoveAt(0);
      list.Clear();
      list.Count.Should().Be(0);
    }

    [TestMethod]
    public void BusinessList_FetchKeepsFlags()
    {
      var list = FetchList(false, false);

      list.Count.Should().Be(3);
      list.AllowNew.Should().BeFalse();
      list.AllowRemove.Should().BeFalse();
    }

    [TestMethod]
    public void BusinessList_AllowNewFalse_AddNewThrows()
    {
      var list = FetchList(false, true);

      FluentActions.Invoking(() => list.AddNew()).Should().Throw<NotSupportedException>();
      list.Count.Should().Be(3);
    }

    [TestMethod]
    public async Task BusinessList_AllowNewFalse_AddNewAsyncThrows()
    {
      var list = FetchList(false, true);

      await FluentActions.Awaiting(() => list.AddNewAsync()).Should().ThrowAsync<NotSupportedException>();
      list.Count.Should().Be(3);
    }

    [TestMethod]
    public void BusinessList_AllowNewFalse_AddAndInsertThrow()
    {
      var list = FetchList(false, true);

      FluentActions.Invoking(() => list.Add(NewChild())).Should().Throw<NotSupportedException>();
      FluentActions.Invoking(() => list.Insert(0, NewChild())).Should().Throw<NotSupportedException>();
      list.Count.Should().Be(3);
    }

    [TestMethod]
    public void BusinessList_AllowRemoveFalse_RemoveThrows()
    {
      var list = FetchList(true, false);
      var item = list[0];

      FluentActions.Invoking(() => list.Remove(item)).Should().Throw<NotSupportedException>();
      FluentActions.Invoking(() => list.RemoveAt(0)).Should().Throw<NotSupportedException>();
      FluentActions.Invoking(() => list.Clear()).Should().Throw<NotSupportedException>();
      list.Count.Should().Be(3);
      list.ContainsDeleted(item).Should().BeFalse();
    }

    [TestMethod]
    public void BusinessList_FlagsFalse_ReplaceAllowed()
    {
      var list = FetchList(false, false);
      var newItem = NewChild();

      list[0] = newItem;

      list[0].Should().BeSameAs(newItem);
      list.Count.Should().Be(3);
    }

    [TestMethod]
    public void BusinessList_CloneKeepsFlagsAndItems()
    {
      var list = FetchList(false, false);

      var clone = list.Clone();

      clone.Count.Should().Be(3);
      clone.AllowNew.Should().BeFalse();
      clone.AllowRemove.Should().BeFalse();
    }

    [TestMethod]
    public void BusinessList_AllowNewFalse_CancelEditRestoresRemovedItem()
    {
      var list = FetchList(false, true);
      var item = list[0];

      list.BeginEdit();
      list.Remove(item);
      list.Count.Should().Be(2);
      list.CancelEdit();

      list.Count.Should().Be(3);
      list.Should().Contain(item);
    }

    [TestMethod]
    public void BusinessList_FlagsFalse_GraphMergeAddsAndRemoves()
    {
      var target = FetchList(false, false);
      var removed = target[0];
      var source = target.Clone();
      source.ForceRemove(source[0]);
      source.ForceAdd(NewChild());
      var added = source[2];

      new GraphMerger(_testHost.ApplicationContext).MergeBusinessListGraph<AllowList, AllowItem>(target, source);

      target.Count.Should().Be(3);
      target.Should().Contain(added);
      target.Should().NotContain(removed);
    }

    #endregion

    #region BusinessBindingListBase

    [TestMethod]
    public void BusinessBindingList_DefaultsAllowAddAndRemove()
    {
      var list = _testHost.GetDataPortal<AllowBindingList>().Create();

      list.AllowNew.Should().BeTrue();
      list.AllowRemove.Should().BeTrue();

      list.AddNew();
      list.Add(NewChild());
      list.Count.Should().Be(2);

      list.RemoveAt(0);
      list.Clear();
      list.Count.Should().Be(0);
    }

    [TestMethod]
    public void BusinessBindingList_FetchKeepsFlags()
    {
      var list = FetchBindingList(false, false);

      list.Count.Should().Be(3);
      list.AllowNew.Should().BeFalse();
      list.AllowRemove.Should().BeFalse();
    }

    [TestMethod]
    public void BusinessBindingList_AllowNewFalse_AddNewThrows()
    {
      var list = FetchBindingList(true, true);
      list.AllowNew = false;

      FluentActions.Invoking(() => list.AddNew()).Should().Throw<NotSupportedException>();
      list.Count.Should().Be(3);
    }

    [TestMethod]
    public void BusinessBindingList_AllowNewFalse_AddAndInsertThrow()
    {
      var list = FetchBindingList(false, true);

      FluentActions.Invoking(() => list.Add(NewChild())).Should().Throw<NotSupportedException>();
      FluentActions.Invoking(() => list.Insert(0, NewChild())).Should().Throw<NotSupportedException>();
      list.Count.Should().Be(3);
    }

    [TestMethod]
    public void BusinessBindingList_AllowRemoveFalse_RemoveThrowsAndKeepsEventHooks()
    {
      var list = FetchBindingList(true, false);
      var item = list[0];

      FluentActions.Invoking(() => list.Remove(item)).Should().Throw<NotSupportedException>();
      FluentActions.Invoking(() => list.RemoveAt(0)).Should().Throw<NotSupportedException>();
      FluentActions.Invoking(() => list.Clear()).Should().Throw<NotSupportedException>();
      list.Count.Should().Be(3);
      list.ContainsDeleted(item).Should().BeFalse();

      // the item's events must still be hooked after the failed removes
      var childChanged = false;
      list.ChildChanged += (_, _) => childChanged = true;
      item.Name = "changed";
      childChanged.Should().BeTrue();
    }

    [TestMethod]
    public void BusinessBindingList_FlagsFalse_ReplaceAllowed()
    {
      var list = FetchBindingList(false, false);
      var newItem = NewChild();

      list[0] = newItem;

      list[0].Should().BeSameAs(newItem);
      list.Count.Should().Be(3);
    }

    [TestMethod]
    public void BusinessBindingList_CloneKeepsFlagsAndItems()
    {
      var list = FetchBindingList(false, false);

      var clone = list.Clone();

      clone.Count.Should().Be(3);
      clone.AllowNew.Should().BeFalse();
      clone.AllowRemove.Should().BeFalse();
    }

    [TestMethod]
    public void BusinessBindingList_AllowNewFalse_CancelEditRestoresRemovedItem()
    {
      var list = FetchBindingList(false, true);
      var item = list[0];

      list.BeginEdit();
      list.Remove(item);
      list.Count.Should().Be(2);
      list.CancelEdit();

      list.Count.Should().Be(3);
      list.Should().Contain(item);
    }

    [TestMethod]
    public void BusinessBindingList_FlagsFalse_GraphMergeAddsAndRemoves()
    {
      var target = FetchBindingList(false, false);
      var removed = target[0];
      var source = target.Clone();
      source.ForceRemove(source[0]);
      source.ForceAdd(NewChild());
      var added = source[2];

      new GraphMerger(_testHost.ApplicationContext).MergeBusinessBindingListGraph<AllowBindingList, AllowItem>(target, source);

      target.Count.Should().Be(3);
      target.Should().Contain(added);
      target.Should().NotContain(removed);
    }

    #endregion

    #region Dynamic lists

    [TestMethod]
    public void DynamicList_FlagsFalse_ChangesThrow()
    {
      var list = _testHost.GetDataPortal<AllowDynamicList>().Create();
      list.Add(_testHost.GetDataPortal<AllowRoot>().Create());
      list.SetFlags(false, false);

      FluentActions.Invoking(() => list.AddNew()).Should().Throw<NotSupportedException>();
      FluentActions.Invoking(() => list.Add(_testHost.GetDataPortal<AllowRoot>().Create())).Should().Throw<NotSupportedException>();
      FluentActions.Invoking(() => list.RemoveAt(0)).Should().Throw<NotSupportedException>();
      FluentActions.Invoking(() => list.Clear()).Should().Throw<NotSupportedException>();
      list.Count.Should().Be(1);
    }

    [TestMethod]
    public async Task DynamicList_AllowRemoveFalse_RemoveItemAsyncThrows()
    {
      var list = _testHost.GetDataPortal<AllowDynamicList>().Create();
      list.Add(_testHost.GetDataPortal<AllowRoot>().Create());
      list.SetFlags(true, false);

      await FluentActions.Awaiting(() => list.RemoveItemAsync(0)).Should().ThrowAsync<NotSupportedException>();
      list.Count.Should().Be(1);
    }

    [TestMethod]
    public void DynamicBindingList_FlagsFalse_ChangesThrowWithoutDeletingItem()
    {
      var list = _testHost.GetDataPortal<AllowDynamicBindingList>().Create();
      var item = _testHost.GetDataPortal<AllowRoot>().Fetch(1);
      list.Add(item);
      list.AllowNew = false;
      list.AllowRemove = false;

      FluentActions.Invoking(() => list.AddNew()).Should().Throw<NotSupportedException>();
      FluentActions.Invoking(() => list.Add(_testHost.GetDataPortal<AllowRoot>().Create())).Should().Throw<NotSupportedException>();
      FluentActions.Invoking(() => list.RemoveAt(0)).Should().Throw<NotSupportedException>();
      FluentActions.Invoking(() => list.Clear()).Should().Throw<NotSupportedException>();
      list.Count.Should().Be(1);
      item.IsDeleted.Should().BeFalse();
    }

    #endregion
  }

  [Serializable]
  public class AllowItem : BusinessBase<AllowItem>
  {
    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
    public string Name
    {
      get => GetProperty(NameProperty);
      set => SetProperty(NameProperty, value);
    }

    [CreateChild]
    private void CreateChild()
    { }

    [FetchChild]
    private void FetchChild(int id)
    {
      LoadProperty(NameProperty, id.ToString());
    }
  }

  [Serializable]
  public class AllowList : BusinessListBase<AllowList, AllowItem>
  {
    public void ForceAdd(AllowItem item)
    {
      var oldAllowNew = AllowNew;
      AllowNew = true;
      Add(item);
      AllowNew = oldAllowNew;
    }

    public void ForceRemove(AllowItem item)
    {
      var oldAllowRemove = AllowRemove;
      AllowRemove = true;
      Remove(item);
      AllowRemove = oldAllowRemove;
    }

    [Create]
    private void Create()
    { }

    [Fetch]
    private void Fetch(bool allowNew, bool allowRemove, [Inject] IChildDataPortal<AllowItem> childDataPortal)
    {
      using (LoadListMode)
      {
        for (var id = 1; id <= 3; id++)
          Add(childDataPortal.FetchChild(id));
      }
      AllowNew = allowNew;
      AllowRemove = allowRemove;
    }
  }

  [Serializable]
  public class AllowBindingList : BusinessBindingListBase<AllowBindingList, AllowItem>
  {
    public void ForceAdd(AllowItem item)
    {
      var oldAllowNew = AllowNew;
      AllowNew = true;
      Add(item);
      AllowNew = oldAllowNew;
    }

    public void ForceRemove(AllowItem item)
    {
      var oldAllowRemove = AllowRemove;
      AllowRemove = true;
      Remove(item);
      AllowRemove = oldAllowRemove;
    }

    [Create]
    private void Create()
    { }

    [Fetch]
    private void Fetch(bool allowNew, bool allowRemove, [Inject] IChildDataPortal<AllowItem> childDataPortal)
    {
      using (LoadListMode)
      {
        for (var id = 1; id <= 3; id++)
          Add(childDataPortal.FetchChild(id));
      }
      AllowNew = allowNew;
      AllowRemove = allowRemove;
    }
  }

  [Serializable]
  public class AllowRoot : BusinessBase<AllowRoot>
  {
    public static readonly PropertyInfo<int> IdProperty = RegisterProperty<int>(nameof(Id));
    public int Id
    {
      get => GetProperty(IdProperty);
      set => SetProperty(IdProperty, value);
    }

    [Create]
    private void Create()
    { }

    [Fetch]
    private void Fetch(int id)
    {
      LoadProperty(IdProperty, id);
    }

    [Insert]
    private void Insert()
    { }

    [Update]
    private void Update()
    { }

    [DeleteSelf]
    private void DeleteSelf()
    { }
  }

  [Serializable]
  public class AllowDynamicList : DynamicListBase<AllowRoot>
  {
    public void SetFlags(bool allowNew, bool allowRemove)
    {
      AllowNew = allowNew;
      AllowRemove = allowRemove;
    }

    [Create]
    private void Create()
    { }
  }

  [Serializable]
  public class AllowDynamicBindingList : DynamicBindingListBase<AllowRoot>
  {
    [Create]
    private void Create()
    { }
  }
}
