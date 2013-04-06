using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Core.FieldManager
{
  /// <summary>
  /// Creates PropertyInfo objects.
  /// </summary>
  internal class DefaultPropertyInfoFactory : Csla.Core.IPropertyInfoFactory
  {
    #region Csla.PropertyInfo<T>Factory Members

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
      return new Csla.PropertyInfo<T>(name);
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
      return new Csla.PropertyInfo<T>(name, friendlyName);
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
      return new Csla.PropertyInfo<T>(name, friendlyName, relationship);
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
      return new Csla.PropertyInfo<T>(name, friendlyName, defaultValue);
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
      return new Csla.PropertyInfo<T>(name, friendlyName, defaultValue, relationship);
    }

    #endregion
  }
}
