//-----------------------------------------------------------------------
// <copyright file="DefaultPropertyInfoFactory.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Creates PropertyInfo objects.</summary>
//-----------------------------------------------------------------------
using System;

namespace Csla.Core.FieldManager
{
  /// <summary>
  /// Creates PropertyInfo objects.
  /// </summary>
  internal class DefaultPropertyInfoFactory : Csla.Core.IPropertyInfoFactory
  {
    /// <summary>
    /// Creates a new instance of PropertyInfo.
    /// </summary>
    /// <param name="containingType">
    /// Type of business class that contains the property
    /// declaration.
    /// </param>
    /// <param name="name">Name of the property.</param>
    public Csla.PropertyInfo<T> Create<T>(Type containingType, string name)
    {
      return new Csla.PropertyInfo<T>(name, null, containingType, PropertyInfo<T>.DataBindingFriendlyDefault(), RelationshipTypes.None);
    }

    /// <summary>
    /// Creates a new instance of PropertyInfo.
    /// </summary>
    /// <param name="containingType">
    /// Type of business class that contains the property
    /// declaration.
    /// </param>
    /// <param name="name">Name of the property.</param>
    /// <param name="friendlyName">
    /// Friendly display name for the property.
    /// </param>
    public Csla.PropertyInfo<T> Create<T>(Type containingType, string name, string friendlyName)
    {
      return new Csla.PropertyInfo<T>(name, friendlyName, containingType, PropertyInfo<T>.DataBindingFriendlyDefault(), RelationshipTypes.None);
    }

    /// <summary>
    /// Creates a new instance of PropertyInfo.
    /// </summary>
    /// <param name="containingType">
    /// Type of business class that contains the property
    /// declaration.
    /// </param>
    /// <param name="name">Name of the property.</param>
    /// <param name="friendlyName">
    /// Friendly display name for the property.
    /// </param>
    /// <param name="relationship">Relationship with
    /// referenced object.</param>
    public Csla.PropertyInfo<T> Create<T>(Type containingType, string name, string friendlyName, RelationshipTypes relationship)
    {
      return new Csla.PropertyInfo<T>(name, friendlyName, containingType, PropertyInfo<T>.DataBindingFriendlyDefault(), relationship);
    }

    /// <summary>
    /// Creates a new instance of PropertyInfo.
    /// </summary>
    /// <param name="containingType">
    /// Type of business class that contains the property
    /// declaration.
    /// </param>
    /// <param name="name">Name of the property.</param>
    /// <param name="friendlyName">
    /// Friendly display name for the property.
    /// </param>
    /// <param name="defaultValue">
    /// Default value for the property.
    /// </param>
    public Csla.PropertyInfo<T> Create<T>(Type containingType, string name, string friendlyName, T defaultValue)
    {
      return new Csla.PropertyInfo<T>(name, friendlyName, containingType, defaultValue, RelationshipTypes.None);
    }

    /// <summary>
    /// Creates a new instance of PropertyInfo.
    /// </summary>
    /// <param name="containingType">
    /// Type of business class that contains the property
    /// declaration.
    /// </param>
    /// <param name="name">Name of the property.</param>
    /// <param name="friendlyName">
    /// Friendly display name for the property.
    /// </param>
    /// <param name="defaultValue">
    /// Default value for the property.
    /// </param>
    /// <param name="relationship">Relationship with
    /// referenced object.</param>
    public Csla.PropertyInfo<T> Create<T>(Type containingType, string name, string friendlyName, T defaultValue, RelationshipTypes relationship)
    {
      return new Csla.PropertyInfo<T>(name, friendlyName, containingType, defaultValue, relationship);
    }
  }
}