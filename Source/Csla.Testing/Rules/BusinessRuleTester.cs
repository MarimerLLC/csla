//-----------------------------------------------------------------------
// <copyright file="BusinessRuleTester.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Executes a business rule in isolation so it can be unit tested</summary>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Security.Principal;
using Csla.Configuration;
using Csla.Core;
using Csla.Rules;
using Microsoft.Extensions.DependencyInjection;

namespace Csla.Testing.Rules
{
  /// <summary>
  /// Executes a single business rule in isolation so its behavior can be
  /// asserted in a unit test, without requiring a business object or any
  /// hand-written <see cref="ApplicationContext"/> bootstrap.
  /// </summary>
  /// <remarks>
  /// <para>
  /// <see cref="ExecuteAsync"/> runs both <see cref="IBusinessRule"/> and
  /// <see cref="IBusinessRuleAsync"/> rules, so a test does not need to branch
  /// on whether the rule under test is synchronous or asynchronous.
  /// </para>
  /// <example>
  /// <code>
  /// var result = await BusinessRuleTester
  ///   .For(new MyRule(MyBO.NameProperty))
  ///   .WithInput(MyBO.NameProperty, "")
  ///   .ExecuteAsync();
  /// </code>
  /// </example>
  /// </remarks>
  public sealed class BusinessRuleTester
  {
    private readonly IBusinessRuleBase _rule;
    private readonly Dictionary<IPropertyInfo, object?> _inputs = [];
    private object? _target;
    private RuleContextModes _mode = RuleContextModes.PropertyChanged;
    private IPrincipal? _principal;
    private Action<CslaOptions>? _configureCsla;
    private Action<IServiceCollection>? _configureServices;
    private ApplicationContext? _applicationContext;

    private BusinessRuleTester(IBusinessRuleBase rule)
    {
      _rule = rule;
    }

    /// <summary>
    /// Creates a tester for the specified business rule.
    /// </summary>
    /// <param name="rule">The rule to execute.</param>
    /// <exception cref="ArgumentNullException"><paramref name="rule"/> is <see langword="null"/>.</exception>
    public static BusinessRuleTester For(IBusinessRuleBase rule)
    {
      if (rule is null)
        throw new ArgumentNullException(nameof(rule));

      return new BusinessRuleTester(rule);
    }

    /// <summary>
    /// Supplies an input property value to the rule.
    /// </summary>
    /// <typeparam name="T">Type of the property value.</typeparam>
    /// <param name="property">The property being supplied.</param>
    /// <param name="value">The value of the property.</param>
    /// <returns>This instance, to support method chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="property"/> is <see langword="null"/>.</exception>
    public BusinessRuleTester WithInput<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(PropertyInfo<T> property, T? value)
    {
      return WithInput((IPropertyInfo)property, value);
    }

    /// <summary>
    /// Supplies an input property value to the rule.
    /// </summary>
    /// <param name="property">The property being supplied.</param>
    /// <param name="value">The value of the property.</param>
    /// <returns>This instance, to support method chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="property"/> is <see langword="null"/>.</exception>
    public BusinessRuleTester WithInput(IPropertyInfo property, object? value)
    {
      if (property is null)
        throw new ArgumentNullException(nameof(property));

      _inputs[property] = value;
      return this;
    }

    /// <summary>
    /// Supplies the target business object for the rule. When the target
    /// implements <see cref="IManageProperties"/>, any property listed in the
    /// rule's <see cref="IBusinessRuleBase.InputProperties"/> that has not been
    /// supplied through <see cref="WithInput(IPropertyInfo, object?)"/> is read
    /// from the target.
    /// </summary>
    /// <param name="target">The target business object.</param>
    /// <returns>This instance, to support method chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="target"/> is <see langword="null"/>.</exception>
    public BusinessRuleTester OnTarget(object target)
    {
      _target = target ?? throw new ArgumentNullException(nameof(target));
      return this;
    }

    /// <summary>
    /// Sets the execution context mode reported to the rule. Defaults to
    /// <see cref="RuleContextModes.PropertyChanged"/>.
    /// </summary>
    /// <param name="mode">The execution context mode.</param>
    /// <returns>This instance, to support method chaining.</returns>
    public BusinessRuleTester InMode(RuleContextModes mode)
    {
      _mode = mode;
      return this;
    }

    /// <summary>
    /// Runs the rule as an authenticated user with the specified name and roles.
    /// </summary>
    /// <param name="name">Name of the user.</param>
    /// <param name="roles">Roles held by the user.</param>
    /// <returns>This instance, to support method chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> or <paramref name="roles"/> is <see langword="null"/>.</exception>
    public BusinessRuleTester AsUser(string name, params string[] roles)
    {
      _principal = TestPrincipalFactory.CreateUser(name, roles);
      return this;
    }

    /// <summary>
    /// Runs the rule as an unauthenticated user.
    /// </summary>
    /// <returns>This instance, to support method chaining.</returns>
    public BusinessRuleTester AsUnauthenticated()
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
    public BusinessRuleTester AsPrincipal(IPrincipal principal)
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
    public BusinessRuleTester ConfigureCsla(Action<CslaOptions> configure)
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
    public BusinessRuleTester ConfigureServices(Action<IServiceCollection> configure)
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
    public BusinessRuleTester UsingApplicationContext(ApplicationContext applicationContext)
    {
      _applicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
      return this;
    }

    /// <summary>
    /// Executes the rule, awaiting it when it is asynchronous, and returns the
    /// outcome.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">The rule implements neither <see cref="IBusinessRule"/> nor <see cref="IBusinessRuleAsync"/>.</exception>
    public async Task<BusinessRuleTestResult> ExecuteAsync()
    {
      CslaTestHost? scope = null;
      try
      {
        var context = CreateContext(ref scope);
        try
        {
          if (_rule is IBusinessRule syncRule)
            syncRule.Execute(context);
          else if (_rule is IBusinessRuleAsync asyncRule)
            await asyncRule.ExecuteAsync(context);
          else
            throw new ArgumentOutOfRangeException(_rule.GetType().FullName);
        }
        finally
        {
          context.Complete();
        }
        return new BusinessRuleTestResult(context, scope);
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
    /// <exception cref="InvalidOperationException">The rule is asynchronous; use <see cref="ExecuteAsync"/> instead.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The rule implements neither <see cref="IBusinessRule"/> nor <see cref="IBusinessRuleAsync"/>.</exception>
    public BusinessRuleTestResult Execute()
    {
      if (_rule.IsAsync || _rule is IBusinessRuleAsync)
        throw new InvalidOperationException($"Rule '{_rule.RuleName}' is asynchronous; use {nameof(ExecuteAsync)} instead.");

      CslaTestHost? scope = null;
      try
      {
        var context = CreateContext(ref scope);
        try
        {
          if (_rule is IBusinessRule syncRule)
            syncRule.Execute(context);
          else
            throw new ArgumentOutOfRangeException(_rule.GetType().FullName);
        }
        finally
        {
          context.Complete();
        }
        return new BusinessRuleTestResult(context, scope);
      }
      catch
      {
        scope?.Dispose();
        throw;
      }
    }

    private IRuleContext CreateContext(ref CslaTestHost? scope)
    {
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

      // the engine only supplies the target to an async rule when the rule opts in
      object? ruleTarget = null;
      if (!_rule.IsAsync || _rule.ProvideTargetWhenAsync)
        ruleTarget = _target;

      return new RuleContext(applicationContext, _ => { }, _rule, ruleTarget, GetInputPropertyValues(), _mode);
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

    private Dictionary<IPropertyInfo, object?> GetInputPropertyValues()
    {
      var inputs = new Dictionary<IPropertyInfo, object?>();
      if (_target is IManageProperties target && _rule.InputProperties is not null)
      {
        foreach (var item in _rule.InputProperties)
        {
          // do not add lazy loaded fields that have no field data
          if ((item.RelationshipType & RelationshipTypes.LazyLoad) == RelationshipTypes.LazyLoad)
          {
            if (target.FieldExists(item))
              inputs[item] = target.ReadProperty(item);
          }
          else
          {
            inputs[item] = target.ReadProperty(item);
          }
        }
      }

      // values supplied explicitly always win over values read from the target
      foreach (var item in _inputs)
        inputs[item.Key] = item.Value;

      return inputs;
    }
  }
}
