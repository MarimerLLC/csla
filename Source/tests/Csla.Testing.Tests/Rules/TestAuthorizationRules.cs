//-----------------------------------------------------------------------
// <copyright file="TestAuthorizationRules.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Sample authorization rules used by the rule tester tests</summary>
//-----------------------------------------------------------------------

using Csla.Rules;

namespace Csla.Testing.Tests.Rules
{
  /// <summary>
  /// Synchronous authorization rule that grants permission when the current
  /// user is in one of the specified roles.
  /// </summary>
  public class TestIsInRoleRule : AuthorizationRule
  {
    private readonly string[] _roles;

    public TestIsInRoleRule(AuthorizationActions action, params string[] roles)
      : base(action)
    {
      _roles = roles;
    }

    protected override void Execute(IAuthorizationContext context)
    {
      var user = context.ApplicationContext.User;
      context.HasPermission = _roles.Any(user.IsInRole);
    }
  }

  /// <summary>
  /// Asynchronous authorization rule that grants permission when the current
  /// user is in one of the specified roles.
  /// </summary>
  public class TestIsInRoleAsyncRule : AuthorizationRuleAsync
  {
    private readonly string[] _roles;

    public TestIsInRoleAsyncRule(AuthorizationActions action, params string[] roles)
      : base(action)
    {
      _roles = roles;
    }

    public CancellationToken SawToken { get; private set; }

    protected override async Task ExecuteAsync(IAuthorizationContext context, CancellationToken ct)
    {
      await Task.Yield();
      SawToken = ct;
      var user = context.ApplicationContext.User;
      context.HasPermission = _roles.Any(user.IsInRole);
    }
  }

  /// <summary>
  /// Authorization rule that records the context it was given, so a test can
  /// assert on criteria, target and target type.
  /// </summary>
  public class ContextProbeAuthorizationRule : AuthorizationRule
  {
    public ContextProbeAuthorizationRule(AuthorizationActions action)
      : base(action)
    {
    }

    public object?[]? SawCriteria { get; private set; }
    public object? SawTarget { get; private set; }
    public Type? SawTargetType { get; private set; }
    public bool SawAuthenticated { get; private set; }

    protected override void Execute(IAuthorizationContext context)
    {
      SawCriteria = context.Criteria;
      SawTarget = context.Target;
      SawTargetType = context.TargetType;
      SawAuthenticated = context.ApplicationContext.User.Identity?.IsAuthenticated ?? false;
      context.HasPermission = true;
    }
  }
}
