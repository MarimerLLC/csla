//-----------------------------------------------------------------------
// <copyright file="RuleCache.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Resets CSLA's per-type rule caches between unit tests</summary>
//-----------------------------------------------------------------------

using System.Collections;
using System.Globalization;
using System.Reflection;
using Csla.Rules;
using Csla.Testing.Properties;

namespace Csla.Testing.Rules
{
  /// <summary>
  /// Resets the per-type business and authorization rule caches, so that the next use of a
  /// business type registers its rules again.
  /// </summary>
  /// <remarks>
  /// <para>
  /// CSLA registers a type's rules once, on first use, and caches them for the life of the
  /// process; <c>AddBusinessRules</c> and <c>AddObjectAuthorizationRules</c> are not called
  /// again for that type. That is correct in an application, and awkward in a test process,
  /// where one test's registration is still in place for every test that follows.
  /// </para>
  /// <para>
  /// Clearing the caches between tests matters wherever registration depends on something that
  /// varies per test — a different rule set, different configuration, or a mutation-testing
  /// runner that reuses a test host across mutants and would otherwise evaluate one mutant's
  /// rules against another's cached registration.
  /// </para>
  /// <example>
  /// <code>
  /// // in a per-test setup
  /// RuleCache.Clear();
  /// </code>
  /// </example>
  /// </remarks>
  public static class RuleCache
  {
    private const string PerTypeRulesField = "_perTypeRules";
    private const string CleanupMethod = "CleanupRulesForType";

    /// <summary>
    /// Clears the cached business and authorization rules for every type.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// The internal members this relies on were not found, or were not of the expected shape,
    /// in the referenced version of CSLA.
    /// </exception>
    public static void Clear()
    {
      ClearBusinessRules();
      ClearAuthorizationRules();
    }

    /// <summary>
    /// Clears the cached business and authorization rules for one type.
    /// </summary>
    /// <param name="type">The business type whose rules should be forgotten.</param>
    /// <exception cref="ArgumentNullException"><paramref name="type"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">
    /// The internal members this relies on were not found, or were not of the expected shape,
    /// in the referenced version of CSLA.
    /// </exception>
    public static void Clear(Type type)
    {
      if (type is null)
        throw new ArgumentNullException(nameof(type));

      ClearBusinessRules(type);
      ClearAuthorizationRules(type);
    }

    /// <summary>
    /// Clears the cached business rules for every type.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// The internal members this relies on were not found, or were not of the expected shape,
    /// in the referenced version of CSLA.
    /// </exception>
    public static void ClearBusinessRules() => ClearAll(typeof(BusinessRuleManager));

    /// <summary>
    /// Clears the cached business rules for one type.
    /// </summary>
    /// <param name="type">The business type whose rules should be forgotten.</param>
    /// <exception cref="ArgumentNullException"><paramref name="type"/> is <see langword="null"/>.</exception>
    public static void ClearBusinessRules(Type type)
    {
      if (type is null)
        throw new ArgumentNullException(nameof(type));

      // No reflection needed: Csla.Testing is in Csla's InternalsVisibleTo list and this method
      // is internal. A rename therefore breaks the build here rather than at run time.
      BusinessRuleManager.CleanupRulesForType(type);
    }

    /// <summary>
    /// Clears the cached authorization rules for every type.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// The internal members this relies on were not found, or were not of the expected shape,
    /// in the referenced version of CSLA.
    /// </exception>
    public static void ClearAuthorizationRules() => ClearAll(typeof(AuthorizationRuleManager));

    /// <summary>
    /// Clears the cached authorization rules for one type.
    /// </summary>
    /// <param name="type">The business type whose rules should be forgotten.</param>
    /// <exception cref="ArgumentNullException"><paramref name="type"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">
    /// The internal members this relies on were not found, or were not of the expected shape,
    /// in the referenced version of CSLA.
    /// </exception>
    public static void ClearAuthorizationRules(Type type)
    {
      if (type is null)
        throw new ArgumentNullException(nameof(type));

      // AuthorizationRuleManager.CleanupRulesForType is private, so unlike its business-rule
      // counterpart it cannot be reached through InternalsVisibleTo.
      // Fully qualified: CSLA has its own Csla.MethodInfo, and this namespace sits under Csla,
      // so the unqualified name binds to that one rather than the reflection type.
      System.Reflection.MethodInfo method = typeof(AuthorizationRuleManager)
        .GetMethod(CleanupMethod, BindingFlags.Static | BindingFlags.NonPublic, null, [typeof(Type)], null)
        ?? throw NotFound(CleanupMethod, typeof(AuthorizationRuleManager));

      method.Invoke(null, [type]);
    }

    /// <summary>
    /// Empties a manager's per-type cache wholesale.
    /// </summary>
    /// <remarks>
    /// The dictionary itself is private on both managers, so there is no way to reach it other
    /// than reflection. It is held in a <see cref="Lazy{T}"/>, and both managers take a lock on
    /// the field before mutating it — so the same lock is taken here, rather than clearing
    /// underneath a registration in progress on another thread.
    /// </remarks>
    private static void ClearAll(Type managerType)
    {
      System.Reflection.FieldInfo field = managerType.GetField(PerTypeRulesField, BindingFlags.Static | BindingFlags.NonPublic)
        ?? throw NotFound(PerTypeRulesField, managerType);

      object? lazy = field.GetValue(null);
      if (lazy is null)
        throw UnexpectedShape(PerTypeRulesField, managerType);

      System.Reflection.PropertyInfo? valueProperty = lazy.GetType().GetProperty("Value");
      if (valueProperty is null)
        throw UnexpectedShape(PerTypeRulesField, managerType);

      // Both managers lock on the field's value (the Lazy instance) around their own mutations.
      lock (lazy)
      {
        if (valueProperty.GetValue(lazy) is not IDictionary cache)
          throw UnexpectedShape(PerTypeRulesField, managerType);

        cache.Clear();
      }
    }

    private static InvalidOperationException NotFound(string member, Type declaringType)
      => new(string.Format(CultureInfo.CurrentCulture, Resources.RuleCacheResetFailed, member, declaringType.FullName));

    private static InvalidOperationException UnexpectedShape(string member, Type declaringType)
      => new(string.Format(CultureInfo.CurrentCulture, Resources.RuleCacheResetUnexpectedShape, member, declaringType.FullName));
  }
}
