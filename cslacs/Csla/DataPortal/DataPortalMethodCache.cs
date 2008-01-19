using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        result = new DataPortalMethodInfo(MethodCaller.GetMethod(objectType, methodName, parameters));
        _cache.Add(key, result);
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

      DataPortalMethodInfo method = null;
      if (criteria is int)
      {
        // an "Integer" criteria is a special flag indicating
        // that criteria is empty and should not be used
        method = GetMethodInfo(objectType, "DataPortal_Create");

      }
      else
      {
        method = GetMethodInfo(objectType, "DataPortal_Create", criteria);
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

      DataPortalMethodInfo method = null;
      if (criteria is int)
      {
        // an "Integer" criteria is a special flag indicating
        // that criteria is empty and should not be used
        method = GetMethodInfo(objectType, "DataPortal_Fetch");

      }
      else
      {
        method = GetMethodInfo(objectType, "DataPortal_Fetch", criteria);
      }
      return method;

    }

    #endregion


  }
}
