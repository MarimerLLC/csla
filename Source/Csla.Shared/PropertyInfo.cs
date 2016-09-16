﻿//-----------------------------------------------------------------------
// <copyright file="PropertyInfo.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Maintains metadata about a property.</summary>
//-----------------------------------------------------------------------
using System;

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
    /// Private default constructor.
    /// </summary>
    private PropertyInfo()
    {
    }

    /// <summary>
    /// Creates a new instance of this class.
    /// </summary>
    /// <param name="name">Name of the property.</param>
    public PropertyInfo(string name) : this(name, string.Empty)
    {
    }

    /// <summary>
    /// Creates a new instance of this class.
    /// </summary>
    /// <param name="name">Name of the property.</param>
    /// <param name="relationship">Relationship with referenced object.</param>
    public PropertyInfo(string name, RelationshipTypes relationship) : this(name, string.Empty)
    {
      _relationshipType = relationship;
    }

    /// <summary>
    /// Creates a new instance of this class.
    /// </summary>
    /// <param name="name">Name of the property.</param>
    /// <param name="defaultValue">
    /// Default value for the property.
    /// </param>
    public PropertyInfo(string name, T defaultValue) : this(name, string.Empty, defaultValue)
    {
    }

    /// <summary>
    /// Creates a new instance of this class.
    /// </summary>
    /// <param name="name">Name of the property.</param>
    /// <param name="friendlyName">
    /// Friendly display name for the property.
    /// </param>
    public PropertyInfo(string name, string friendlyName)
    {
      _name = name;
      _friendlyName = friendlyName;

      // We treat the default behavior of a string as string.empty for databinding purposes.
      if (typeof(T).Equals(typeof(string)))
        _defaultValue = (T)((object)string.Empty);
      else
        _defaultValue = default(T);
    }

    /// <summary>
    /// Creates a new instance of this class.
    /// </summary>
    /// <param name="name">Name of the property.</param>
    /// <param name="friendlyName">
    /// Friendly display name for the property.
    /// </param>
    /// <param name="defaultValue">
    /// Default value for the property.
    /// </param>
    public PropertyInfo(string name, string friendlyName, T defaultValue)
    {
      _name = name;
      _defaultValue = defaultValue;
      _friendlyName = friendlyName;
    }

    /// <summary>
    /// Creates a new instance of this class.
    /// </summary>
    /// <param name="name">Name of the property.</param>
    /// <param name="friendlyName">
    /// Friendly display name for the property.
    /// </param>
    /// <param name="relationship">Relationship with referenced object.</param>
    public PropertyInfo(string name, string friendlyName, RelationshipTypes relationship) : this(name, friendlyName)
    {
      _relationshipType = relationship;
    }

    /// <summary>
    /// Creates a new instance of this class.
    /// </summary>
    /// <param name="name">Name of the property.</param>
    /// <param name="friendlyName">
    /// Friendly display name for the property.
    /// </param>
    /// <param name="defaultValue">
    /// Default value for the property.
    /// </param>
    /// <param name="relationship">Relationship with
    /// referenced object.</param>
    public PropertyInfo(string name, string friendlyName, T defaultValue, RelationshipTypes relationship) : this(name, friendlyName, defaultValue)
    {
      _relationshipType = relationship;
    }

    private string _name;
    /// <summary>
    /// Gets the property name value.
    /// </summary>
    public string Name
    {
      get
      {
        return _name;
      }
    }

    /// <summary>
    /// Gets the type of the property.
    /// </summary>
    public Type Type
    {
      get
      {
        return typeof(T);
      }
    }

    private string _friendlyName;
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
        return !string.IsNullOrWhiteSpace(_friendlyName) ? _friendlyName : _name;
      }
    }

    private T _defaultValue;
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
      get
      {
        return _defaultValue;
      }
    }

    object Core.IPropertyInfo.DefaultValue
    {
      get
      {
        return DefaultValue;
      }
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
      get
      {
        return _index;
      }
      set
      {
        _index = value;
      }
    }

    #region IComparable Members

    int IComparable.CompareTo(object obj)
    {
      return _name.CompareTo(((Core.IPropertyInfo)obj).Name);
    }

    #endregion
  }
}