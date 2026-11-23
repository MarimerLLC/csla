//-----------------------------------------------------------------------
// <copyright file="IFieldManagerAccess.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Gives the not-null invariant check access to a field manager.</summary>
//-----------------------------------------------------------------------

using Csla.Core.FieldManager;

namespace Csla.Core
{
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
