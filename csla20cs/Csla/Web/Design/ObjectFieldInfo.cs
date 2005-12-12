using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.Design;
using System.ComponentModel;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Csla.Web.Design
{
  /// <summary>
  /// Contains schema information for a single
  /// object property.
  /// </summary>
  public class ObjectFieldInfo : IDataSourceFieldSchema
  {

    private PropertyDescriptor _field;
    private bool _retrievedMetaData;
    private bool _primaryKey;
    private bool _isIdentity;
    private bool _isNullable;
    private int _length;

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="field">The PropertyInfo object
    /// describing the property.</param>
    public ObjectFieldInfo(PropertyDescriptor field)
    {
      _field = field;
    }

    private void EnsureMetaData()
    {
      if (!_retrievedMetaData)
      {
        DataObjectFieldAttribute attribute1 = (DataObjectFieldAttribute)_field.Attributes[typeof(DataObjectFieldAttribute)];
        if (attribute1 != null)
        {
          _primaryKey = attribute1.PrimaryKey;
          _isIdentity = attribute1.IsIdentity;
          _isNullable = attribute1.IsNullable;
          _length = attribute1.Length;
        }
        _retrievedMetaData = true;
      }
    }

    /// <summary>
    /// Gets the data type of the property.
    /// </summary>
    public Type DataType
    {
      get
      {
        Type type1 = _field.PropertyType;
        if (type1.IsGenericType && (type1.GetGenericTypeDefinition() == typeof(Nullable)))
          return type1.GetGenericArguments()[0];
        return type1;
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
      get { return !_field.IsReadOnly; }
    }

    /// <summary>
    /// Gets a value indicating whether this property
    /// must contain a unique value.
    /// </summary>
    /// <returns>Always returns False.</returns>
    public bool IsUnique
    {
      get { return false; }
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
      get { return _field.Name; }
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
      {
        Type t = _field.PropertyType;
        if (!t.IsValueType || _isNullable)
          return true;
        if (t.IsGenericType)
          return (t.GetGenericTypeDefinition() == typeof(Nullable));
        return false;
      }
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
