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
      var key = new MethodCacheKey(objectType.Name, methodName, MethodCaller.GetParameterTypes(parameters));
      DataPortalMethodInfo result = null;
      if (!_cache.TryGetValue(key, out result))
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
        if (criteria is int)
          method = GetMethodInfo(objectType, "DataPortal_Create");
        else
          method = GetMethodInfo(objectType, "DataPortal_Create", criteria);
      }
      else
      {
        var factoryType = FactoryDataPortal.FactoryLoader.GetFactoryType(factoryInfo.FactoryTypeName);
        if (factoryType != null)
        {
          if (criteria is int)
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
        if (criteria is int)
          method = GetMethodInfo(objectType, "DataPortal_Fetch");
        else
          method = GetMethodInfo(objectType, "DataPortal_Fetch", criteria);
      }
      else
      {
        var factoryType = FactoryDataPortal.FactoryLoader.GetFactoryType(factoryInfo.FactoryTypeName);
        if (factoryType != null)
        {
          if (criteria is int)
            method = GetMethodInfo(
              factoryType,
              factoryInfo.FetchMethodName);
          else
            method = GetMethodInfo(
              FactoryDataPortal.FactoryLoader.GetFactoryType(factoryInfo.FactoryTypeName),
              factoryInfo.FetchMethodName,
              criteria);
        }
        else
          method = new DataPortalMethodInfo();
      }
      return method;
    }

    #endregion


  }
}
