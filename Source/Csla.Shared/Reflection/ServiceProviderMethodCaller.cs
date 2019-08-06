#if !NET40
//-----------------------------------------------------------------------
// <copyright file="ServiceProviderMethodCaller.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Dynamically find/invoke methods with DI provided params</summary>
//-----------------------------------------------------------------------
using System;
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
    private static readonly BindingFlags _bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

    /// <summary>
    /// Find a method based on data portal criteria
    /// and providing any remaining parameters with
    /// values from an IServiceProvider
    /// </summary>
    /// <param name="target">Object with methods</param>
    /// <param name="attributeType">Data portal operation attribute</param>
    /// <param name="criteria">Data portal criteria values</param>
    public static System.Reflection.MethodInfo FindDataPortalMethod(object target, Type attributeType, object[] criteria)
    {
      if (target == null)
        throw new ArgumentNullException("target");

      var targetType = target.GetType();
      return FindDataPortalMethod(targetType, attributeType, criteria);
    }

    /// <summary>
    /// Find a method based on data portal criteria
    /// and providing any remaining parameters with
    /// values from an IServiceProvider
    /// </summary>
    /// <param name="targetType">Type of domain object</param>
    /// <param name="attributeType">Data portal operation attribute</param>
    /// <param name="criteria">Data portal criteria values</param>
    public static System.Reflection.MethodInfo FindDataPortalMethod(Type targetType, Type attributeType, object[] criteria)
    {
      if (targetType == null)
        throw new ArgumentNullException("targetType");

      var candidates = new List<System.Reflection.MethodInfo>();

      var factoryInfo = Csla.Server.ObjectFactoryAttribute.GetObjectFactoryAttribute(targetType);
      if (factoryInfo != null)
      {
        var factoryType = Csla.Server.FactoryDataPortal.FactoryLoader.GetFactoryType(factoryInfo.FactoryTypeName);
        if (factoryType != null)
        {
          if (attributeType == typeof(CreateAttribute))
            candidates = factoryType.GetMethods(_bindingAttr).Where(m => m.Name == factoryInfo.CreateMethodName).ToList();
          else if (attributeType == typeof(FetchAttribute))
            candidates = factoryType.GetMethods(_bindingAttr).Where(m => m.Name == factoryInfo.FetchMethodName).ToList();
          else if (attributeType == typeof(DeleteAttribute))
            candidates = factoryType.GetMethods(_bindingAttr).Where(m => m.Name == factoryInfo.DeleteMethodName).ToList();
          else if (attributeType == typeof(ExecuteAttribute))
            candidates = factoryType.GetMethods(_bindingAttr).Where(m => m.Name == factoryInfo.ExecuteMethodName).ToList();
          else
            candidates = factoryType.GetMethods(_bindingAttr).Where(m => m.Name == factoryInfo.UpdateMethodName).ToList();
        }
      }
      else
      {
        var tType = targetType;
        do
        {
          candidates.AddRange(tType.GetMethods(_bindingAttr).
            Where(m => m.CustomAttributes.Count(a => a.AttributeType == attributeType) > 0).ToList());
          tType = tType.BaseType;
        } while (tType != null);

        // if no attribute-based methods found, look for legacy methods
        if (candidates.Count == 0)
        {
          var attributeName = attributeType.Name.Substring(0, attributeType.Name.IndexOf("Attribute"));
          if (attributeName.Contains("Child"))
          {
            var methodName = "Child_" + attributeName.Substring(0, attributeName.IndexOf("Child"));
            tType = targetType;
            do
            {
              candidates.AddRange(tType.GetMethods(_bindingAttr).Where(
                m => m.Name == methodName && candidates.Count(c => c.ToString() == m.ToString()) == 0));
              tType = tType.BaseType;
            } while (tType != null);
          }
          else
          {
            var methodName = "DataPortal_" + attributeName;
            tType = targetType;
            do
            {
              candidates.AddRange(tType.GetMethods(_bindingAttr).Where(
                m => m.Name == methodName && candidates.Count(c => c.ToString() == m.ToString()) == 0));
              tType = tType.BaseType;
            } while (tType != null);
          }
        }
      }
      if (candidates.Count == 0)
        throw new MissingMethodException($"{targetType.FullName}" + ".[" + attributeType.Name + "]");
      
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
          if (methodParams.Count() >= criteriaLength)
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
                  score++;
                else if (methodParams[index].ParameterType != typeof(object))
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
          if (GetCriteriaParameters(item).Count() == 0)
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
            lastParam.CustomAttributes != null &&
            lastParam.CustomAttributes.Where(a => a.AttributeType.Equals(typeof(ParamArrayAttribute))).Count() > 0)
          {
            matches.Add(new ScoredMethodInfo { MethodInfo = item, Score = 1 });
          }
        }
      }
      if (matches.Count == 0)
        throw new TargetParameterCountException(targetType.FullName + ".[" + attributeType.Name + "]");

      var result = matches[0];
      if (matches.Count > 1)
      {
        // disambiguate if necessary, using a greedy algorithm
        // so more DI parameters are better
        foreach (var item in matches)
          item.Score += GetDIParameters(item.MethodInfo).Count();

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
          throw new AmbiguousMatchException(targetType.FullName + ".[" + attributeType.Name + "]");
      }
      return result.MethodInfo;
    }

    private static ParameterInfo[] GetCriteriaParameters(System.Reflection.MethodInfo method)
    {
      var result = new List<ParameterInfo>();
      foreach (var item in method.GetParameters())
      {
        if (item.CustomAttributes.Count(a => a.AttributeType == typeof(InjectAttribute)) == 0)
          result.Add(item);
      }
      return result.ToArray();
    }

    private static ParameterInfo[] GetDIParameters(System.Reflection.MethodInfo method)
    {
      var result = new List<ParameterInfo>();
      foreach (var item in method.GetParameters())
      {
        if (item.CustomAttributes.Count(a => a.AttributeType == typeof(InjectAttribute)) > 0)
          result.Add(item);
      }
      return result.ToArray();
    }

    /// <summary>
    /// Invoke a method async if possible, providing
    /// paramters from the params array and from DI
    /// </summary>
    /// <param name="obj">Target object</param>
    /// <param name="info">Method to invoke</param>
    /// <param name="parameters">Criteria params array</param>
    /// <returns></returns>
    public static async Task<object> CallMethodTryAsync(object obj, System.Reflection.MethodInfo info, object[] parameters)
    {
      if (info == null)
        throw new NotImplementedException(obj.GetType().Name + "." + info.Name + " " + Resources.MethodNotImplemented);

      var methodParameters = info.GetParameters();
      var plist = new object[methodParameters.Count()];
      int index = 0;
      int criteriaIndex = 0;

      IServiceProvider service = ApplicationContext.ScopedServiceProvider;

      foreach (var item in methodParameters)
      {
        if (item.CustomAttributes.Where(a => a.AttributeType == typeof(InjectAttribute)).Count() > 0)
        {
          if (service != null)
            plist[index] = service.GetService(item.ParameterType);
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

      var isAsyncTask = (info.ReturnType == typeof(Task));
      var isAsyncTaskObject = (info.ReturnType.IsGenericType && (info.ReturnType.GetGenericTypeDefinition() == typeof(Task<>)));
      try
      {
        if (isAsyncTask)
        {
          await (Task)info.Invoke(obj, plist);
          return null;
        }
        else if (isAsyncTaskObject)
        {
          return await (Task<object>)info.Invoke(obj, plist);
        }
        else
        {
          var result = info.Invoke(obj, plist);
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