//-----------------------------------------------------------------------
// <copyright file="DataPortalMethodCache.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
//-----------------------------------------------------------------------

#if NET8_0_OR_GREATER
using System.Runtime.Loader;

using Csla.Runtime;
#endif
using Csla.Reflection;
using Csla.Properties;
using System.Diagnostics.CodeAnalysis;

namespace Csla.Server
{
  internal static class DataPortalMethodCache
  {
#if NET8_0_OR_GREATER
    private static readonly Dictionary<MethodCacheKey, Tuple<string?, DataPortalMethodInfo>> _cache = [];
#else
    private static readonly Dictionary<MethodCacheKey, DataPortalMethodInfo> _cache = [];
#endif

    public static DataPortalMethodInfo GetMethodInfo([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.NonPublicMethods)] Type objectType, string methodName, params object[] parameters)
    {
      var key = new MethodCacheKey(objectType.FullName!, methodName, MethodCaller.GetParameterTypes(parameters));

#if NET8_0_OR_GREATER
      if (_cache.TryGetValue(key, out var methodInfo))
      {
        return methodInfo.Item2;
      }

      lock (_cache)
      {
        if (_cache.TryGetValue(key, out methodInfo))
        {
          return methodInfo.Item2;
        }

        var method = GetMethodOfCaller(objectType, methodName, parameters);

        var result = new DataPortalMethodInfo(method);
        var cacheInstance = AssemblyLoadContextManager.CreateCacheInstance(objectType, result, OnAssemblyLoadContextUnload);
        _cache.Add(key, cacheInstance);
        return result;
      }
#else
      if (_cache.TryGetValue(key, out var result))
        return result;

      lock (_cache)
      {
        if (!_cache.TryGetValue(key, out result))
        {
          var method = GetMethodOfCaller(objectType, methodName, parameters);
          result = new DataPortalMethodInfo(method);

          _cache.Add(key, result);
        }
      }

      return result;
#endif
    }

    private static System.Reflection.MethodInfo GetMethodOfCaller([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.NonPublicMethods)] Type objectType, string methodName, object[] parameters)
    {
      return MethodCaller.GetMethod(objectType, methodName, parameters) ?? throw new InvalidOperationException(string.Format(Resources.NoSuchMethod, $"{objectType.FullName}.{methodName}"));
    }

#if NET8_0_OR_GREATER
    private static void OnAssemblyLoadContextUnload(AssemblyLoadContext context)
    {
      lock (_cache)
        AssemblyLoadContextManager.RemoveFromCache((IDictionary<string, Tuple<string?, DynamicMemberHandle>?>)_cache, context);
    }
#endif
  }
}
