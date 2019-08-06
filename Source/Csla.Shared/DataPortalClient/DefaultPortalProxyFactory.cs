//-----------------------------------------------------------------------
// <copyright file="DefaultPortalProxyFactory.cs" company="Marimer LLC">
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
  
  internal class DefaultPortalProxyFactory : IDataPortalProxyFactory
  {
    private static Type _proxyType;

    /// <summary>
    /// Creates the DataPortalProxy to use for DataPortal call on the objectType.
    /// </summary>
    /// <param name="objectType"></param>
    /// <returns></returns>
     public IDataPortalProxy Create(Type objectType)
     {
       if ( _proxyType == null)
       {
         string proxyTypeName = ApplicationContext.DataPortalProxy;
         if (proxyTypeName == "Local")
           _proxyType = typeof(LocalProxy);
         else
#if NETFX_CORE
           _proxyType = Type.GetType(proxyTypeName);
#else
           _proxyType = Type.GetType(proxyTypeName, true, true);
#endif
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
  }
}