//-----------------------------------------------------------------------
// <copyright file="BusinessRules.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Tracks the business rules for a business object.</summary>
//-----------------------------------------------------------------------

using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Csla.Core;
using Csla.Serialization.Mobile;
using Csla.Server;
using Csla.Threading;

namespace Csla.Rules
{
  /// <summary>
  /// Tracks the business rules for a business object.
  /// </summary>
  [Serializable]
  public class BusinessRules : MobileObject, ISerializationNotification, IBusinessRules, IUseApplicationContext, IRuleRunnerHost
  {
    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    [Obsolete(MobileFormatter.DefaultCtorObsoleteMessage, error: true)]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable. Only for mobile formatter
    public BusinessRules()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    { }

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="applicationContext"></param>
    /// <param name="target">Target business object.</param>
    /// <param name="unhandledAsyncRuleExceptionHandler">Handler for unhandled exceptions that occur during asynchronous rule execution.</param>
    /// <exception cref="ArgumentNullException"><paramref name="applicationContext"/> or <paramref name="target"/> is <see langword="null"/>.</exception>
    public BusinessRules(ApplicationContext applicationContext, IHostRules target, IUnhandledAsyncRuleExceptionHandler unhandledAsyncRuleExceptionHandler)
    {
      _applicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
      _target = target ?? throw new ArgumentNullException(nameof(target));
      _unhandledAsyncRuleExceptionHandler = unhandledAsyncRuleExceptionHandler ?? throw new ArgumentNullException(nameof(unhandledAsyncRuleExceptionHandler));
    }

#if NET9_0_OR_GREATER
    [NonSerialized]
    private Lock _syncRoot = new();
#else
    [NonSerialized]
    private object _syncRoot = new();
#endif
    [NonSerialized]
    private readonly IUnhandledAsyncRuleExceptionHandler _unhandledAsyncRuleExceptionHandler;

    private ApplicationContext _applicationContext;

    /// <inheritdoc />
    ApplicationContext IUseApplicationContext.ApplicationContext { get => _applicationContext; set => _applicationContext = value ?? throw new ArgumentNullException(nameof(ApplicationContext)); }

    // list of broken rules for this business object.
    private BrokenRulesCollection? _brokenRules;
    private BrokenRulesCollection BrokenRules
    {
      get => _brokenRules ??= new BrokenRulesCollection(true);
    }

    private bool _suppressRuleChecking;
    /// <summary>
    /// Gets or sets a value indicating whether calling
    /// CheckRules should result in rule
    /// methods being invoked.
    /// </summary>
    /// <value>True to suppress all rule method invocation.</value>
    public bool SuppressRuleChecking
    {
      get => _suppressRuleChecking;
      set => _suppressRuleChecking = value;
    }

    private int _processThroughPriority;
    /// <summary>
    /// Gets or sets the priority through which
    /// all rules will be processed.
    /// </summary>
    public int ProcessThroughPriority
    {
      get => _processThroughPriority;
      set => _processThroughPriority = value;
    }

    private string? _ruleSet = null;
    /// <summary>
    /// Gets or sets the rule set to use for this
    /// business object instance.
    /// </summary>
    public string RuleSet
    {
      get => string.IsNullOrEmpty(_ruleSet) ? ApplicationContext.DefaultRuleSet : _ruleSet!;
      set
      {
        _typeRules = null;
        _typeAuthRules = null;
        _ruleSet = value == ApplicationContext.DefaultRuleSet ? null : value;
        if (BrokenRules.Count > 0)
        {
          BrokenRules.ClearRules();
        }
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether rule engine should cascade n-levels when property value is changed from OuputPropertyValues. 
    /// </summary>
    /// <value>
    ///   <c>true</c> if [cascade when changed]; otherwise, <c>false</c>.
    /// </value>
    public bool CascadeOnDirtyProperties
    {
      get => _cascadeOnDirtyProperties;
      set => _cascadeOnDirtyProperties = value;
    }

    [NonSerialized]
    private BusinessRuleManager? _typeRules;
    internal BusinessRuleManager TypeRules => _typeRules ??= BusinessRuleManager.GetRulesForType(_target.GetType(), _ruleSet);

    [NonSerialized]
    private AuthorizationRuleManager? _typeAuthRules;
    internal AuthorizationRuleManager TypeAuthRules => _typeAuthRules ??= AuthorizationRuleManager.GetRulesForType(_applicationContext, _target.GetType(), _ruleSet);

    /// <summary>
    /// Gets a list of rule:// URI values for
    /// the rules defined in the object.
    /// </summary>
    public string[] GetRuleDescriptions()
    {
      var result = new List<string>();
      foreach (var item in TypeRules.Rules)
        result.Add(item.RuleName);
      return result.ToArray();
    }

    // reference to current business object
    [NonSerialized]
    private IHostRules _target;

    internal void SetTarget(IHostRules target)
    {
      _target = target;
    }

    internal object Target => _target;

    /// <summary>
    /// Associates a business rule with the business object.
    /// </summary>
    /// <param name="rule">Rule object.</param>
    /// <exception cref="ArgumentNullException"><paramref name="rule"/> is <see langword="null"/>.</exception>
    public void AddRule(IBusinessRuleBase rule)
    {
      if (rule is null)
        throw new ArgumentNullException(nameof(rule));
      TypeRules.Rules.Add(rule);
    }

    /// <summary>
    /// Associates a business rule with the business object.
    /// </summary>
    /// <param name="rule">Rule object.</param>
    /// <param name="ruleSet">Rule set name.</param>
    /// <exception cref="ArgumentNullException"><paramref name="rule"/> is <see langword="null"/>.</exception>
    public void AddRule(IBusinessRuleBase rule, string? ruleSet)
    {
      if (rule is null)
        throw new ArgumentNullException(nameof(rule));
      var typeRules = BusinessRuleManager.GetRulesForType(_target.GetType(), ruleSet);
      typeRules.Rules.Add(rule);
    }

    /// <summary>
    /// Associates an authorization rule with the business object.
    /// </summary>
    /// <param name="rule">Rule object.</param>
    /// <exception cref="ArgumentNullException"><paramref name="rule"/> is <see langword="null"/>.</exception>
    public void AddRule(IAuthorizationRuleBase rule)
    {
      if (rule is null)
        throw new ArgumentNullException(nameof(rule));
      EnsureUniqueRule(TypeAuthRules, rule);
      TypeAuthRules.Rules.Add(rule);
    }

    /// <summary>
    /// Associates a per-type authorization rule with 
    /// the business type in the default rule set.
    /// </summary>
    /// <param name="objectType">Type of business object.</param>
    /// <param name="rule">Rule object.</param>
    /// <exception cref="ArgumentNullException"><paramref name="objectType"/> or <paramref name="rule"/> is <see langword="null"/>.</exception>
    public static void AddRule(Type objectType, IAuthorizationRuleBase rule)
    {
      AddRule(objectType, rule, ApplicationContext.DefaultRuleSet);
    }

    /// <summary>
    /// Associates a per-type authorization rule with 
    /// the business type.
    /// </summary>
    /// <param name="objectType">Type of business object.</param>
    /// <param name="rule">Rule object.</param>
    /// <param name="ruleSet">Rule set name.</param>
    /// <exception cref="ArgumentNullException"><paramref name="objectType"/> or <paramref name="rule"/> is <see langword="null"/>.</exception>
    public static void AddRule(Type objectType, IAuthorizationRuleBase rule, string ruleSet)
    {
      AddRule(null, objectType, rule, ruleSet);
    }

    /// <summary>
    /// Associates a per-type authorization rule with 
    /// the business type.
    /// </summary>
    /// <param name="applicationContext">ApplicationContext instance</param>
    /// <param name="objectType">Type of business object.</param>
    /// <param name="rule">Rule object.</param>
    /// <param name="ruleSet">Rule set name.</param>
    /// <exception cref="ArgumentNullException"><paramref name="objectType"/> or <paramref name="rule"/> is <see langword="null"/>.</exception>
    public static void AddRule(ApplicationContext? applicationContext, Type objectType, IAuthorizationRuleBase rule, string ruleSet)
    {
      if (objectType is null)
        throw new ArgumentNullException(nameof(objectType));
      if (rule is null)
        throw new ArgumentNullException(nameof(rule));

      var typeRules = AuthorizationRuleManager.GetRulesForType(applicationContext, objectType, ruleSet);
      EnsureUniqueRule(typeRules, rule);
      typeRules.Rules.Add(rule);
    }

    private static void EnsureUniqueRule(AuthorizationRuleManager mgr, IAuthorizationRuleBase rule)
    {
      IAuthorizationRuleBase? oldRule;
      if (rule.Element != null)
        oldRule = mgr.Rules.FirstOrDefault(c => c.Element != null && c.Element.Name == rule.Element.Name && c.Action == rule.Action);
      else
        oldRule = mgr.Rules.FirstOrDefault(c => c.Element == null && c.Action == rule.Action);
      if (oldRule != null)
        throw new ArgumentException(nameof(rule));
    }

    /// <summary>
    /// Gets a value indicating whether there are
    /// any currently broken rules, which would
    /// mean the object is not valid.
    /// </summary>
    public bool IsValid => BrokenRules.ErrorCount == 0;

    /// <summary>
    /// Gets the broken rules list.
    /// </summary>
    public BrokenRulesCollection GetBrokenRules()
    {
      return BrokenRules;
    }

    [NonSerialized]
    private bool _runningRules;
    /// <summary>
    /// Gets a value indicating whether a CheckRules
    /// operation is in progress.
    /// </summary>
    public bool RunningRules
    {
      get => _runningRules;
      private set => _runningRules = value;
    }

    [NonSerialized]
    private bool _isBusy;

    [NonSerialized]
    private AsyncManualResetEvent? _busyChanged;
    private AsyncManualResetEvent BusyChanged => _busyChanged ??= new AsyncManualResetEvent();

    /// <summary>
    /// Gets a value indicating whether any async
    /// rules are currently executing.
    /// </summary>
    public bool RunningAsyncRules
    {
      get => _isBusy;
      set
      {
        _isBusy = value;
        if (_isBusy)
          BusyChanged.Reset();
        else
          BusyChanged.Set();
      }
    }

    /// <summary>
    /// Gets a value indicating whether a specific
    /// property has any async rules running.
    /// </summary>
    /// <param name="property">Property to check.</param>
    /// <exception cref="ArgumentNullException"><paramref name="property"/> is <see langword="null"/>.</exception>
    public bool GetPropertyBusy(IPropertyInfo property)
    {
      if (property is null)
        throw new ArgumentNullException(nameof(property));
      return BusyProperties.Contains(property);
    }

    /// <summary>
    /// Checks per-type authorization rules.
    /// </summary>
    /// <param name="applicationContext"></param>
    /// <param name="action">Authorization action.</param>
    /// <param name="objectType">Type of business object.</param>
    /// <exception cref="ArgumentNullException"><paramref name="applicationContext"/> or <paramref name="objectType"/> is <see langword="null"/>.</exception>
    public static bool HasPermission(ApplicationContext applicationContext, AuthorizationActions action, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type objectType)
    {
      if (applicationContext == null)
        throw new ArgumentNullException(nameof(applicationContext));
      if (objectType is null)
        throw new ArgumentNullException(nameof(objectType));
      var activator = applicationContext.GetRequiredService<IDataPortalActivator>();
      var realType = activator.ResolveType(objectType);
      // no object specified so must use RuleSet from ApplicationContext
      return HasPermission(action, null, applicationContext, realType, null, applicationContext.RuleSet);
    }

    /// <summary>
    /// Checks per-type authorization rules.
    /// </summary>
    /// <param name="applicationContext"></param>
    /// <param name="action">Authorization action.</param>
    /// <param name="objectType">Type of business object.</param>
    /// <param name="criteria">The criteria object provided.</param>
    /// <exception cref="ArgumentNullException"><paramref name="applicationContext"/> or <paramref name="objectType"/> is <see langword="null"/>.</exception>
    public static bool HasPermission(ApplicationContext applicationContext, AuthorizationActions action, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type objectType, object?[]? criteria)
    {
      if (applicationContext == null)
        throw new ArgumentNullException(nameof(applicationContext));
      if (objectType is null)
        throw new ArgumentNullException(nameof(objectType));
      var activator = applicationContext.GetRequiredService<IDataPortalActivator>();
      var realType = activator.ResolveType(objectType);
      // no object specified so must use RuleSet from ApplicationContext
      return HasPermission(action, null, applicationContext, realType, criteria, applicationContext.RuleSet);
    }

    /// <summary>
    /// Checks per-type authorization rules.
    /// </summary>
    /// <param name="applicationContext"></param>
    /// <param name="action">Authorization action.</param>
    /// <param name="objectType">Type of business object.</param>
    /// <param name="ruleSet">The rule set.</param>
    /// <returns>
    /// 	<c>true</c> if the specified action has permission; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="applicationContext"/> or <paramref name="objectType"/> is <see langword="null"/>.</exception>
    public static bool HasPermission(ApplicationContext applicationContext, AuthorizationActions action, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type objectType, string? ruleSet)
    {
      if (applicationContext == null)
        throw new ArgumentNullException(nameof(applicationContext));
      if (objectType is null)
        throw new ArgumentNullException(nameof(objectType));
      var activator = applicationContext.GetRequiredService<IDataPortalActivator>();
      var realType = activator.ResolveType(objectType);
      return HasPermission(action, null, applicationContext, realType, null, ruleSet);
    }

    /// <summary>
    /// Checks per-instance authorization rules.
    /// </summary>
    /// <param name="applicationContext"></param>
    /// <param name="action">Authorization action.</param>
    /// <param name="obj">Business object instance.</param>
    /// <exception cref="ArgumentNullException"><paramref name="applicationContext"/> or <paramref name="obj"/> is <see langword="null"/>.</exception>
    public static bool HasPermission(ApplicationContext applicationContext, AuthorizationActions action, object obj)
    {
      if (applicationContext == null)
        throw new ArgumentNullException(nameof(applicationContext));
      if (obj is null)
        throw new ArgumentNullException(nameof(obj));
      return HasPermission(action, obj, applicationContext, obj.GetType(), null, applicationContext.RuleSet);
    }

    private static bool HasPermission(AuthorizationActions action, object? obj, ApplicationContext applicationContext, Type objType, object?[]? criteria, string? ruleSet)
    {
      if (action == AuthorizationActions.ReadProperty || action == AuthorizationActions.WriteProperty || action == AuthorizationActions.ExecuteMethod)
        throw new ArgumentOutOfRangeException($"{nameof(action)}, {action}");

      bool result = true;
      var rule = AuthorizationRuleManager.GetRulesForType(applicationContext, objType, ruleSet).Rules.FirstOrDefault(c => c.Element == null && c.Action == action);
      if (rule != null)
      {
        if (rule is IAuthorizationRule sync)
        {
          var context = new AuthorizationContext(applicationContext, rule, obj, objType, criteria);
          sync.Execute(context);
          result = context.HasPermission;
        }
        else if (rule is IAuthorizationRuleAsync nsync)
        {
          var context = new AuthorizationContext(applicationContext, rule, obj, objType, criteria);
          ExecuteAsyncRuleSynchronously(nsync, context);
          result = context.HasPermission;
        }
        else
          throw new ArgumentOutOfRangeException(rule.GetType().FullName);
      }
      return result;
    }

    /// <summary>
    /// Checks per-property authorization rules.
    /// </summary>
    /// <param name="applicationContext"></param>
    /// <param name="action">Authorization action.</param>
    /// <param name="element">Property or method to check.</param>
    /// <exception cref="ArgumentNullException"><paramref name="applicationContext"/> or <paramref name="element"/> is <see langword="null"/>.</exception>
    public bool HasPermission(ApplicationContext applicationContext, AuthorizationActions action, IMemberInfo element)
    {
      if (_suppressRuleChecking)
        return true;

      if (action == AuthorizationActions.CreateObject ||
          action == AuthorizationActions.DeleteObject ||
          action == AuthorizationActions.GetObject ||
          action == AuthorizationActions.EditObject)
        throw new ArgumentOutOfRangeException($"{nameof(action)}, {action}");
      if (element is null)
        throw new ArgumentNullException(nameof(element));
      if (applicationContext is null)
        throw new ArgumentNullException(nameof(applicationContext));

      bool result = true;
      var rule =
        TypeAuthRules.Rules.FirstOrDefault(c => c.Element != null && c.Element.Name == element.Name && c.Action == action);
      if (rule != null)
      {
        if (rule is IAuthorizationRule sync)
        {
          var context = new AuthorizationContext(applicationContext, rule, Target, Target.GetType());
          sync.Execute(context);
          result = context.HasPermission;
        }
        else if (rule is IAuthorizationRuleAsync nsync)
        {
          var context = new AuthorizationContext(applicationContext, rule, Target, Target.GetType());
          ExecuteAsyncRuleSynchronously(nsync, context);
          result = context.HasPermission;
        }
        else
          throw new ArgumentOutOfRangeException(rule.GetType().FullName);
      }
      return result;
    }

    /// <summary>
    /// Executes an async authorization rule from a synchronous
    /// HasPermission call. Rules that complete synchronously are
    /// supported; the task is never blocked on, so a rule that
    /// is still running results in an exception.
    /// </summary>
    private static void ExecuteAsyncRuleSynchronously(IAuthorizationRuleAsync rule, IAuthorizationContext context)
    {
      var task = rule.ExecuteAsync(context, CancellationToken.None);
      if (!task.IsCompleted)
        throw new InvalidOperationException(string.Format(Properties.Resources.AsyncAuthorizationRuleRequiresHasPermissionAsync, rule.GetType().FullName));
      task.GetAwaiter().GetResult();
    }

    /// <summary>
    /// Checks per-type authorization rules.
    /// </summary>
    /// <param name="applicationContext">The application context.</param>
    /// <param name="action">The authorization action.</param>
    /// <param name="objectType">The type of the business object.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation that returns a boolean indicating whether the permission is granted.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="applicationContext"/> or <paramref name="objectType"/> is <see langword="null"/>.</exception>
    public static Task<bool> HasPermissionAsync(ApplicationContext applicationContext, AuthorizationActions action, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type objectType, CancellationToken ct)
    {
      if (applicationContext == null)
        throw new ArgumentNullException(nameof(applicationContext));
      if (objectType is null)
        throw new ArgumentNullException(nameof(objectType));

      var activator = applicationContext.GetRequiredService<IDataPortalActivator>();
      var realType = activator.ResolveType(objectType);

      // no object specified so must use RuleSet from ApplicationContext
      return HasPermissionAsync(action, null, applicationContext, realType, null, applicationContext.RuleSet, ct);
    }

    /// <summary>
    /// Checks per-type authorization rules.
    /// </summary>
    /// <param name="applicationContext">The application context.</param>
    /// <param name="action">The authorization action.</param>
    /// <param name="objectType">The type of the business object.</param>
    /// <param name="criteria">The criteria object provided.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a boolean value indicating whether the permission is granted.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="applicationContext"/> or <paramref name="objectType"/> is <see langword="null"/>.</exception>
    public static Task<bool> HasPermissionAsync(ApplicationContext applicationContext, AuthorizationActions action, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type objectType, object?[]? criteria, CancellationToken ct)
    {
      if (applicationContext == null)
        throw new ArgumentNullException(nameof(applicationContext));
      if (objectType is null)
        throw new ArgumentNullException(nameof(objectType));

      var activator = applicationContext.GetRequiredService<IDataPortalActivator>();
      var realType = activator.ResolveType(objectType);

      // no object specified so must use RuleSet from ApplicationContext
      return HasPermissionAsync(action, null, applicationContext, realType, criteria, applicationContext.RuleSet, ct);
    }

    /// <summary>
    /// Checks per-type authorization rules.
    /// </summary>
    /// <param name="applicationContext">The application context.</param>
    /// <param name="action">Authorization action.</param>
    /// <param name="objectType">Type of business object.</param>
    /// <param name="ruleSet">The rule set.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>
    /// 	<c>true</c> if the specified action has permission; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="applicationContext"/> or <paramref name="objectType"/> is <see langword="null"/>.</exception>
    public static Task<bool> HasPermissionAsync(ApplicationContext applicationContext, AuthorizationActions action, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type objectType, string? ruleSet, CancellationToken ct)
    {
      if (applicationContext == null)
        throw new ArgumentNullException(nameof(applicationContext));
      if (objectType is null)
        throw new ArgumentNullException(nameof(objectType));
      var activator = applicationContext.GetRequiredService<IDataPortalActivator>();
      var realType = activator.ResolveType(objectType);
      return HasPermissionAsync(action, null, applicationContext, realType, null, ruleSet, ct);
    }

    /// <summary>
    /// Checks per-instance authorization rules.
    /// </summary>
    /// <param name="applicationContext">The application context.</param>
    /// <param name="action">The authorization action.</param>
    /// <param name="obj">The business object instance.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <exception cref="ArgumentNullException"><paramref name="applicationContext"/> or <paramref name="obj"/> is <see langword="null"/>.</exception>
    public static Task<bool> HasPermissionAsync(ApplicationContext applicationContext, AuthorizationActions action, object obj, CancellationToken ct)
    {
      if (applicationContext == null)
        throw new ArgumentNullException(nameof(applicationContext));
      if (obj is null)
        throw new ArgumentNullException(nameof(obj));

      return HasPermissionAsync(action, obj, applicationContext, obj.GetType(), null, applicationContext.RuleSet, ct);
    }

    private static async Task<bool> HasPermissionAsync(AuthorizationActions action, object? obj, ApplicationContext applicationContext, Type objType, object?[]? criteria, string? ruleSet, CancellationToken ct)
    {
      if (action == AuthorizationActions.ReadProperty || action == AuthorizationActions.WriteProperty || action == AuthorizationActions.ExecuteMethod)
        throw new ArgumentOutOfRangeException($"{nameof(action)}, {action}");

      bool result = true;
      var rule =
        AuthorizationRuleManager.GetRulesForType(applicationContext, objType, ruleSet).Rules.FirstOrDefault(c => c.Element == null && c.Action == action);
      if (rule != null)
      {
        if (rule is IAuthorizationRule sync)
        {
          var context = new AuthorizationContext(applicationContext, rule, obj, objType, criteria);
          sync.Execute(context);
          result = context.HasPermission;
        }
        else if (rule is IAuthorizationRuleAsync nsync)
        {
          var context = new AuthorizationContext(applicationContext, rule, obj, objType, criteria);
          await nsync.ExecuteAsync(context, ct);
          result = context.HasPermission;
        }
        else
          throw new ArgumentOutOfRangeException(rule.GetType().FullName);
      }
      return result;
    }

    /// <summary>
    /// Checks per-property authorization rules.
    /// </summary>
    /// <param name="applicationContext">The application context.</param>
    /// <param name="action">The authorization action.</param>
    /// <param name="element">The property or method to check.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a boolean value indicating whether the permission is granted.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="applicationContext"/> or <paramref name="element"/> is <see langword="null"/>.</exception>
    public async Task<bool> HasPermissionAsync(ApplicationContext applicationContext, AuthorizationActions action, IMemberInfo element, CancellationToken ct)
    {
      if (_suppressRuleChecking)
        return true;

      if (action == AuthorizationActions.CreateObject || action == AuthorizationActions.DeleteObject || action == AuthorizationActions.GetObject || action == AuthorizationActions.EditObject)
        throw new ArgumentOutOfRangeException($"{nameof(action)}, {action}");
      if (element is null)
        throw new ArgumentNullException(nameof(element));
      if (applicationContext is null)
        throw new ArgumentNullException(nameof(applicationContext));

      bool result = true;
      var rule =
          TypeAuthRules.Rules.FirstOrDefault(c => c.Element != null && c.Element.Name == element.Name && c.Action == action);
      if (rule != null)
      {
        if (rule is IAuthorizationRule sync)
        {
          var context = new AuthorizationContext(applicationContext, rule, Target, Target.GetType());
          sync.Execute(context);
          result = context.HasPermission;
        }
        else if (rule is IAuthorizationRuleAsync nsync)
        {
          var context = new AuthorizationContext(applicationContext, rule, Target, Target.GetType());
          await nsync.ExecuteAsync(context, ct);
          result = context.HasPermission;
        }
        else
          throw new ArgumentOutOfRangeException(rule.GetType().FullName);
      }
      return result;
    }

    /// <summary>
    /// Gets a value indicating whether the permission
    /// result can be cached.
    /// </summary>
    /// <param name="action">Authorization action.</param>
    /// <param name="element">Property or method to check.</param>
    /// <exception cref="ArgumentNullException"><paramref name="element"/> is <see langword="null"/>.</exception>
    public bool CachePermissionResult(AuthorizationActions action, IMemberInfo element)
    {
      // cannot cache result when suppressRuleChecking as HasPermission is then short circuited to return true.
      if (_suppressRuleChecking)
        return false;
      if (element is null)
        throw new ArgumentNullException(nameof(element));

      bool result = true;
      var rule =
        TypeAuthRules.Rules.FirstOrDefault(c => c.Element != null && c.Element.Name == element.Name && c.Action == action);
      if (rule != null)
        result = rule.CacheResult;
      return result;
    }

    /// <summary>
    /// Invokes all rules for a specific property of the business type.
    /// </summary>
    /// <param name="property">Property to check.</param>
    /// <returns>
    /// Returns a list of property names affected by the invoked rules.
    /// The PropertyChanged event should be raised for each affected
    /// property.
    /// </returns>
    /// <exception cref="System.ArgumentNullException">If property is null</exception>
    public Task<List<string>> CheckRulesAsync(IPropertyInfo property)
    {
      return CheckRulesAsync(property, Timeout.InfiniteTimeSpan);
    }

    /// <summary>
    /// Invokes all rules for a specific property of the business type.
    /// </summary>
    /// <param name="property">Property to check.</param>
    /// <param name="timeout">Timeout to wait for the rule completion.</param>
    /// <returns>
    /// Returns a list of property names affected by the invoked rules.
    /// The PropertyChanged event should be raised for each affected
    /// property.
    /// </returns>
    /// <exception cref="System.ArgumentNullException">If property is null</exception>
    public async Task<List<string>> CheckRulesAsync(IPropertyInfo property, TimeSpan timeout)
    {
      // async, so exceptions fault the returned task
      if (property == null)
        throw new ArgumentNullException(nameof(property));
      if (_suppressRuleChecking)
        return await AwaitRulesAsync(null, [], timeout);

      var stream = CheckRulesCore(RuleStreamKind.Property, property, awaited: true, out var affectedProperties);
      return await AwaitRulesAsync(stream, affectedProperties, timeout);
    }

    /// <summary>
    /// Invokes all rules for the business type.
    /// </summary>
    /// <returns>
    /// Returns a list of property names affected by the invoked rules.
    /// The PropertyChanged event should be raised for each affected
    /// property. Does not return until all async rules are complete.
    /// </returns>
    public Task<List<string>> CheckRulesAsync()
    {
      return CheckRulesAsync(int.MaxValue);
    }

    /// <summary>
    /// Invokes all rules for the business type.
    /// </summary>
    /// <param name="timeout">Timeout value in milliseconds</param>
    /// <returns>
    /// Returns a list of property names affected by the invoked rules.
    /// The PropertyChanged event should be raised for each affected
    /// property. Does not return until all async rules are complete.
    /// </returns>
    public Task<List<string>> CheckRulesAsync(int timeout)
    {
      return CheckRulesAsync(TimeSpan.FromMilliseconds(timeout));
    }

    /// <summary>
    /// Invokes all rules for the business type.
    /// </summary>
    /// <param name="timeout">Timeout value.</param>
    /// <returns>
    /// Returns a list of property names affected by the invoked rules.
    /// The PropertyChanged event should be raised for each affected
    /// property. Does not return until all async rules are complete.
    /// </returns>
    public async Task<List<string>> CheckRulesAsync(TimeSpan timeout)
    {
      // async, so exceptions fault the returned task
      if (_suppressRuleChecking)
        return await AwaitRulesAsync(null, [], timeout);

      var stream = CheckRulesCore(RuleStreamKind.AllRules, null, awaited: true, out var affectedProperties);
      return await AwaitRulesAsync(stream, affectedProperties, timeout);
    }

    /// <summary>
    /// Waits until the object is idle, then returns the affected properties
    /// of the whole rule stream: the synchronous part, the parts that ran
    /// after a sequential async rule, and the async rules the stream
    /// started, with their completion cascades.
    /// </summary>
    private async Task<List<string>> AwaitRulesAsync(RuleStream? stream, List<string> affectedProperties, TimeSpan timeout)
    {
      await WaitForAsyncRulesToComplete(timeout);
      if (stream == null)
        return affectedProperties;
      if (stream.Error != null)
        System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(stream.Error).Throw();
      if (!stream.CollectsAsyncResults)
        return affectedProperties;
      return stream.GetAllAffected();
    }

    private async Task WaitForAsyncRulesToComplete(TimeSpan timeout)
    {
      if (!RunningAsyncRules)
      {
        return;
      }

      var tasks = new Task[] { BusyChanged.WaitAsync(), Task.Delay(timeout) };
      var final = await Task.WhenAny(tasks);
      if (final == tasks[1])
        throw new TimeoutException(nameof(CheckRulesAsync));
    }

    /// <summary>
    /// Invokes all rules for the business type.
    /// </summary>
    /// <returns>
    /// Returns a list of property names affected by the invoked rules.
    /// The PropertyChanged event should be raised for each affected
    /// property.
    /// </returns>
    /// <remarks>
    /// If a sequential async rule is still running, this method returns
    /// when the rules reach it, with the properties affected so far. The
    /// rules after it run when it completes, and raise PropertyChanged for
    /// the properties they affect.
    /// </remarks>
    public List<string> CheckRules()
    {
      if (_suppressRuleChecking)
        return new List<string>();

      CheckRulesCore(RuleStreamKind.AllRules, null, awaited: false, out var affectedProperties);
      return affectedProperties;
    }

    /// <summary>
    /// Invokes all rules attached at the class level
    /// of the business type.
    /// </summary>
    /// <returns>
    /// Returns a list of property names affected by the invoked rules.
    /// The PropertyChanged event should be raised for each affected
    /// property.
    /// </returns>
    public List<string> CheckObjectRules()
    {
      if (_suppressRuleChecking)
        return new List<string>();

      CheckRulesCore(RuleStreamKind.ObjectRules, null, awaited: false, out var affectedProperties);
      return affectedProperties;
    }

    /// <summary>
    /// Invokes all rules for a specific property of the business type.
    /// </summary>
    /// <param name="property">Property to check.</param>
    /// <returns>
    /// Returns a list of property names affected by the invoked rules.
    /// The PropertyChanged event should be raised for each affected
    /// property.
    /// </returns>
    /// <exception cref="System.ArgumentNullException">If property is null</exception>
    public List<string> CheckRules(IPropertyInfo property)
    {
      if (property == null)
        throw new ArgumentNullException(nameof(property));

      if (_suppressRuleChecking)
        return new List<string>();

      CheckRulesCore(RuleStreamKind.Property, property, awaited: false, out var affectedProperties);
      return affectedProperties;
    }

    /// <summary>
    /// Runs a rule stream up to its end or its first wait for a
    /// sequential async rule.
    /// </summary>
    /// <param name="kind">What the stream runs.</param>
    /// <param name="root">The property, for a property stream.</param>
    /// <param name="awaited">True if CheckRulesAsync awaits the stream.</param>
    /// <param name="affectedProperties">Receives the distinct names of the properties affected so far.</param>
    /// <returns>The stream.</returns>
    private RuleStream CheckRulesCore(RuleStreamKind kind, IPropertyInfo? root, bool awaited, out List<string> affectedProperties)
    {
      var oldRR = RunningRules;
      RunningRules = true;
      RuleStream stream;
      try
      {
        // a paused stream this run replaces must not continue
        var discarded = DiscardStreams(kind, root);
        try
        {
          stream = StartStream(kind, root, awaited);
        }
        catch
        {
          // release the replaced streams, but don't restart them after a failed run
          if (discarded.Count > 0)
            EndDiscardedStreams(discarded, kind, root, restart: false);
          throw;
        }
        affectedProperties = stream.GetRunnerAffected().Distinct().ToList();
        if (discarded.Count > 0)
          EndDiscardedStreams(discarded, kind, root, restart: true);
      }
      finally
      {
        RunningRules = kind == RuleStreamKind.AllRules ? false : oldRR;
      }
      if (!RunningRules && !RunningAsyncRules)
        _target.AllRulesComplete();
      return stream;
    }

    [NonSerialized]
    private IRuleRunner? _ruleRunner;

    /// <summary>
    /// Gets the rule runner that decides which rules run and in what order.
    /// </summary>
    private IRuleRunner RuleRunner => _ruleRunner ??= _applicationContext.GetRequiredService<IRuleRunner>();

    /// <summary>
    /// Determines whether this rule can run the specified context mode.
    /// </summary>
    /// <param name="applicationContext"></param>
    /// <param name="rule">The rule.</param>
    /// <param name="contextMode">The context mode.</param>
    /// <returns>
    /// 	<c>true</c> if this instance [can run rule] the specified context mode; otherwise, <c>false</c>.
    /// </returns>
    internal static bool CanRunRule(ApplicationContext applicationContext, IBusinessRuleBase rule, RuleContextModes contextMode)
    {
      // default then just return true
      if (rule.RunMode == RunModes.Default)
        return true;

      bool canRun = true;

      if ((contextMode & RuleContextModes.AsAffectedProperty) > 0)
        canRun &= (rule.RunMode & RunModes.DenyAsAffectedProperty) == 0;

      if ((rule.RunMode & RunModes.DenyOnServerSidePortal) > 0)
        canRun &= applicationContext.LogicalExecutionLocation != ApplicationContext.LogicalExecutionLocations.Server;

      if ((contextMode & RuleContextModes.CheckRules) > 0)
        canRun &= (rule.RunMode & RunModes.DenyCheckRules) == 0;

      return canRun;
    }

    [NonSerialized]
    private List<IPropertyInfo>? _busyProperties;

    private bool _cascadeOnDirtyProperties;

    private List<IPropertyInfo> BusyProperties => _busyProperties ??= [];

    #region IRuleRunnerHost

    ApplicationContext IRuleRunnerHost.ApplicationContext => _applicationContext;

    IReadOnlyList<IBusinessRuleBase> IRuleRunnerHost.Rules => TypeRules.Rules;

    bool IRuleRunnerHost.HasSequentialAsyncRules => TypeRules.HasSequentialAsyncRules;

    void IRuleRunnerHost.ClearBrokenRules(IPropertyInfo? property) => BrokenRules.ClearRules(property);

    void IRuleRunnerHost.ClearBrokenRules(IPropertyInfo? property, ICollection<string> keepRuleNames) => BrokenRules.ClearRules(property, keepRuleNames);

    RuleRunOutcome IRuleRunnerHost.RunRule(IBusinessRuleBase rule, RuleContextModes executionContext, AsyncRuleCompletedHandler? onAsyncRuleCompleted, bool foldIntoStream)
    {
      bool complete = false;
      bool hasBrokenError = false;

      // an async rule started by a rule stream is tracked by the stream
      AsyncRuleRun? run = null;
      if (rule.IsAsync)
      {
        var stream = CurrentStream;
        if (stream != null && (foldIntoStream || stream.CollectsAsyncResults))
          run = new AsyncRuleRun(rule, stream, foldIntoStream, onAsyncRuleCompleted);
      }

      object? ruleTarget = null;
      if (!rule.IsAsync || rule.ProvideTargetWhenAsync)
        ruleTarget = _target;
      // set up context
      var context = new RuleContext(_applicationContext, rule, executionContext, ruleTarget, r =>
      {
        if (r.Rule.IsAsync)
        {
          lock (_syncRoot)
          {
            // a newer run replaced the stream that owns this rule and reruns
            // the rule: its results are stale (unless the newer run failed)
            if (run != null && run.IsOwned && run.Stream.IsDiscarded && !run.Stream.KeepsResults)
            {
              run.Complete(r, RuleRunOutcome.Pending);
              CompleteAsyncRule(r.Rule, []);
              return;
            }

            // update output values
            if (r.OutputPropertyValues != null)
              foreach (var item in r.OutputPropertyValues)
              {
                // value is changed add to dirtyValues
                if (((IManageProperties)_target).LoadPropertyMarkDirty(item.Key, item.Value))
                  r.AddDirtyProperty(item.Key);
              }
            // update broken rules list
            BrokenRules.SetBrokenRules(r.Results, r.OriginPropertyName, rule.Priority);

            if (run == null)
            {
              // run rules on affected properties for this async rule
              var affected = onAsyncRuleCompleted?.Invoke(r) ?? [];
              CompleteAsyncRule(r.Rule, affected);
            }
            else
            {
              CompleteAsyncRuleRun(run, r);
            }
          }
        }
        else  // Rule is Sync
        {
          // update output values
          if (r.OutputPropertyValues != null)
            foreach (var item in r.OutputPropertyValues)
            {
              // value is changed add to dirtyValues
              if (((IManageProperties)_target).LoadPropertyMarkDirty(item.Key, item.Value))
                r.AddDirtyProperty(item.Key);
            }

          // update broken rules list
          if (r.Results != null)
          {
            BrokenRules.SetBrokenRules(r.Results, r.OriginPropertyName, rule.Priority);

            // is any rules here broken with severity Error
            if (r.Results.Any(p => !p.Success && p.Severity == RuleSeverity.Error))
              hasBrokenError = true;
          }

          complete = true;
        }
      });

      // get input properties
      if (rule.InputProperties != null)
      {
        var target = (IManageProperties)_target;
        foreach (var item in rule.InputProperties)
        {
          // do not add lazy loaded fields that have no field data.
          if ((item.RelationshipType & RelationshipTypes.LazyLoad) == RelationshipTypes.LazyLoad)
          {
            if (target.FieldExists(item))
              context.InputPropertyValues.Add(item, target.ReadProperty(item));
          }
          else
            context.InputPropertyValues.Add(item, target.ReadProperty(item));
        }
      }

      // mark properties busy
      if (rule.IsAsync)
      {
        lock (_syncRoot)
        {
          // mark each property as busy
          foreach (var item in rule.AffectedProperties)
          {
            var alreadyBusy = BusyProperties.Contains(item);
            BusyProperties.Add(item);
            RunningAsyncRules = true;
            if (!alreadyBusy)
              _target.RuleStart(item);
          }
        }
      }

      // execute (or start executing) rule
      try
      {
        if (rule is IBusinessRule syncRule)
          syncRule.Execute(context);
        else if (rule is IBusinessRuleAsync asyncRule)
          RunAsyncRule(asyncRule, context, _unhandledAsyncRuleExceptionHandler, catchAll: run != null && run.IsOwned && rule.IsSequential);
        else
          throw new ArgumentOutOfRangeException(rule.GetType().FullName);
      }
      catch (Exception ex)
      {
        context.AddErrorResult($"{rule.RuleName}:{ex.Message}");
        if (rule.IsAsync)
          context.Complete();
      }

      if (rule.IsAsync)
        return run != null ? new RuleRunOutcome(run) : RuleRunOutcome.Pending;

      // process results
      if (!complete)
        context.Complete();
      // copy affected property names and output property names
      var affectedProperties = new List<string>(rule.AffectedProperties.Count + context.OutputPropertyValues.Count);
      affectedProperties.AddRange(rule.AffectedProperties.Select(c => c.Name));
      affectedProperties.AddRange(context.OutputPropertyValues.Select(c => c.Key.Name));
      // copy dirty properties
      var dirtyProperties = context.DirtyProperties.Select(c => c.Name).ToList();
      // explicit short-circuiting
      var stopProcessing = context.Results.Any(r => r.StopProcessing);
      return new RuleRunOutcome(affectedProperties, dirtyProperties, hasBrokenError, stopProcessing);
    }

    #endregion

    #region Rule streams

    // A rule stream is one run of CheckRules(), CheckRules(property) or
    // CheckObjectRules(). It waits at a sequential async rule (#5014) and
    // resumes, on the thread that completed the rule, inside the lock on
    // _syncRoot, as the completion cascade of an async rule always ran.

    [ThreadStatic]
    private static RuleStream? t_currentStream;

    /// <summary>
    /// Gets the stream whose rules run on this thread for this object, or null.
    /// </summary>
    private RuleStream? CurrentStream
    {
      get
      {
        var stream = t_currentStream;
        return stream != null && ReferenceEquals(stream.Host, this) ? stream : null;
      }
    }

    // the streams waiting at a pause; guarded by _syncRoot
    [NonSerialized]
    private List<RuleStream>? _streams;

    private bool HasPausedStreams
    {
      get
      {
        if (_streams == null)
          return false;
        foreach (var stream in _streams)
          if (stream.IsPaused)
            return true;
        return false;
      }
    }

    /// <summary>
    /// Creates a stream and runs it to its end or its first pause.
    /// </summary>
    private RuleStream StartStream(RuleStreamKind kind, IPropertyInfo? root, bool awaited, IRuleContext? completedContext = null, RuleStream? parent = null)
    {
      var runner = RuleRunner;
      // only an awaited stream (or a cascade collecting for one) needs the
      // affected properties of its async rules
      var collects = runner.CollectsAsyncResults && (awaited || parent != null);
      var stream = new RuleStream(this, kind, root, collects, completedContext, parent) { HasAwaiter = awaited };
      stream.Steps = runner.Run(this, stream).GetEnumerator();
      DriveStream(stream);
      return stream;
    }

    /// <summary>
    /// Runs a stream until it ends or waits for async rules that are not complete.
    /// </summary>
    private void DriveStream(RuleStream stream)
    {
      var previous = t_currentStream;
      t_currentStream = stream;
      try
      {
        var steps = stream.Steps!;
        while (true)
        {
          bool more;
          try
          {
            more = !stream.IsDiscarded && steps.MoveNext();
          }
          catch
          {
            FinishStream(stream);
            throw;
          }
          if (!more)
          {
            FinishStream(stream);
            return;
          }

          var pause = steps.Current;
          lock (_syncRoot)
          {
            if (stream.IsDiscarded)
            {
              FinishStream(stream);
              return;
            }
            // the rules may have completed while the stream got here
            if (pause.IsSatisfied)
              continue;

            stream.CurrentPause = pause;
            if (stream.Kind != RuleStreamKind.AllRules && stream.Coverage == null)
              stream.Coverage = GetCoverage(stream);
            _streams ??= [];
            if (!_streams.Contains(stream))
              _streams.Add(stream);
            // the properties whose rules wait are busy; the object is busy
            // until the stream ends
            var wasBusy = _isBusy;
            foreach (var item in pause.WaitingProperties)
            {
              var alreadyBusy = BusyProperties.Contains(item);
              BusyProperties.Add(item);
              stream.WaitEntries.Add(item);
              if (!alreadyBusy)
                _target.RuleStart(item);
            }
            RunningAsyncRules = true;
            // with no busy property (a rule with no affected properties),
            // report the object busy itself
            if (!wasBusy && BusyProperties.Count == 0)
              (_target as IHostRulesBusy)?.ObjectBusyChanged(true);
            return;
          }
        }
      }
      finally
      {
        t_currentStream = previous;
      }
    }

    /// <summary>
    /// Ends a stream. Async rules that completed and that the stream did
    /// not fold in run their completion cascade now. Async rules still
    /// running run it when they finish. A discarded stream runs no
    /// cascades: the run that replaced it reruns its rules.
    /// </summary>
    /// <returns>The names of the properties whose rules those completions ran.</returns>
    private List<string> FinishStream(RuleStream stream)
    {
      lock (_syncRoot)
      {
        var affected = new List<string>();
        if (stream.IsFinished)
          return affected;
        stream.IsFinished = true;
        stream.CurrentPause = null;
        _streams?.Remove(stream);
        if (!stream.IsDiscarded || stream.KeepsResults)
        {
          foreach (var run in stream.UnfoldedRuns)
          {
            if (run.IsCompleted && !run.IsDetached && run.Context != null)
            {
              run.IsDetached = true;
              affected.AddRange(RunCompletionCascade(run, run.Context));
            }
          }
        }
        stream.UnfoldedRuns.Clear();
        stream.AddAffected(affected);
        stream.Steps?.Dispose();
        return affected;
      }
    }

    /// <summary>
    /// Records the completion of an async rule started by a stream, and
    /// resumes the stream when it waits for no other rule.
    /// </summary>
    /// <remarks>Called inside the lock on _syncRoot, after the rule's output values and broken rules are applied.</remarks>
    private void CompleteAsyncRuleRun(AsyncRuleRun run, IRuleContext context)
    {
      var rule = context.Rule;
      var affectedNames = new List<string>(rule.AffectedProperties.Count + context.OutputPropertyValues.Count);
      affectedNames.AddRange(rule.AffectedProperties.Select(c => c.Name));
      affectedNames.AddRange(context.OutputPropertyValues.Select(c => c.Key.Name));
      var outcome = new RuleRunOutcome(
        affectedNames,
        context.DirtyProperties.Select(c => c.Name).ToList(),
        context.Results.Any(p => !p.Success && p.Severity == RuleSeverity.Error),
        context.Results.Any(p => p.StopProcessing));
      var stream = run.Stream;

      if (!run.IsOwned || stream.IsFinished || stream.IsDiscarded)
      {
        // completes outside its stream
        run.IsDetached = true;
        if (stream.CollectsAsyncResults)
          stream.AddAsyncAffected(affectedNames);
        var cascade = RunCompletionCascade(run, context);
        if (stream.CollectsAsyncResults)
          stream.AddAsyncAffected(cascade);
        run.Complete(context, outcome);
        CompleteAsyncRule(rule, cascade);
        return;
      }

      if (stream.CollectsAsyncResults)
        stream.AddAsyncAffected(affectedNames);
      run.Complete(context, outcome);
      if (stream.IsPaused && stream.CurrentPause!.IsSatisfied)
        ResumeStream(stream, rule.AffectedProperties);
      else
        CompleteAsyncRule(rule, []);
    }

    /// <summary>
    /// Runs the next part of a stream that no longer waits, then releases
    /// the busy state of the completed rule and of the stream's old pause.
    /// The object stays busy between the parts of a stream.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <param name="completedEntries">The busy entries of the rule whose completion resumes the stream.</param>
    /// <remarks>Called inside the lock on _syncRoot.</remarks>
    private void ResumeStream(RuleStream stream, IReadOnlyList<IPropertyInfo> completedEntries)
    {
      var entries = new List<IPropertyInfo>(completedEntries);
      entries.AddRange(stream.WaitEntries);
      stream.WaitEntries = [];
      stream.CurrentPause = null;
      // a running stream is not waiting: a nested run on this thread must
      // not discard it (DriveStream adds it again if it pauses)
      _streams?.Remove(stream);
      var start = stream.RunnerAffectedCount;
      System.Runtime.ExceptionServices.ExceptionDispatchInfo? error = null;
      try
      {
        DriveStream(stream);
      }
      catch (Exception ex)
      {
        error = System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(ex);
        if (stream.HasAwaiter)
          stream.Error = ex;
      }

      // PropertyChanged for the properties the resumed rules affected
      ReleaseBusy(entries, stream.GetRunnerAffected(start));

      // with no CheckRulesAsync to report it to, the exception propagates
      // from the rule completion, as one in a completion cascade does
      if (error != null && !stream.HasAwaiter)
        error.Throw();
    }

    /// <summary>
    /// Runs the completion cascade of an async rule that completed outside
    /// its stream: the rules of its affected properties. For a rule a
    /// stream owned (the rules have sequential async rules), the cascade
    /// runs as a stream of its own, so sequential rules in it wait.
    /// </summary>
    /// <returns>The names of the properties the cascade affected so far.</returns>
    /// <remarks>Called inside the lock on _syncRoot.</remarks>
    private List<string> RunCompletionCascade(AsyncRuleRun run, IRuleContext context)
    {
      if (run.OnCompleted == null)
        return [];
      if (!run.IsOwned)
        return InvokeInStream(run.Stream, run.OnCompleted, context);
      // collect into the root stream, and only if it collects (CheckRulesAsync awaits it)
      var collector = run.Stream.Parent ?? run.Stream;
      var cascade = StartStream(RuleStreamKind.Cascade, null, awaited: false, context, collector.CollectsAsyncResults ? collector : null);
      return cascade.GetRunnerAffected().Distinct().ToList();
    }

    /// <summary>
    /// Runs an async rule completion handler with the stream as the current
    /// stream, so the async rules it starts belong to the stream.
    /// </summary>
    private List<string> InvokeInStream(RuleStream stream, AsyncRuleCompletedHandler? handler, IRuleContext context)
    {
      if (handler == null)
        return [];
      var previous = t_currentStream;
      t_currentStream = stream;
      try
      {
        return handler(context).ToList();
      }
      finally
      {
        t_currentStream = previous;
      }
    }

    /// <summary>
    /// Discards the paused streams a new run replaces: any stream for a
    /// new CheckRules(); streams whose rules can run for the property for
    /// CheckRules(property); object-rule streams for CheckObjectRules().
    /// </summary>
    private List<RuleStream> DiscardStreams(RuleStreamKind kind, IPropertyInfo? root)
    {
      lock (_syncRoot)
      {
        if (_streams == null || _streams.Count == 0)
          return [];
        var discarded = _streams.Where(s => kind switch
        {
          RuleStreamKind.AllRules => true,
          RuleStreamKind.Property => s.Covers(root!),
          _ => s.Kind == RuleStreamKind.ObjectRules
        }).ToList();
        foreach (var stream in discarded)
        {
          stream.IsDiscarded = true;
          _streams.Remove(stream);
        }
        return discarded;
      }
    }

    /// <summary>
    /// Ends the discarded streams after the run that replaced them, and
    /// restarts those of another kind or root, so the rules they had not
    /// reached still run.
    /// </summary>
    /// <param name="discarded">The discarded streams.</param>
    /// <param name="kind">What the replacing run ran.</param>
    /// <param name="root">The property of the replacing run.</param>
    /// <param name="restart">
    /// False if the replacing run failed: the discarded streams only
    /// release their busy state.
    /// </param>
    private void EndDiscardedStreams(List<RuleStream> discarded, RuleStreamKind kind, IPropertyInfo? root, bool restart)
    {
      var notify = new List<string>();
      lock (_syncRoot)
      {
        foreach (var stream in discarded)
        {
          // nothing reruns the rules of a failed replacement: keep their results
          if (!restart)
            stream.KeepsResults = true;
          notify.AddRange(FinishStream(stream));
          var entries = stream.WaitEntries;
          stream.WaitEntries = [];
          if (entries.Count > 0)
            ReleaseBusy(entries, []);
        }
      }
      foreach (var name in notify.Distinct())
        _target.RuleComplete(name);

      // a new CheckRules() ran every rule: nothing to restart
      if (!restart || kind == RuleStreamKind.AllRules)
        return;
      var restarted = new List<(RuleStreamKind Kind, IPropertyInfo? Root)>();
      foreach (var stream in discarded)
      {
        if (stream.Kind == kind && ReferenceEquals(stream.Root, root))
          continue;
        if (stream.Kind != RuleStreamKind.Cascade)
        {
          if (restarted.Exists(r => r.Kind == stream.Kind && ReferenceEquals(r.Root, stream.Root)))
            continue;
          restarted.Add((stream.Kind, stream.Root));
        }
        var restartedStream = StartStream(stream.Kind, stream.Root, awaited: false, stream.CompletedContext, stream.Parent);
        foreach (var name in restartedStream.GetRunnerAffected().Distinct())
          _target.RuleComplete(name);
      }
    }

    /// <summary>
    /// Gets the names of the properties whose rules a property or
    /// object-rule stream can run: from the root, the affected and input
    /// properties of covered rules, and the primary properties of rules
    /// that take a covered property as input.
    /// </summary>
    private HashSet<string> GetCoverage(RuleStream stream)
    {
      var rules = TypeRules.Rules;
      var covered = new HashSet<string>();
      if (stream.Root != null)
      {
        covered.Add(stream.Root.Name);
      }
      else if (stream.CompletedContext != null)
      {
        // a completion cascade runs the rules of the completed rule's affected properties
        foreach (var item in stream.CompletedContext.Rule.AffectedProperties)
          if (!ReferenceEquals(item, stream.CompletedContext.Rule.PrimaryProperty))
            covered.Add(item.Name);
      }
      else
      {
        foreach (var rule in rules)
          if (rule.PrimaryProperty == null)
          {
            foreach (var item in rule.AffectedProperties)
              covered.Add(item.Name);
            foreach (var item in rule.InputProperties)
              covered.Add(item.Name);
          }
      }

      var changed = true;
      while (changed)
      {
        changed = false;
        foreach (var rule in rules)
        {
          if (rule.PrimaryProperty == null)
            continue;
          if (covered.Contains(rule.PrimaryProperty.Name))
          {
            foreach (var item in rule.AffectedProperties)
              changed |= covered.Add(item.Name);
            foreach (var item in rule.InputProperties)
              changed |= covered.Add(item.Name);
          }
          else if (rule.InputProperties.Exists(p => covered.Contains(p.Name)))
          {
            changed |= covered.Add(rule.PrimaryProperty.Name);
          }
        }
      }
      return covered;
    }

    #endregion

    /// <summary>
    /// Raises the notifications for a completed async rule, then clears
    /// its busy state. The object reports itself idle last, so once
    /// IsBusy is false (and WaitForIdle or CheckRulesAsync returns), no
    /// more completion work runs for the rule.
    /// </summary>
    /// <param name="rule">The completed rule.</param>
    /// <param name="affected">Names of further properties whose rules ran on completion.</param>
    /// <remarks>Called inside the lock on _syncRoot.</remarks>
    private void CompleteAsyncRule(IBusinessRuleBase rule, IEnumerable<string> affected) =>
      ReleaseBusy(rule.AffectedProperties, affected);

    /// <summary>
    /// Raises the completion notifications for a list of busy entries,
    /// then removes them. The object reports itself idle last, so once
    /// IsBusy is false (and WaitForIdle or CheckRulesAsync returns), no
    /// more completion work runs.
    /// </summary>
    /// <param name="entries">
    /// The busy entries to release: one per property for a running rule
    /// (its affected properties) or for a stream's wait.
    /// </param>
    /// <param name="affected">Names of further properties whose rules ran.</param>
    /// <remarks>Called inside the lock on _syncRoot.</remarks>
    private void ReleaseBusy(IReadOnlyList<IPropertyInfo> entries, IEnumerable<string> affected)
    {
      // BusyProperties holds one entry per running rule for each of its affected
      // properties, and one per waiting property for each paused stream. The
      // handlers of the notifications below can start rules, or complete other
      // rules, so each decision checks the live list, discounting the entries
      // being released (they stay until the object reports idle).

      // raise the completion notifications while the object is still busy
      var completed = new List<IPropertyInfo>();
      foreach (var item in entries.Distinct())
      {
        if (!IsBusyForOthers(item, entries))
        {
          completed.Add(item);
          _target.RuleComplete(item);
        }
      }

      foreach (var property in affected.Distinct())
      {
        // property is not in the entries (already signalled to UI)
        if (!entries.Any(p => p.Name == property))
          _target.RuleComplete(property);
      }

      if (!RunningRules && BusyProperties.Count == entries.Count && !HasPausedStreams)
        _target.AllRulesComplete();

      // clear the busy state
      foreach (var item in entries)
        BusyProperties.Remove(item);
      var idle = entries.Distinct().Where(p => !BusyProperties.Contains(p)).ToList();

      // another rule for a property may have kept it busy when its notification
      // was due, then completed in a handler above without notifying, because
      // these entries were still there
      foreach (var item in idle)
      {
        if (!completed.Contains(item))
          _target.RuleComplete(item);
      }

      // report each property idle; the object becomes idle with the last one
      for (var i = 0; i < idle.Count; i++)
      {
        if (i == idle.Count - 1)
          _isBusy = BusyProperties.Count > 0 || HasPausedStreams;
        // a handler may have started a rule for this property again
        if (!BusyProperties.Contains(idle[i]))
          _target.PropertyIdle(idle[i]);
      }
      // a stream with no busy property (such as one waiting for an object
      // rule with no affected properties) kept the object busy; with no
      // property to report idle, report the object idle itself, last
      if (idle.Count == 0 && _isBusy && BusyProperties.Count == 0 && !HasPausedStreams)
      {
        _isBusy = false;
        (_target as IHostRulesBusy)?.ObjectBusyChanged(false);
      }

      // release CheckRulesAsync last
      if (!_isBusy)
        BusyChanged.Set();
    }

    /// <summary>
    /// Gets a value indicating whether busy entries other than the
    /// specified ones keep a property busy.
    /// </summary>
    private bool IsBusyForOthers(IPropertyInfo property, IReadOnlyList<IPropertyInfo> entries) =>
      BusyProperties.Count(p => Equals(p, property)) > entries.Count(p => Equals(p, property));

    private static async void RunAsyncRule(IBusinessRuleAsync asyncRule, IRuleContext context, IUnhandledAsyncRuleExceptionHandler unhandledAsyncRuleExceptionHandler, bool catchAll)
    {
      try
      {
        await asyncRule.ExecuteAsync(context);
      }
      catch (Exception exc) when (unhandledAsyncRuleExceptionHandler.CanHandle(exc, asyncRule))
      {
        await unhandledAsyncRuleExceptionHandler.Handle(exc, asyncRule, context);
      }
      catch (Exception exc) when (catchAll)
      {
        // a sequential rule completes with an error result, which
        // short-circuits the rules after it like any broken rule
        context.AddErrorResult($"{asyncRule.RuleName}:{exc.Message}");
      }
      finally
      {
        context.Complete();
      }
    }

    #region DataAnnotations

    /// <summary>
    /// Adds validation rules corresponding to property
    /// data annotation attributes.
    /// </summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    [UnconditionalSuppressMessage("Trimming", "IL2072", Justification = Csla.Core.TrimmingMessages.BusinessTypeJustification)]
    public void AddDataAnnotations()
    {
      var options = _applicationContext.GetRequiredService<Configuration.CslaOptions>();
      if (options.ScanDataAnnotations)
      {
        Type metadataType;
#if !NETSTANDARD2_0 || NET8_0_OR_GREATER
        // add data annotations from metadata class if specified
        var classAttList = _target.GetType().GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.MetadataTypeAttribute), true);
        if (classAttList.Length > 0)
        {
          metadataType = ((System.ComponentModel.DataAnnotations.MetadataTypeAttribute)classAttList[0]).MetadataClassType;
          AddDataAnnotationsFromType(metadataType);
        }
#endif

        // attributes on class
        metadataType = _target.GetType();
        AddDataAnnotationsFromType(metadataType);
      }
    }

    /// <summary>
    /// Adds data annotation validation rules from the given type.
    /// </summary>
    /// <param name="metadataType">Type of the metadata.</param>
    private void AddDataAnnotationsFromType([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] Type metadataType)
    {
      var attList = metadataType.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.ValidationAttribute), true);
      foreach (var att in attList)
        AddRule(new CommonRules.DataAnnotation(null, (System.ComponentModel.DataAnnotations.ValidationAttribute)att));

      // attributes on properties
      var propList = metadataType.GetProperties();
      foreach (var prop in propList)
      {
        attList = prop.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.ValidationAttribute), true);
        foreach (var att in attList)
        {
          var target = (IManageProperties)_target;
          var pi = target.GetManagedProperties().First(c => c.Name == prop.Name);
          AddRule(new CommonRules.DataAnnotation(pi, (System.ComponentModel.DataAnnotations.ValidationAttribute)att));
        }
      }
    }

    #endregion

    #region MobileObject overrides

    /// <summary>
    /// Override this method to insert your field values
    /// into the MobileFormatter serialization stream.
    /// </summary>
    /// <param name="info">
    /// Object containing the data to serialize.
    /// </param>
    protected override void OnGetState(SerializationInfo info)
    {
      info.AddValue("_processThroughPriority", _processThroughPriority);
      info.AddValue("_ruleSet", _ruleSet);
      info.AddValue("_cascadeWhenChanged", _cascadeOnDirtyProperties);
      base.OnGetState(info);
    }

    /// <summary>
    /// Override this method to retrieve your field values
    /// from the MobileFormatter serialization stream.
    /// </summary>
    /// <param name="info">
    /// Object containing the data to serialize.
    /// </param>
    protected override void OnSetState(SerializationInfo info)
    {
      _processThroughPriority = info.GetValue<int>("_processThroughPriority");
      _ruleSet = info.GetValue<string>("_ruleSet");
      _cascadeOnDirtyProperties = info.GetValue<bool>("_cascadeWhenChanged");
      base.OnSetState(info);
    }

    /// <summary>
    /// Override this method to insert your child object
    /// references into the MobileFormatter serialization stream.
    /// </summary>
    /// <param name="info">
    /// Object containing the data to serialize.
    /// </param>
    /// <param name="formatter">
    /// Reference to MobileFormatter instance. Use this to
    /// convert child references to/from reference id values.
    /// </param>
    protected override void OnGetChildren(SerializationInfo info, MobileFormatter formatter)
    {
      if (_brokenRules != null && _brokenRules.Count > 0)
      {
        SerializationInfo brInfo = formatter.SerializeObject(_brokenRules);
        info.AddChild("_brokenRules", brInfo.ReferenceId);
      }

      base.OnGetChildren(info, formatter);
    }

    /// <summary>
    /// Override this method to retrieve your child object
    /// references from the MobileFormatter serialization stream.
    /// </summary>
    /// <param name="info">
    /// Object containing the data to serialize.
    /// </param>
    /// <param name="formatter">
    /// Reference to MobileFormatter instance. Use this to
    /// convert child references to/from reference id values.
    /// </param>
    protected override void OnSetChildren(SerializationInfo info, MobileFormatter formatter)
    {
      if (info.Children.TryGetValue("_brokenRules", out var child))
      {
        int referenceId = child.ReferenceId;
        _brokenRules = (BrokenRulesCollection?)formatter.GetObject(referenceId);
      }

      base.OnSetChildren(info, formatter);
    }
    #endregion

    #region Serialization Notification

    void ISerializationNotification.Deserialized()
    {
      _syncRoot = new();
      Deserialized();
    }

    /// <summary>
    /// Invoked after the object has been deserialized to allow derived classes to perform custom post-deserialization
    /// processing.
    /// </summary>
    /// <remarks>Override this method in a derived class to implement actions that should occur immediately
    /// after deserialization, such as initializing transient fields or validating object state. This method is called
    /// automatically during the deserialization process.</remarks>
    protected virtual void Deserialized()
    {
      // for override
    }

    #endregion

    #region Get All Broken Rules (tree)

    /// <summary>
    /// Gets all nodes in tree that have IsValid = false (and all parents)
    /// </summary>
    /// <param name="root">The root.</param>
    /// <returns>BrukenRulesTree list</returns>
    /// <exception cref="ArgumentNullException"><paramref name="root"/> is <see langword="null"/>.</exception>
    public static BrokenRulesTree GetAllBrokenRules(object root)
    {
      return GetAllBrokenRules(root, true);
    }
    /// <summary>
    /// Gets all nodes in tree that have broken rules.
    /// </summary>
    /// <param name="root">The root.</param>
    /// <param name="errorsOnly">if set to <c>true</c> will only return objects that gave IsValid = false.</param>
    /// <exception cref="ArgumentNullException"><paramref name="root"/> is <see langword="null"/>.</exception>
    public static BrokenRulesTree GetAllBrokenRules(object root, bool errorsOnly)
    {
      if (root is null)
        throw new ArgumentNullException(nameof(root));
      var list = new BrokenRulesTree();
      long counter = 1;
      long childBrokenRuleCount = 0;
      AddNodeToBrokenRules(ref list, ref counter, null, root, errorsOnly, ref childBrokenRuleCount);

      return list;
    }

    private static void AddNodeToBrokenRules(ref BrokenRulesTree list, ref long counter, object? parentKey, object obj, bool errorsOnly, ref long childBrokenRuleCount)
    {
      // is this a single editable object
      if (obj is BusinessBase bbase)
      {
        var nodeKey = counter++;
        var bo = bbase;
        long myChildBrokenRuleCount = bo.BrokenRulesCollection.Count;
        var node = new BrokenRulesNode(parentKey, nodeKey, bo.BrokenRulesCollection, obj);
        list.Add(node);

        // get managed child properties 
        foreach (var child in ((IManageProperties)bo).GetChildren())
        {
          AddNodeToBrokenRules(ref list, ref counter, nodeKey, child, errorsOnly, ref myChildBrokenRuleCount);
        }

        // remove node if it has no child with broken rules. 
        if (!errorsOnly && myChildBrokenRuleCount == 0)
        {
          list.Remove(node);
        }
        if (errorsOnly && bo.IsValid)
        {
          list.Remove(node);
        }
        childBrokenRuleCount += myChildBrokenRuleCount;
      }

      // or a list of EditableObject (both BindingList and ObservableCollection)
      else if (obj is IEditableCollection collection)
      {
        var nodeKey = counter++;
        var isValid = collection.IsValid;
        var node = new BrokenRulesNode(parentKey, nodeKey, new BrokenRulesCollection(true), collection);
        long myChildBrokenRuleCount = 0;

        list.Add(node);

        foreach (var child in (IEnumerable)collection)
        {
          AddNodeToBrokenRules(ref list, ref counter, nodeKey, child, errorsOnly, ref myChildBrokenRuleCount);
        }

        // remove node if it has no child with broken rules. 
        if (!errorsOnly && myChildBrokenRuleCount == 0)
        {
          list.Remove(node);
        }
        if (errorsOnly && isValid)
        {
          list.Remove(node);
        }
        childBrokenRuleCount += myChildBrokenRuleCount;
      }
      return;
    }

    #endregion


    object IBusinessRules.Target => Target;
  }
}