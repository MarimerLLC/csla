//-----------------------------------------------------------------------
// <copyright file="IPropertyInfoFactory.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Defines the interface for a factory object</summary>
//-----------------------------------------------------------------------

namespace Csla.Core
{
  /// <summary>
  /// Defines the interface for a factory object
  /// that creates IPropertyInfo objects.
  /// </summary>
  public interface IPropertyInfoFactory
  {
    /// <summary>
    /// Creates a new instance of PropertyInfo.
    /// </summary>
    /// <param name="containingType">
    /// Type of business class that contains the property
    /// declaration.
    /// </param>
    /// <param name="name">Name of the property.</param>
    /// <exception cref="ArgumentNullException"><paramref name="containingType"/> or <paramref name="name"/> is <see langword="null"/>.</exception>
    PropertyInfo<T> Create<T>(Type containingType, string name);
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
    /// <exception cref="ArgumentNullException"><paramref name="containingType"/> or <paramref name="name"/> is <see langword="null"/>.</exception>
    PropertyInfo<T> Create<T>(Type containingType, string name, string? friendlyName);
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
    /// <exception cref="ArgumentNullException"><paramref name="containingType"/> or <paramref name="name"/> is <see langword="null"/>.</exception>
    PropertyInfo<T> Create<T>(Type containingType, string name, string? friendlyName, RelationshipTypes relationship);
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
    /// <exception cref="ArgumentNullException"><paramref name="containingType"/> or <paramref name="name"/> is <see langword="null"/>.</exception>
    PropertyInfo<T> Create<T>(Type containingType, string name, string? friendlyName, T? defaultValue);
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
    /// <exception cref="ArgumentNullException"><paramref name="containingType"/> or <paramref name="name"/> is <see langword="null"/>.</exception>
    PropertyInfo<T> Create<T>(Type containingType, string name, string? friendlyName, T? defaultValue, RelationshipTypes relationship);
    /// <summary>
    /// Creates a new instance of PropertyInfo.
    /// </summary>
    /// <param name="containingType">
    /// Type of business class that contains the property
    /// declaration.
    /// </param>
    /// <param name="name">Name of the property.</param>
    /// <param name="isSerializable">If property is serializable</param>
    /// <exception cref="ArgumentNullException"><paramref name="containingType"/> or <paramref name="name"/> is <see langword="null"/>.</exception>
    PropertyInfo<T> Create<T>(Type containingType, string name, bool isSerializable);
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
    /// <param name="isSerializable">If property is serializable</param>
    /// <exception cref="ArgumentNullException"><paramref name="containingType"/> or <paramref name="name"/> is <see langword="null"/>.</exception>
    PropertyInfo<T> Create<T>(Type containingType, string name, string? friendlyName, bool isSerializable);
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
    /// <param name="isSerializable">If property is serializable</param>
    /// <exception cref="ArgumentNullException"><paramref name="containingType"/> or <paramref name="name"/> is <see langword="null"/>.</exception>
    PropertyInfo<T> Create<T>(Type containingType, string name, string? friendlyName, RelationshipTypes relationship, bool isSerializable);
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
    /// <param name="isSerializable">If property is serializable</param>
    /// <exception cref="ArgumentNullException"><paramref name="containingType"/> or <paramref name="name"/> is <see langword="null"/>.</exception>
    PropertyInfo<T> Create<T>(Type containingType, string name, string? friendlyName, T? defaultValue, bool isSerializable);
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
    /// <param name="isSerializable">If property is serializable</param>
    /// <exception cref="ArgumentNullException"><paramref name="containingType"/> or <paramref name="name"/> is <see langword="null"/>.</exception>
    PropertyInfo<T> Create<T>(Type containingType, string name, string? friendlyName, T? defaultValue, RelationshipTypes relationship, bool isSerializable);
  }
}