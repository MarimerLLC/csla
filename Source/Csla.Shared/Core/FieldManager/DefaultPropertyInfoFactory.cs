//-----------------------------------------------------------------------
// <copyright file="DefaultPropertyInfoFactory.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Creates PropertyInfo objects.</summary>
//-----------------------------------------------------------------------
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Csla;
using System.Reflection;
using Csla.Reflection;

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
      var friendlyName = GetFriendlyNameFromAttributes(containingType, name);
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
    public Csla.PropertyInfo<T> Create<T>(Type containingType, string name, string friendlyName)
    {
      if (string.IsNullOrWhiteSpace(friendlyName))
        friendlyName = GetFriendlyNameFromAttributes(containingType, name);
      
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
      if(string.IsNullOrWhiteSpace(friendlyName))
        friendlyName = GetFriendlyNameFromAttributes(containingType, name);

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
      if(string.IsNullOrWhiteSpace(friendlyName))
        friendlyName = GetFriendlyNameFromAttributes(containingType, name);

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
      if(string.IsNullOrWhiteSpace(friendlyName))
        friendlyName = GetFriendlyNameFromAttributes(containingType, name);

      return new Csla.PropertyInfo<T>(name, friendlyName, defaultValue, relationship);
    }

    #endregion

    #region Helpers

    /// <summary>
    /// Looks at the passed in type and trys to resolve the friendly name via DataAnnotations or the ComponentModel attribute.
    /// 
    /// Note: This code should not throw any exceptions because the property must exist!
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="name">The name of the property.</param>
    /// <returns>Returns the display name or the property name.</returns>
    private static string GetFriendlyNameFromAttributes(Type type, string name)
    {
      // If name is blank then check the DataAnnotations attribute and then the ComponentModel attribute.
      var propertyInfo = type.GetProperty(name);
      if (propertyInfo != null)
      {
        // DataAnnotations attribute.
        var display = propertyInfo.GetCustomAttributes(typeof(DisplayAttribute), true).OfType<DisplayAttribute>().FirstOrDefault();
        if (display != null)
          name = display.GetName();

#if !(ANDROID || IOS) && !NETFX_CORE
        else
        {
          // ComponentModel attribute.
          var displayName = propertyInfo.GetCustomAttributes(typeof(DisplayNameAttribute), true).OfType<DisplayNameAttribute>().FirstOrDefault();
          if (displayName != null)
            name = displayName.DisplayName;
        }
#endif
      }
      return name;
    }

    #endregion
  }
}