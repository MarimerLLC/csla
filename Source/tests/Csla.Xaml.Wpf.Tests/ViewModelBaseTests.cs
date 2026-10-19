//-----------------------------------------------------------------------
// <copyright file="ViewModelBaseTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Tests for the WPF ViewModelBase wrapper over Csla.Mvvm.ViewModel</summary>
//-----------------------------------------------------------------------

using Csla.Configuration;
using Csla.Core;
using Csla.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Xaml.Wpf.Tests
{
  [TestClass]
  public class ViewModelBaseTests
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

    private class ReplacingViewModel : ViewModel<SavableRoot>
    {
      public SavableRoot Replacement { get; set; }

      protected override Task<SavableRoot> DoSaveAsync(ISavable cloned) => Task.FromResult(Replacement);
    }

    private class TrackingSetterViewModel : ViewModel<SavableRoot>
    {
      public List<bool> CanSaveValues { get; } = [];

      public override bool CanSave
      {
        get => base.CanSave;
        protected set
        {
          CanSaveValues.Add(value);
          base.CanSave = value;
        }
      }
    }

    private class DenyEditViewModel : ViewModel<SavableRoot>
    {
      public override bool CanEditObject => false;
    }

    // a common MVVM shape: state created in the constructor, used from OnPropertyChanged
    private class CommandRefreshingViewModel : ViewModel<SavableRoot>
    {
      private readonly List<string> _refreshed;

      public CommandRefreshingViewModel()
      {
        _refreshed = [];
      }

      protected override void OnPropertyChanged(string propertyName)
      {
        base.OnPropertyChanged(propertyName);
        _refreshed.Add(propertyName);   // null during base construction
      }
    }

    [TestMethod]
    public void DefaultsToManagingObjectLifetime()
    {
      DispatcherContext.Run(async () =>
      {
        var model = await _testHost.GetDataPortal<SavableRoot>().CreateAsync();
        var vm = new ViewModel<SavableRoot>();

        vm.Model = model;

        Assert.IsTrue(vm.ManageObjectLifetime);
        Assert.AreEqual(1, ((IUndoableObject)model).EditLevel);
      });
    }

    [TestMethod]
    public void SettingModelDependencyProperty_HooksModel()
    {
      DispatcherContext.Run(async () =>
      {
        var model = await _testHost.GetDataPortal<SavableRoot>().CreateAsync();
        var vm = new ViewModel<SavableRoot>();
        object sender = null;
        vm.ModelPropertyChanged += (s, _) => sender = s;

        vm.SetValue(ViewModelBase<SavableRoot>.ModelProperty, model);
        model.Name = "Changed";

        Assert.AreSame(vm, sender);
        Assert.IsTrue(vm.IsDirty);
      });
    }

    [TestMethod]
    public void SaveAsync_ReplacedModel_UpdatesDependencyProperty()
    {
      DispatcherContext.Run(async () =>
      {
        var model = await _testHost.GetDataPortal<SavableRoot>().CreateAsync();
        var replacement = await _testHost.GetDataPortal<SavableRoot>().CreateAsync();
        var vm = new ReplacingViewModel { Model = model, Replacement = replacement };
        model.Name = "Valid";

        await vm.SaveAsync();

        Assert.IsNull(vm.Exception);
        Assert.AreSame(replacement, vm.GetValue(ViewModelBase<SavableRoot>.ModelProperty));
        Assert.AreSame(replacement, vm.Model);
      });
    }

    [TestMethod]
    public void SaveAsync_Failure_CapturesExceptionWithViewModelAsSender()
    {
      DispatcherContext.Run(async () =>
      {
        var model = await _testHost.GetDataPortal<SavableRoot>().CreateAsync();
        var vm = new ViewModel<SavableRoot> { Model = model };
        object errorSender = null;
        vm.Error += (s, _) => errorSender = s;
        model.Name = SavableRoot.FailOnSave;

        await vm.SaveAsync();

        Assert.IsNotNull(vm.Exception);
        Assert.AreNotEqual(string.Empty, vm.ViewModelErrorText);
        Assert.AreSame(vm, errorSender);
      });
    }

    [TestMethod]
    public void OverriddenStatusSetter_IsCalledByStatusCalculation()
    {
      DispatcherContext.Run(async () =>
      {
        var model = await _testHost.GetDataPortal<SavableRoot>().CreateAsync();
        var vm = new TrackingSetterViewModel { Model = model };

        model.Name = "Valid";

        Assert.IsTrue(vm.CanSave);
        CollectionAssert.Contains(vm.CanSaveValues, true);
      });
    }

    [TestMethod]
    public void OverriddenObjectPermission_IsUsedForStatus()
    {
      DispatcherContext.Run(async () =>
      {
        var model = await _testHost.GetDataPortal<SavableRoot>().CreateAsync();
        var vm = new DenyEditViewModel { Model = model };

        model.Name = "Valid";

        Assert.IsTrue(model.IsSavable);
        Assert.IsFalse(vm.CanSave);
      });
    }

    [TestMethod]
    public void Construction_DoesNotCallOnPropertyChangedEarly()
    {
      DispatcherContext.Run(async () =>
      {
        await Task.Yield();
        _ = new CommandRefreshingViewModel();   // throws NullReferenceException today
      });
    }
  }

  [Serializable]
  public class SavableRoot : BusinessBase<SavableRoot>
  {
    public const string FailOnSave = "fail";

    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
    public string Name
    {
      get => GetProperty(NameProperty);
      set => SetProperty(NameProperty, value);
    }

    [Create]
    private void Create()
    { }

    [Insert]
    [Update]
    private void Save()
    {
      if (Name == FailOnSave)
        throw new InvalidOperationException("Save failed");
    }
  }
}
