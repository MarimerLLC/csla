using System;
using Csla.Server;

namespace Csla.DataPortalClient
{
  /// <summary>
  /// Object that creates the appropriate
  /// data portal proxy object based on
  /// current configuration.
  /// </summary>
  public class ProxyFactory
  {
    /// <summary>
    /// Gets an instance of the proxy.
    /// </summary>
    /// <typeparam name="T">Type of business object
    /// for proxy.</typeparam>
    protected internal virtual IDataPortalProxy<T> GetProxy<T>() where T : Csla.Serialization.Mobile.IMobileObject
    {
      return GetProxy<T>(DataPortal.ProxyModes.Auto);
    }

    /// <summary>
    /// Gets an instance of the proxy.
    /// </summary>
    /// <typeparam name="T">Type of business object
    /// for proxy.</typeparam>
    /// <param name="proxyMode">
    /// Force the use of a local proxy.
    /// </param>
    protected internal virtual IDataPortalProxy<T> GetProxy<T>(DataPortal.ProxyModes proxyMode) 
      where T : Csla.Serialization.Mobile.IMobileObject
    {
      if (DataPortal.IsInDesignMode)
      {
        return new DesignTimeProxy<T>();
      }
      else
      {
        if (proxyMode == DataPortal.ProxyModes.LocalOnly || Csla.DataPortal.ProxyTypeName == "Local")
        {
          ObjectFactoryAttribute factoryInfo = ObjectFactoryAttribute.GetObjectFactoryAttribute(typeof(T));
          if (factoryInfo != null)
            return new FactoryProxy<T>(factoryInfo);
          else
            return new LocalProxy<T>();
        }
        else
        {
          Type proxyType = Csla.Reflection.MethodCaller.GetType(Csla.DataPortal.ProxyTypeName);
          Type generixProxyType = proxyType.MakeGenericType(typeof(T));
          return (IDataPortalProxy<T>)Activator.CreateInstance(generixProxyType);
        }
      }
    }

    
  }
}
