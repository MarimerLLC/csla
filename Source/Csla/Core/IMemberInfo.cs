//-----------------------------------------------------------------------
// <copyright file="IMemberInfo.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Maintains metadata about a method or property.</summary>
//-----------------------------------------------------------------------

namespace Csla.Core
{
  /// <summary>
  /// Maintains metadata about a method or property.
  /// </summary>
  public interface IMemberInfo
  {
    /// <summary>
    /// Gets the member name value.
    /// </summary>
    string Name { get; }
  }
}