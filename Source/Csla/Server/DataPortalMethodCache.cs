//-----------------------------------------------------------------------
// <copyright file="DataPortalMethodCache.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
#if NET5_0_OR_GREATER
using System.Runtime.Loader;

using Csla.Runtime;
#endif
using Csla.Reflection;

namespace Csla.Server
{
  internal static class DataPortalMethodCache
  {
#if NET5_0_OR_GREATER
    private static Dictionary<MethodCacheKey, Tuple<string, DataPortalMethodInfo>> _cache =
      new Dictionary<MethodCacheKey, Tuple<string, DataPortalMethodInfo>>();
#else
    private static Dictionary<MethodCacheKey, DataPortalMethodInfo> _cache =
      new Dictionary<MethodCacheKey, DataPortalMethodInfo>();
#endif

    public static DataPortalMethodInfo GetMethodInfo(Type objectType, string methodName, params object[] parameters)
    {
      var key = new MethodCacheKey(objectType.FullName, methodName, MethodCaller.GetParameterTypes(parameters));

      DataPortalMethodInfo result = null;

      var found = false;

#if NET5_0_OR_GREATER
      try
      {
        found = _cache.TryGetValue(key, out var methodInfo);

        result = methodInfo?.Item2;
      }
      catch
      { /* failure will drop into !found block */ }

      if (!found)
      {
        lock (_cache)
        {
          found = _cache.TryGetValue(key, out var methodInfo);

          result = methodInfo?.Item2;

          if (!found)
          {
            result = new DataPortalMethodInfo(MethodCaller.GetMethod(objectType, methodName, parameters));

            var cacheInstance = AssemblyLoadContextManager.CreateCacheInstance(objectType, result, OnAssemblyLoadContextUnload);

            _cache.Add(key, cacheInstance);
          }
        }
      }
#else
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
#endif

      return result;
    }

#if NET5_0_OR_GREATER
    private static void OnAssemblyLoadContextUnload(AssemblyLoadContext context)
    {
      lock (_cache)
        AssemblyLoadContextManager.RemoveFromCache(_cache, context);
    }
#endif
  }
}
