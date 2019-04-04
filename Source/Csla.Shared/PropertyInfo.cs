//-----------------------------------------------------------------------
// <copyright file="PropertyInfo.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Maintains metadata about a property.</summary>
//-----------------------------------------------------------------------
using System;
using System.Linq;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Csla
{
  /// <summary>
  /// Maintains metadata about a property.
  /// </summary>
  /// <typeparam name="T">
  /// Data type of the property.
  /// </typeparam>
  public class PropertyInfo<T> : Core.IPropertyInfo, IComparable
  {
    /// <summary>
    /// Creates a new instance of this class.
    /// </summary>
    /// <param name="name">Name of the property.</param>
    public PropertyInfo(string name)
      : this(name, null, null, DataBindingFriendlyDefault(), RelationshipTypes.None)
    { }

    /// <summary>
    /// Creates a new instance of this class.
    /// </summary>
    /// <param name="name">Name of the property.</param>
    /// <param name="relationship">Relationship with referenced object.</param>
    public PropertyInfo(string name, RelationshipTypes relationship)
      : this(name, null, null, DataBindingFriendlyDefault(), relationship)
    { }

    /// <summary>
    /// Creates a new instance of this class.
    /// </summary>
    /// <param name="name">Name of the property.</param>
    /// <param name="defaultValue">
    /// Default value for the property.
    /// </param>
    public PropertyInfo(string name, T defaultValue)
      : this(name, null, null, defaultValue, RelationshipTypes.None)
    { }

    /// <summary>
    /// Creates a new instance of this class.
    /// </summary>
    /// <param name="name">Name of the property.</param>
    /// <param name="friendlyName">
    /// Friendly display name for the property.
    /// </param>
    public PropertyInfo(string name, string friendlyName)
        : this(name, friendlyName, null, DataBindingFriendlyDefault(), RelationshipTypes.None)
    { }

    /// <summary>
    /// Creates a new instance of this class.
    /// </summary>
    /// <param name="name">Name of the property.</param>
    /// <param name="friendlyName">
    /// Friendly display name for the property.
    /// </param>
    /// <param name="containingType">
    /// Factory to provide display name from attributes.
    /// </param>
    public PropertyInfo(string name, string friendlyName, Type containingType)
        : this(name, friendlyName, containingType, DataBindingFriendlyDefault(), RelationshipTypes.None)
    { }

    /// <summary>
    /// Creates a new instance of this class.
    /// </summary>
    /// <param name="name">Name of the property.</param>
    /// <param name="friendlyName">
    /// Friendly display name for the property.
    /// </param>
    /// <param name="containingType">
    /// Factory to provide display name from attributes.
    /// </param>
    /// <param name="defaultValue">
    /// Default value for the property.
    /// </param>
    public PropertyInfo(string name, string friendlyName, Type containingType, T defaultValue)
        : this(name, friendlyName, containingType, defaultValue, RelationshipTypes.None)
    { }

    /// <summary>
    /// Creates a new instance of this class.
    /// </summary>
    /// <param name="name">Name of the property.</param>
    /// <param name="friendlyName">
    /// Friendly display name for the property.
    /// </param>
    /// <param name="containingType">
    /// Factory to provide display name from attributes.
    /// </param>
    /// <param name="relationship">Relationship with referenced object.</param>
    public PropertyInfo(string name, string friendlyName, Type containingType, RelationshipTypes relationship) 
      : this(name, friendlyName, null, DataBindingFriendlyDefault(), relationship)
    { }

    /// <summary>
    /// Creates a new instance of this class.
    /// </summary>
    /// <param name="name">Name of the property.</param>
    /// <param name="friendlyName">
    /// Friendly display name for the property.
    /// </param>
    /// <param name="containingType">
    /// Factory to provide display name from attributes.
    /// </param>
    /// <param name="defaultValue">
    /// Default value for the property.
    /// </param>
    /// <param name="relationship">Relationship with
    /// referenced object.</param>
    public PropertyInfo(string name, string friendlyName, Type containingType, T defaultValue, RelationshipTypes relationship)
    {
      Name = name;
      _friendlyName = friendlyName;
      _relationshipType = relationship;
      if (containingType != null)
        _propertyInfo = containingType.GetProperty(Name);

      _defaultValue = defaultValue;
    }

    /// <summary>
    /// Gets the property name value.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Gets the type of the property.
    /// </summary>
    public Type Type
    {
      get { return typeof(T); }
    }

    private readonly System.Reflection.PropertyInfo _propertyInfo;

    private readonly string _friendlyName;
    /// <summary>
    /// Gets the friendly display name
    /// for the property.
    /// </summary>
    /// <remarks>
    /// If no friendly name was provided, the
    /// property name itself is returned as a
    /// result.
    /// </remarks>
    public virtual string FriendlyName
    {
      get
      {
        string result = Name;
        if (!string.IsNullOrWhiteSpace(_friendlyName))
        {
          result = _friendlyName;
        }
        else if (_propertyInfo != null)
        {
          var display = _propertyInfo.GetCustomAttributes(typeof(DisplayAttribute), true).OfType<DisplayAttribute>().FirstOrDefault();
          if (display != null)
          {
            // DataAnnotations attribute.
            result = display.GetName();
          }
          else
          {
            // ComponentModel attribute.
            var displayName = _propertyInfo.GetCustomAttributes(typeof(DisplayNameAttribute), true).OfType<DisplayNameAttribute>().FirstOrDefault();
            if (displayName != null)
              result = displayName.DisplayName;
          }
        }
        return result;
      }
    }

    private readonly T _defaultValue;
    /// <summary>
    /// Gets the default initial value for the property.
    /// </summary>
    /// <remarks>
    /// This value is used to initialize the property's
    /// value, and is returned from a property get
    /// if the user is not authorized to 
    /// read the property.
    /// </remarks>
    public virtual T DefaultValue
    {
      get { return _defaultValue; }
    }

    object Core.IPropertyInfo.DefaultValue
    {
      get { return DefaultValue; }
    }

    Core.FieldManager.IFieldData Core.IPropertyInfo.NewFieldData(string name)
    {
      return NewFieldData(name);
    }

    /// <summary>
    /// Create and return a new IFieldData object
    /// to store an instance value for this
    /// property.
    /// </summary>
    /// <param name="name">
    /// Property name.
    /// </param>
    protected virtual Core.FieldManager.IFieldData NewFieldData(string name)
    {
      return new Core.FieldManager.FieldData<T>(name);
    }


    // Default value is ManagedField
    private RelationshipTypes _relationshipType = RelationshipTypes.None;

    /// <summary>
    /// Gets the relationship between the declaring object
    /// and the object reference in the property.
    /// </summary>
    public RelationshipTypes RelationshipType
    {
      get { return _relationshipType; }
    }

    private int _index = -1;

    /// <summary>
    /// Gets or sets the index position for the managed
    /// field storage behind the property. FOR
    /// INTERNAL CSLA .NET USE ONLY.
    /// </summary>
    int Core.IPropertyInfo.Index
    {
      get { return _index; }
      set { _index = value; }
    }

    #region IComparable Members

    int IComparable.CompareTo(object obj)
    {
      return Name.CompareTo(((Core.IPropertyInfo)obj).Name);
    }

    #endregion

    /// <summary>
    /// Creates the CSLA Data Binding Friendly default for the given type T.
    /// </summary>
    /// <returns>Default value for T which is compatible with Data Binding</returns>
    public static T DataBindingFriendlyDefault()
    {
      // if T is string we need an empty string, not null, for data binding
      if (typeof(T) == typeof(string))
        return (T)(object)string.Empty;

      return default(T);
    }
  }
}