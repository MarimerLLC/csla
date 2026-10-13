//-----------------------------------------------------------------------
// <copyright file="SavablePermission.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Evaluates the per-type authorization rule used by IsSavable</summary>
//-----------------------------------------------------------------------

using System.Security.Principal;

namespace Csla.Rules
{
  /// <summary>
  /// Evaluates the per-type authorization rule used by IsSavable,
  /// supporting async rules that do not complete synchronously.
  /// </summary>
  /// <remarks>
  /// Rules that complete successfully and synchronously are
  /// evaluated on every call.
  /// When a rule is still running, false is returned and the owner
  /// is notified so it can mark itself busy; when the rule completes,
  /// its result is cached for the same action and principal, and the
  /// owner is notified again. A rule that fails, whether synchronously
  /// or asynchronously, is handled the same way: false is cached and
  /// the exception is passed to the owner.
  /// </remarks>
  internal sealed class SavablePermission
  {
    private AuthorizationActions _action;
    private IPrincipal? _principal;
    private bool _hasResult;
    private bool _result;
    private int _version;
    private int _pendingCount;

    /// <summary>
    /// Gets a value indicating whether an async
    /// rule is still being evaluated.
    /// </summary>
    public bool IsPending => _pendingCount > 0;

    /// <summary>
    /// Checks the per-type authorization rule for the action.
    /// </summary>
    /// <param name="applicationContext">The application context.</param>
    /// <param name="action">The authorization action.</param>
    /// <param name="target">The business object.</param>
    /// <param name="onPending">Invoked when an async rule starts running.</param>
    /// <param name="onCompleted">Invoked when that rule completes, with the exception if it failed.</param>
    /// <returns>The permission, or false while an async rule is running.</returns>
    public bool HasPermission(ApplicationContext applicationContext, AuthorizationActions action, object target, Action onPending, Action<Exception?> onCompleted)
    {
      var principal = applicationContext.User;
      if (_hasResult && _action == action && ReferenceEquals(_principal, principal))
        return _result;

      var task = BusinessRules.HasPermissionAsync(applicationContext, action, target, CancellationToken.None);
      // only fast-path success; a faulted or canceled task (e.g. a rule
      // that throws before its first await) goes through CompleteAsync
      // so the failure is handled the same way as a later failure
      if (task.Status == TaskStatus.RanToCompletion)
        return task.Result;

      _action = action;
      _principal = principal;
      _hasResult = false;
      var version = ++_version;
      _pendingCount++;
      onPending();
      _ = CompleteAsync(task, version, onCompleted);
      return false;
    }

    private async Task CompleteAsync(Task<bool> task, int version, Action<Exception?> onCompleted)
    {
      var result = false;
      Exception? error = null;
      try
      {
        result = await task;
      }
      catch (Exception ex)
      {
        error = ex;
      }

      if (version == _version)
      {
        _result = result;
        _hasResult = true;
      }
      _pendingCount--;
      onCompleted(error);
    }
  }
}
