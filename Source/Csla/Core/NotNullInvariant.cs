//-----------------------------------------------------------------------
// <copyright file="NotNullInvariant.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Verifies the not-null property invariant of a business object.</summary>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Csla.Core.FieldManager;
using Csla.Properties;

namespace Csla.Core
{
  /// <summary>
  /// Verifies that every not-null property (<see cref="INotNullPropertyInfo"/>)
  /// of a business object holds a value.
  /// </summary>
  internal static class NotNullInvariant
  {
    /// <summary>
    /// Gets the not-null reference-type properties of <paramref name="obj"/>
    /// that hold null, or were never loaded and have no non-null default.
    /// An object without a field manager (a list) gives an empty list.
    /// The object is not changed, and no field manager is created.
    /// </summary>
    [UnconditionalSuppressMessage("Trimming", "IL2072", Justification = TrimmingMessages.BusinessTypeJustification)]
    internal static IReadOnlyList<IPropertyInfo> GetUnsatisfied(object obj)
    {
      if (obj is not IUseFieldManager target)
        return [];
      if (obj is IFieldManagerAccess access && access.FieldManagerIfCreated is null)
        return FieldDataManager.GetUnsatisfiedNotNullProperties(obj.GetType());
      return target.FieldManager.GetUnsatisfiedNotNullProperties();
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
  }
}
