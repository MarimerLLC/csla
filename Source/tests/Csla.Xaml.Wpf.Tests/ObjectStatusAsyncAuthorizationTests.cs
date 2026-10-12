//-----------------------------------------------------------------------
// <copyright file="ObjectStatusAsyncAuthorizationTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Tests for ObjectStatus with async authorization rules</summary>
//-----------------------------------------------------------------------

using Csla.Configuration;
using Csla.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Xaml.Wpf.Tests
{
  [TestClass]
  public class ObjectStatusAsyncAuthorizationTests
  {
    private static CslaTestHost _testHost;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
      _testHost = CslaTestHost.Create(o => o.ConfigureCsla(c => c.AddXaml()));
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
      _testHost?.Dispose();
    }

    [TestMethod]
    public void AsyncRules_PermissionsAreSetWhenRulesComplete()
    {
      DispatcherContext.Run(async () =>
      {
        var model = await _testHost.GetDataPortal<AsyncDenyRoot>().CreateAsync();
        var status = new ObjectStatus();

        status.DataContext = model;
        await DispatcherContext.WaitUntilAsync(() => status.CanGetObject);
        await Task.Delay(100);

        Assert.IsTrue(status.CanCreateObject);
        Assert.IsTrue(status.CanGetObject);
        Assert.IsFalse(status.CanEditObject);
        Assert.IsFalse(status.CanDeleteObject);
      });
    }

    [TestMethod]
    public void SyncRules_PermissionsAreSetImmediately()
    {
      DispatcherContext.Run(async () =>
      {
        var model = await _testHost.GetDataPortal<SyncDenyEditRoot>().CreateAsync();
        var status = new ObjectStatus();

        status.DataContext = model;

        Assert.IsTrue(status.CanCreateObject);
        Assert.IsTrue(status.CanGetObject);
        Assert.IsFalse(status.CanEditObject);
        Assert.IsTrue(status.CanDeleteObject);
      });
    }
  }
}
