//-----------------------------------------------------------------------
// <copyright file="PropertyInfoManager.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Indicates that the specified property belongs</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using Csla.Properties;
using System.Linq;
using Csla.Reflection;

namespace Csla.Core.FieldManager
{
  /// <summary>
  /// Compare to PropertyInfo x with y for sorting
  /// </summary>
  internal class PropertyComparer : Comparer<IPropertyInfo>
  {
    public override int Compare(IPropertyInfo x, IPropertyInfo y)
    {
      return StringComparer.InvariantCulture.Compare(x.Name, y.Name);
    }
  }

  /// <summary>
  /// Manages the PropertyInfo data for business object types.
  /// </summary>
  public static class PropertyInfoManager
  {
    private static object _cacheLock = new object();
    private static Dictionary<Type, PropertyInfoList> _propertyInfoCache;

    private static Dictionary<Type, PropertyInfoList> PropertyInfoCache
    {
      get
      {
        if (_propertyInfoCache == null)
        {
          lock (_cacheLock)
          {
            if (_propertyInfoCache == null)
              _propertyInfoCache = new Dictionary<Type, PropertyInfoList>();
          }
        }
        return _propertyInfoCache;
      }
    }

    internal static PropertyInfoList GetPropertyListCache(Type objectType)
    {
      var cache = PropertyInfoCache;
      var found = cache.TryGetValue(objectType, out PropertyInfoList list);
      if (!found)
      {
        lock (cache)
        {
          found = cache.TryGetValue(objectType, out list);
          if (!found)
          {
            list = new PropertyInfoList();
            cache.Add(objectType, list);
            FieldDataManager.ForceStaticFieldInit(objectType);
          }
        }
      }
      return list;
    }

    /// <summary>
    /// Indicates that the specified property belongs
    /// to the type.
    /// </summary>
    /// <typeparam name="T">
    /// Type of property.
    /// </typeparam>
    /// <param name="objectType">
    /// Type of object to which the property belongs.
    /// </param>
    /// <param name="info">
    /// PropertyInfo object for the property.
    /// </param>
    /// <returns>
    /// The provided IPropertyInfo object.
    /// </returns>
    internal static PropertyInfo<T> RegisterProperty<T>(Type objectType, PropertyInfo<T> info)
    {
      var list = GetPropertyListCache(objectType);
      lock (list)
      {
        if (list.IsLocked)
          throw new InvalidOperationException(string.Format(Resources.PropertyRegisterNotAllowed, info.Name, objectType.Name));

        // This is the semantic code for RegisterProperty
        //if (list.Any(pi => pi.Name == info.Name))
        //  throw new InvalidOperationException(string.Format(Resources.PropertyRegisterDuplicateNotAllowed, info.Name));
        //list.Add(info);
        //list.Sort();

        // Optimized code
        // BinarySearch uses the same comparer as list.Sort() to find the item in a sorted list.
        // If not found then returns the negative index for item in sorted list (to insert). 
        // This allows us to insert the item right away with no need for explicit Sort on the list.
        var index = list.BinarySearch(info, new PropertyComparer());
        // if found then throw DuplicateNotAllowed
        if (index >= 0)
          throw new InvalidOperationException(string.Format(Resources.PropertyRegisterDuplicateNotAllowed, info.Name));

        // insert info at correct sorted index
        list.Insert(~index, info);
      }
      return info;
    }

    /// <summary>
    /// Returns a copy of the property list for
    /// a given business object type. Returns
    /// null if there are no properties registered
    /// for the type.
    /// </summary>
    /// <param name="objectType">
    /// The business object type.
    /// </param>
    /// <remarks>
    /// Registered property information is only available after at least one instance
    /// of the object type has been created within the current AppDomain.
    /// </remarks>
    public static PropertyInfoList GetRegisteredProperties(Type objectType)
    {
      // return a copy of the list to avoid
      // possible locking issues
      var list = GetPropertyListCache(objectType);
      lock (list)
        return new PropertyInfoList(list);
    }

    /// <summary>
    /// Returns a registered property for a given type and property name.
    /// </summary>
    /// <param name="objectType">The business object type.</param>
    /// <param name="propertyName">The name of the property.</param>
    /// <returns></returns>
    public static IPropertyInfo GetRegisteredProperty(Type objectType, string propertyName)
    {
      return GetRegisteredProperties(objectType).FirstOrDefault(p => p.Name == propertyName);
    }
  }
}