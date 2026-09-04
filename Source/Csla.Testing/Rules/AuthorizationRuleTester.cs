//-----------------------------------------------------------------------
// <copyright file="AuthorizationRuleTester.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Executes an authorization rule in isolation so it can be unit tested</summary>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Security.Principal;
using Csla.Configuration;
using Csla.Rules;
using Microsoft.Extensions.DependencyInjection;

namespace Csla.Testing.Rules
{
  /// <summary>
  /// Executes a single authorization rule in isolation so its behavior can be
  /// asserted in a unit test, without requiring a business object or any
  /// hand-written <see cref="ApplicationContext"/> bootstrap.
  /// </summary>
  /// <remarks>
  /// <para>
  /// <see cref="ExecuteAsync"/> runs both <see cref="IAuthorizationRule"/> and
  /// <see cref="IAuthorizationRuleAsync"/> rules, so a test does not need to
  /// branch on whether the rule under test is synchronous or asynchronous.
  /// </para>
  /// <example>
  /// <code>
  /// var result = await AuthorizationRuleTester
  ///   .For(new IsInRole(AuthorizationActions.EditObject, "Admin"))
  ///   .ForType&lt;MyBO&gt;()
  ///   .AsUser("rocky", "Admin")
  ///   .ExecuteAsync();
  /// </code>
  /// </example>
  /// </remarks>
  public sealed class AuthorizationRuleTester
  {
    private readonly IAuthorizationRuleBase _rule;
    private object? _target;
    private Type? _targetType;
    private object?[]? _criteria;
    private IPrincipal? _principal;
    private Action<CslaOptions>? _configureCsla;
    private Action<IServiceCollection>? _configureServices;
    private ApplicationContext? _applicationContext;

    private AuthorizationRuleTester(IAuthorizationRuleBase rule)
    {
      _rule = rule;
    }

    /// <summary>
    /// Creates a tester for the specified authorization rule.
    /// </summary>
    /// <param name="rule">The rule to execute.</param>
    /// <exception cref="ArgumentNullException"><paramref name="rule"/> is <see langword="null"/>.</exception>
    public static AuthorizationRuleTester For(IAuthorizationRuleBase rule)
    {
      if (rule is null)
        throw new ArgumentNullException(nameof(rule));

      return new AuthorizationRuleTester(rule);
    }

    /// <summary>
    /// Supplies the target business object for the rule. When
    /// <see cref="ForType(Type)"/> is not used, the target type is inferred
    /// from this object.
    /// </summary>
    /// <param name="target">The target business object.</param>
    /// <returns>This instance, to support method chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="target"/> is <see langword="null"/>.</exception>
    public AuthorizationRuleTester OnTarget(object target)
    {
      _target = target ?? throw new ArgumentNullException(nameof(target));
      return this;
    }

    /// <summary>
    /// Supplies the type of the target business class.
    /// </summary>
    /// <param name="targetType">Type of the target business class.</param>
    /// <returns>This instance, to support method chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="targetType"/> is <see langword="null"/>.</exception>
    public AuthorizationRuleTester ForType(Type targetType)
    {
      _targetType = targetType ?? throw new ArgumentNullException(nameof(targetType));
      return this;
    }

    /// <summary>
    /// Supplies the type of the target business class.
    /// </summary>
    /// <typeparam name="T">Type of the target business class.</typeparam>
    /// <returns>This instance, to support method chaining.</returns>
    public AuthorizationRuleTester ForType<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>() => ForType(typeof(T));

    /// <summary>
    /// Supplies the criteria made available to the rule, as the data portal
    /// does for a type-level authorization rule.
    /// </summary>
    /// <param name="criteria">The criteria values.</param>
    /// <returns>This instance, to support method chaining.</returns>
    public AuthorizationRuleTester WithCriteria(params object?[]? criteria)
    {
      _criteria = criteria;
      return this;
    }

    /// <summary>
    /// Runs the rule as an authenticated user with the specified name and roles.
    /// </summary>
    /// <param name="name">Name of the user.</param>
    /// <param name="roles">Roles held by the user.</param>
    /// <returns>This instance, to support method chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> or <paramref name="roles"/> is <see langword="null"/>.</exception>
    public AuthorizationRuleTester AsUser(string name, params string[] roles)
    {
      _principal = TestPrincipalFactory.CreateUser(name, roles);
      return this;
    }

    /// <summary>
    /// Runs the rule as an unauthenticated user.
    /// </summary>
    /// <returns>This instance, to support method chaining.</returns>
    public AuthorizationRuleTester AsUnauthenticated()
    {
      _principal = TestPrincipalFactory.CreateUnauthenticated();
      return this;
    }

    /// <summary>
    /// Runs the rule as the specified principal.
    /// </summary>
    /// <param name="principal">The principal to use.</param>
    /// <returns>This instance, to support method chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="principal"/> is <see langword="null"/>.</exception>
    public AuthorizationRuleTester AsPrincipal(IPrincipal principal)
    {
      _principal = principal ?? throw new ArgumentNullException(nameof(principal));
      return this;
    }

    /// <summary>
    /// Configures the CSLA .NET options used to create the application context
    /// for the rule. Ignored when <see cref="UsingApplicationContext"/> is used.
    /// </summary>
    /// <param name="configure">Callback used to configure CSLA .NET.</param>
    /// <returns>This instance, to support method chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="configure"/> is <see langword="null"/>.</exception>
    public AuthorizationRuleTester ConfigureCsla(Action<CslaOptions> configure)
    {
      _configureCsla = configure ?? throw new ArgumentNullException(nameof(configure));
      return this;
    }

    /// <summary>
    /// Registers additional services available to the rule through the
    /// application context. Ignored when <see cref="UsingApplicationContext"/>
    /// is used.
    /// </summary>
    /// <param name="configure">Callback used to register services.</param>
    /// <returns>This instance, to support method chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="configure"/> is <see langword="null"/>.</exception>
    public AuthorizationRuleTester ConfigureServices(Action<IServiceCollection> configure)
    {
      _configureServices = configure ?? throw new ArgumentNullException(nameof(configure));
      return this;
    }

    /// <summary>
    /// Uses an existing application context instead of creating one. This opts
    /// out of the built-in service bootstrap entirely.
    /// </summary>
    /// <param name="applicationContext">The application context to use.</param>
    /// <returns>This instance, to support method chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="applicationContext"/> is <see langword="null"/>.</exception>
    public AuthorizationRuleTester UsingApplicationContext(ApplicationContext applicationContext)
    {
      _applicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
      return this;
    }

    /// <summary>
    /// Executes the rule, awaiting it when it is asynchronous, and returns the
    /// outcome.
    /// </summary>
    /// <param name="ct">Cancellation token passed to an asynchronous rule.</param>
    /// <exception cref="InvalidOperationException">Neither <see cref="OnTarget"/> nor <see cref="ForType(Type)"/> was used.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The rule implements neither <see cref="IAuthorizationRule"/> nor <see cref="IAuthorizationRuleAsync"/>.</exception>
    public async Task<AuthorizationRuleTestResult> ExecuteAsync(CancellationToken ct = default)
    {
      CslaTestHost? scope = null;
      try
      {
        var context = CreateContext(ref scope);
        if (_rule is IAuthorizationRule syncRule)
          syncRule.Execute(context);
        else if (_rule is IAuthorizationRuleAsync asyncRule)
          await asyncRule.ExecuteAsync(context, ct);
        else
          throw new ArgumentOutOfRangeException(_rule.GetType().FullName);

        return new AuthorizationRuleTestResult(context, scope);
      }
      catch
      {
        scope?.Dispose();
        throw;
      }
    }

    /// <summary>
    /// Executes a synchronous rule and returns the outcome.
    /// </summary>
    /// <exception cref="InvalidOperationException">The rule is asynchronous; use <see cref="ExecuteAsync"/> instead, or neither <see cref="OnTarget"/> nor <see cref="ForType(Type)"/> was used.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The rule implements neither <see cref="IAuthorizationRule"/> nor <see cref="IAuthorizationRuleAsync"/>.</exception>
    public AuthorizationRuleTestResult Execute()
    {
      if (_rule is IAuthorizationRuleAsync)
        throw new InvalidOperationException($"Rule '{_rule.GetType().FullName}' is asynchronous; use {nameof(ExecuteAsync)} instead.");

      CslaTestHost? scope = null;
      try
      {
        var context = CreateContext(ref scope);
        if (_rule is IAuthorizationRule syncRule)
          syncRule.Execute(context);
        else
          throw new ArgumentOutOfRangeException(_rule.GetType().FullName);

        return new AuthorizationRuleTestResult(context, scope);
      }
      catch
      {
        scope?.Dispose();
        throw;
      }
    }

    private IAuthorizationContext CreateContext(ref CslaTestHost? scope)
    {
      var targetType = _targetType ?? _target?.GetType();
      if (targetType is null)
        throw new InvalidOperationException($"A target type is required; call {nameof(OnTarget)} or {nameof(ForType)} before executing the rule.");

      ApplicationContext applicationContext;
      if (_applicationContext is not null)
      {
        applicationContext = _applicationContext;
      }
      else
      {
        scope = CreateHost();
        applicationContext = scope.ApplicationContext;
      }

      if (_principal is not null)
        applicationContext.User = _principal;

      return new AuthorizationContext(applicationContext, _rule, _target, targetType, _criteria);
    }

    private CslaTestHost CreateHost()
    {
      return CslaTestHost.Create(options =>
      {
        if (_configureCsla is not null)
          options.ConfigureCsla(_configureCsla);
        if (_configureServices is not null)
          options.ConfigureServices(_configureServices);
      });
    }
  }
}
