using System;
using Csla.Server;


namespace Csla.DataPortalClient
{
  public class ProxyFactory
  {

    protected internal virtual IDataPortalProxy<T> GetProxy<T>() where T : Csla.Serialization.Mobile.IMobileObject
    {
      return GetProxy<T>(DataPortal.ProxyModes.Auto);
    }

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
          Type proxyType = Type.GetType(Csla.DataPortal.ProxyTypeName);
          Type generixProxyType = proxyType.MakeGenericType(typeof(T));
          return (IDataPortalProxy<T>)Activator.CreateInstance(generixProxyType);
        }
      }
    }

    
  }
}
