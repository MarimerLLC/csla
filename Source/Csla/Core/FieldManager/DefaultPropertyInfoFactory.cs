//-----------------------------------------------------------------------
// <copyright file="DefaultPropertyInfoFactory.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Creates PropertyInfo objects.</summary>
//-----------------------------------------------------------------------

namespace Csla.Core.FieldManager
{
  /// <summary>
  /// Creates PropertyInfo objects.
  /// </summary>
  internal class DefaultPropertyInfoFactory : IPropertyInfoFactory
  {
    /// <inheritdoc />
    public PropertyInfo<T> Create<T>(Type containingType, string name)
    {
      return Create<T>(containingType, name, friendlyName: null);
    }

    /// <inheritdoc />
    public PropertyInfo<T> Create<T>(Type containingType, string name, string? friendlyName)
    {
      return Create<T>(containingType, name, friendlyName, RelationshipTypes.None);
    }

    /// <inheritdoc />
    public PropertyInfo<T> Create<T>(Type containingType, string name, string? friendlyName, RelationshipTypes relationship)
    {
      return Create<T>(containingType, name, friendlyName, PropertyInfo<T>.DataBindingFriendlyDefault(), relationship);
    }

    /// <inheritdoc />
    public PropertyInfo<T> Create<T>(Type containingType, string name, string? friendlyName, T? defaultValue)
    {
      return Create<T>(containingType, name, friendlyName, defaultValue, RelationshipTypes.None);
    }

    /// <inheritdoc />
    public PropertyInfo<T> Create<T>(Type containingType, string name, string? friendlyName, T? defaultValue, RelationshipTypes relationship)
    {
      if (containingType is null)
        throw new ArgumentNullException(nameof(containingType));
      if (name is null)
        throw new ArgumentNullException(nameof(name));

      return new PropertyInfo<T>(name, friendlyName, containingType, defaultValue, relationship);
    }
  }
}