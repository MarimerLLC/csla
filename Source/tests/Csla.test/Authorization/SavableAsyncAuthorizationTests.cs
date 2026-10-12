//-----------------------------------------------------------------------
// <copyright file="SavableAsyncAuthorizationTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>IsSavable with async per-type authorization rules</summary>
//-----------------------------------------------------------------------

using System.ComponentModel;
using System.Security.Claims;
using Csla.Core;
using Csla.Rules;
using Csla.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.Authorization
{
  [TestClass]
  public class SavableAsyncAuthorizationTests
  {
    private static CslaTestHost _testHost;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
      _testHost = CslaTestHost.Create(o => o.ConfigureCsla(c => c.BindingOptions.PropertyChangedMode = ApplicationContext.PropertyChangedModes.Xaml));
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
      _testHost?.Dispose();
    }

    [TestMethod]
    public async Task AsyncRule_IsSavableIsFalseAndBusyUntilRuleCompletes()
    {
      var root = await _testHost.GetDataPortal<SavableAsyncAllowRoot>().CreateAsync();
      var busyChanges = new List<bool>();
      var changed = new List<string>();
      root.BusyChanged += (_, e) => busyChanges.Add(e.Busy);
      root.PropertyChanged += (_, e) => changed.Add(e.PropertyName);

      Assert.IsFalse(root.IsSavable);
      Assert.IsTrue(root.IsBusy);

      await WaitForIdleAsync(root);

      Assert.IsTrue(root.IsSavable);
      CollectionAssert.AreEqual(new[] { true, false }, busyChanges);
      CollectionAssert.Contains(changed, nameof(root.IsSavable));
    }

    [TestMethod]
    public async Task AsyncRule_ResultIsCachedForSameActionAndUser()
    {
      var root = await _testHost.GetDataPortal<SavableAsyncDenyRoot>().CreateAsync();
      var initialCount = SavableAsyncDenyRoot.Rule.Invocations;

      _ = root.IsSavable;
      await WaitForIdleAsync(root);

      Assert.IsFalse(root.IsSavable);
      Assert.IsFalse(root.IsSavable);
      Assert.IsFalse(root.IsBusy);
      Assert.AreEqual(initialCount + 1, SavableAsyncDenyRoot.Rule.Invocations);
    }

    [TestMethod]
    public async Task AsyncRule_IsReevaluatedWhenUserChanges()
    {
      var root = await _testHost.GetDataPortal<SavableAsyncDenyRoot>().CreateAsync();
      var applicationContext = _testHost.ApplicationContext;
      var originalUser = applicationContext.User;
      try
      {
        _ = root.IsSavable;
        await WaitForIdleAsync(root);
        var count = SavableAsyncDenyRoot.Rule.Invocations;

        applicationContext.User = new ClaimsPrincipal(new ClaimsIdentity("Test"));
        Assert.IsFalse(root.IsSavable);
        Assert.IsTrue(root.IsBusy);
        await WaitForIdleAsync(root);

        Assert.AreEqual(count + 1, SavableAsyncDenyRoot.Rule.Invocations);
      }
      finally
      {
        applicationContext.User = originalUser;
      }
    }

    [TestMethod]
    public async Task AsyncRuleThrows_IsSavableIsFalseAndUnhandledAsyncExceptionIsRaised()
    {
      var root = await _testHost.GetDataPortal<SavableAsyncThrowRoot>().CreateAsync();
      Csla.Core.ErrorEventArgs error = null;
      root.UnhandledAsyncException += (_, e) => error = e;

      _ = root.IsSavable;
      await WaitForIdleAsync(root);

      Assert.IsFalse(root.IsSavable);
      Assert.IsFalse(root.IsBusy);
      Assert.IsNotNull(error);
      Assert.IsInstanceOfType(error.Error, typeof(InvalidOperationException));
    }

    [TestMethod]
    public async Task SyncRule_IsEvaluatedOnEveryReadAndNeverBusy()
    {
      var root = await _testHost.GetDataPortal<SavableSyncRoot>().CreateAsync();
      var busyChanges = 0;
      root.BusyChanged += (_, _) => busyChanges++;
      var initialCount = SavableSyncRoot.Rule.Invocations;

      Assert.IsTrue(root.IsSavable);
      Assert.IsTrue(root.IsSavable);

      Assert.IsFalse(root.IsBusy);
      Assert.AreEqual(0, busyChanges);
      Assert.AreEqual(initialCount + 2, SavableSyncRoot.Rule.Invocations);
    }

    [TestMethod]
    public async Task List_AsyncRule_IsSavableIsFalseAndBusyUntilRuleCompletes()
    {
      var list = await _testHost.GetDataPortal<SavableAsyncList>().CreateAsync();
      list.AddNew();
      var busyChanges = new List<bool>();
      var changed = new List<string>();
      list.BusyChanged += (_, e) => busyChanges.Add(e.Busy);
      ((INotifyPropertyChanged)list).PropertyChanged += (_, e) => changed.Add(e.PropertyName);

      Assert.IsFalse(list.IsSavable);
      Assert.IsTrue(list.IsBusy);

      await WaitForIdleAsync(list);

      Assert.IsTrue(list.IsSavable);
      CollectionAssert.AreEqual(new[] { true, false }, busyChanges);
      CollectionAssert.Contains(changed, nameof(list.IsSavable));
    }

    private static async Task WaitForIdleAsync(INotifyBusy obj)
    {
      var start = Environment.TickCount64;
      while (obj.IsBusy)
      {
        if (Environment.TickCount64 - start > 5000)
          throw new TimeoutException("Object did not become idle");
        await Task.Delay(10);
      }
    }
  }

  public class CountingDelayedAuthRule : AuthorizationRuleAsync
  {
    private readonly bool _hasPermission;
    private readonly bool _throw;
    private int _invocations;

    public CountingDelayedAuthRule(AuthorizationActions action, bool hasPermission, bool throwException = false)
      : base(action)
    {
      _hasPermission = hasPermission;
      _throw = throwException;
    }

    public int Invocations => _invocations;

    protected override async Task ExecuteAsync(IAuthorizationContext context, CancellationToken ct)
    {
      Interlocked.Increment(ref _invocations);
      await Task.Delay(10, ct);
      if (_throw)
        throw new InvalidOperationException("Rule failed");
      context.HasPermission = _hasPermission;
    }
  }

  public class CountingSyncAuthRule : AuthorizationRule
  {
    private int _invocations;

    public CountingSyncAuthRule(AuthorizationActions action)
      : base(action)
    { }

    public int Invocations => _invocations;

    protected override void Execute(IAuthorizationContext context)
    {
      Interlocked.Increment(ref _invocations);
      context.HasPermission = true;
    }
  }

  [Serializable]
  public class SavableAsyncAllowRoot : BusinessBase<SavableAsyncAllowRoot>
  {
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void AddObjectAuthorizationRules()
    {
      BusinessRules.AddRule(typeof(SavableAsyncAllowRoot), new CountingDelayedAuthRule(AuthorizationActions.CreateObject, true));
    }

    [Create]
    private void Create()
    {
      BusinessRules.CheckRules();
    }
  }

  [Serializable]
  public class SavableAsyncDenyRoot : BusinessBase<SavableAsyncDenyRoot>
  {
    // the data portal checks CreateObject, so deny EditObject
    // and mark the object as old and dirty
    public static readonly CountingDelayedAuthRule Rule = new(AuthorizationActions.EditObject, false);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void AddObjectAuthorizationRules()
    {
      BusinessRules.AddRule(typeof(SavableAsyncDenyRoot), Rule);
    }

    [Create]
    private void Create()
    {
      BusinessRules.CheckRules();
      MarkOld();
      MarkDirty();
    }
  }

  [Serializable]
  public class SavableAsyncThrowRoot : BusinessBase<SavableAsyncThrowRoot>
  {
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void AddObjectAuthorizationRules()
    {
      BusinessRules.AddRule(typeof(SavableAsyncThrowRoot), new CountingDelayedAuthRule(AuthorizationActions.EditObject, true, throwException: true));
    }

    [Create]
    private void Create()
    {
      BusinessRules.CheckRules();
      MarkOld();
      MarkDirty();
    }
  }

  [Serializable]
  public class SavableSyncRoot : BusinessBase<SavableSyncRoot>
  {
    public static readonly CountingSyncAuthRule Rule = new(AuthorizationActions.CreateObject);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void AddObjectAuthorizationRules()
    {
      BusinessRules.AddRule(typeof(SavableSyncRoot), Rule);
    }

    [Create]
    private void Create()
    {
      BusinessRules.CheckRules();
    }
  }

  [Serializable]
  public class SavableAsyncList : BusinessListBase<SavableAsyncList, SavableAsyncChild>
  {
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void AddObjectAuthorizationRules()
    {
      BusinessRules.AddRule(typeof(SavableAsyncList), new CountingDelayedAuthRule(AuthorizationActions.EditObject, true));
    }

    [Create]
    private void Create()
    { }
  }

  [Serializable]
  public class SavableAsyncChild : BusinessBase<SavableAsyncChild>
  {
    [CreateChild]
    private void Create()
    {
      BusinessRules.CheckRules();
    }
  }
}
