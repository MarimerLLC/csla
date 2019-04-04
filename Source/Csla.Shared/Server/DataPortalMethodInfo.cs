//-----------------------------------------------------------------------
// <copyright file="DataPortalMethodInfo.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Linq;
using System.Reflection;

namespace Csla.Server
{
  internal class DataPortalMethodInfo
  {
    public bool RunLocal { get; private set; }
#if !(ANDROID || IOS) && !NETFX_CORE 
    public TransactionalAttribute TransactionalAttribute { get; private set; }
#else
    public TransactionalTypes TransactionalType { get; private set; }
#endif

    public DataPortalMethodInfo()
    {
#if !(ANDROID || IOS) && !NETFX_CORE 
      TransactionalAttribute = new TransactionalAttribute(TransactionalTypes.Manual);
#else
      TransactionalType = TransactionalTypes.Manual;
#endif

    }

    public DataPortalMethodInfo(System.Reflection.MethodInfo info)
      : this()
    {
      if (info != null)
      {
        RunLocal = IsRunLocal(info);
#if !(ANDROID || IOS) && !NETFX_CORE 
        TransactionalAttribute = GetTransactionalAttribute(info);
#else
        TransactionalType = TransactionalTypes.Manual;
#endif
      }
    }

    private static bool IsRunLocal(System.Reflection.MethodInfo method)
    {
#if NETFX_CORE
      return method.CustomAttributes.Count(r => r.AttributeType.Equals(typeof(RunLocalAttribute))) > 0;
#else
      return Attribute.IsDefined(method, typeof(RunLocalAttribute), false);
#endif
    }

#if !(ANDROID || IOS) && !NETFX_CORE 
    private static bool IsTransactionalMethod(System.Reflection.MethodInfo method)
    {
      return Attribute.IsDefined(method, typeof(TransactionalAttribute));
    }

    private static TransactionalAttribute GetTransactionalAttribute(System.Reflection.MethodInfo method)
    {
      TransactionalAttribute result;
      if (IsTransactionalMethod(method))
      {
        result =
          (TransactionalAttribute)Attribute.GetCustomAttribute(
          method, typeof(TransactionalAttribute));
      }
      else
        result = new TransactionalAttribute(TransactionalTypes.Manual);
      return result;
    }
#endif
  }
}