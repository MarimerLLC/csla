using System;
using System.Collections.Generic;
using Csla.Properties;

namespace Csla.Core.FieldManager
{
  internal static class PropertyInfoManager
  {
    private static Dictionary<Type, PropertyInfoList> _propertyInfoCache;

    private static Dictionary<Type, PropertyInfoList> PropertyInfoCache
    {
      get
      {
        if (_propertyInfoCache == null)
        {
          lock (typeof(BusinessBase))
          {
            if (_propertyInfoCache == null)
              _propertyInfoCache = new Dictionary<Type, PropertyInfoList>();
          }
        }
        return _propertyInfoCache;
      }
    }

    public static PropertyInfoList GetPropertyListCache(Type objectType)
    {
      var cache = PropertyInfoCache;
      PropertyInfoList list = null;
      if (!(cache.TryGetValue(objectType, out list)))
      {
        lock (cache)
        {
          if (!(cache.TryGetValue(objectType, out list)))
          {
            list = new PropertyInfoList();
            cache.Add(objectType, list);
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
    public static PropertyInfo<T> RegisterProperty<T>(Type objectType, PropertyInfo<T> info)
    {
      var list = GetPropertyListCache(objectType);
      lock (list)
      {
        if (list.IsLocked)
          throw new InvalidOperationException(string.Format(Resources.PropertyRegisterNotAllowed, info.Name));
        list.Add(info);
        list.Sort();
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
    public static PropertyInfoList GetRegisteredProperties(Type objectType)
    {
      // return a copy of the list to avoid
      // possible locking issues
      return new PropertyInfoList(GetPropertyListCache(objectType));
    }
  }
}
