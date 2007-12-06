using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Reflection;

namespace Csla.Reflection
{
  /// <summary>
  /// Maintains a cache of property and
  /// field info about types in the AppDomain.
  /// </summary>
  internal class TypeInfoCache
  {
    #region PropertyInfo

    private static Dictionary<Type, List<PropertyInfo>> _propertyInfoCache = 
      new Dictionary<Type, List<PropertyInfo>>();

    /// <summary>
    /// Gets a list of PropertyInfo objects for
    /// all properties on the specified type.
    /// </summary>
    /// <param name="type">Type of object for which to get properties.</param>
    /// <returns>
    /// Value is returned from the cache if possible, otherwise the
    /// information is loaded using reflection and placed in the
    /// cache for future use.
    /// </returns>
    /// <remarks>
    /// Only public instance properties are returned by this method, and
    /// the inheritance hierarchy is flattened.
    /// </remarks>
    public static List<PropertyInfo> GetPropertyInfo(Type type)
    {
      if (!_propertyInfoCache.ContainsKey(type))
        lock (_propertyInfoCache)
          if (!_propertyInfoCache.ContainsKey(type))
            _propertyInfoCache.Add(type, new List<PropertyInfo>(type.GetProperties(BindingFlags.Public |
                                                                                   BindingFlags.Instance |
                                                                                   BindingFlags.FlattenHierarchy)));
      return _propertyInfoCache[type];
    }

    #endregion

    #region BrowsablePropertyInfo

    private static Dictionary<Type, List<PropertyInfo>> _browsablePropertyInfoCache =
      new Dictionary<Type, List<PropertyInfo>>();

    /// <summary>
    /// Gets a list of Browsable PropertyInfo objects for
    /// all properties on the specified type.
    /// </summary>
    /// <param name="type">Type of object for which to get properties.</param>
    /// <returns>
    /// Value is returned from the cache if possible, otherwise the
    /// information is loaded using reflection and placed in the
    /// cache for future use.
    /// </returns>
    /// <remarks>
    /// The Browsable attribute indicates whether a property can
    /// be used by data binding. Browsable properties can be
    /// data bound, non-browsable properties can not. This method
    /// only returns a list of Browsable PropertyDescriptor objects
    /// for the specified type, so all items returned can
    /// be used by data binding.
    /// </remarks>
    public static List<PropertyInfo> GetBrowsablePropertyInfo(Type type)
    {
      if (!_browsablePropertyInfoCache.ContainsKey(type))
        lock (_browsablePropertyInfoCache)
          if (!_browsablePropertyInfoCache.ContainsKey(type))
          {
            List<PropertyInfo> tmp = new List<PropertyInfo>();
            PropertyDescriptorCollection list = TypeDescriptor.GetProperties(type);
            foreach (PropertyDescriptor item in list)
              if (item.IsBrowsable)
                tmp.Add(type.GetProperty(item.Name));
            _browsablePropertyInfoCache.Add(type, tmp);
          }
      return _browsablePropertyInfoCache[type];
    }

    #endregion

    #region PropertyDescriptor

    private static Dictionary<Type, PropertyDescriptorCollection> _propertyDescriptorCache =
      new Dictionary<Type, PropertyDescriptorCollection>();

    /// <summary>
    /// Gets a list of PropertyDescriptor objects for
    /// all properties on the specified type.
    /// </summary>
    /// <param name="type">Type of object for which to get properties.</param>
    /// <returns>
    /// Value is returned from the cache if possible, otherwise the
    /// information is loaded using reflection and placed in the
    /// cache for future use.
    /// </returns>
    public static PropertyDescriptorCollection GetPropertyDescriptors(Type type)
    {
      if (!_propertyDescriptorCache.ContainsKey(type))
        lock (_propertyDescriptorCache)
          if (!_propertyDescriptorCache.ContainsKey(type))
            _propertyDescriptorCache.Add(type, TypeDescriptor.GetProperties(type));
      return _propertyDescriptorCache[type];
    }

    #endregion

    #region BrowsablePropertyDescriptor

    private static Dictionary<Type, List<PropertyDescriptor>> _browsablePropertyDescriptorCache =
      new Dictionary<Type, List<PropertyDescriptor>>();

    /// <summary>
    /// Gets a list of Browsable PropertyDescriptor 
    /// objects for all properties on the specified type.
    /// </summary>
    /// <param name="type">Type of object for which to get properties.</param>
    /// <returns>
    /// Value is returned from the cache if possible, otherwise the
    /// information is loaded using reflection and placed in the
    /// cache for future use.
    /// </returns>
    /// <remarks>
    /// The Browsable attribute indicates whether a property can
    /// be used by data binding. Browsable properties can be
    /// data bound, non-browsable properties can not. This method
    /// only returns a list of Browsable PropertyDescriptor objects
    /// for the specified type, so all items returned can
    /// be used by data binding.
    /// </remarks>
    public static List<PropertyDescriptor> GetBrowsablePropertyDescriptors(Type type)
    {
      if (!_browsablePropertyDescriptorCache.ContainsKey(type))
        lock (_browsablePropertyDescriptorCache)
          if (!_browsablePropertyDescriptorCache.ContainsKey(type))
          {
            List<PropertyDescriptor> tmp = new List<PropertyDescriptor>();
            PropertyDescriptorCollection list = TypeDescriptor.GetProperties(type);
            foreach (PropertyDescriptor item in list)
              if (item.IsBrowsable)
                tmp.Add(item);
            _browsablePropertyDescriptorCache.Add(type, tmp);
          }
      return _browsablePropertyDescriptorCache[type];
    }

    #endregion

    #region FieldInfo

    private static Dictionary<Type, List<FieldInfo>> _fieldInfoCache =
      new Dictionary<Type, List<FieldInfo>>();

    /// <summary>
    /// Gets a list of FieldInfo objects for
    /// all instance fields (public and non-public) on 
    /// the specified type.
    /// </summary>
    /// <param name="type">Type of object for which to get fields.</param>
    /// <returns>
    /// Value is returned from the cache if possible, otherwise the
    /// information is loaded using reflection and placed in the
    /// cache for future use.
    /// </returns>
    public static List<FieldInfo> GetFieldInfo(Type type)
    {
      if (!_fieldInfoCache.ContainsKey(type))
        lock (_fieldInfoCache)
          if (!_fieldInfoCache.ContainsKey(type))
            _fieldInfoCache.Add(type, new List<FieldInfo>(type.GetFields(BindingFlags.NonPublic |
                                                                         BindingFlags.Instance |
                                                                         BindingFlags.Public)));
      return _fieldInfoCache[type];
    }

    #endregion
  }
}
