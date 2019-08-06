//-----------------------------------------------------------------------
// <copyright file="IFieldData.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Defines the members required by a field</summary>
//-----------------------------------------------------------------------
using Csla.Serialization.Mobile;

namespace Csla.Core.FieldManager
{
  /// <summary>
  /// Defines the members required by a field
  /// data storage object.
  /// </summary>
  public interface IFieldData : ITrackStatus, IMobileObject
  {
    /// <summary>
    /// Gets the name of the field.
    /// </summary>
    string Name { get; }
    /// <summary>
    /// Gets or sets the field value.
    /// </summary>
    /// <value>The value of the field.</value>
    /// <returns>The value of the field.</returns>
    object Value { get; set; }
    /// <summary>
    /// Marks the field as unchanged.
    /// </summary>
    void MarkClean();
  }
}