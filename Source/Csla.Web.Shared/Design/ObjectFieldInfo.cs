//-----------------------------------------------------------------------
// <copyright file="ObjectFieldInfo.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Contains schema information for a single</summary>
//-----------------------------------------------------------------------
using System;
using System.Web.UI.Design;
using System.ComponentModel;

namespace Csla.Web.Design
{
  /// <summary>
  /// Contains schema information for a single
  /// object property.
  /// </summary>
  [Serializable]
  public class ObjectFieldInfo : IDataSourceFieldSchema
  {
    private Type _dataType;
    private bool _primaryKey;
    private bool _isIdentity;
    private bool _isNullable;
    private int _length;
    private bool _isReadOnly;
    private string _name;
    private bool _nullable;

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="field">The PropertyInfo object
    /// describing the property.</param>
    public ObjectFieldInfo(PropertyDescriptor field)
    {
      DataObjectFieldAttribute attribute =
        (DataObjectFieldAttribute)
        field.Attributes[typeof(DataObjectFieldAttribute)];
      if (attribute != null)
      {
        _primaryKey = attribute.PrimaryKey;
        _isIdentity = attribute.IsIdentity;
        _isNullable = attribute.IsNullable;
        _length = attribute.Length;
      }
      _dataType = Utilities.GetPropertyType(
          field.PropertyType);
      _isReadOnly = field.IsReadOnly;
      _name = field.Name;

      // nullable
      Type t = field.PropertyType;
      if (!t.IsValueType || _isNullable)
        _nullable = true;
      else
      {
        if (t.IsGenericType)
          _nullable = (t.GetGenericTypeDefinition() == typeof(Nullable<>));
        else
          _nullable = false;
      }
    }

    /// <summary>
    /// Gets the data type of the property.
    /// </summary>
    public Type DataType
    {
      get
      {
        return _dataType;
      }
    }

    /// <summary>
    /// Gets a value indicating whether this property
    /// is an identity key for the object.
    /// </summary>
    /// <remarks>
    /// Returns the optional value provided through
    /// the <see cref="DataObjectFieldAttribute">DataObjectField</see>
    /// attribute on the property.
    /// </remarks>
    public bool Identity
    {
      get { return _isIdentity; }
    }

    /// <summary>
    /// Gets a value indicating whether this property
    /// is readonly.
    /// </summary>
    public bool IsReadOnly
    {
      get { return _isReadOnly; }
    }

    /// <summary>
    /// Gets a value indicating whether this property
    /// must contain a unique value.
    /// </summary>
    /// <returns>
    /// Always returns True if the property
    /// is marked as a primary key, otherwise
    /// returns False.
    /// </returns>
    public bool IsUnique
    {
      get { return _primaryKey; }
    }

    /// <summary>
    /// Gets the length of the property value.
    /// </summary>
    /// <remarks>
    /// Returns the optional value provided through
    /// the <see cref="DataObjectFieldAttribute">DataObjectField</see>
    /// attribute on the property.
    /// </remarks>
    public int Length
    {
      get { return _length; }
    }

    /// <summary>
    /// Gets the property name.
    /// </summary>
    public string Name
    {
      get { return _name; }
    }

    /// <summary>
    /// Gets a value indicating whether the property
    /// is nullable
    /// </summary>
    /// <remarks>
    /// Returns True for reference types, and for
    /// value types wrapped in the Nullable generic.
    /// The result can also be set to True through
    /// the <see cref="DataObjectFieldAttribute">DataObjectField</see>
    /// attribute on the property.
    /// </remarks>
    public bool Nullable
    {
      get
      { return _nullable; }
    }

    /// <summary>
    /// Gets the property's numeric precision.
    /// </summary>
    /// <returns>Always returns -1.</returns>
    public int Precision
    {
      get { return -1; }
    }

    /// <summary>
    /// Gets a value indicating whether the property
    /// is a primary key value.
    /// </summary>
    /// <remarks>
    /// Returns the optional value provided through
    /// the <see cref="DataObjectFieldAttribute">DataObjectField</see>
    /// attribute on the property.
    /// </remarks>
    public bool PrimaryKey
    {
      get { return _primaryKey; }
    }

    /// <summary>
    /// Gets the property's scale.
    /// </summary>
    /// <returns>Always returns -1.</returns>
    public int Scale
    {
      get { return -1; }
    }
  }
}
