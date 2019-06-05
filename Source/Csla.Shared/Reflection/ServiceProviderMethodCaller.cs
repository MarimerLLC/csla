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
using System.Text;
using System.Threading.Tasks;
using Csla.Properties;
using Csla.Server;

namespace Csla.Reflection
{
  /// <summary>
  /// Methods to dynamically find/invoke methods
  /// with data portal and DI provided params
  /// </summary>
  public static class ServiceProviderMethodCaller
  {
    ///// <summary>
    ///// Invoke a method based on data portal criteria
    ///// and providing any remaining parameters with
    ///// values from an IServiceProvider
    ///// </summary>
    ///// <param name="target">Object with methods</param>
    ///// <param name="candidates">List of candidate method names</param>
    ///// <param name="criteria">Data portal criteria values</param>
    //public static void Invoke(object target, string[] candidates, object[] criteria)
    //{
    //  var method = FindMethodForCriteria(target, candidates, criteria);
    //}

    private static readonly BindingFlags _bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

    /// <summary>
    /// Find a method based on data portal criteria
    /// and providing any remaining parameters with
    /// values from an IServiceProvider
    /// </summary>
    /// <param name="target">Object with methods</param>
    /// <param name="attributeType">Data portal operation attribute</param>
    /// <param name="criteria">Data portal criteria values</param>
    public static System.Reflection.MethodInfo FindMethodForCriteria(object target, Type attributeType, object[] criteria)
    {
      if (target == null)
        throw new ArgumentNullException("target");

      var targetType = target.GetType();
      var candidates = targetType.GetMethods(_bindingAttr).
        Where(m => m.CustomAttributes.Count(a => a.AttributeType == attributeType) > 0).ToList();
      if (candidates.Count == 0)
      {
        var attributeName = attributeType.Name.Substring(0, attributeType.Name.IndexOf("Attribute"));
        if (attributeName.Contains("Child"))
          candidates.AddRange(targetType.GetMethods(_bindingAttr).Where(m => m.Name == "Child_" + attributeName.Substring(0, attributeName.IndexOf("Child"))));
        else
          candidates.AddRange(targetType.GetMethods(_bindingAttr).Where(m => m.Name == "DataPortal_" + attributeName));
      }
      if (candidates.Count == 0)
        throw new MissingMethodException($"{target.GetType().FullName}" + ".[" + attributeType.Name + "]");
      
      int criteriaLength = 0;
      if (criteria != null)
        criteriaLength = criteria.GetLength(0);
      List<System.Reflection.MethodInfo> matches = new List<System.Reflection.MethodInfo>();
      if (criteriaLength > 0)
      {
        foreach (var item in candidates)
        {
          var methodParams = GetCriteriaParameters(item);
          if (methodParams.Count() >= criteriaLength)
          {
            var index = 0;
            foreach (var c in criteria)
            {
              if (c != null && c.GetType() != methodParams[index].ParameterType)
                break;
              index++;
            }
            if (index == criteriaLength)
              matches.Add(item);
          }
        }
      }
      else
      {
        foreach (var item in candidates)
        {
          if (GetCriteriaParameters(item).Count() == 0)
            matches.Add(item);
        }
      }
      if (matches.Count == 0)
        throw new TargetParameterCountException(target.GetType().FullName + ".[" + attributeType.Name + "]");
      var result = matches[0];
      if (matches.Count > 1)
      {
        int max = 0;
        int maxCount = 0;
        foreach (var item in matches)
        {
          var count = item.GetParameters().Count();
          if (count > max)
          {
            maxCount = 1;
            max = count;
            result = item;
          }
          else if (count == max)
          {
            maxCount++;
          }
        }
        if (maxCount > 1)
          throw new AmbiguousMatchException(target.GetType().FullName + ".[" + attributeType.Name + "]");
      }
      return result;
    }

    private static ParameterInfo[] GetCriteriaParameters(System.Reflection.MethodInfo method)
    {
      var result = new List<ParameterInfo>();
      foreach (var item in method.GetParameters())
      {
        if (item.CustomAttributes.Count(a => a.AttributeType == typeof(FromServicesAttribute)) == 0)
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
        if (item.CustomAttributes.Where(a => a.AttributeType == typeof(FromServicesAttribute)).Count() > 0)
        {
          if (service != null)
            plist[index] = service.GetService(item.ParameterType);
        }
        else
        {
          plist[index] = parameters[criteriaIndex];
          criteriaIndex++;
        }
        index++;
      }

      var isAsyncTask = (info.ReturnType == typeof(Task));
      var isAsyncTaskObject = (info.ReturnType.IsGenericType && (info.ReturnType.GetGenericTypeDefinition() == typeof(Task<>)));
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
  }
}
#endif