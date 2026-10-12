//-----------------------------------------------------------------------
// <copyright file="TestObjects.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Business types with per-type authorization rules</summary>
//-----------------------------------------------------------------------

using System.ComponentModel;
using Csla.Rules;

namespace Csla.Xaml.Wpf.Tests
{
  /// <summary>
  /// Async rule that only completes after yielding,
  /// as a rule that calls a remote service would.
  /// </summary>
  public class DelayedAuthRule : AuthorizationRuleAsync
  {
    private readonly bool _hasPermission;

    public DelayedAuthRule(AuthorizationActions action, bool hasPermission)
      : base(action)
    {
      _hasPermission = hasPermission;
    }

    protected override async Task ExecuteAsync(IAuthorizationContext context, CancellationToken ct)
    {
      await Task.Delay(10, ct);
      context.HasPermission = _hasPermission;
    }
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

  [Serializable]
  public class AsyncAllowRoot : BusinessBase<AsyncAllowRoot>
  {
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void AddObjectAuthorizationRules()
    {
      BusinessRules.AddRule(typeof(AsyncAllowRoot), new DelayedAuthRule(AuthorizationActions.CreateObject, true));
      BusinessRules.AddRule(typeof(AsyncAllowRoot), new DelayedAuthRule(AuthorizationActions.GetObject, true));
      BusinessRules.AddRule(typeof(AsyncAllowRoot), new DelayedAuthRule(AuthorizationActions.EditObject, true));
      BusinessRules.AddRule(typeof(AsyncAllowRoot), new DelayedAuthRule(AuthorizationActions.DeleteObject, true));
    }

    [Create]
    private void Create()
    {
      BusinessRules.CheckRules();
    }
  }

  [Serializable]
  public class AsyncDenyRoot : BusinessBase<AsyncDenyRoot>
  {
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void AddObjectAuthorizationRules()
    {
      BusinessRules.AddRule(typeof(AsyncDenyRoot), new DelayedAuthRule(AuthorizationActions.CreateObject, true));
      BusinessRules.AddRule(typeof(AsyncDenyRoot), new DelayedAuthRule(AuthorizationActions.GetObject, true));
      BusinessRules.AddRule(typeof(AsyncDenyRoot), new DelayedAuthRule(AuthorizationActions.EditObject, false));
      BusinessRules.AddRule(typeof(AsyncDenyRoot), new DelayedAuthRule(AuthorizationActions.DeleteObject, false));
    }

    [Create]
    private void Create()
    {
      BusinessRules.CheckRules();
    }
  }

  [Serializable]
  public class SyncDenyEditRoot : BusinessBase<SyncDenyEditRoot>
  {
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void AddObjectAuthorizationRules()
    {
      BusinessRules.AddRule(typeof(SyncDenyEditRoot), new SyncAuthRule(AuthorizationActions.EditObject, false));
    }

    [Create]
    private void Create()
    {
      BusinessRules.CheckRules();
    }
  }

  [Serializable]
  public class AsyncAuthList : BusinessListBase<AsyncAuthList, AsyncAuthChild>
  {
    [Create]
    private void Create()
    { }
  }

  [Serializable]
  public class AsyncAuthChild : BusinessBase<AsyncAuthChild>
  {
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void AddObjectAuthorizationRules()
    {
      BusinessRules.AddRule(typeof(AsyncAuthChild), new DelayedAuthRule(AuthorizationActions.CreateObject, true));
      BusinessRules.AddRule(typeof(AsyncAuthChild), new DelayedAuthRule(AuthorizationActions.DeleteObject, false));
    }

    [CreateChild]
    private void Create()
    { }
  }
}
