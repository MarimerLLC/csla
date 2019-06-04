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
        Where(m => m.CustomAttributes.Count(a => a.AttributeType == attributeType) > 0).ToArray();
      if (candidates.GetLength(0) == 0)
        throw new InvalidOperationException($"Invoke {target.GetType().FullName} 0 candidates");
      
      int criteriaLength = 0;
      if (criteria != null)
        criteriaLength = criteria.GetLength(0);
      List<System.Reflection.MethodInfo> matches = new List<System.Reflection.MethodInfo>();
      if (criteriaLength > 0)
      {
        foreach (var item in candidates)
        {
          var methodParams = item.GetParameters();
          if (methodParams.Count() >= criteriaLength)
          {
            var index = 0;
            foreach (var c in criteria)
            {
              if (c != null && c.GetType() != methodParams[index].GetType())
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
        throw new TargetParameterCountException(target.GetType().FullName + "." + candidates[0]);
      if (matches.Count > 1)
        throw new AmbiguousMatchException(target.GetType().FullName + "." + candidates[0]);
      return matches[0];
    }

    private static ParameterInfo[] GetCriteriaParameters(System.Reflection.MethodInfo method)
    {
      var result = new List<ParameterInfo>();
      foreach (var item in method.GetParameters())
      {
        if (item.CustomAttributes.Count(a => a.AttributeType == typeof(FromServices)) == 0)
          result.Add(item);
      }
      return result.ToArray();
    }
  }
}
#endif