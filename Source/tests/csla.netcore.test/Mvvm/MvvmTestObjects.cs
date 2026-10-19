//-----------------------------------------------------------------------
// <copyright file="MvvmTestObjects.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Business types used by the Csla.Mvvm tests</summary>
//-----------------------------------------------------------------------

using System.ComponentModel;
using Csla.Core;
using Csla.Rules;
using Csla.Rules.CommonRules;

namespace Csla.Test.Mvvm
{
  public class TestViewModel<T> : Csla.Mvvm.ViewModel<T>
  {
    public TestViewModel(ApplicationContext applicationContext)
      : base(applicationContext)
    { }

    public void Cancel() => DoCancel();
    public object AddNew() => DoAddNew();
    public void Remove(object item) => DoRemove(item);
    public void Delete() => DoDelete();

    public Func<ISavable, Task<T>> SaveOverride { get; set; }

    protected override Task<T> DoSaveAsync(ISavable cloned)
      => SaveOverride is null ? base.DoSaveAsync(cloned) : SaveOverride(cloned);
  }

  public class NoContextViewModel<T> : Csla.Mvvm.ViewModel<T>
  {
  }

  [Serializable]
  public class MvvmRoot : BusinessBase<MvvmRoot>
  {
    public const string FailOnSave = "fail";

    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
    public string Name
    {
      get => GetProperty(NameProperty);
      set => SetProperty(NameProperty, value);
    }

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();
      BusinessRules.AddRule(new Required(NameProperty));
    }

    [Create]
    private void Create()
    {
      BusinessRules.CheckRules();
    }

    [Fetch]
    private void Fetch(int id)
    {
      if (id < 0)
        throw new ArgumentOutOfRangeException(nameof(id));
      using (BypassPropertyChecks)
        Name = $"Fetched {id}";
      BusinessRules.CheckRules();
    }

    [Insert]
    [Update]
    private void Save()
    {
      if (Name == FailOnSave)
        throw new InvalidOperationException("Save failed");
    }

    [DeleteSelf]
    private void DeleteSelf()
    { }
  }

  [Serializable]
  public class BusyRoot : BusinessBase<BusyRoot>
  {
    public void StayBusy() => MarkBusy();

    [Create]
    private void Create()
    { }
  }

  [Serializable]
  public class MvvmChild : BusinessBase<MvvmChild>
  {
    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
    public string Name
    {
      get => GetProperty(NameProperty);
      set => SetProperty(NameProperty, value);
    }

    [CreateChild]
    private void Create()
    { }
  }

  [Serializable]
  public class MvvmList : BusinessListBase<MvvmList, MvvmChild>
  {
    [Create]
    private void Create()
    { }
  }

  public class SyncAuthRule : AuthorizationRule
  {
    private readonly bool _hasPermission;

    public SyncAuthRule(AuthorizationActions action, bool hasPermission)
      : base(action)
    {
      _hasPermission = hasPermission;
    }

    protected override void Execute(IAuthorizationContext context)
    {
      context.HasPermission = _hasPermission;
    }
  }

  /// <summary>
  /// Throws on its first evaluation only, to simulate a transient failure.
  /// </summary>
  public class FlakyAuthRule : AuthorizationRule
  {
    public static int Calls;

    public FlakyAuthRule(AuthorizationActions action)
      : base(action)
    { }

    protected override void Execute(IAuthorizationContext context)
    {
      if (Interlocked.Increment(ref Calls) == 1)
        throw new InvalidOperationException("transient failure in an authorization rule");
      context.HasPermission = true;
    }
  }

  [Serializable]
  public class FlakyAuthRoot : BusinessBase<FlakyAuthRoot>
  {
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void AddObjectAuthorizationRules()
    {
      BusinessRules.AddRule(typeof(FlakyAuthRoot), new FlakyAuthRule(AuthorizationActions.CreateObject));
    }

    [Fetch]
    private void Fetch(int id)
    { }
  }

  [Serializable]
  public class MvvmDenyEditRoot : BusinessBase<MvvmDenyEditRoot>
  {
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void AddObjectAuthorizationRules()
    {
      BusinessRules.AddRule(typeof(MvvmDenyEditRoot), new SyncAuthRule(AuthorizationActions.EditObject, false));
    }

    [Create]
    private void Create()
    { }
  }
}
