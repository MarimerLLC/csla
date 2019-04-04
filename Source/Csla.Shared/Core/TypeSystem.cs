#if !NETFX_CORE
//-----------------------------------------------------------------------
// <copyright file="TypeSystem.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;

namespace Csla.Core
{
  internal static class TypeSystem
  {
    internal static Type GetElementType(Type seqType)
    {
      Type ienum = FindIEnumerable(seqType);
      if (ienum == null) return seqType;
      return ienum.GetGenericArguments()[0];
    }

    private static Type FindIEnumerable(Type seqType)
    {
      if (seqType == null || seqType == typeof(string))
        return null;

      if (seqType.IsArray)
        return typeof(IEnumerable<>).MakeGenericType(seqType.GetElementType());

      if (seqType.IsGenericType)
      {
        foreach (Type arg in seqType.GetGenericArguments())
        {
          Type ienum = typeof(IEnumerable<>).MakeGenericType(arg);
          if (ienum.IsAssignableFrom(seqType))
          {
            return ienum;
          }
        }
      }

      Type[] ifaces = seqType.GetInterfaces();
      if (ifaces != null && ifaces.Length > 0)
      {
        foreach (Type iface in ifaces)
        {
          Type ienum = FindIEnumerable(iface);
          if (ienum != null) return ienum;
        }
      }

      if (seqType.BaseType != null && seqType.BaseType != typeof(object))
      {
        return FindIEnumerable(seqType.BaseType);
      }

      return null;
    }
  }
}
#endif