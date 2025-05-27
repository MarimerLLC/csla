//-----------------------------------------------------------------------
// <copyright file="BusyStatusTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using Csla;
using Csla.Configuration;
using Csla.Test;
using Csla.TestHelpers;
using Csla.Testing.Business.BusyStatus;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace cslalighttest.BusyStatus
{
  [TestClass]
  public class BusyStatusTests
  {
    private static TestDIContext _testDIContext;
    private static TestDIContext _noCloneOnUpdateDIContext;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
      _testDIContext = TestDIContextFactory.CreateDefaultContext();
      _noCloneOnUpdateDIContext = TestDIContextFactory.CreateContext(opt => opt.
        DataPortal(dpo => dpo.AddClientSideDataPortal(o => o.
          AutoCloneOnUpdate = false)));
    }

    [TestInitialize]
    public void Initialize()
    {
      TestResults.Reinitialise();
    }

    [TestMethod]
    public async Task TestBusy()
    {
      IDataPortal<ItemWithAsynchRule> dataPortal = _noCloneOnUpdateDIContext.CreateDataPortal<ItemWithAsynchRule>();

      var item = await dataPortal.FetchAsync("an id");
      item.RuleField = "some value";
      Assert.IsTrue(item.IsBusy, "Should be busy");
      Assert.IsFalse(item.IsSavable, "Should not be savable");
    }

    [TestMethod]
    public void ListTestBusy()
    {
      IDataPortal<ItemWithAsynchRuleList> dataPortal = _noCloneOnUpdateDIContext.CreateDataPortal<ItemWithAsynchRuleList>();

      ItemWithAsynchRuleList items = ItemWithAsynchRuleList.GetListWithItems(dataPortal);
      items[0].RuleField = "some value";
      Assert.IsTrue(items.IsBusy);
      Assert.IsFalse(items.IsSavable);
    }

    [TestMethod]
    public async Task TestSaveWhileBusy()
    {
      IDataPortal<ItemWithAsynchRule> dataPortal = _testDIContext.CreateDataPortal<ItemWithAsynchRule>();

      var item = await dataPortal.FetchAsync("an id");
      item.RuleField = "some value";
      Assert.IsTrue(item.IsBusy);
      Assert.IsFalse(item.IsSavable);

      try
      {
        await item.SaveAsync();
      }
      catch (Exception ex)
      {
        var error = ex as InvalidOperationException;
        Assert.IsNotNull(error);
        Assert.IsTrue(error.Message.ToLower().Contains("busy"));
        Assert.IsTrue(error.Message.ToLower().Contains("save"));
      }
    }

    [TestMethod]
    public async Task ListTestSaveWhileBusy()
    {
      IDataPortal<ItemWithAsynchRuleList> dataPortal = _noCloneOnUpdateDIContext.CreateDataPortal<ItemWithAsynchRuleList>();

      ItemWithAsynchRuleList items = ItemWithAsynchRuleList.GetListWithItems(dataPortal);
      items[0].RuleField = "some value";
      Assert.IsTrue(items.IsBusy);
      Assert.IsFalse(items.IsSavable);

      try
      {
        await items.SaveAsync();
      }
      catch (Exception ex)
      {
        var error = ex as InvalidOperationException;
        Assert.IsNotNull(error);
        Assert.IsTrue(error.Message.ToLower().Contains("busy"));
        Assert.IsTrue(error.Message.ToLower().Contains("save"));
      }
    }

    [TestMethod]
    public async Task TestNotBusy()
    {
      IDataPortal<ItemWithAsynchRule> dataPortal = _testDIContext.CreateDataPortal<ItemWithAsynchRule>();

      var item = await dataPortal.FetchAsync("an id");
      item.ValidationComplete += (_, _) =>
      {
        Assert.IsFalse(item.IsBusy);
        Assert.IsTrue(item.IsSavable);
      };
      item.RuleField = "some value";
      Assert.IsTrue(item.IsBusy);
      Assert.IsFalse(item.IsSavable);
    }

    [TestMethod]
    public void ListTestNotBusy()
    {
      IDataPortal<ItemWithAsynchRuleList> dataPortal = _noCloneOnUpdateDIContext.CreateDataPortal<ItemWithAsynchRuleList>();

      ItemWithAsynchRuleList items = ItemWithAsynchRuleList.GetListWithItems(dataPortal);
      items[0].ValidationComplete += (_, _) =>
      {
        Assert.IsFalse(items.IsBusy);
        Assert.IsTrue(items.IsSavable);
      };

      items[0].RuleField = "some value";
      Assert.IsTrue(items.IsBusy);
      Assert.IsFalse(items.IsSavable);
    }

    [TestMethod]
    public async Task ListTestSaveWhileNotBusy()
    {
      IDataPortal<ItemWithAsynchRuleList> dataPortal = _noCloneOnUpdateDIContext.CreateDataPortal<ItemWithAsynchRuleList>();

      ItemWithAsynchRuleList items = ItemWithAsynchRuleList.GetListWithItems(dataPortal);
      var tcs = new TaskCompletionSource();
      items[0].ValidationComplete += async (_, _) =>
      {
        try
        {
          Assert.IsFalse(items.IsBusy);
          Assert.IsTrue(items.IsSavable);
          items = await items.SaveAsync();
          string actual = items[0].OperationResult;
          Assert.AreEqual("DataPortal_Update", actual);
        }
        catch (Exception ex)
        {
          tcs.SetException(ex);
        }
        finally
        {
          tcs.TrySetResult();
        }
      };

      items[0].RuleField = "some value";
      Assert.IsTrue(items.IsBusy);
      Assert.IsFalse(items.IsSavable);
      await tcs.Task;
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public async Task TestSaveWhileBusyNetOnly()
    {
      IDataPortal<ItemWithAsynchRule> dataPortal = _testDIContext.CreateDataPortal<ItemWithAsynchRule>();

      var item = await dataPortal.FetchAsync("an id");
      item.RuleField = "some value";
      Assert.IsTrue(item.IsBusy);
      Assert.IsFalse(item.IsSavable);
      item.Save();
    }

    [TestMethod]
    public void ListTestSaveWhileBusyNetOnly()
    {
      IDataPortal<ItemWithAsynchRuleList> dataPortal = _noCloneOnUpdateDIContext.CreateDataPortal<ItemWithAsynchRuleList>();

      ItemWithAsynchRuleList items = ItemWithAsynchRuleList.GetListWithItems(dataPortal);

      items[0].RuleField = "some value";
      Assert.IsTrue(items.IsBusy);
      Assert.IsFalse(items.IsSavable);
      bool gotError = false;
      try
      {
        items.Save();
      }
      catch (InvalidOperationException)
      {
        gotError = true;
      }
      Assert.IsTrue(gotError);
    }

    [TestMethod]
    public async Task TestSaveWhileNotBusyNetOnly()
    {
      IDataPortal<ItemWithAsynchRule> dataPortal = _noCloneOnUpdateDIContext.CreateDataPortal<ItemWithAsynchRule>();

      var item = await dataPortal.FetchAsync("an id");
      item.ValidationComplete += (_, _) =>
      {
        Assert.IsFalse(item.IsBusy);
        Assert.IsTrue(item.IsSavable);
        item = item.Save();
        Assert.AreEqual("DataPortal_Update", item.OperationResult);
      };
      item.RuleField = "some value";
      Assert.IsTrue(item.IsBusy);
      Assert.IsFalse(item.IsSavable);
    }

    [TestMethod]
    public void ListTestSaveWhileNotBusyNetOnly()
    {
      IDataPortal<ItemWithAsynchRuleList> dataPortal = _noCloneOnUpdateDIContext.CreateDataPortal<ItemWithAsynchRuleList>();

      ItemWithAsynchRuleList items = ItemWithAsynchRuleList.GetListWithItems(dataPortal);

      items[0].ValidationComplete += (_, _) =>
      {
        Assert.IsFalse(items.IsBusy);
        Assert.IsTrue(items.IsSavable);
        items = items.Save();
        string actual = items[0].OperationResult;
        Assert.AreEqual("DataPortal_Update", actual);
      };

      items[0].RuleField = "some value";
      Assert.IsTrue(items.IsBusy);
      Assert.IsFalse(items.IsSavable);
    }

    [TestMethod]
    public async Task TestSaveWhileNotBusyNoActiveRuleNetOnly()
    {
      IDataPortal<ItemWithAsynchRule> dataPortal = _noCloneOnUpdateDIContext.CreateDataPortal<ItemWithAsynchRule>();

      var item = await dataPortal.FetchAsync("an id");
      item.OperationResult = "something";
      Assert.IsFalse(item.IsBusy);
      Assert.IsTrue(item.IsSavable);
      item = item.Save();
      Assert.AreEqual("DataPortal_Update", item.OperationResult);
    }

    [TestMethod]
    public void ListTestSaveWhileNotBusyNoActiveRuleNetOnly()
    {
      IDataPortal<ItemWithAsynchRuleList> dataPortal = _noCloneOnUpdateDIContext.CreateDataPortal<ItemWithAsynchRuleList>();

      ItemWithAsynchRuleList items = ItemWithAsynchRuleList.GetListWithItems(dataPortal);

      items[0].OperationResult = "something";
      Assert.IsFalse(items.IsBusy);
      Assert.IsTrue(items.IsSavable);
      items = items.Save();
      string actual = items[0].OperationResult;
      Assert.AreEqual("DataPortal_Update", actual);
    }

    [TestMethod]
    public async Task WaitForIdle_WhenBusyWillWaitUntilNotBusyAnymore()
    {
      IDataPortal<ItemWithAsynchRule> dataPortal = _noCloneOnUpdateDIContext.CreateDataPortal<ItemWithAsynchRule>();

      var item = await dataPortal.FetchAsync("an id");
      item.RuleField = "some value";

      await item.WaitForIdle(TimeSpan.FromSeconds(5)); // Timeout should _never_ happen

      item.IsBusy.Should().BeFalse(because: $"{nameof(ItemWithAsynchRule.IsBusy)} is still true even though we waited with {nameof(ItemWithAsynchRule.WaitForIdle)}.");
    }

    [TestMethod]
    public async Task WaitForIdle_WhenReachingTheTimeoutATimeoutExceptionIsThrown()
    {

      IDataPortal<ItemWithAsynchRule> dataPortal = _noCloneOnUpdateDIContext.CreateDataPortal<ItemWithAsynchRule>();

      var item = await dataPortal.FetchAsync("an id");
      item.RuleField = "some value";

      await item.Invoking(i => i.WaitForIdle(TimeSpan.FromMilliseconds(500))).Should().ThrowAsync<TimeoutException>();
    }
  }
}
