using System;

namespace Csla.DataPortalClient
{
  public class ProxyFactory
  {
    protected internal virtual IDataPortalProxy<T> GetProxy<T>() where T : Csla.Serialization.Mobile.IMobileObject
    {
      if (Csla.DataPortal.ProxyTypeName == "Local")
        return new LocalProxy<T>();
      else
        return new WcfProxy<T>();
    }
  }
}
