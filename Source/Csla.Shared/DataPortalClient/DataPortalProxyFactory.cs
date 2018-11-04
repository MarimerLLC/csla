//-----------------------------------------------------------------------
// <copyright file="DataPortalProxyFactory.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Creates the DataPortalProxy to use for DataPortal call on the objectType.</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Reflection;

namespace Csla.DataPortalClient
{
  /// <summary>
  /// Default data portal proxy factory that 
  /// creates the IDataPortalProxy instance 
  /// to use for the DataPortal server call.
  /// </summary>
  public class DataPortalProxyFactory : IDataPortalProxyFactory
  {
    private static Type _proxyType;

    /// <summary>
    /// Creates the DataPortalProxy to use for DataPortal call on the objectType.
    /// </summary>
    /// <param name="objectType">Root business object type</param>
    /// <returns></returns>
    public IDataPortalProxy Create(Type objectType)
    {
      if (DataPortalTypeProxyDescriptors != null)
      {
        if (DataPortalTypeProxyDescriptors.TryGetValue(GetTypeKey(objectType), out DataPortalProxyDescriptor descriptor))
        {
          var type = Type.GetType(descriptor.ProxyTypeName);
          return (IDataPortalProxy)Activator.CreateInstance(type, descriptor.DataPortalUrl);
        }
      }

      if (_proxyType == null)
      {
        string proxyTypeName = ApplicationContext.DataPortalProxy;
        if (proxyTypeName == "Local")
          _proxyType = typeof(LocalProxy);
        else
          _proxyType = Type.GetType(proxyTypeName, true, true);
      }
      return (IDataPortalProxy)MethodCaller.CreateInstance(_proxyType);
    }

    /// <summary>
    /// Resets the data portal proxy type, so the
    /// next data portal call will reload the proxy
    /// type based on current configuration values.
    /// </summary>
    public void ResetProxyType()
    {
      _proxyType = null;
    }

    /// <summary>
    /// Gets or sets the list of proxy-type mapping descriptors used to
    /// create specific proxy objects for specific business classes.
    /// </summary>
    /// <remarks>
    /// If a business type is not listed in this mapping then the default
    /// proxy and URL values from ApplicationContext are used to create a proxy.
    /// 
    /// The key value is the first two elements of an assembly qualified type name
    /// (e.g. 'System.String, mscorlib').
    /// 
    /// Or the key value can be a resource id used to describe a business class
    /// via the DataPortalServerResource attribute.
    /// (e.g. 'Resource: 123').
    /// </remarks>
    public static Dictionary<string, DataPortalProxyDescriptor> DataPortalTypeProxyDescriptors
    { get; set; }

    /// <summary>
    /// Returns a type name formatted to act as a key
    /// in the DataPortalTypeProxyDescriptors dictionary.
    /// </summary>
    /// <param name="objectType">Object type</param>
    public static string GetTypeName(Type objectType)
    {
      return $"{objectType.Name}, {objectType.Assembly.FullName.Substring(0, objectType.Assembly.FullName.IndexOf(","))}";
    }

    /// <summary>
    /// Returns the key used by the proxy factory to locate
    /// a DataPortalProxyDescriptor in the DataPortalProxyDescriptors
    /// dictionary
    /// </summary>
    /// <param name="objectType">Object type</param>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static string GetTypeKey(Type objectType)
    {
      var attributes = objectType.GetCustomAttributes(typeof(DataPortalServerResourceAttribute), true);
      if (attributes != null && attributes.Count() > 0)
        return $"Resource: {((DataPortalServerResourceAttribute)attributes[0]).ResourceId}";
      else
        return GetTypeName(objectType);
    }
  }
}