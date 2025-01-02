//-----------------------------------------------------------------------
// <copyright file="DataMapper.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Map data from a source into a target object</summary>
//-----------------------------------------------------------------------

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Csla.Core;
using Csla.Properties;
using Csla.Reflection;

namespace Csla.Data
{
  /// <summary>
  /// Map data from a source into a target object
  /// by copying public property values.
  /// </summary>
  /// <remarks></remarks>
  public static class DataMapper
  {

    #region Map from IDictionary

    /// <summary>
    /// Copies values from the source into the
    /// properties of the target.
    /// </summary>
    /// <param name="source">A name/value dictionary containing the source values.</param>
    /// <param name="target">An object with properties to be set from the dictionary.</param>
    /// <remarks>
    /// The key names in the dictionary must match the property names on the target
    /// object. Target properties may not be readonly or indexed.
    /// </remarks>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="target"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"></exception>
    public static void Map(System.Collections.IDictionary source, object target)
    {
      Map(source, target, false);
    }

    /// <summary>
    /// Copies values from the source into the
    /// properties of the target.
    /// </summary>
    /// <param name="source">A name/value dictionary containing the source values.</param>
    /// <param name="target">An object with properties to be set from the dictionary.</param>
    /// <param name="ignoreList">A list of property names to ignore. 
    /// These properties will not be set on the target object.</param>
    /// <remarks>
    /// The key names in the dictionary must match the property names on the target
    /// object. Target properties may not be readonly or indexed.
    /// </remarks>
    /// <exception cref="ArgumentNullException"><paramref name="source"/>, <paramref name="target"/> or <paramref name="ignoreList"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"></exception>
    public static void Map(System.Collections.IDictionary source, object target, params string[] ignoreList)
    {
      Map(source, target, false, ignoreList);
    }

    /// <summary>
    /// Copies values from the source into the
    /// properties of the target.
    /// </summary>
    /// <param name="source">A name/value dictionary containing the source values.</param>
    /// <param name="target">An object with properties to be set from the dictionary.</param>
    /// <param name="ignoreList">A list of property names to ignore. 
    /// These properties will not be set on the target object.</param>
    /// <param name="suppressExceptions">If true, any exceptions will be supressed.</param>
    /// <remarks>
    /// The key names in the dictionary must match the property names on the target
    /// object. Target properties may not be readonly or indexed.
    /// </remarks>
    /// <exception cref="ArgumentNullException"><paramref name="source"/>, <paramref name="target"/> or <paramref name="ignoreList"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"></exception>
    public static void Map(System.Collections.IDictionary source, object target, bool suppressExceptions, params string[] ignoreList)
    {
      if (source is null)
        throw new ArgumentNullException(nameof(source));
      if (target is null)
        throw new ArgumentNullException(nameof(target));
      if (ignoreList is null)
        throw new ArgumentNullException(nameof(ignoreList));

      List<string> ignore = [.. ignoreList];
      foreach (string propertyName in source.Keys)
      {
        if (!ignore.Contains(propertyName))
        {
          try
          {
            SetPropertyValue(target, propertyName, source[propertyName]);
          }
          catch (Exception ex)
          {
            if (!suppressExceptions)
              throw new ArgumentException(
                String.Format("{0} ({1})",
                Resources.PropertyCopyFailed, propertyName), ex);
          }
        }
      }
    }

    #endregion

    #region Map to Dictionary

    /// <summary>
    /// Copies values from the source into the target.
    /// </summary>
    /// <param name="source">An object with properties to be loaded into the dictionary.</param>
    /// <param name="target">A name/value dictionary containing the source values.</param>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="target"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"></exception>
    public static void Map(object source, Dictionary<string, object?> target)
    {
      Map(source, target, false);
    }

    /// <summary>
    /// Copies values from the source into the target.
    /// </summary>
    /// <param name="source">An object with properties to be loaded into the dictionary.</param>
    /// <param name="target">A name/value dictionary containing the source values.</param>
    /// <param name="ignoreList">A list of property names to ignore. 
    /// These properties will not be set on the target object.</param>
    /// <exception cref="ArgumentNullException"><paramref name="source"/>, <paramref name="target"/> or <paramref name="ignoreList"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"></exception>
    public static void Map(object source, Dictionary<string, object?> target, params string[] ignoreList)
    {
      Map(source, target, false, ignoreList);
    }

    /// <summary>
    /// Copies values from the source into the target.
    /// </summary>
    /// <param name="source">An object with properties to be loaded into the dictionary.</param>
    /// <param name="target">A name/value dictionary containing the source values.</param>
    /// <param name="ignoreList">A list of property names to ignore. 
    /// These properties will not be set on the target object.</param>
    /// <param name="suppressExceptions">If true, any exceptions will be supressed.</param>
    /// <exception cref="ArgumentNullException"><paramref name="source"/>, <paramref name="target"/> or <paramref name="ignoreList"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"></exception>
    public static void Map(object source, Dictionary<string, object?> target, bool suppressExceptions, params string[] ignoreList)
    {
      if (source is null)
        throw new ArgumentNullException(nameof(source));
      if (target is null)
        throw new ArgumentNullException(nameof(target));
      if (ignoreList is null)
        throw new ArgumentNullException(nameof(ignoreList));

      List<string> ignore = [.. ignoreList];
      foreach (var propertyName in GetPropertyNames(source.GetType()))
      {
        if (!ignore.Contains(propertyName))
        {
          try
          {
            target.Add(propertyName, MethodCaller.CallPropertyGetter(source, propertyName));
          }
          catch (Exception ex)
          {
            if (!suppressExceptions)
              throw new ArgumentException(
                String.Format("{0} ({1})",
                Resources.PropertyCopyFailed, propertyName), ex);
          }
        }
      }
    }

    #endregion

    #region Map from Object

    /// <summary>
    /// Copies values from the source into the
    /// properties of the target.
    /// </summary>
    /// <param name="source">An object containing the source values.</param>
    /// <param name="target">An object with properties to be set from the dictionary.</param>
    /// <remarks>
    /// The property names and types of the source object must match the property names and types
    /// on the target object. Source properties may not be indexed. 
    /// Target properties may not be readonly or indexed.
    /// </remarks>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="target"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"></exception>
    public static void Map(object source, object target)
    {
      Map(source, target, false);
    }

    /// <summary>
    /// Copies values from the source into the
    /// properties of the target.
    /// </summary>
    /// <param name="source">An object containing the source values.</param>
    /// <param name="target">An object with properties to be set from the dictionary.</param>
    /// <param name="ignoreList">A list of property names to ignore. 
    /// These properties will not be set on the target object.</param>
    /// <remarks>
    /// The property names and types of the source object must match the property names and types
    /// on the target object. Source properties may not be indexed. 
    /// Target properties may not be readonly or indexed.
    /// </remarks>
    /// <exception cref="ArgumentNullException"><paramref name="source"/>, <paramref name="target"/> or <paramref name="ignoreList"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"></exception>
    public static void Map(object source, object target, params string[] ignoreList)
    {
      Map(source, target, false, ignoreList);
    }

    /// <summary>
    /// Copies values from the source into the
    /// properties of the target.
    /// </summary>
    /// <param name="source">An object containing the source values.</param>
    /// <param name="target">An object with properties to be set from the dictionary.</param>
    /// <param name="ignoreList">A list of property names to ignore. 
    /// These properties will not be set on the target object.</param>
    /// <param name="suppressExceptions">If true, any exceptions will be supressed.</param>
    /// <remarks>
    /// <para>
    /// The property names and types of the source object must match the property names and types
    /// on the target object. Source properties may not be indexed. 
    /// Target properties may not be readonly or indexed.
    /// </para><para>
    /// Properties to copy are determined based on the source object. Any properties
    /// on the source object marked with the <see cref="BrowsableAttribute"/> equal
    /// to false are ignored.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException"><paramref name="source"/>, <paramref name="target"/> or <paramref name="ignoreList"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"></exception>
    public static void Map(object source, object target, bool suppressExceptions, params string[] ignoreList)
    {
      if (source is null)
        throw new ArgumentNullException(nameof(source));
      if (target is null)
        throw new ArgumentNullException(nameof(target));
      if (ignoreList is null)
        throw new ArgumentNullException(nameof(ignoreList));

      List<string> ignore = [.. ignoreList];
      foreach (var propertyName in GetPropertyNames(source.GetType()))
      {
        if (!ignore.Contains(propertyName))
        {
          try
          {
            object? value = MethodCaller.CallPropertyGetter(source, propertyName);
            SetPropertyValue(target, propertyName, value);
          }
          catch (Exception ex)
          {
            if (!suppressExceptions)
              throw new ArgumentException(
                  String.Format("{0} ({1})",
                                Resources.PropertyCopyFailed, propertyName), ex);
          }
        }
      }
    }

    /// <summary>
    /// Copies values from the source into the
    /// properties of the target.
    /// </summary>
    /// <param name="source">An object containing the source values.</param>
    /// <param name="target">An object with properties to be set from the dictionary.</param>
    /// <param name="map">A DataMap object containing the mappings to use during the copy process.</param>
    /// <remarks>
    /// The property names and types of the source object must match the property names and types
    /// on the target object. Source properties may not be indexed. 
    /// Target properties may not be readonly or indexed.
    /// </remarks>
    /// <exception cref="ArgumentNullException"><paramref name="source"/>, <paramref name="target"/> or <paramref name="map"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"></exception>
    public static void Map(object source, object target, DataMap map)
    {
      Map(source, target, map, false);
    }

    /// <summary>
    /// Copies values from the source into the
    /// properties of the target.
    /// </summary>
    /// <param name="source">An object containing the source values.</param>
    /// <param name="target">An object with properties to be set from the dictionary.</param>
    /// <param name="suppressExceptions">If true, any exceptions will be supressed.</param>
    /// <param name="map">A DataMap object containing the mappings to use during the copy process.</param>
    /// <remarks>
    /// The property names and types of the source object must match the property names and types
    /// on the target object. Source properties may not be indexed. 
    /// Target properties may not be readonly or indexed.
    /// </remarks>
    /// <exception cref="ArgumentNullException"><paramref name="source"/>, <paramref name="target"/> or <paramref name="map"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"></exception>
    public static void Map(object source, object target, DataMap map, bool suppressExceptions)
    {
      if (source is null)
        throw new ArgumentNullException(nameof(source));
      if (target is null)
        throw new ArgumentNullException(nameof(target));
      if (map is null)
        throw new ArgumentNullException(nameof(map));

      foreach (DataMap.MemberMapping mapping in map.GetMap())
      {
        try
        {
          object? value = mapping.FromMemberHandle.DynamicMemberGet(source);
          SetValueWithCoercion(target, mapping.ToMemberHandle, value);
        }
        catch (Exception ex)
        {
          if (!suppressExceptions)
            throw new ArgumentException(
              String.Format("{0} ({1})",
              Resources.PropertyCopyFailed, mapping.FromMemberHandle.MemberName), ex);
        }
      }
    }

    private static IList<string> GetPropertyNames([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type sourceType)
    {
      List<string> result = [];
      PropertyDescriptorCollection props = TypeDescriptor.GetProperties(sourceType);
      foreach (PropertyDescriptor item in props)
        if (item.IsBrowsable)
          result.Add(item.Name);
      return result;
    }

    #endregion

    #region  Load from IDictionary

    /// <summary>
    /// Copies values from the source into the
    /// target.
    /// </summary>
    /// <param name="source">
    /// Dictionary containing the source values.
    /// </param>
    /// <param name="target">
    /// Business object with managed fields that
    /// will contain the copied values.
    /// </param>
    /// <param name="nameMapper">
    /// A function that translates the target
    /// property names into key values for the
    /// source dictionary.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="source"/>, <paramref name="target"/> or <paramref name="nameMapper"/> is <see langword="null"/>.</exception>
    /// <exception cref="NotSupportedException"><paramref name="target"/> is not based on a csla business object type.</exception>
    public static void Load(System.Collections.IDictionary source, object target, Func<string, object> nameMapper)
    {
      if (source is null)
        throw new ArgumentNullException(nameof(source));
      if (target is null)
        throw new ArgumentNullException(nameof(target));
      if (nameMapper is null)
        throw new ArgumentNullException(nameof(nameMapper));

      if (target is not IManageProperties validTarget)
        throw new NotSupportedException();
      var propertyList = validTarget.GetManagedProperties();
      foreach (var p in propertyList)
        validTarget.LoadProperty(p, source[nameMapper(p.Name)]);
    }

    #endregion

    #region  Load to IDictionary

    /// <summary>
    /// Copies values from the source into the
    /// target.
    /// </summary>
    /// <param name="source">
    /// Business object with managed fields that
    /// contain the source values.
    /// </param>
    /// <param name="target">
    /// Dictionary that will contain the resulting values.
    /// </param>
    /// <param name="nameMapper">
    /// A function that translates the source
    /// property names into key values for the
    /// target dictionary.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="source"/>, <paramref name="target"/> or <paramref name="nameMapper"/> is <see langword="null"/>.</exception>
    public static void Load(object source, System.Collections.IDictionary target, Func<string, object> nameMapper)
    {
      if (source is null)
        throw new ArgumentNullException(nameof(source));
      if (target is null)
        throw new ArgumentNullException(nameof(target));
      if (nameMapper is null)
        throw new ArgumentNullException(nameof(nameMapper));

      if (source is not IManageProperties validSource)
        throw new NotSupportedException();
      var propertyList = validSource.GetManagedProperties();
      foreach (var p in propertyList)
        target[nameMapper(p.Name)] = validSource.ReadProperty(p);
    }

    #endregion

    #region SetValue

    /// <summary>
    /// Sets an object's property with the specified value,
    /// coercing that value to the appropriate type if possible.
    /// </summary>
    /// <param name="target">Object containing the property to set.</param>
    /// <param name="propertyName">Name of the property to set.</param>
    /// <param name="value">Value to set into the property.</param>
    /// <exception cref="ArgumentNullException"><paramref name="target"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="propertyName"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    public static void SetPropertyValue(object target, string propertyName, object? value)
    {
      if (target is null)
        throw new ArgumentNullException(nameof(target));
      if (string.IsNullOrWhiteSpace(propertyName))
        throw new ArgumentException(string.Format(Resources.StringNotNullOrWhiteSpaceException, nameof(propertyName)), nameof(propertyName));

      DynamicMemberHandle handle = MethodCaller.GetCachedProperty(target.GetType(), propertyName);
      SetValueWithCoercion(target, handle, value);
    }

    private static void SetValueWithCoercion(object target, DynamicMemberHandle handle, object? value)
    {
      var oldValue = handle.DynamicMemberGet(target);

      Type pType = handle.MemberType;

      var isGeneric = pType.IsGenericType;
      var isPrimitive = pType.IsPrimitive;
      var isValueType = pType.IsValueType;
      if (!isGeneric
          || (isGeneric && pType.GetGenericTypeDefinition() != typeof(Nullable<>)))
      {
        if (isValueType && (isPrimitive || pType == typeof(decimal)) && value == null)
        {
          value = 0;
        }
      }

      if (value != null)
      {
        Type vType = Utilities.GetPropertyType(value.GetType());
        value = Utilities.CoerceValue(pType, vType, oldValue, value);
      }
      handle.DynamicMemberSet(target, value);
    }

    /// <summary>
    /// Sets an object's field with the specified value,
    /// coercing that value to the appropriate type if possible.
    /// </summary>
    /// <param name="target">Object containing the field to set.</param>
    /// <param name="fieldName">Name of the field (public or non-public) to set.</param>
    /// <param name="value">Value to set into the field.</param>
    /// <exception cref="ArgumentNullException"><paramref name="target"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="fieldName"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    public static void SetFieldValue(object target, string fieldName, object? value)
    {
      if (target is null)
        throw new ArgumentNullException(nameof(target));
      if (string.IsNullOrWhiteSpace(fieldName))
        throw new ArgumentException(string.Format(Resources.StringNotNullOrWhiteSpaceException, nameof(fieldName)), nameof(fieldName));

      DynamicMemberHandle handle = MethodCaller.GetCachedField(target.GetType(), fieldName);
      SetValueWithCoercion(target, handle, value);
    }

    /// <summary>
    /// Gets an object's field value.
    /// </summary>
    /// <param name="target">Object whose field value to get.</param>
    /// <param name="fieldName">The name of the field.</param>
    /// <returns>The value of the field.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="target"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="fieldName"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    public static object? GetFieldValue(object target, string fieldName)
    {
      if (target is null)
        throw new ArgumentNullException(nameof(target));
      if (string.IsNullOrWhiteSpace(fieldName))
        throw new ArgumentException(string.Format(Resources.StringNotNullOrWhiteSpaceException, nameof(fieldName)), nameof(fieldName));

      DynamicMemberHandle handle = MethodCaller.GetCachedField(target.GetType(), fieldName);
      return handle.DynamicMemberGet.Invoke(target);
    }

    #endregion
  }
}
