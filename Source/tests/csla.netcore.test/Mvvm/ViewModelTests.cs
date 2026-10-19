//-----------------------------------------------------------------------
// <copyright file="ViewModelTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Tests for Csla.Mvvm.ViewModel</summary>
//-----------------------------------------------------------------------

using System.ComponentModel;
using Csla.Core;
using Csla.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.Mvvm
{
  [TestClass]
  public class ViewModelTests
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

    private static TestViewModel<T> CreateViewModel<T>() => new(_testHost.ApplicationContext);

    private static Task<MvvmRoot> CreateRootAsync() => _testHost.GetDataPortal<MvvmRoot>().CreateAsync();

    [TestMethod]
    public void ParameterlessConstructor_WithoutContext_Throws()
    {
      var vm = new NoContextViewModel<MvvmRoot>();

      var act = () => vm.CanEditObject;

      act.Should().Throw<InvalidOperationException>();
    }

    [TestMethod]
    public async Task ParameterlessConstructor_WithoutContext_SettingModelThrows()
    {
      var root = await CreateRootAsync();
      var vm = new NoContextViewModel<MvvmRoot>();

      var act = () => vm.Model = root;

      act.Should().Throw<InvalidOperationException>();
    }

    [TestMethod]
    public async Task SettingModel_SetsStatusAndRaisesPropertyChanged()
    {
      var root = await CreateRootAsync();
      var vm = CreateViewModel<MvvmRoot>();
      var changed = new List<string>();
      vm.PropertyChanged += (_, e) => changed.Add(e.PropertyName);

      vm.Model = root;

      changed.Should().Contain(nameof(vm.Model));
      vm.IsValid.Should().BeFalse();
      vm.CanSave.Should().BeFalse();

      root.Name = "Valid";

      vm.IsValid.Should().BeTrue();
      vm.CanSave.Should().BeTrue();
      changed.Should().Contain(nameof(vm.CanSave));
    }

    [TestMethod]
    public async Task ModelPropertyChanged_IsRaisedWithViewModelAsSender()
    {
      var root = await CreateRootAsync();
      var vm = CreateViewModel<MvvmRoot>();
      vm.Model = root;
      object sender = null;
      vm.ModelPropertyChanged += (s, _) => sender = s;

      root.Name = "Changed";

      sender.Should().BeSameAs(vm);
    }

    [TestMethod]
    public async Task ReplacingModel_UnhooksOldModel()
    {
      var oldRoot = await CreateRootAsync();
      var vm = CreateViewModel<MvvmRoot>();
      vm.Model = oldRoot;
      vm.Model = await CreateRootAsync();
      var raised = false;
      vm.ModelPropertyChanged += (_, _) => raised = true;

      oldRoot.Name = "Changed";

      raised.Should().BeFalse();
    }

    [TestMethod]
    public async Task SaveAsync_Success_RaisesSavedAndKeepsInstance()
    {
      var root = await CreateRootAsync();
      root.Name = "Valid";
      var vm = CreateViewModel<MvvmRoot>();
      vm.Model = root;
      var saved = false;
      vm.Saved += () => saved = true;

      var result = await vm.SaveAsync();

      saved.Should().BeTrue();
      vm.Exception.Should().BeNull();
      vm.ViewModelErrorText.Should().BeEmpty();
      result.Should().BeSameAs(root);
      root.IsNew.Should().BeFalse();
    }

    [TestMethod]
    public async Task SaveAsync_Failure_CapturesExceptionAndRaisesError()
    {
      var root = await CreateRootAsync();
      root.Name = MvvmRoot.FailOnSave;
      var vm = CreateViewModel<MvvmRoot>();
      vm.Model = root;
      object errorSender = null;
      vm.Error += (s, _) => errorSender = s;

      await vm.SaveAsync();

      vm.Exception.Should().NotBeNull();
      vm.ViewModelErrorText.Should().NotBeEmpty();
      errorSender.Should().BeSameAs(vm);
      vm.IsBusy.Should().BeFalse();
    }

    [TestMethod]
    public async Task SaveAsync_InvalidModel_SetsErrorTextWithoutSaving()
    {
      var root = await CreateRootAsync();
      var vm = CreateViewModel<MvvmRoot>();
      vm.Model = root;
      var saved = false;
      vm.Saved += () => saved = true;

      await vm.SaveAsync();

      saved.Should().BeFalse();
      vm.Exception.Should().BeNull();
      vm.ViewModelErrorText.Should().NotBeEmpty();
      root.IsNew.Should().BeTrue();
    }

    private static int CountNameChanges<T>(TestViewModel<T> vm, Action change)
    {
      var count = 0;
      PropertyChangedEventHandler handler = (_, e) =>
      {
        if (e.PropertyName == nameof(MvvmRoot.Name))
          count++;
      };
      vm.ModelPropertyChanged += handler;
      change();
      vm.ModelPropertyChanged -= handler;
      return count;
    }

    [TestMethod]
    public async Task SaveAsync_InvalidModel_DoesNotDuplicateHandlers()
    {
      var root = await CreateRootAsync();
      var vm = CreateViewModel<MvvmRoot>();
      vm.Model = root;

      await vm.SaveAsync();

      CountNameChanges(vm, () => root.Name = "Changed").Should().Be(1);
    }

    [TestMethod]
    public async Task SaveAsync_ReplacedModel_HooksNewModelOnce()
    {
      var root = await CreateRootAsync();
      root.Name = "Valid";
      var replacement = await CreateRootAsync();
      var vm = CreateViewModel<MvvmRoot>();
      vm.SaveOverride = _ => Task.FromResult(replacement);
      vm.Model = root;

      await vm.SaveAsync();

      vm.Model.Should().BeSameAs(replacement);
      CountNameChanges(vm, () => replacement.Name = "Changed").Should().Be(1);

      vm.Model = await CreateRootAsync();

      CountNameChanges(vm, () => replacement.Name = "Changed again").Should().Be(0);
    }

    [TestMethod]
    public async Task SaveAsync_WhileSaving_CanSaveIsFalse()
    {
      var root = await CreateRootAsync();
      root.Name = "Valid";
      var vm = CreateViewModel<MvvmRoot>();
      var pending = new TaskCompletionSource<MvvmRoot>();
      vm.SaveOverride = _ => pending.Task;
      vm.Model = root;
      vm.CanSave.Should().BeTrue();

      var save = vm.SaveAsync();

      vm.IsBusy.Should().BeTrue();
      vm.CanSave.Should().BeFalse();
      vm.CanCancel.Should().BeFalse();

      pending.SetResult(root);
      await save;

      vm.IsBusy.Should().BeFalse();
      vm.CanSave.Should().BeTrue();
    }

    [TestMethod]
    public async Task SaveAsync_WithoutManagedLifetime_StaysBusyUntilSaved()
    {
      var root = await CreateRootAsync();
      root.Name = "Valid";
      var vm = CreateViewModel<MvvmRoot>();
      vm.Model = root;
      bool? busyWhenSaved = null;
      vm.Saved += () => busyWhenSaved = vm.IsBusy;

      await vm.SaveAsync();

      vm.Exception.Should().BeNull();
      busyWhenSaved.Should().BeTrue();
      vm.IsBusy.Should().BeFalse();
    }

    [TestMethod]
    public async Task SaveAsync_NullModel_DoesNotRaiseSaved()
    {
      var vm = CreateViewModel<MvvmRoot>();
      var saved = false;
      vm.Saved += () => saved = true;

      await vm.SaveAsync();

      saved.Should().BeFalse();
      vm.Exception.Should().BeNull();
    }

    [TestMethod]
    public async Task SaveAsync_BusyPastTimeout_ReportsTimeoutException()
    {
      var root = await _testHost.GetDataPortal<BusyRoot>().CreateAsync();
      root.StayBusy();
      var vm = CreateViewModel<BusyRoot>();
      vm.Model = root;
      vm.BusyTimeout = TimeSpan.FromMilliseconds(200);

      await vm.SaveAsync();

      vm.Exception.Should().BeOfType<TimeoutException>();
    }

    [TestMethod]
    public async Task ManageObjectLifetime_SaveKeepsEditLevel()
    {
      var root = await CreateRootAsync();
      var vm = CreateViewModel<MvvmRoot>();
      vm.ManageObjectLifetime = true;
      vm.Model = root;
      ((IUndoableObject)vm.Model).EditLevel.Should().Be(1);
      vm.Model.Name = "Valid";

      await vm.SaveAsync();

      vm.Exception.Should().BeNull();
      ((IUndoableObject)vm.Model).EditLevel.Should().Be(1);
      vm.Model.IsNew.Should().BeFalse();
    }

    [TestMethod]
    public async Task ManageObjectLifetime_CancelRevertsChanges()
    {
      var root = await CreateRootAsync();
      root.Name = "Original";
      var vm = CreateViewModel<MvvmRoot>();
      vm.ManageObjectLifetime = true;
      vm.Model = root;
      root.Name = "Changed";

      vm.Cancel();

      root.Name.Should().Be("Original");
      ((IUndoableObject)root).EditLevel.Should().Be(1);
    }

    [TestMethod]
    public async Task ManageObjectLifetime_SaveSucceeds_CancelKeepsSavedValues()
    {
      var root = await CreateRootAsync();
      root.Name = "Original";
      var vm = CreateViewModel<MvvmRoot>();
      vm.ManageObjectLifetime = true;
      vm.Model = root;
      root.Name = "Saved";

      await vm.SaveAsync();
      vm.Exception.Should().BeNull();

      vm.Cancel();

      vm.Model.Name.Should().Be("Saved");
      ((IUndoableObject)vm.Model).EditLevel.Should().Be(1);
    }

    [TestMethod]
    public async Task ManageObjectLifetime_SaveFails_CancelRestoresOriginal()
    {
      var root = await CreateRootAsync();
      root.Name = "Original";
      var vm = CreateViewModel<MvvmRoot>();
      vm.ManageObjectLifetime = true;
      vm.Model = root;                  // BeginEdit: snapshot has "Original"
      root.Name = MvvmRoot.FailOnSave;  // the edit the server rejects

      await vm.SaveAsync();
      vm.Exception.Should().NotBeNull();

      vm.Cancel();

      vm.Model.Name.Should().Be("Original");
    }

    [TestMethod]
    public async Task RefreshAsync_Success_SetsModel()
    {
      var vm = CreateViewModel<MvvmRoot>();

      var result = await vm.RefreshAsync(() => _testHost.GetDataPortal<MvvmRoot>().FetchAsync(1));

      result.Should().BeSameAs(vm.Model);
      vm.Model.Name.Should().Be("Fetched 1");
      vm.Exception.Should().BeNull();
      vm.IsBusy.Should().BeFalse();
    }

    [TestMethod]
    public async Task RefreshAsync_Failure_CapturesException()
    {
      var vm = CreateViewModel<MvvmRoot>();
      vm.Model = await CreateRootAsync();

      await vm.RefreshAsync(() => _testHost.GetDataPortal<MvvmRoot>().FetchAsync(-1));

      vm.Model.Should().BeNull();
      vm.Exception.Should().NotBeNull();
      vm.ViewModelErrorText.Should().NotBeEmpty();
      vm.IsBusy.Should().BeFalse();
    }

    [TestMethod]
    public async Task RefreshFailure_RaisesError()
    {
      var vm = CreateViewModel<MvvmRoot>();
      var raised = false;
      vm.Error += (_, _) => raised = true;

      await vm.RefreshAsync(() => _testHost.GetDataPortal<MvvmRoot>().FetchAsync(-1));

      vm.Exception.Should().NotBeNull();
      raised.Should().BeTrue();
    }

    [TestMethod]
    public async Task ListModel_AddNewAndRemove()
    {
      var list = await _testHost.GetDataPortal<MvvmList>().CreateAsync();
      var vm = CreateViewModel<MvvmList>();
      vm.Model = list;
      vm.CanAddNew.Should().BeTrue();
      vm.CanRemove.Should().BeFalse();

      var item = vm.AddNew();

      list.Should().HaveCount(1);
      vm.CanRemove.Should().BeTrue();

      vm.Remove(item);

      list.Should().BeEmpty();
      vm.CanRemove.Should().BeFalse();
    }

    [TestMethod]
    public async Task SyncRules_PermissionsAreSetImmediately()
    {
      var root = await _testHost.GetDataPortal<MvvmDenyEditRoot>().CreateAsync();
      var vm = CreateViewModel<MvvmDenyEditRoot>();

      vm.Model = root;

      vm.CanCreateObject.Should().BeTrue();
      vm.CanGetObject.Should().BeTrue();
      vm.CanEditObject.Should().BeFalse();
      vm.CanDeleteObject.Should().BeTrue();
      vm.CanSave.Should().BeFalse();
      vm.CanDelete.Should().BeTrue();
    }

    [TestMethod]
    public async Task PermissionRuleFailure_IsReportedThroughError()
    {
      var root = await _testHost.GetDataPortal<ThrowingAuthRoot>().CreateAsync();
      var vm = CreateViewModel<ThrowingAuthRoot>();
      Exception reported = null;
      vm.Error += (_, e) => reported = e.Error;

      vm.Model = root;

      vm.Exception.Should().NotBeNull();
      vm.ViewModelErrorText.Should().NotBeEmpty();
      reported.Should().BeSameAs(vm.Exception);
      vm.CanSave.Should().BeFalse();
    }

    [TestMethod]
    public async Task PersistentStatusRuleFailure_IsReportedOnce()
    {
      InstanceOnlyFailRule.Failing = true;
      var root = await _testHost.GetDataPortal<InstanceFailRoot>().CreateAsync();
      var vm = CreateViewModel<InstanceFailRoot>();
      var errors = 0;
      vm.Error += (_, _) => errors++;

      vm.Model = root;
      root.Name = "a";
      root.Name = "ab";
      root.Name = "abc";

      errors.Should().Be(1);
    }

    [TestMethod]
    public async Task StatusRuleFailure_IsReportedAgainAfterRecovery()
    {
      InstanceOnlyFailRule.Failing = true;
      var root = await _testHost.GetDataPortal<InstanceFailRoot>().CreateAsync();
      var vm = CreateViewModel<InstanceFailRoot>();
      var errors = 0;
      vm.Error += (_, _) => errors++;

      try
      {
        vm.Model = root;
        InstanceOnlyFailRule.Failing = false;
        root.Name = "a";
        vm.CanDelete.Should().BeTrue();
        InstanceOnlyFailRule.Failing = true;
        root.Name = "ab";
      }
      finally
      {
        InstanceOnlyFailRule.Failing = true;
      }

      errors.Should().Be(2);
    }

    [TestMethod]
    public async Task TransientPermissionFailure_LaterRefreshSucceeds()
    {
      FlakyAuthRule.Calls = 0;
      var portal = _testHost.GetDataPortal<FlakyAuthRoot>();
      var vm = CreateViewModel<FlakyAuthRoot>();

      await vm.RefreshAsync(() => portal.FetchAsync(1));   // fails: transient
      await vm.RefreshAsync(() => portal.FetchAsync(1));   // should succeed

      vm.Exception.Should().BeNull();
    }

    [TestMethod]
    public async Task Delete_MarksModelDeleted()
    {
      var root = await CreateRootAsync();
      var vm = CreateViewModel<MvvmRoot>();
      vm.Model = root;

      vm.Delete();

      root.IsDeleted.Should().BeTrue();
    }
  }
}
