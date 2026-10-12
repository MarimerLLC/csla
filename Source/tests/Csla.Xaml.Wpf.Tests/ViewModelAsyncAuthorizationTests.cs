//-----------------------------------------------------------------------
// <copyright file="ViewModelAsyncAuthorizationTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Tests for ViewModel with async authorization rules</summary>
//-----------------------------------------------------------------------

using Csla.Configuration;
using Csla.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Xaml.Wpf.Tests
{
  [TestClass]
  public class ViewModelAsyncAuthorizationTests
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
    public void AsyncRulesAllowed_PermissionsAreSetWhenRulesComplete()
    {
      DispatcherContext.Run(async () =>
      {
        var model = await _testHost.GetDataPortal<AsyncAllowRoot>().CreateAsync();
        var vm = new ViewModel<AsyncAllowRoot>();

        vm.Model = model;
        await DispatcherContext.WaitUntilAsync(() => vm.CanDelete && vm.CanSave);

        Assert.IsTrue(vm.CanCreateObject);
        Assert.IsTrue(vm.CanGetObject);
        Assert.IsTrue(vm.CanEditObject);
        Assert.IsTrue(vm.CanDeleteObject);
        Assert.IsTrue(vm.CanSave);
        Assert.IsTrue(vm.CanCancel);
      });
    }

    [TestMethod]
    public void AsyncRulesDenied_PermissionsAreSetWhenRulesComplete()
    {
      DispatcherContext.Run(async () =>
      {
        var model = await _testHost.GetDataPortal<AsyncDenyRoot>().CreateAsync();
        var vm = new ViewModel<AsyncDenyRoot>();

        vm.Model = model;
        await DispatcherContext.WaitUntilAsync(() => vm.CanGetObject);
        // let the instance-level rules complete too
        await Task.Delay(100);

        Assert.IsTrue(vm.CanCreateObject);
        Assert.IsTrue(vm.CanGetObject);
        Assert.IsFalse(vm.CanEditObject);
        Assert.IsFalse(vm.CanDeleteObject);
        Assert.IsFalse(vm.CanSave);
        Assert.IsFalse(vm.CanCancel);
        Assert.IsFalse(vm.CanDelete);
      });
    }

    [TestMethod]
    public void AsyncRules_PropertyChangedIsRaisedWhenRulesComplete()
    {
      DispatcherContext.Run(async () =>
      {
        var model = await _testHost.GetDataPortal<AsyncAllowRoot>().CreateAsync();
        var vm = new ViewModel<AsyncAllowRoot>();
        var changed = new List<string>();
        vm.PropertyChanged += (_, e) => changed.Add(e.PropertyName);

        vm.Model = model;
        await DispatcherContext.WaitUntilAsync(() => vm.CanDelete && vm.CanSave);

        CollectionAssert.Contains(changed, nameof(vm.CanEditObject));
        CollectionAssert.Contains(changed, nameof(vm.CanSave));
        CollectionAssert.Contains(changed, nameof(vm.CanDelete));
      });
    }

    [TestMethod]
    public void AsyncCreateRule_CanSaveIsSetWhenIsSavableRuleCompletes()
    {
      DispatcherContext.Run(async () =>
      {
        var model = await _testHost.GetDataPortal<AsyncAllowRoot>().CreateAsync();
        var vm = new ViewModel<AsyncAllowRoot>();
        var modelBusy = new List<bool>();
        model.BusyChanged += (_, e) => modelBusy.Add(e.Busy);

        vm.Model = model;
        Assert.IsFalse(vm.CanSave);

        await DispatcherContext.WaitUntilAsync(() => vm.CanSave);

        // IsSavable ran the async CreateObject rule, so the model was
        // busy until the rule completed
        CollectionAssert.AreEqual(new[] { true, false }, modelBusy);
        Assert.IsFalse(model.IsBusy);
        Assert.IsFalse(vm.IsBusy);
        Assert.IsTrue(model.IsSavable);
      });
    }

    [TestMethod]
    public void AsyncChildRules_ListPermissionsAreSetWhenRulesComplete()
    {
      DispatcherContext.Run(async () =>
      {
        var model = await _testHost.GetDataPortal<AsyncAuthList>().CreateAsync();
        var vm = new ViewModel<AsyncAuthList>();

        vm.Model = model;
        await DispatcherContext.WaitUntilAsync(() => vm.CanAddNew);

        Assert.IsTrue(vm.CanAddNew);
        Assert.IsFalse(vm.CanRemove);
      });
    }

    [TestMethod]
    public void SyncRules_PermissionsAreSetImmediately()
    {
      DispatcherContext.Run(async () =>
      {
        var model = await _testHost.GetDataPortal<SyncDenyEditRoot>().CreateAsync();
        var vm = new ViewModel<SyncDenyEditRoot>();

        vm.Model = model;

        // no await between setting Model and checking the values
        Assert.IsTrue(vm.CanCreateObject);
        Assert.IsTrue(vm.CanGetObject);
        Assert.IsFalse(vm.CanEditObject);
        Assert.IsTrue(vm.CanDeleteObject);
        Assert.IsFalse(vm.CanSave);
        Assert.IsTrue(vm.CanDelete);
      });
    }
  }
}
