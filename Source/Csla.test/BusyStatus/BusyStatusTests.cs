//-----------------------------------------------------------------------
// <copyright file="BusyStatusTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using Csla;
using Csla.DataPortalClient;
using Csla.Testing.Business.ReadOnlyTest;
using System;
using UnitDriven;
using Csla.Testing.Business.BusyStatus;
using System.Threading.Tasks;

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

namespace cslalighttest.BusyStatus
{
  [TestClass]
  public class BusyStatusTests : TestBase
  {

    [TestMethod]
    public async Task TestBusy()
    {
      System.Configuration.ConfigurationManager.AppSettings["CslaAutoCloneOnUpdate"] = "false";
      UnitTestContext context = GetContext();
      var item = await DataPortal.FetchAsync<ItemWithAsynchRule>("an id");
      item.RuleField = "some value";
      context.Assert.IsTrue(item.IsBusy, "Should be busy");
      context.Assert.IsFalse(item.IsSavable, "Should not be savable");
      context.Assert.Success();
      context.Complete();
    }

    [TestMethod]
    public void ListTestBusy()
    {
      System.Configuration.ConfigurationManager.AppSettings["CslaAutoCloneOnUpdate"] = "false";
      UnitTestContext context = GetContext();
      ItemWithAsynchRuleList items = ItemWithAsynchRuleList.GetListWithItems();
      items[0].RuleField = "some value";
      context.Assert.IsTrue(items.IsBusy);
      context.Assert.IsFalse(items.IsSavable);
      context.Assert.Success();
      context.Complete();
    }

    [TestMethod]
    public async Task TestSaveWhileBusy()
    {
      UnitTestContext context = GetContext();
      var item = await DataPortal.FetchAsync<ItemWithAsynchRule>("an id");
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
      System.Configuration.ConfigurationManager.AppSettings["CslaAutoCloneOnUpdate"] = "false";
      UnitTestContext context = GetContext();
      ItemWithAsynchRuleList items = ItemWithAsynchRuleList.GetListWithItems();
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
      UnitTestContext context = GetContext();
      var item = await DataPortal.FetchAsync<ItemWithAsynchRule>("an id");
      item.ValidationComplete += (o2, e2) =>
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
      System.Configuration.ConfigurationManager.AppSettings["CslaAutoCloneOnUpdate"] = "false";
      UnitTestContext context = GetContext();
      ItemWithAsynchRuleList items = ItemWithAsynchRuleList.GetListWithItems();
      items[0].ValidationComplete += (o2, e2) =>
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

      System.Configuration.ConfigurationManager.AppSettings["CslaAutoCloneOnUpdate"] = "false";
      UnitTestContext context = GetContext();
      ItemWithAsynchRuleList items = ItemWithAsynchRuleList.GetListWithItems();
      items[0].ValidationComplete += async (o2, e2) =>
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

      UnitTestContext context = GetContext();
      var item = await DataPortal.FetchAsync<ItemWithAsynchRule>("an id");
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
      System.Configuration.ConfigurationManager.AppSettings["CslaAutoCloneOnUpdate"] = "false";
      UnitTestContext context = GetContext();
      ItemWithAsynchRuleList items = ItemWithAsynchRuleList.GetListWithItems();

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
      UnitTestContext context = GetContext();
      System.Configuration.ConfigurationManager.AppSettings["CslaAutoCloneOnUpdate"] = "false";
      var item = await DataPortal.FetchAsync<ItemWithAsynchRule>("an id");
      item.ValidationComplete += (o2, e2) =>
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
      System.Configuration.ConfigurationManager.AppSettings["CslaAutoCloneOnUpdate"] = "false";
      UnitTestContext context = GetContext();
      ItemWithAsynchRuleList items = ItemWithAsynchRuleList.GetListWithItems();

     

      items[0].ValidationComplete += (o2, e2) =>
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
      UnitTestContext context = GetContext();
      System.Configuration.ConfigurationManager.AppSettings["CslaAutoCloneOnUpdate"] = "false";
      var item = await DataPortal.FetchAsync<ItemWithAsynchRule>("an id");
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
      System.Configuration.ConfigurationManager.AppSettings["CslaAutoCloneOnUpdate"] = "false";
      UnitTestContext context = GetContext();
      ItemWithAsynchRuleList items = ItemWithAsynchRuleList.GetListWithItems();

      items[0].OperationResult = "something";
      context.Assert.IsFalse(items.IsBusy);
      context.Assert.IsTrue(items.IsSavable);
      items = items.Save();
      context.Assert.AreEqual("DataPortal_Update", items[0].OperationResult);
      context.Assert.Success();
      context.Complete();
    }
  }
}