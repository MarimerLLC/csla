//-----------------------------------------------------------------------
// <copyright file="DataPortalProxyFactory.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
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
          if (ApplicationContext.CurrentServiceProvider != null)
          {
            var httpClient = ApplicationContext.CurrentServiceProvider.GetService(typeof(System.Net.Http.HttpClient));
            if (httpClient == null)
              return (IDataPortalProxy)MethodCaller.CreateInstance(type, descriptor.DataPortalUrl);
            else
              return (IDataPortalProxy)MethodCaller.CreateInstance(_proxyType, httpClient, descriptor.DataPortalUrl);
          }
          else
          {
            return (IDataPortalProxy)MethodCaller.CreateInstance(type, descriptor.DataPortalUrl);
          }
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
      var provider = ApplicationContext.CurrentServiceProvider;
      if (provider == null)
        return (IDataPortalProxy)MethodCaller.CreateInstance(_proxyType);
      else
      {
        if (_proxyType.Equals(typeof(HttpProxy)))
        {
          var httpClient = ApplicationContext.CurrentServiceProvider.GetService(typeof(System.Net.Http.HttpClient));
          if (httpClient == null)
            return (IDataPortalProxy)MethodCaller.CreateInstance(_proxyType);
          else
            return (IDataPortalProxy)MethodCaller.CreateInstance(_proxyType, httpClient);
        }
        else
        {
          return (IDataPortalProxy)Microsoft.Extensions.DependencyInjection.ActivatorUtilities.CreateInstance(provider, _proxyType);
        }
      }
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
    /// Gets the list of proxy-type mapping descriptors used to
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
    { get; private set; }

    private static void InitializeDictionary()
    {
      if (DataPortalTypeProxyDescriptors == null)
        DataPortalTypeProxyDescriptors = new Dictionary<string, DataPortalProxyDescriptor>();
    }

    /// <summary>
    /// Add a proxy descriptor for the specified root business type.
    /// </summary>
    /// <param name="objectType">Root business type</param>
    /// <param name="descriptor">Data Portal proxy descriptor</param>
    public static void AddDescriptor(Type objectType, DataPortalProxyDescriptor descriptor)
    {
      InitializeDictionary();
      DataPortalTypeProxyDescriptors.Add(GetTypeName(objectType), descriptor);
    }

    /// <summary>
    /// Add a proxy descriptor for the specified root business type.
    /// </summary>
    /// <param name="typeName">Type of the root business type</param>
    /// <param name="descriptor">Data Portal proxy descriptor</param>
    public static void AddDescriptor(string typeName, DataPortalProxyDescriptor descriptor)
    {
      InitializeDictionary();
      DataPortalTypeProxyDescriptors.Add(typeName, descriptor);
    }

    /// <summary>
    /// Add a proxy descriptor for the specified root business type.
    /// </summary>
    /// <param name="resourceId">Server resource id</param>
    /// <param name="descriptor">Data Portal proxy descriptor</param>
    public static void AddDescriptor(int resourceId, DataPortalProxyDescriptor descriptor)
    {
      InitializeDictionary();
      DataPortalTypeProxyDescriptors.Add(resourceId.ToString(), descriptor);
    }

    /// <summary>
    /// Returns a type name formatted to act as a key
    /// in the DataPortalTypeProxyDescriptors dictionary.
    /// </summary>
    /// <param name="objectType">Object type</param>
    public static string GetTypeName(Type objectType)
    {
      return $"{objectType.FullName}, {objectType.Assembly.FullName.Substring(0, objectType.Assembly.FullName.IndexOf(","))}";
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
        return ((DataPortalServerResourceAttribute)attributes[0]).ResourceId.ToString();
      else
        return GetTypeName(objectType);
    }
  }
}