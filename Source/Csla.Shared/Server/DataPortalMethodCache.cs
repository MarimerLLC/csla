//-----------------------------------------------------------------------
// <copyright file="DataPortalMethodCache.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Gets a reference to the DataPortal_Create method for</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;

#if NET5_0_OR_GREATER
using System.Runtime.Loader;

using Csla.ALC;
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

            var cacheInstance = ALCManager.CreateCacheInstance(objectType, result, OnAssemblyLoadContextUnload);

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

#if NET40
    #region Data Portal Methods

    /// <summary>
    /// Gets a reference to the DataPortal_Create method for
    /// the specified business object type.
    /// </summary>
    /// <param name="objectType">Type of the business object.</param>
    /// <param name="criteria">Criteria parameter value.</param>
    /// <remarks>
    /// If the criteria parameter value is an integer, that is a special
    /// flag indicating that the parameter should be considered missing
    /// (not Nothing/null - just not there).
    /// </remarks>
    internal static DataPortalMethodInfo GetCreateMethod(Type objectType, object criteria)
    {
      // an "Integer" criteria is a special flag indicating
      // that criteria is empty and should not be used
      DataPortalMethodInfo method = null;
      var factoryInfo = ObjectFactoryAttribute.GetObjectFactoryAttribute(objectType);
      if (factoryInfo == null)
      {
        if (criteria is EmptyCriteria)
          method = GetMethodInfo(objectType, "DataPortal_Create");
        else
          method = GetMethodInfo(objectType, "DataPortal_Create", criteria);
      }
      else
      {
        var factoryType = FactoryDataPortal.FactoryLoader.GetFactoryType(factoryInfo.FactoryTypeName);
        if (factoryType != null)
        {
          if (criteria is EmptyCriteria)
            method = GetMethodInfo(
              factoryType,
              factoryInfo.CreateMethodName);
          else
            method = GetMethodInfo(
              factoryType,
              factoryInfo.CreateMethodName,
              criteria);
        }
        else
          method = new DataPortalMethodInfo();
      }
      return method;
    }

    /// <summary>
    /// Gets a reference to the DataPortal_Fetch method for
    /// the specified business object type.
    /// </summary>
    /// <param name="objectType">Type of the business object.</param>
    /// <param name="criteria">Criteria parameter value.</param>
    /// <remarks>
    /// If the criteria parameter value is an integer, that is a special
    /// flag indicating that the parameter should be considered missing
    /// (not Nothing/null - just not there).
    /// </remarks>
    internal static DataPortalMethodInfo GetFetchMethod(Type objectType, object criteria)
    {
      // an "Integer" criteria is a special flag indicating
      // that criteria is empty and should not be used
      DataPortalMethodInfo method = null;
      var factoryInfo = ObjectFactoryAttribute.GetObjectFactoryAttribute(objectType);
      if (factoryInfo == null)
      {
        if (criteria is EmptyCriteria)
          method = GetMethodInfo(objectType, "DataPortal_Fetch");
        else
          method = GetMethodInfo(objectType, "DataPortal_Fetch", criteria);
      }
      else
      {
        var factoryType = FactoryDataPortal.FactoryLoader.GetFactoryType(factoryInfo.FactoryTypeName);
        if (factoryType != null)
        {
          if (criteria is EmptyCriteria)
            method = GetMethodInfo(
              factoryType,
              factoryInfo.FetchMethodName);
          else
            method = GetMethodInfo(
              factoryType,
              factoryInfo.FetchMethodName,
              criteria);
        }
        else
          method = new DataPortalMethodInfo();
      }
      return method;
    }

    internal static DataPortalMethodInfo GetDeleteMethod(Type objectType, object criteria)
    {
      // an "Integer" criteria is a special flag indicating
      // that criteria is empty and should not be used
      DataPortalMethodInfo method = null;
      var factoryInfo = ObjectFactoryAttribute.GetObjectFactoryAttribute(objectType);
      if (factoryInfo == null)
      {
        if (criteria is EmptyCriteria)
          method = GetMethodInfo(objectType, "DataPortal_Delete");
        else
          method = GetMethodInfo(objectType, "DataPortal_Delete", criteria);
      }
      else
      {
        var factoryType = FactoryDataPortal.FactoryLoader.GetFactoryType(factoryInfo.FactoryTypeName);
        if (factoryType != null)
        {
          if (criteria is EmptyCriteria)
            method = GetMethodInfo(factoryType, factoryInfo.DeleteMethodName);
          else
            method = GetMethodInfo(factoryType, factoryInfo.DeleteMethodName, criteria);
        }
        else
          method = new DataPortalMethodInfo();
      }
      return method;
    }
    #endregion
#endif

#if NET5_0_OR_GREATER
    private static void OnAssemblyLoadContextUnload(AssemblyLoadContext context)
    {
      lock (_cache)
        ALCManager.RemoveFromCache(_cache, context);
    }
#endif
  }
}
