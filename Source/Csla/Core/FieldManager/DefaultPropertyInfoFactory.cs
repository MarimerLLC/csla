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
    /// <inheritdoc />
    public PropertyInfo<T> Create<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(Type containingType, string name)
    {
      return Create<T>(containingType, name, friendlyName: null);
    }

    /// <inheritdoc />
    public PropertyInfo<T> Create<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(Type containingType, string name, bool isSerializable)
    {
      return Create<T>(containingType, name, null, isSerializable);
    }

    /// <inheritdoc />
    public PropertyInfo<T> Create<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(Type containingType, string name, string? friendlyName, bool isSerializable)
    {
      return Create<T>(containingType, name, friendlyName, RelationshipTypes.None, isSerializable);
    }

    /// <inheritdoc />
    public PropertyInfo<T> Create<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(Type containingType, string name, string? friendlyName, RelationshipTypes relationship, bool isSerializable)
    {
      return Create<T>(containingType, name, friendlyName, PropertyInfo<T>.DataBindingFriendlyDefault(), relationship, isSerializable);
    }

    /// <inheritdoc />
    public PropertyInfo<T> Create<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(Type containingType, string name, string? friendlyName, T? defaultValue, bool isSerializable)
    {
      return Create<T>(containingType, name, friendlyName, defaultValue, RelationshipTypes.None, isSerializable);
    }

    /// <inheritdoc />
    public PropertyInfo<T> Create<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(Type containingType, string name, string? friendlyName)
    {
      return Create<T>(containingType, name, friendlyName, RelationshipTypes.None);
    }

    /// <inheritdoc />
    public PropertyInfo<T> Create<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(Type containingType, string name, string? friendlyName, RelationshipTypes relationship)
    {
      return Create<T>(containingType, name, friendlyName, PropertyInfo<T>.DataBindingFriendlyDefault(), relationship);
    }

    /// <inheritdoc />
    public PropertyInfo<T> Create<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(Type containingType, string name, string? friendlyName, T? defaultValue)
    {
      return Create<T>(containingType, name, friendlyName, defaultValue, RelationshipTypes.None);
    }

    /// <inheritdoc />
    public PropertyInfo<T> Create<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(Type containingType, string name, string? friendlyName, T? defaultValue, RelationshipTypes relationship)
    {
      return Create<T>(containingType, name, friendlyName, defaultValue, relationship, null);
    }

    /// <inheritdoc />
    public PropertyInfo<T> Create<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(Type containingType, string name, string? friendlyName, T? defaultValue, RelationshipTypes relationship, bool isSerializable)
    {
      return Create<T>(containingType, name, friendlyName, defaultValue, relationship, (bool?)isSerializable);
    }

    private PropertyInfo<T> Create<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(Type containingType, string name, string ?friendlyName, T? defaultValue, RelationshipTypes relationship, bool? isSerializable)
    {
      if (containingType is null)
        throw new ArgumentNullException(nameof(containingType));
      if (name is null)
        throw new ArgumentNullException(nameof(name));

      return new PropertyInfo<T>(name, friendlyName, containingType, defaultValue, relationship, isSerializable);
    }
  }
}