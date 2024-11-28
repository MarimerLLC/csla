//-----------------------------------------------------------------------
// <copyright file="DefaultPropertyInfoFactory.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Creates PropertyInfo objects.</summary>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace Csla.Core.FieldManager
{
  /// <summary>
  /// Creates PropertyInfo objects.
  /// </summary>
  internal class DefaultPropertyInfoFactory : IPropertyInfoFactory
  {
    /// <summary>
    /// Creates a new instance of PropertyInfo.
    /// </summary>
    /// <param name="containingType">
    /// Type of business class that contains the property
    /// declaration.
    /// </param>
    /// <param name="name">Name of the property.</param>
    public PropertyInfo<T> Create<
#if NET8_0_OR_GREATER
      [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
#endif
      T>(Type containingType, string name)
    {
      return new PropertyInfo<T>(name, null, containingType, PropertyInfo<T>.DataBindingFriendlyDefault(), RelationshipTypes.None, null);
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
    public PropertyInfo<T> Create<
#if NET8_0_OR_GREATER
      [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
#endif
      T>(Type containingType, string name, string friendlyName)
    {
      return new PropertyInfo<T>(name, friendlyName, containingType, PropertyInfo<T>.DataBindingFriendlyDefault(), RelationshipTypes.None, null);
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
    public PropertyInfo<T> Create<
#if NET8_0_OR_GREATER
      [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
#endif
      T>(Type containingType, string name, string friendlyName, RelationshipTypes relationship)
    {
      return new PropertyInfo<T>(name, friendlyName, containingType, PropertyInfo<T>.DataBindingFriendlyDefault(), relationship, null);
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
    public PropertyInfo<T> Create<
#if NET8_0_OR_GREATER
      [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
#endif
      T>(Type containingType, string name, string friendlyName, T defaultValue)
    {
      return new PropertyInfo<T>(name, friendlyName, containingType, defaultValue, RelationshipTypes.None, null);
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
    public PropertyInfo<T> Create<
#if NET8_0_OR_GREATER
      [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
#endif
      T>(Type containingType, string name, string friendlyName, T defaultValue, RelationshipTypes relationship)
    {
      return new PropertyInfo<T>(name, friendlyName, containingType, defaultValue, relationship, null);
    }

    /// <summary>
    /// Creates a new instance of PropertyInfo.
    /// </summary>
    /// <param name="containingType">
    /// Type of business class that contains the property
    /// declaration.
    /// </param>
    /// <param name="name">Name of the property.</param>
    /// <param name="isSerializable">If property is serializable</param>
    public PropertyInfo<T> Create<
#if NET8_0_OR_GREATER
      [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
#endif
      T>(Type containingType, string name, bool isSerializable)
    {
      return new PropertyInfo<T>(name, null, containingType, PropertyInfo<T>.DataBindingFriendlyDefault(), RelationshipTypes.None, isSerializable);
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
    /// <param name="isSerializable">If property is serializable</param>
    public PropertyInfo<T> Create<
#if NET8_0_OR_GREATER
      [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
#endif
      T>(Type containingType, string name, string friendlyName, bool isSerializable)
    {
      return new PropertyInfo<T>(name, friendlyName, containingType, PropertyInfo<T>.DataBindingFriendlyDefault(), RelationshipTypes.None, isSerializable);
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
    /// <param name="isSerializable">If property is serializable</param>
    public PropertyInfo<T> Create<
#if NET8_0_OR_GREATER
      [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
#endif
      T>(Type containingType, string name, string friendlyName, RelationshipTypes relationship, bool isSerializable)
    {
      return new PropertyInfo<T>(name, friendlyName, containingType, PropertyInfo<T>.DataBindingFriendlyDefault(), relationship, isSerializable);
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
    /// <param name="isSerializable">If property is serializable</param>
    public PropertyInfo<T> Create<
#if NET8_0_OR_GREATER
      [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
#endif
      T>(Type containingType, string name, string friendlyName, T defaultValue, bool isSerializable)
    {
      return new PropertyInfo<T>(name, friendlyName, containingType, defaultValue, RelationshipTypes.None, isSerializable);
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
    /// <param name="isSerializable">If property is serializable</param>
    public PropertyInfo<T> Create<
#if NET8_0_OR_GREATER
      [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
#endif
      T>(Type containingType, string name, string friendlyName, T defaultValue, RelationshipTypes relationship, bool isSerializable)
    {
      return new PropertyInfo<T>(name, friendlyName, containingType, defaultValue, relationship, isSerializable);
    }
  }
}