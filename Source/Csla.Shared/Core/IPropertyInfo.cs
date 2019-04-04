//-----------------------------------------------------------------------
// <copyright file="IPropertyInfo.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Maintains metadata about a property.</summary>
//-----------------------------------------------------------------------
using System;

namespace Csla.Core
{
  /// <summary>
  /// Maintains metadata about a property.
  /// </summary>
  public interface IPropertyInfo : IMemberInfo
  {
    /// <summary>
    /// Gets the type of the property.
    /// </summary>
    Type Type { get; }
    /// <summary>
    /// Gets the friendly display name
    /// for the property.
    /// </summary>
    string FriendlyName { get; }
    /// <summary>
    /// Gets the default initial value for the property.
    /// </summary>
    /// <remarks>
    /// This value is used to initialize the property's
    /// value, and is returned from a property get
    /// if the user is not authorized to 
    /// read the property.
    /// </remarks>
    object DefaultValue { get; }
    /// <summary>
    /// Gets a new field data container for the property.
    /// </summary>
    Core.FieldManager.IFieldData NewFieldData(string name);
    /// <summary>
    /// Gets the relationship between the declaring object
    /// and the object reference in the property.
    /// </summary>
    RelationshipTypes RelationshipType { get; }
	  /// <summary>
	  /// Gets or sets the index position for the managed
	  /// field storage behind the property. FOR
	  /// INTERNAL CSLA .NET USE ONLY.
	  /// </summary>
    int Index { get; set; }
  }
}