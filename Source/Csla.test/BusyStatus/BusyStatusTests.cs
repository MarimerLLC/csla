//-----------------------------------------------------------------------
// <copyright file="BusyStatusTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using Csla;
using Csla.Configuration;
using System;
using UnitDriven;
using Csla.Testing.Business.BusyStatus;
using System.Threading.Tasks;
using Csla.TestHelpers;
using Csla.Test;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace cslalighttest.BusyStatus
{
  [TestClass]
  public class BusyStatusTests : TestBase
  {

    private static TestDIContext _testDIContext;
    private static TestDIContext _noCloneOnUpdateDIContext;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
      _testDIContext = TestDIContextFactory.CreateDefaultContext();
      _noCloneOnUpdateDIContext = TestDIContextFactory.CreateContext(opt => opt.
        DataPortal(dpo => dpo.ClientSideDataPortal(o => o.
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

      UnitTestContext context = GetContext();
      var item = await dataPortal.FetchAsync("an id");
      item.RuleField = "some value";
      context.Assert.IsTrue(item.IsBusy, "Should be busy");
      context.Assert.IsFalse(item.IsSavable, "Should not be savable");
      context.Assert.Success();
      context.Complete();
    }

    [TestMethod]
    public void ListTestBusy()
    {
      IDataPortal<ItemWithAsynchRuleList> dataPortal = _noCloneOnUpdateDIContext.CreateDataPortal<ItemWithAsynchRuleList>();

      UnitTestContext context = GetContext();
      ItemWithAsynchRuleList items = ItemWithAsynchRuleList.GetListWithItems(dataPortal);
      items[0].RuleField = "some value";
      context.Assert.IsTrue(items.IsBusy);
      context.Assert.IsFalse(items.IsSavable);
      context.Assert.Success();
      context.Complete();
    }

    [TestMethod]
    public async Task TestSaveWhileBusy()
    {
      IDataPortal<ItemWithAsynchRule> dataPortal = _testDIContext.CreateDataPortal<ItemWithAsynchRule>();

      UnitTestContext context = GetContext();
      var item = await dataPortal.FetchAsync("an id");
      item.RuleField = "some value";
      context.Assert.IsTrue(item.IsBusy);
      context.Assert.IsFalse(item.IsSavable);

      try
      {
        await item.SaveAsync();
      }
      catch (Exception ex)
      {
        var error = ex as InvalidOperationException;
        context.Assert.IsNotNull(error);
        context.Assert.IsTrue(error.Message.ToLower().Contains("busy"));
        context.Assert.IsTrue(error.Message.ToLower().Contains("save"));
        context.Assert.Success();
      }
      context.Complete();
    }

    [TestMethod]
    public async Task ListTestSaveWhileBusy()
    {
      IDataPortal<ItemWithAsynchRuleList> dataPortal = _noCloneOnUpdateDIContext.CreateDataPortal<ItemWithAsynchRuleList>();

      UnitTestContext context = GetContext();
      ItemWithAsynchRuleList items = ItemWithAsynchRuleList.GetListWithItems(dataPortal);
      items[0].RuleField = "some value";
      context.Assert.IsTrue(items.IsBusy);
      context.Assert.IsFalse(items.IsSavable);

      try
      {
        await items.SaveAsync();
      }
      catch (Exception ex)
      {
        var error = ex as InvalidOperationException;
        context.Assert.IsNotNull(error);
        context.Assert.IsTrue(error.Message.ToLower().Contains("busy"));
        context.Assert.IsTrue(error.Message.ToLower().Contains("save"));
        context.Assert.Success();
      }
      context.Complete();
    }

    [TestMethod]
    public async Task TestNotBusy()
    {
      IDataPortal<ItemWithAsynchRule> dataPortal = _testDIContext.CreateDataPortal<ItemWithAsynchRule>();

      UnitTestContext context = GetContext();
      var item = await dataPortal.FetchAsync("an id");
      item.ValidationComplete += (_, _) =>
      {
        context.Assert.IsFalse(item.IsBusy);
        context.Assert.IsTrue(item.IsSavable);
        context.Assert.Success();
      };
      item.RuleField = "some value";
      context.Assert.IsTrue(item.IsBusy);
      context.Assert.IsFalse(item.IsSavable);
      context.Complete();
    }

    [TestMethod]
    public void ListTestNotBusy()
    {
      IDataPortal<ItemWithAsynchRuleList> dataPortal = _noCloneOnUpdateDIContext.CreateDataPortal<ItemWithAsynchRuleList>();

      UnitTestContext context = GetContext();
      ItemWithAsynchRuleList items = ItemWithAsynchRuleList.GetListWithItems(dataPortal);
      items[0].ValidationComplete += (_, _) =>
      {
        context.Assert.IsFalse(items.IsBusy);
        context.Assert.IsTrue(items.IsSavable);
        context.Assert.Success();
      };

      items[0].RuleField = "some value";
      context.Assert.IsTrue(items.IsBusy);
      context.Assert.IsFalse(items.IsSavable);
    }

    [TestMethod]
    public void ListTestSaveWhileNotBusy()
    {
      IDataPortal<ItemWithAsynchRuleList> dataPortal = _noCloneOnUpdateDIContext.CreateDataPortal<ItemWithAsynchRuleList>();

      UnitTestContext context = GetContext();
      ItemWithAsynchRuleList items = ItemWithAsynchRuleList.GetListWithItems(dataPortal);
      items[0].ValidationComplete += async (_, _) =>
      {
        context.Assert.IsFalse(items.IsBusy);
        context.Assert.IsTrue(items.IsSavable);
        items = await items.SaveAsync();
        context.Assert.AreEqual("DataPortal_Update", items[0].OperationResult);
        context.Assert.Success();
      };

      items[0].RuleField = "some value";
      context.Assert.IsTrue(items.IsBusy);
      context.Assert.IsFalse(items.IsSavable);
      context.Complete();
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public async Task TestSaveWhileBusyNetOnly()
    {
      IDataPortal<ItemWithAsynchRule> dataPortal = _testDIContext.CreateDataPortal<ItemWithAsynchRule>();

      UnitTestContext context = GetContext();
      var item = await dataPortal.FetchAsync("an id");
      item.RuleField = "some value";
      context.Assert.IsTrue(item.IsBusy);
      context.Assert.IsFalse(item.IsSavable);
      item.Save();
      context.Assert.Success();
      context.Complete();
    }

    [TestMethod]
    public void ListTestSaveWhileBusyNetOnly()
    {
      IDataPortal<ItemWithAsynchRuleList> dataPortal = _noCloneOnUpdateDIContext.CreateDataPortal<ItemWithAsynchRuleList>();

      UnitTestContext context = GetContext();
      ItemWithAsynchRuleList items = ItemWithAsynchRuleList.GetListWithItems(dataPortal);

      items[0].RuleField = "some value";
      context.Assert.IsTrue(items.IsBusy);
      context.Assert.IsFalse(items.IsSavable);
      bool gotError = false;
      try
      {
        items.Save();
      }
      catch (InvalidOperationException)
      {
        gotError = true;
      }
      context.Assert.IsTrue(gotError);
      context.Assert.Success();

      context.Complete();


    }

    [TestMethod]
    public async Task TestSaveWhileNotBusyNetOnly()
    {
      IDataPortal<ItemWithAsynchRule> dataPortal = _noCloneOnUpdateDIContext.CreateDataPortal<ItemWithAsynchRule>();

      UnitTestContext context = GetContext();
      var item = await dataPortal.FetchAsync("an id");
      item.ValidationComplete += (_, _) =>
      {
        context.Assert.IsFalse(item.IsBusy);
        context.Assert.IsTrue(item.IsSavable);
        item = item.Save();
        context.Assert.AreEqual("DataPortal_Update", item.OperationResult);
        context.Assert.Success();
      };
      item.RuleField = "some value";
      context.Assert.IsTrue(item.IsBusy);
      context.Assert.IsFalse(item.IsSavable);
      context.Complete();
    }

    [TestMethod]
    public void ListTestSaveWhileNotBusyNetOnly()
    {
      IDataPortal<ItemWithAsynchRuleList> dataPortal = _noCloneOnUpdateDIContext.CreateDataPortal<ItemWithAsynchRuleList>();

      UnitTestContext context = GetContext();
      ItemWithAsynchRuleList items = ItemWithAsynchRuleList.GetListWithItems(dataPortal);

     

      items[0].ValidationComplete += (_, _) =>
      {
        context.Assert.IsFalse(items.IsBusy);
        context.Assert.IsTrue(items.IsSavable);
        items = items.Save();
        context.Assert.AreEqual("DataPortal_Update", items[0].OperationResult);
        context.Assert.Success();
      };

      items[0].RuleField = "some value";
      context.Assert.IsTrue(items.IsBusy);
      context.Assert.IsFalse(items.IsSavable);

      context.Complete();

    }

    [TestMethod]
    public async Task TestSaveWhileNotBusyNoActiveRuleNetOnly()
    {
      IDataPortal<ItemWithAsynchRule> dataPortal = _noCloneOnUpdateDIContext.CreateDataPortal<ItemWithAsynchRule>();

      UnitTestContext context = GetContext();
      var item = await dataPortal.FetchAsync("an id");
      item.OperationResult = "something";
      context.Assert.IsFalse(item.IsBusy);
      context.Assert.IsTrue(item.IsSavable);
      item = item.Save();
      context.Assert.AreEqual("DataPortal_Update", item.OperationResult);
      context.Assert.Success();
      context.Complete();
    }

    [TestMethod]
    public void ListTestSaveWhileNotBusyNoActiveRuleNetOnly()
    {
      IDataPortal<ItemWithAsynchRuleList> dataPortal = _noCloneOnUpdateDIContext.CreateDataPortal<ItemWithAsynchRuleList>();

      UnitTestContext context = GetContext();
      ItemWithAsynchRuleList items = ItemWithAsynchRuleList.GetListWithItems(dataPortal);

      items[0].OperationResult = "something";
      context.Assert.IsFalse(items.IsBusy);
      context.Assert.IsTrue(items.IsSavable);
      items = items.Save();
      context.Assert.AreEqual("DataPortal_Update", items[0].OperationResult);
      context.Assert.Success();
      context.Complete();
    }

    [TestMethod]
    public async Task WaitForIdle_WhenBusyWillWaitUntilNotBusyAnymore() 
    {
      IDataPortal<ItemWithAsynchRule> dataPortal = _noCloneOnUpdateDIContext.CreateDataPortal<ItemWithAsynchRule>();

      UnitTestContext context = GetContext();
      var item = await dataPortal.FetchAsync("an id");
      item.RuleField = "some value";
      
      await item.WaitForIdle(TimeSpan.FromSeconds(5)); // Timeout should _never_ happen

      item.IsBusy.Should().BeFalse(because: $"{nameof(ItemWithAsynchRule.IsBusy)} is still true even though we waited with {nameof(ItemWithAsynchRule.WaitForIdle)}.");
    }

    [TestMethod]
    public async Task WaitForIdle_WhenReachingTheTimeoutATimeoutExceptionIsThrown() 
    {

      IDataPortal<ItemWithAsynchRule> dataPortal = _noCloneOnUpdateDIContext.CreateDataPortal<ItemWithAsynchRule>();

      UnitTestContext context = GetContext();
      var item = await dataPortal.FetchAsync("an id");
      item.RuleField = "some value";

      await item.Invoking(i => i.WaitForIdle(TimeSpan.FromMilliseconds(500))).Should().ThrowAsync<TimeoutException>();
    }
  }
}