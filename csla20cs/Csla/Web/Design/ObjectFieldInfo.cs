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
      GetDataObjectAttributes();
    }

    private void GetDataObjectAttributes()
    {
      DataObjectFieldAttribute attribute =
        (DataObjectFieldAttribute)
        _field.Attributes[typeof(DataObjectFieldAttribute)];
      if (attribute != null)
      {
        _primaryKey = attribute.PrimaryKey;
        _isIdentity = attribute.IsIdentity;
        _isNullable = attribute.IsNullable;
        _length = attribute.Length;
      }
    }

    /// <summary>
    /// Gets the data type of the property.
    /// </summary>
    public Type DataType
    {
      get
      {
        return Utilities.GetPropertyType(
          _field.PropertyType);
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
          return (t.GetGenericTypeDefinition() == typeof(Nullable<>));
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
