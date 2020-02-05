#if !NET40
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
using Csla.Properties;

namespace Csla.Reflection
{
  /// <summary>
  /// Methods to dynamically find/invoke methods
  /// with data portal and DI provided params
  /// </summary>
  public static class ServiceProviderMethodCaller
  {
    private static readonly BindingFlags _bindingAttr = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
    private static readonly BindingFlags _factoryBindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
    private static readonly ConcurrentDictionary<string, ServiceProviderMethodInfo> _methodCache =
      new ConcurrentDictionary<string, ServiceProviderMethodInfo>();

    /// <summary>
    /// Find a method based on data portal criteria
    /// and providing any remaining parameters with
    /// values from an IServiceProvider
    /// </summary>
    /// <param name="target">Object with methods</param>
    /// <param name="criteria">Data portal criteria values</param>
    public static ServiceProviderMethodInfo FindDataPortalMethod<T>(object target, object[] criteria)
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
    public static ServiceProviderMethodInfo FindDataPortalMethod<T>(Type targetType, object[] criteria, bool throwOnError = true)
      where T : DataPortalOperationAttribute
    {
      if (targetType == null)
        throw new ArgumentNullException(nameof(targetType));

      var typeOfOperation = typeof(T);

      var cacheKey = GetCacheKeyName(targetType, typeOfOperation, criteria);
      if (_methodCache.TryGetValue(cacheKey, out ServiceProviderMethodInfo cachedMethod))
        if (!throwOnError || cachedMethod != null)
          return cachedMethod;

      IEnumerable<System.Reflection.MethodInfo> candidates = null;
      var factoryInfo = Csla.Server.ObjectFactoryAttribute.GetObjectFactoryAttribute(targetType);
      if (factoryInfo != null)
      {
        var factoryType = Csla.Server.FactoryDataPortal.FactoryLoader.GetFactoryType(factoryInfo.FactoryTypeName);
        if (factoryType != null)
        {
          if (typeOfOperation == typeof(CreateAttribute))
            candidates = factoryType.GetMethods(_factoryBindingAttr).Where(m => m.Name == factoryInfo.CreateMethodName);
          else if (typeOfOperation == typeof(FetchAttribute))
            candidates = factoryType.GetMethods(_factoryBindingAttr).Where(m => m.Name == factoryInfo.FetchMethodName);
          else if (typeOfOperation == typeof(DeleteAttribute))
            candidates = factoryType.GetMethods(_factoryBindingAttr).Where(m => m.Name == factoryInfo.DeleteMethodName);
          else if (typeOfOperation == typeof(ExecuteAttribute))
            candidates = factoryType.GetMethods(_factoryBindingAttr).Where(m => m.Name == factoryInfo.ExecuteMethodName);
          else
            candidates = factoryType.GetMethods(_factoryBindingAttr).Where(m => m.Name == factoryInfo.UpdateMethodName);
        }
      }
      else
      {
        candidates = targetType.GetMethods(_bindingAttr).
            Where(m => m.GetCustomAttributes<T>().Any());

        // if no attribute-based methods found, look for legacy methods
        if (!candidates.Any())
        {
          var attributeName = typeOfOperation.Name.Substring(0, typeOfOperation.Name.IndexOf("Attribute"));
          var methodName = attributeName.Contains("Child") ?
              "Child_" + attributeName.Substring(0, attributeName.IndexOf("Child")) :
              "DataPortal_" + attributeName;
          candidates = targetType.GetMethods(_bindingAttr).Where(
            m => m.Name == methodName);
        }
      }

      ScoredMethodInfo result = null;

      if (candidates != null && candidates.Any())
      {
        // scan candidate methods for matching criteria parameters
        int criteriaLength = 0;
        if (criteria != null)
          criteriaLength = criteria.GetLength(0);
        var matches = new List<ScoredMethodInfo>();
        if (criteriaLength > 0)
        {
          foreach (var item in candidates)
          {
            int score = 0;
            var methodParams = GetCriteriaParameters(item);
            if (methodParams.Length == criteriaLength)
            {
              var index = 0;
              foreach (var c in criteria)
              {
                if (c == null)
                {
                  if (methodParams[index].ParameterType.IsPrimitive)
                    break;
                  else if (methodParams[index].ParameterType == typeof(object))
                    score++;
                }
                else
                {
                  if (c.GetType() == methodParams[index].ParameterType)
                    score += 2;
                  else if (methodParams[index].ParameterType.IsAssignableFrom(c.GetType()))
                    score++;
                  else
                    break;
                }
                index++;
              }
              if (index == criteriaLength)
                matches.Add(new ScoredMethodInfo { MethodInfo = item, Score = score });
            }
          }
        }
        else
        {
          foreach (var item in candidates)
          {
            if (GetCriteriaParameters(item).Length == 0)
              matches.Add(new ScoredMethodInfo { MethodInfo = item });
          }
        }
        if (matches.Count == 0)
        {
          // look for params array
          foreach (var item in candidates)
          {
            var lastParam = item.GetParameters().LastOrDefault();
            if (lastParam != null && lastParam.ParameterType.Equals(typeof(object[])) &&
              lastParam.GetCustomAttributes<ParamArrayAttribute>().Any())
            {
              matches.Add(new ScoredMethodInfo { MethodInfo = item, Score = 1 });
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
        catch(TargetParameterCountException ex)
        {
          throw new TargetParameterCountException(cacheKey, ex);
        }
        catch(AmbiguousMatchException ex)
        {
          throw new AmbiguousMatchException($"{targetType.FullName}.[{typeOfOperation.Name.Replace("Attribute", "")}]{GetCriteriaTypeNames(criteria)}.", ex);
        }
      }
      _methodCache.TryAdd(cacheKey, resultingMethod);
      return resultingMethod;
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
    public static async Task<object> CallMethodTryAsync(object obj, ServiceProviderMethodInfo method, object[] parameters)
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
#if !NET40 && !NET45
        var service = ApplicationContext.ScopedServiceProvider;
#endif
        foreach (var item in method.Parameters)
        {
          if (method.IsInjected[index])
          {
#if !NET40 && !NET45
            if (service != null)
              plist[index] = service.GetService(item.ParameterType);
#endif
          }
          else
          {
            if (parameters == null || parameters.Length - 1 < criteriaIndex)
              plist[index] = null;
            else
              plist[index] = parameters[criteriaIndex];
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

    private class ScoredMethodInfo
    {
      public int Score { get; set; }
      public System.Reflection.MethodInfo MethodInfo { get; set; }
    }
  }
}
#endif
