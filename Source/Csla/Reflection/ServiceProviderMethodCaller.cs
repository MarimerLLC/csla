//-----------------------------------------------------------------------
// <copyright file="ServiceProviderMethodCaller.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Dynamically find/invoke methods with DI provided params</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
#if NET5_0_OR_GREATER
using System.Runtime.Loader;

using Csla.Runtime;
#endif
using Csla.Properties;

namespace Csla.Reflection
{
  /// <summary>
  /// Methods to dynamically find/invoke methods
  /// with data portal and DI provided params
  /// </summary>
  public class ServiceProviderMethodCaller : Core.IUseApplicationContext
  {
    private static readonly BindingFlags _bindingAttr = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly | BindingFlags.DeclaredOnly;
    private static readonly BindingFlags _factoryBindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

#if NET5_0_OR_GREATER
    private static readonly ConcurrentDictionary<string, Tuple<string, ServiceProviderMethodInfo>> _methodCache =
      new ConcurrentDictionary<string, Tuple<string, ServiceProviderMethodInfo>>();
#else
    private static readonly ConcurrentDictionary<string, ServiceProviderMethodInfo> _methodCache =
      new ConcurrentDictionary<string, ServiceProviderMethodInfo>();
#endif

    ApplicationContext Core.IUseApplicationContext.ApplicationContext { get => ApplicationContext; set => ApplicationContext = value; }
    private ApplicationContext ApplicationContext { get; set; }

    /// <summary>
    /// Find a method based on data portal criteria
    /// and providing any remaining parameters with
    /// values from an IServiceProvider
    /// </summary>
    /// <param name="target">Object with methods</param>
    /// <param name="criteria">Data portal criteria values</param>
    public ServiceProviderMethodInfo FindDataPortalMethod<T>(object target, object[] criteria)
      where T : DataPortalOperationAttribute
    {
      if (target == null)
        throw new ArgumentNullException(nameof(target));

      var targetType = target.GetType();
      return FindDataPortalMethod<T>(targetType, criteria);
    }

    /// <summary>
    /// Find a method based on data portal criteria
    /// and providing any remaining parameters with
    /// values from an IServiceProvider
    /// </summary>
    /// <param name="targetType">Type of domain object</param>
    /// <param name="criteria">Data portal criteria values</param>
    /// <param name="throwOnError">Throw exceptions on error</param>
    public ServiceProviderMethodInfo FindDataPortalMethod<T>(Type targetType, object[] criteria, bool throwOnError = true)
      where T : DataPortalOperationAttribute
    {
      if (targetType == null)
        throw new ArgumentNullException(nameof(targetType));

      var typeOfOperation = typeof(T);

      var cacheKey = GetCacheKeyName(targetType, typeOfOperation, criteria);

#if NET5_0_OR_GREATER
      if (_methodCache.TryGetValue(cacheKey, out var unloadableCachedMethodInfo))
      {
        var cachedMethod = unloadableCachedMethodInfo?.Item2;

#else
      if (_methodCache.TryGetValue(cacheKey, out ServiceProviderMethodInfo cachedMethod))
      {
#endif
        if (!throwOnError || cachedMethod != null)
          return cachedMethod;
      }

      var candidates = new List<ScoredMethodInfo>();
      var factoryInfo = Csla.Server.ObjectFactoryAttribute.GetObjectFactoryAttribute(targetType);
      if (factoryInfo != null)
      {
        var factoryType = Csla.Server.FactoryDataPortal.FactoryLoader.GetFactoryType(factoryInfo.FactoryTypeName);
        var ftList = new List<System.Reflection.MethodInfo>();
        var level = 0;
        while (factoryType != null)
        {
          ftList.Clear();
          if (typeOfOperation == typeof(CreateAttribute))
            ftList.AddRange(factoryType.GetMethods(_factoryBindingAttr).Where(m => m.Name == factoryInfo.CreateMethodName));
          else if (typeOfOperation == typeof(FetchAttribute))
            ftList.AddRange(factoryType.GetMethods(_factoryBindingAttr).Where(m => m.Name == factoryInfo.FetchMethodName));
          else if (typeOfOperation == typeof(DeleteAttribute))
            ftList.AddRange(factoryType.GetMethods(_factoryBindingAttr).Where(m => m.Name == factoryInfo.DeleteMethodName));
          else if (typeOfOperation == typeof(ExecuteAttribute))
            ftList.AddRange(factoryType.GetMethods(_factoryBindingAttr).Where(m => m.Name == factoryInfo.ExecuteMethodName));
          else if (typeOfOperation == typeof(CreateChildAttribute))
            ftList.AddRange(factoryType.GetMethods(_factoryBindingAttr).Where(m => m.Name == "Child_Create"));
          else
            ftList.AddRange(factoryType.GetMethods(_factoryBindingAttr).Where(m => m.Name == factoryInfo.UpdateMethodName));
          factoryType = factoryType.BaseType;
          candidates.AddRange(ftList.Select(r => new ScoredMethodInfo { MethodInfo = r, Score = level }));
          level--;
        }
        if (!candidates.Any() && typeOfOperation == typeof(CreateChildAttribute))
        {
          var ftlist = targetType.GetMethods(_bindingAttr).Where(m => m.Name == "Child_Create");
          candidates.AddRange(ftlist.Select(r => new ScoredMethodInfo { MethodInfo = r, Score = 0 }));
        }
      }
      else // not using factory types
      {
        var tt = targetType;
        var level = 0;
        while (tt != null)
        {
          var ttList = tt.GetMethods(_bindingAttr).Where(m => m.GetCustomAttributes<T>().Any());
          candidates.AddRange(ttList.Select(r => new ScoredMethodInfo { MethodInfo = r, Score = level }));
          tt = tt.BaseType;
          level--;
        }

        // if no attribute-based methods found, look for legacy methods
        if (!candidates.Any())
        {
          var attributeName = typeOfOperation.Name.Substring(0, typeOfOperation.Name.IndexOf("Attribute"));
          var methodName = attributeName.Contains("Child") ?
              "Child_" + attributeName.Substring(0, attributeName.IndexOf("Child")) :
              "DataPortal_" + attributeName;
          tt = targetType;
          level = 0;
          while (tt != null)
          {
            var ttList = tt.GetMethods(_bindingAttr).Where(m => m.Name == methodName);
            candidates.AddRange(ttList.Select(r => new ScoredMethodInfo { MethodInfo = r, Score = level }));
            tt = tt.BaseType;
            level--;
          }
        }
      }

      ScoredMethodInfo result = null;

      if (candidates != null && candidates.Any())
      {
        // scan candidate methods for matching criteria parameters
        int criteriaLength = 0;
        if (criteria != null)
          if (criteria.GetType().Equals(typeof(object[])))
            criteriaLength = criteria.GetLength(0);
          else
            criteriaLength = 1;

        var matches = new List<ScoredMethodInfo>();
        if (criteriaLength > 0)
        {
          foreach (var item in candidates)
          {
            int score = 0;
            var methodParams = GetCriteriaParameters(item.MethodInfo);
            if (methodParams.Length == criteriaLength)
            {
              var index = 0;
              if (criteria.GetType().Equals(typeof(object[])))
              {
                foreach (var c in criteria)
                {
                  var currentScore = CalculateParameterScore(methodParams[index], c);
                  if (currentScore == 0)
                  {
                    break;
                  }

                  score += currentScore;
                  index++;
                }
              }
              else
              {
                var currentScore = CalculateParameterScore(methodParams[index], criteria);
                if (currentScore != 0)
                {
                  score += currentScore;
                  index++;
                }
              }

              if (index == criteriaLength)
                matches.Add(new ScoredMethodInfo { MethodInfo = item.MethodInfo, Score = score + item.Score });
            }
          }
          if (matches.Count() == 0 && 
             (typeOfOperation == typeof(DeleteSelfAttribute) || typeOfOperation == typeof(DeleteSelfChildAttribute)
             || typeOfOperation == typeof(UpdateChildAttribute)))
          {
            // implement zero parameter fallback if other matches not found
            foreach (var item in candidates)
            {
              if (GetCriteriaParameters(item.MethodInfo).Length == 0)
                matches.Add(new ScoredMethodInfo { MethodInfo = item.MethodInfo, Score = item.Score });
            }
          }
        }
        else
        {
          foreach (var item in candidates)
          {
            if (GetCriteriaParameters(item.MethodInfo).Length == 0)
              matches.Add(new ScoredMethodInfo { MethodInfo = item.MethodInfo, Score = item.Score });
          }
        }
        if (matches.Count == 0)
        {
          // look for params array
          foreach (var item in candidates)
          {
            var lastParam = item.MethodInfo.GetParameters().LastOrDefault();
            if (lastParam != null && lastParam.ParameterType.Equals(typeof(object[])) &&
              lastParam.GetCustomAttributes<ParamArrayAttribute>().Any())
            {
              matches.Add(new ScoredMethodInfo { MethodInfo = item.MethodInfo, Score = 1 + item.Score });
            }
          }
        }

        if (matches.Count > 0)
        {
          result = matches[0];

          if (matches.Count > 1)
          {
            // disambiguate if necessary, using a greedy algorithm
            // so more DI parameters are better
            foreach (var item in matches)
              item.Score += GetDIParameters(item.MethodInfo).Length;

            var maxScore = int.MinValue;
            var maxCount = 0;
            foreach (var item in matches)
            {
              if (item.Score > maxScore)
              {
                maxScore = item.Score;
                maxCount = 1;
                result = item;
              }
              else if (item.Score == maxScore)
              {
                maxCount++;
              }
            }
            if (maxCount > 1)
            {
              if (throwOnError)
              {
                throw new AmbiguousMatchException($"{targetType.FullName}.[{typeOfOperation.Name.Replace("Attribute", "")}]{GetCriteriaTypeNames(criteria)}. Matches: {string.Join(", ", matches.Select(m => $"{m.MethodInfo.DeclaringType.FullName}[{m.MethodInfo}]"))}");
              }
              else
              {
                _methodCache.TryAdd(cacheKey, null);

                return null;
              }
            }
          }
        }
      }

      ServiceProviderMethodInfo resultingMethod = null;
      if (result != null)
      {
        resultingMethod = new ServiceProviderMethodInfo { MethodInfo = result.MethodInfo };
      }
      else
      {
        var baseType = targetType.BaseType;
        if (baseType == null)
        {
          if (throwOnError)
            throw new TargetParameterCountException(cacheKey);
          else
          {
            _methodCache.TryAdd(cacheKey, null);

            return null;
          }
        }
        try
        {
          resultingMethod = FindDataPortalMethod<T>(baseType, criteria, throwOnError);
        }
        catch (TargetParameterCountException ex)
        {
          throw new TargetParameterCountException(cacheKey, ex);
        }
        catch (AmbiguousMatchException ex)
        {
          throw new AmbiguousMatchException($"{targetType.FullName}.[{typeOfOperation.Name.Replace("Attribute", "")}]{GetCriteriaTypeNames(criteria)}.", ex);
        }
      }

#if NET5_0_OR_GREATER
      var cacheInstance = AssemblyLoadContextManager.CreateCacheInstance(targetType, resultingMethod, OnAssemblyLoadContextUnload);

      _ = _methodCache.TryAdd(cacheKey, cacheInstance);
#else
      _methodCache.TryAdd(cacheKey, resultingMethod);
#endif

      return resultingMethod;
    }

    private static int CalculateParameterScore(ParameterInfo methodParam, object c)
    {
      if (c == null)
      {
        if (methodParam.ParameterType.IsPrimitive)
          return 0;
        else if (methodParam.ParameterType == typeof(object))
          return 2;
        else if (methodParam.ParameterType == typeof(object[]))
          return 2;
        else if (methodParam.ParameterType.IsClass)
          return 1;
        else if (methodParam.ParameterType.IsArray)
          return 1;
        else if (methodParam.ParameterType.IsInterface)
          return 1;
        else if (Nullable.GetUnderlyingType(methodParam.ParameterType) != null)
          return 2;
      }
      else
      {
        if (c.GetType() == methodParam.ParameterType)
          return 3;
        else if (methodParam.ParameterType.Equals(typeof(object)))
          return 1;
        else if (methodParam.ParameterType.IsAssignableFrom(c.GetType()))
          return 2;
      }

      return 0;
    }

    private static string GetCacheKeyName(Type targetType, Type operationType, object[] criteria)
    {
      return $"{targetType.FullName}.[{operationType.Name.Replace("Attribute", "")}]{GetCriteriaTypeNames(criteria)}";
    }

    private static string GetCriteriaTypeNames(object[] criteria)
    {
      var result = new System.Text.StringBuilder();
      result.Append("(");
      if (criteria != null)
      {
        if (criteria.GetType().Equals(typeof(object[])))
        {
          bool first = true;
          foreach (var item in criteria)
          {
            if (first)
              first = false;
            else
              result.Append(",");
            if (item == null)
              result.Append("null");
            else
              result.Append(GetTypeName(item.GetType()));
          }
        }
        else
          result.Append(GetTypeName(criteria.GetType()));
      }

      result.Append(")");
      return result.ToString();
    }

    private static string GetTypeName(Type type)
    {
      if (type.IsArray)
      {
        return $"{GetTypeName(type.GetElementType())}[]";
      }

      if (!type.IsGenericType)
      {
        return type.Name;
      }

      var result = new System.Text.StringBuilder();
      var genericArguments = type.GetGenericArguments();
      result.Append(type.Name);
      result.Append("<");

      for (int i = 0; i < genericArguments.Length; i++)
      {
        if (i > 0)
        {
          result.Append(",");
        }

        result.Append(GetTypeName(genericArguments[i]));
      }

      result.Append(">");

      return result.ToString();
    }

    private static ParameterInfo[] GetCriteriaParameters(System.Reflection.MethodInfo method)
    {
      var result = new List<ParameterInfo>();
      foreach (var item in method.GetParameters())
      {
        if (!item.GetCustomAttributes<InjectAttribute>().Any())
          result.Add(item);
      }
      return result.ToArray();
    }

    private static ParameterInfo[] GetDIParameters(System.Reflection.MethodInfo method)
    {
      var result = new List<ParameterInfo>();
      foreach (var item in method.GetParameters())
      {
        if (item.GetCustomAttributes<InjectAttribute>().Any())
          result.Add(item);
      }
      return result.ToArray();
    }

    /// <summary>
    /// Invoke a method async if possible, providing
    /// parameters from the params array and from DI
    /// </summary>
    /// <param name="obj">Target object</param>
    /// <param name="method">Method to invoke</param>
    /// <param name="parameters">Criteria params array</param>
    /// <returns></returns>
    public async Task<object> CallMethodTryAsync(object obj, ServiceProviderMethodInfo method, object[] parameters)
    {
      if (method == null)
        throw new ArgumentNullException(obj.GetType().FullName + ".<null>() " + Resources.MethodNotImplemented);

      var info = method.MethodInfo;
      method.PrepForInvocation();

      object[] plist;

      if (method.TakesParamArray)
      {
        plist = new object[] { parameters };
      }
      else
      {
        plist = new object[method.Parameters.Length];
        int index = 0;
        int criteriaIndex = 0;
        var service = ApplicationContext.CurrentServiceProvider;
        foreach (var item in method.Parameters)
        {
          if (method.IsInjected[index])
          {
            if (service == null)
            {
              throw new NullReferenceException(nameof(service));
            }
            plist[index] = service.GetService(item.ParameterType);

          }
          else
          {
            if (parameters.GetType().Equals(typeof(object[])))
            {
              if (parameters == null || parameters.Length - 1 < criteriaIndex)
                plist[index] = null;
              else
                plist[index] = parameters[criteriaIndex];
            }
            else
              plist[index] = parameters;
            criteriaIndex++;
          }
          index++;
        }
      }

      try
      {
        if (method.IsAsyncTask)
        {
          await ((Task)method.DynamicMethod(obj, plist)).ConfigureAwait(false);
          return null;
        }
        else if (method.IsAsyncTaskObject)
        {
          return await ((Task<object>)method.DynamicMethod(obj, plist)).ConfigureAwait(false);
        }
        else
        {
          var result = method.DynamicMethod(obj, plist);
          return result;
        }
      }
      catch (Exception ex)
      {
        Exception inner = null;
        if (ex.InnerException == null)
          inner = ex;
        else
          inner = ex.InnerException;
        throw new CallMethodException(obj.GetType().Name + "." + info.Name + " " + Resources.MethodCallFailed, inner);
      }
    }
#if NET5_0_OR_GREATER

    private static void OnAssemblyLoadContextUnload(AssemblyLoadContext context)
    {
      AssemblyLoadContextManager.RemoveFromCache(_methodCache, context, true);
    }
#endif

    private class ScoredMethodInfo
    {
      public int Score { get; set; }
      public System.Reflection.MethodInfo MethodInfo { get; set; }
    }
  }
}
