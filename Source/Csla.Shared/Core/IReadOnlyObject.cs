//-----------------------------------------------------------------------
// <copyright file="IReadOnlyObject.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Specifies that the object is a readonly</summary>
//-----------------------------------------------------------------------
namespace Csla.Core
{
  /// <summary>
  /// Specifies that the object is a readonly
  /// business object.
  /// </summary>
  public interface IReadOnlyObject : IBusinessObject
  {
    /// <summary>
    /// Returns true if the user is allowed to read the
    /// calling property.
    /// </summary>
    /// <param name="propertyName">Name of the property to read.</param>
    bool CanReadProperty(string propertyName);
  }
}