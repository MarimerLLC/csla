//-----------------------------------------------------------------------
// <copyright file="ObjectFieldInfo.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Contains schema information for a single</summary>
//-----------------------------------------------------------------------

using System.ComponentModel;
using System.Web.UI.Design;

namespace Csla.Web.Design
{
  /// <summary>
  /// Contains schema information for a single
  /// object property.
  /// </summary>
  [Serializable]
  public class ObjectFieldInfo : IDataSourceFieldSchema
  {
    private bool _isNullable;

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="field">The PropertyInfo object describing the property.</param>
    /// <exception cref="ArgumentNullException"><paramref name="field"/> is <see langword="null"/>.</exception>
    public ObjectFieldInfo(PropertyDescriptor field)
    {
      if (field is null)
        throw new ArgumentNullException(nameof(field));

      DataObjectFieldAttribute attribute = (DataObjectFieldAttribute)field.Attributes[typeof(DataObjectFieldAttribute)];
      if (attribute != null)
      {
        IsUnique = attribute.PrimaryKey;
        Identity = attribute.IsIdentity;
        _isNullable = attribute.IsNullable;
        Length = attribute.Length;
      }
      DataType = Utilities.GetPropertyType(field.PropertyType);
      IsReadOnly = field.IsReadOnly;
      Name = field.Name;

      // nullable
      Type t = field.PropertyType;
      if (!t.IsValueType || _isNullable)
        Nullable = true;
      else
      {
        if (t.IsGenericType)
          Nullable = (t.GetGenericTypeDefinition() == typeof(Nullable<>));
        else
          Nullable = false;
      }
    }

    /// <summary>
    /// Gets the data type of the property.
    /// </summary>
    public Type DataType { get; }

    /// <summary>
    /// Gets a value indicating whether this property
    /// is an identity key for the object.
    /// </summary>
    /// <remarks>
    /// Returns the optional value provided through
    /// the <see cref="DataObjectFieldAttribute">DataObjectField</see>
    /// attribute on the property.
    /// </remarks>
    public bool Identity { get; }

    /// <summary>
    /// Gets a value indicating whether this property
    /// is readonly.
    /// </summary>
    public bool IsReadOnly { get; }

    /// <summary>
    /// Gets a value indicating whether this property
    /// must contain a unique value.
    /// </summary>
    /// <returns>
    /// Always returns True if the property
    /// is marked as a primary key, otherwise
    /// returns False.
    /// </returns>
    public bool IsUnique { get; }

    /// <summary>
    /// Gets the length of the property value.
    /// </summary>
    /// <remarks>
    /// Returns the optional value provided through
    /// the <see cref="DataObjectFieldAttribute">DataObjectField</see>
    /// attribute on the property.
    /// </remarks>
    public int Length { get; }

    /// <summary>
    /// Gets the property name.
    /// </summary>
    public string Name { get; }

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
    public bool Nullable { get; }

    /// <summary>
    /// Gets the property's numeric precision.
    /// </summary>
    /// <returns>Always returns -1.</returns>
    public int Precision => -1;

    /// <summary>
    /// Gets a value indicating whether the property
    /// is a primary key value.
    /// </summary>
    /// <remarks>
    /// Returns the optional value provided through
    /// the <see cref="DataObjectFieldAttribute">DataObjectField</see>
    /// attribute on the property.
    /// </remarks>
    public bool PrimaryKey => IsUnique;

    /// <summary>
    /// Gets the property's scale.
    /// </summary>
    /// <returns>Always returns -1.</returns>
    public int Scale => -1;
  }
}
