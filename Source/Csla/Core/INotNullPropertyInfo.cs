//-----------------------------------------------------------------------
// <copyright file="INotNullPropertyInfo.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Marks a property whose value must not be null.</summary>
//-----------------------------------------------------------------------

namespace Csla.Core
{
  /// <summary>
  /// Marks a registered property whose value must not be null.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Implemented by <see cref="NotNullPropertyInfo{T}"/>. The framework rejects
  /// null writes to such a property, and the data portal verifies at the end of
  /// the operation that completes an object (Create/Fetch, or Execute for a command)
  /// that every not-null property holds a value.
  /// </para>
  /// <para>
  /// This interface has no members. Test for it with a type check
  /// (<c>propertyInfo is INotNullPropertyInfo</c>).
  /// </para>
  /// </remarks>
  public interface INotNullPropertyInfo;
}
