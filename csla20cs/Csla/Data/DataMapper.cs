using System;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel;
using Csla.Properties;

namespace Csla.Data
{
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
    /// <param name="suppressExceptions">If True, any exceptions will be supressed.</param>
    /// <remarks>
    /// The key names in the dictionary must match the property names on the target
    /// object. Target properties may not be readonly or indexed.
    /// </remarks>
    public static void Map(
      System.Collections.IDictionary source, 
      object target, bool suppressExceptions, 
      params string[] ignoreList)
    {
      List<string> ignore = new List<string>(ignoreList);
      Type targetType = target.GetType();
      foreach (string propertyName in source.Keys)
      {
        if (!ignore.Contains(propertyName))
        {
          try
          {
            SetValue(target, propertyName, source[propertyName]);
          }
          catch
          {
            if (!suppressExceptions)
              throw new ArgumentException(
                String.Format("{0} ({1})", 
                Resources.PropertyCopyFailed, propertyName));
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
    /// <param name="suppressExceptions">If True, any exceptions will be supressed.</param>
    /// <remarks>
    /// The property names and types of the source object must match the property names and types
    /// on the target object. Source properties may not be indexed. 
    /// Target properties may not be readonly or indexed.
    /// </remarks>
    public static void Map(object source, object target, bool suppressExceptions, params string[] ignoreList)
    {
      List<string> ignore = new List<string>(ignoreList);
      Type sourceType = source.GetType();
      PropertyInfo[] sourceProperties = GetSourceProperties(sourceType);
      Type targetType = target.GetType();
      foreach (PropertyInfo sourceProperty in sourceProperties)
      {
        string propertyName = sourceProperty.Name;
        if (!ignore.Contains(propertyName))
        {
          try
          {
            PropertyInfo propertyInfo;
            propertyInfo = sourceType.GetProperty(propertyName);
            SetValue(target, propertyName, propertyInfo.GetValue(source, null));
          }
          catch
          {
            if (!suppressExceptions)
              throw new ArgumentException(
                String.Format("{0} ({1})", Resources.PropertyCopyFailed, propertyName));
          }
        }
      }
    }

    static PropertyInfo[] GetSourceProperties(Type sourceType)
    {
      List<PropertyInfo> result = new List<PropertyInfo>();
      PropertyDescriptorCollection props =
        TypeDescriptor.GetProperties(sourceType);
      foreach (PropertyDescriptor item in props)
        if (item.IsBrowsable)
          result.Add(sourceType.GetProperty(item.Name));
      return result.ToArray();
    }

    #endregion

    static void SetValue(
      object target, string propertyName, object value)
    {
      PropertyInfo propertyInfo =
        target.GetType().GetProperty(propertyName);
      Type pType = propertyInfo.PropertyType;
      if (value == null)
        propertyInfo.SetValue(target, value, null);
      else
      {
        if (pType.Equals(value.GetType()))
        {
          // types match, just copy value
          propertyInfo.SetValue(target, value, null);
        }
        else
        {
          // types don't match, try to coerce
          if (pType.Equals(typeof(Guid)))
            propertyInfo.SetValue(
              target, new Guid(value.ToString()), null);
          else
            propertyInfo.SetValue(
              target, Convert.ChangeType(value, pType), null);
        }
      }
    }
  }
}
