//-----------------------------------------------------------------------
// <copyright file="NotNullInvariant.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Verifies the not-null property invariant of a business object.</summary>
//-----------------------------------------------------------------------

using Csla.Core.FieldManager;
using Csla.Properties;

namespace Csla.Core
{
  /// <summary>
  /// Verifies that every not-null property (<see cref="INotNullPropertyInfo"/>)
  /// of a business object holds a value.
  /// </summary>
  /// <remarks>
  /// <para>
  /// A not-null property satisfies the invariant when its field holds a value,
  /// or when it was never loaded and has a non-null default value. Value-type
  /// properties always satisfy it.
  /// </para>
  /// <para>
  /// The data portal checks the invariant itself: on exit from Create (not for
  /// commands), Fetch and Execute, and on entry to (not for commands) and exit
  /// from Update, for root objects and for child objects that go through the
  /// child data portal. Use this class for objects the data portal does not see,
  /// such as the child objects and list items an <see cref="Server.ObjectFactory"/>
  /// builds by hand.
  /// </para>
  /// <para>
  /// Each call checks one object and does not recurse into its children.
  /// Objects without managed properties, such as lists, always satisfy it.
  /// </para>
  /// </remarks>
  public static class NotNullInvariant
  {
    /// <summary>
    /// Gets the not-null properties of an object that do not satisfy the
    /// invariant: they hold null, or were never loaded and have no default value.
    /// </summary>
    /// <param name="obj">The object to check. It is not changed.</param>
    /// <returns>The failing properties, or an empty list when the invariant holds.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="obj"/> is <see langword="null"/>.</exception>
    public static IReadOnlyList<IPropertyInfo> GetUnsatisfiedProperties(object obj)
    {
      if (obj is null)
        throw new ArgumentNullException(nameof(obj));

      return GetUnsatisfied(obj);
    }

    /// <summary>
    /// Verifies that every not-null property of an object satisfies the invariant.
    /// </summary>
    /// <param name="obj">The object to check. It is not changed.</param>
    /// <exception cref="ArgumentNullException"><paramref name="obj"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">A not-null property is null, or was never
    /// loaded and has no default value. The message names the type and every failing property.</exception>
    public static void Verify(object obj)
    {
      if (obj is null)
        throw new ArgumentNullException(nameof(obj));

      var failing = GetUnsatisfied(obj);
      if (failing.Count == 0)
        return;

      throw new InvalidOperationException(string.Format(
        Resources.NotNullInvariantViolated,
        obj.GetType().FullName,
        string.Join(", ", failing.Select(p => p.Name))));
    }

    /// <summary>
    /// Gets the not-null reference-type properties of <paramref name="obj"/>
    /// that hold null, or were never loaded and have no non-null default.
    /// An object without a field manager (a list) gives an empty list.
    /// The object is not changed, and no field manager is created.
    /// </summary>
    internal static IReadOnlyList<IPropertyInfo> GetUnsatisfied(object obj)
    {
      if (obj is IFieldManagerAccess access)
        return access.GetUnsatisfiedNotNullProperties();
      if (obj is IUseFieldManager target)
        return target.FieldManager.GetUnsatisfiedNotNullProperties();
      return [];
    }

    /// <summary>
    /// Throws <see cref="InvalidOperationException"/> naming the type and every
    /// failing property when <paramref name="obj"/> breaks the invariant.
    /// </summary>
    /// <param name="obj">The object to check; null is ignored.</param>
    /// <param name="operation">The data portal operation, for the message.</param>
    /// <param name="isEntry">True when the operation has not started yet.</param>
    internal static void Check(object? obj, string operation, bool isEntry)
    {
      if (obj is null)
        return;

      var failing = GetUnsatisfied(obj);
      if (failing.Count == 0)
        return;

      throw new InvalidOperationException(string.Format(
        isEntry ? Resources.NotNullInvariantViolatedOnEntry : Resources.NotNullInvariantViolatedOnExit,
        operation,
        obj.GetType().FullName,
        string.Join(", ", failing.Select(p => p.Name))));
    }
  }

  /// <summary>
  /// Gives the not-null invariant check access to a business object's
  /// field manager without creating it.
  /// </summary>
  internal interface IFieldManagerAccess
  {
    /// <summary>
    /// Gets the field manager, or null if it has not been created.
    /// </summary>
    FieldDataManager? FieldManagerIfCreated { get; }

    /// <summary>
    /// Gets the unsatisfied not-null properties; without a field manager,
    /// from the type's cached property list. Implemented by the base classes,
    /// which carry DynamicallyAccessedMembers.All, so GetType() is preserved.
    /// </summary>
    IReadOnlyList<IPropertyInfo> GetUnsatisfiedNotNullProperties();
  }
}
