//-----------------------------------------------------------------------
// <copyright file="IFieldDataT.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Defines the members required by a field</summary>
//-----------------------------------------------------------------------
namespace Csla.Core.FieldManager
{
  /// <summary>
  /// Defines the members required by a field
  /// data storage object.
  /// </summary>
  public interface IFieldData<T> : IFieldData
  {
    /// <summary>
    /// Gets or sets the field value.
    /// </summary>
    /// <value>The value of the field.</value>
    /// <returns>The value of the field.</returns>
    new T Value { get; set; }
  }
}