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
using Csla.Testing;
using Csla.TestHelpers;
using Csla.Testing.Business.BusyStatus;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace cslalighttest.BusyStatus
{
  [TestClass]
  public class BusyStatusTests
  {
    private static CslaTestHost _testHost;
    private static CslaTestHost _noCloneOnUpdateHost;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
      _testHost = CslaTestHost.Create();
      _noCloneOnUpdateHost = CslaTestHost.Create(t => t.ConfigureCsla(opt => opt.
        DataPortal(dpo => dpo.AddClientSideDataPortal(o => o.
          AutoCloneOnUpdate = false))));
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
      _testHost?.Dispose();
      _noCloneOnUpdateHost?.Dispose();
    }

    [TestInitialize]
    public void Initialize()
    {
      TestResults.Reinitialise();
    }

    [TestMethod]
    public async Task TestBusy()
    {
      IDataPortal<ItemWithAsynchRule> dataPortal = _noCloneOnUpdateHost.GetDataPortal<ItemWithAsynchRule>();

      var item = await dataPortal.FetchAsync("an id");
      item.RuleField = "some value";
      Assert.IsTrue(item.IsBusy, "Should be busy");
      Assert.IsFalse(item.IsSavable, "Should not be savable");
    }

    [TestMethod]
    public void ListTestBusy()
    {
      IDataPortal<ItemWithAsynchRuleList> dataPortal = _noCloneOnUpdateHost.GetDataPortal<ItemWithAsynchRuleList>();

      ItemWithAsynchRuleList items = ItemWithAsynchRuleList.GetListWithItems(dataPortal);
      items[0].RuleField = "some value";
      Assert.IsTrue(items.IsBusy);
      Assert.IsFalse(items.IsSavable);
    }

    [TestMethod]
    public async Task TestSaveWhileBusy()
    {
      IDataPortal<ItemWithAsynchRule> dataPortal = _testHost.GetDataPortal<ItemWithAsynchRule>();

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
      IDataPortal<ItemWithAsynchRuleList> dataPortal = _noCloneOnUpdateHost.GetDataPortal<ItemWithAsynchRuleList>();

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
      IDataPortal<ItemWithAsynchRule> dataPortal = _testHost.GetDataPortal<ItemWithAsynchRule>();

      var item = await dataPortal.FetchAsync("an id");
      item.RuleField = "some value";
      Assert.IsTrue(item.IsBusy);
      Assert.IsFalse(item.IsSavable);

      await item.WaitForIdle();

      Assert.IsFalse(item.IsBusy);
      Assert.IsTrue(item.IsSavable);
    }

    [TestMethod]
    public async Task ListTestNotBusy()
    {
      IDataPortal<ItemWithAsynchRuleList> dataPortal = _noCloneOnUpdateHost.GetDataPortal<ItemWithAsynchRuleList>();

      ItemWithAsynchRuleList items = ItemWithAsynchRuleList.GetListWithItems(dataPortal);

      items[0].RuleField = "some value";
      Assert.IsTrue(items.IsBusy);
      Assert.IsFalse(items.IsSavable);

      await items.WaitForIdle();

      Assert.IsFalse(items.IsBusy);
      Assert.IsTrue(items.IsSavable);
    }

    [TestMethod]
    public async Task ListTestSaveWhileNotBusy()
    {
      IDataPortal<ItemWithAsynchRuleList> dataPortal = _noCloneOnUpdateHost.GetDataPortal<ItemWithAsynchRuleList>();

      ItemWithAsynchRuleList items = ItemWithAsynchRuleList.GetListWithItems(dataPortal);
      var tcs = new TaskCompletionSource<(bool WasBusy, bool WasSavable, string ReturnedOperationResult)>();
      var item = items[0];
      EventHandler handler = null;
      handler = async (_, _) =>
      {
        // saving runs the item's rules again, which raises ValidationComplete again
        item.ValidationComplete -= handler;
        try
        {
          // completion notifications are raised while the object is still busy (#4991)
          var wasBusyInHandler = items.IsBusy;
          await items.WaitForIdle();
          var wasSavableInHandler = items.IsSavable;
          items = await items.SaveAsync();
          var returnedOperationResultInHandler = items[0].OperationResult;
          tcs.TrySetResult((wasBusyInHandler, wasSavableInHandler, returnedOperationResultInHandler));
        }
        catch (Exception ex)
        {
          tcs.SetException(ex);
        }
      };
      item.ValidationComplete += handler;

      items[0].RuleField = "some value";
      Assert.IsTrue(items.IsBusy);
      Assert.IsFalse(items.IsSavable);
      var (wasBusy, wasSavable, returnedOperationResult) = await tcs.Task;
      using (new AssertionScope())
      {
        wasBusy.Should().BeTrue();
        wasSavable.Should().BeTrue();
        returnedOperationResult.Should().Be("DataPortal_Update");
      }
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public async Task TestSaveWhileBusyNetOnly()
    {
      IDataPortal<ItemWithAsynchRule> dataPortal = _testHost.GetDataPortal<ItemWithAsynchRule>();

      var item = await dataPortal.FetchAsync("an id");
      item.RuleField = "some value";
      Assert.IsTrue(item.IsBusy);
      Assert.IsFalse(item.IsSavable);
      item.Save();
    }

    [TestMethod]
    public void ListTestSaveWhileBusyNetOnly()
    {
      IDataPortal<ItemWithAsynchRuleList> dataPortal = _noCloneOnUpdateHost.GetDataPortal<ItemWithAsynchRuleList>();

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
      IDataPortal<ItemWithAsynchRule> dataPortal = _noCloneOnUpdateHost.GetDataPortal<ItemWithAsynchRule>();

      var item = await dataPortal.FetchAsync("an id");
      bool? busyInValidationComplete = null;
      item.ValidationComplete += (_, _) => busyInValidationComplete = item.IsBusy;
      item.RuleField = "some value";
      Assert.IsTrue(item.IsBusy);
      Assert.IsFalse(item.IsSavable);

      await item.WaitForIdle();

      // completion notifications are raised while the object is still busy (#4991)
      Assert.AreEqual(true, busyInValidationComplete);
      Assert.IsFalse(item.IsBusy);
      Assert.IsTrue(item.IsSavable);
      item = item.Save();
      Assert.AreEqual("DataPortal_Update", item.OperationResult);
    }

    [TestMethod]
    public async Task ListTestSaveWhileNotBusyNetOnly()
    {
      IDataPortal<ItemWithAsynchRuleList> dataPortal = _noCloneOnUpdateHost.GetDataPortal<ItemWithAsynchRuleList>();

      ItemWithAsynchRuleList items = ItemWithAsynchRuleList.GetListWithItems(dataPortal);
      bool? busyInValidationComplete = null;
      items[0].ValidationComplete += (_, _) => busyInValidationComplete = items.IsBusy;

      items[0].RuleField = "some value";
      Assert.IsTrue(items.IsBusy);
      Assert.IsFalse(items.IsSavable);

      await items.WaitForIdle();

      // completion notifications are raised while the object is still busy (#4991)
      Assert.AreEqual(true, busyInValidationComplete);
      Assert.IsFalse(items.IsBusy);
      Assert.IsTrue(items.IsSavable);
      items = items.Save();
      Assert.AreEqual("DataPortal_Update", items[0].OperationResult);
    }

    [TestMethod]
    public async Task TestSaveWhileNotBusyNoActiveRuleNetOnly()
    {
      IDataPortal<ItemWithAsynchRule> dataPortal = _noCloneOnUpdateHost.GetDataPortal<ItemWithAsynchRule>();

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
      IDataPortal<ItemWithAsynchRuleList> dataPortal = _noCloneOnUpdateHost.GetDataPortal<ItemWithAsynchRuleList>();

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
      IDataPortal<ItemWithAsynchRule> dataPortal = _noCloneOnUpdateHost.GetDataPortal<ItemWithAsynchRule>();

      var item = await dataPortal.FetchAsync("an id");
      item.RuleField = "some value";

      await item.WaitForIdle(TimeSpan.FromSeconds(5)); // Timeout should _never_ happen

      item.IsBusy.Should().BeFalse(because: $"{nameof(ItemWithAsynchRule.IsBusy)} is still true even though we waited with {nameof(ItemWithAsynchRule.WaitForIdle)}.");
    }

    [TestMethod]
    public async Task WaitForIdle_WhenReachingTheTimeoutATimeoutExceptionIsThrown()
    {

      IDataPortal<ItemWithAsynchRule> dataPortal = _noCloneOnUpdateHost.GetDataPortal<ItemWithAsynchRule>();

      var item = await dataPortal.FetchAsync("an id");
      item.RuleField = "some value";

      await item.Invoking(i => i.WaitForIdle(TimeSpan.FromMilliseconds(500))).Should().ThrowAsync<TimeoutException>();
    }
  }
}
