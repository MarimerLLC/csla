//-----------------------------------------------------------------------
// <copyright file="DataPortalMethodCache.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Gets a reference to the DataPortal_Create method for</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using Csla.Reflection;

namespace Csla.Server
{
  internal static class DataPortalMethodCache
  {
    private static Dictionary<MethodCacheKey, DataPortalMethodInfo> _cache = 
      new Dictionary<MethodCacheKey, DataPortalMethodInfo>();

    public static DataPortalMethodInfo GetMethodInfo(Type objectType, string methodName, params object[] parameters)
    {
      var key = new MethodCacheKey(objectType.FullName, methodName, MethodCaller.GetParameterTypes(parameters));
      DataPortalMethodInfo result = null;
      var found = false;
      try
      {
        found = _cache.TryGetValue(key, out result);
      }
      catch
      { /* failure will drop into !found block */ }
      if (!found)
      {
        lock (_cache)
        {
          if (!_cache.TryGetValue(key, out result))
          {
            result = new DataPortalMethodInfo(MethodCaller.GetMethod(objectType, methodName, parameters));
            _cache.Add(key, result);
          }
        }
      }
      return result;
    }
  }
}