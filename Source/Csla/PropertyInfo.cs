//-----------------------------------------------------------------------
// <copyright file="PropertyInfo.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Maintains metadata about a property.</summary>
//-----------------------------------------------------------------------

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Csla
{
  /// <summary>
  /// Maintains metadata about a property.
  /// </summary>
  /// <typeparam name="T">
  /// Data type of the property.
  /// </typeparam>
  public class PropertyInfo<
#if NET8_0_OR_GREATER
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
#endif
    T> : Core.IPropertyInfo, IComparable
  {
    /// <summary>
    /// Creates a new instance of this class.
    /// </summary>
    /// <param name="name">Name of the property.</param>
    /// <param name="isSerializable">If property is serializable</param>
    public PropertyInfo(string name, bool? isSerializable = null)
      : this(name, null, null, DataBindingFriendlyDefault(), RelationshipTypes.None, isSerializable)
    { }

    /// <summary>
    /// Creates a new instance of this class.
    /// </summary>
    /// <param name="name">Name of the property.</param>
    /// <param name="relationship">Relationship with referenced object.</param>
    /// <param name="isSerializable">If property is serializable</param>
    public PropertyInfo(string name, RelationshipTypes relationship, bool? isSerializable = null)
      : this(name, null, null, DataBindingFriendlyDefault(), relationship, isSerializable)
    { }

    /// <summary>
    /// Creates a new instance of this class.
    /// </summary>
    /// <param name="name">Name of the property.</param>
    /// <param name="defaultValue">
    /// Default value for the property.
    /// </param>
    /// <param name="isSerializable">If property is serializable</param>
    public PropertyInfo(string name, T defaultValue, bool? isSerializable = null)
      : this(name, null, null, defaultValue, RelationshipTypes.None, isSerializable)
    { }

    /// <summary>
    /// Creates a new instance of this class.
    /// </summary>
    /// <param name="name">Name of the property.</param>
    /// <param name="friendlyName">
    /// Friendly display name for the property.
    /// </param>
    /// <param name="isSerializable">If property is serializable</param>
    public PropertyInfo(string name, string friendlyName, bool? isSerializable = null)
        : this(name, friendlyName, null, DataBindingFriendlyDefault(), RelationshipTypes.None, isSerializable)
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
    /// <param name="isSerializable">If property is serializable</param>
    public PropertyInfo(string name, string friendlyName, Type containingType, bool? isSerializable = null)
        : this(name, friendlyName, containingType, DataBindingFriendlyDefault(), RelationshipTypes.None, isSerializable)
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
    /// <param name="isSerializable">If property is serializable</param>
    public PropertyInfo(string name, string friendlyName, Type containingType, T defaultValue, bool? isSerializable = null)
        : this(name, friendlyName, containingType, defaultValue, RelationshipTypes.None, isSerializable)
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
    /// <param name="isSerializable">If property is serializable</param>

    public PropertyInfo(string name, string friendlyName, Type containingType, RelationshipTypes relationship, bool? isSerializable = null)
      : this(name, friendlyName, containingType, DataBindingFriendlyDefault(), relationship, isSerializable)
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
    /// <param name="isSerializable">If property is serializable</param>
    public PropertyInfo(string name, string friendlyName, Type containingType, T defaultValue, RelationshipTypes relationship, bool? isSerializable)
    {
      Name = name;
      _friendlyName = friendlyName;
      RelationshipType = relationship;
      if (containingType != null)
        _propertyInfo = containingType.GetProperty(Name);

      DefaultValue = defaultValue;
      _isSerializable = isSerializable;
    }

    /// <summary>
    /// Gets the property name value.
    /// </summary>
    public string Name { get; }

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

    private bool? _isSerializable;
    /// <summary>
    /// Gets or sets a value indicating whether this property is serializable.
    /// </summary>
    /// <remarks>
    /// If the property is marked with the <see cref="NonSerializedPropertyAttribute"/>,
    /// it is considered not serializable. Otherwise, it is considered serializable.
    /// </remarks>
    public virtual bool IsSerializable
    {
      get
      {
        if (_isSerializable.HasValue)
        {
          return _isSerializable.Value;
        }
        else if (_propertyInfo != null)
        {
          var nonSerialized = _propertyInfo.GetCustomAttributes(typeof(NonSerializedPropertyAttribute), true).OfType<NonSerializedPropertyAttribute>().Any();
          _isSerializable = !nonSerialized;
          return !nonSerialized;
        }
        return true;
      }
    }

    /// <summary>
    /// Gets the default initial value for the property.
    /// </summary>
    /// <remarks>
    /// This value is used to initialize the property's
    /// value, and is returned from a property get
    /// if the user is not authorized to 
    /// read the property.
    /// </remarks>
    public virtual T DefaultValue { get; }

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
      return new Core.FieldManager.FieldData<T>(name, IsSerializable);
    }


    // Default value is ManagedField

    /// <summary>
    /// Gets the relationship between the declaring object
    /// and the object reference in the property.
    /// </summary>
    public RelationshipTypes RelationshipType { get; } = RelationshipTypes.None;

    /// <summary>
    /// Gets or sets the index position for the managed
    /// field storage behind the property. FOR
    /// INTERNAL CSLA .NET USE ONLY.
    /// </summary>
    int Core.IPropertyInfo.Index { get; set; } = -1;

    /// <summary>
    /// Gets a value indicating whether this property
    /// references a child in the object graph.
    /// </summary>
    public bool IsChild { get => typeof(Core.IBusinessObject).IsAssignableFrom(Type); }

    /// <summary>
    /// Gets the System.Reflection.PropertyInfo object
    /// representing the property.
    /// </summary>
    public System.Reflection.PropertyInfo GetPropertyInfo() => _propertyInfo;

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