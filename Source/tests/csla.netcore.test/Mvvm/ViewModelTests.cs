//-----------------------------------------------------------------------
// <copyright file="ViewModelTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Tests for Csla.Mvvm.ViewModel</summary>
//-----------------------------------------------------------------------

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
