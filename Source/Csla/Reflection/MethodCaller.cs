//-----------------------------------------------------------------------
// <copyright file="MethodCaller.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Provides methods to dynamically find and call methods.</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Csla.Properties;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Csla.Reflection
{
  /// <summary>
  /// Provides methods to dynamically find and call methods.
  /// </summary>
  public static class MethodCaller
  {
    private const BindingFlags allLevelFlags 
      = BindingFlags.FlattenHierarchy 
      | BindingFlags.Instance 
      | BindingFlags.Public 
      | BindingFlags.NonPublic
      ;

    private const BindingFlags oneLevelFlags 
      = BindingFlags.DeclaredOnly 
      | BindingFlags.Instance 
      | BindingFlags.Public 
      | BindingFlags.NonPublic
      ;

    private const BindingFlags ctorFlags 
      = BindingFlags.Instance 
      | BindingFlags.Public 
      | BindingFlags.NonPublic
      ;

    private const BindingFlags factoryFlags =
      BindingFlags.Static |
      BindingFlags.Public |
      BindingFlags.FlattenHierarchy;

    private const BindingFlags privateMethodFlags =
      BindingFlags.Public |
      BindingFlags.NonPublic |
      BindingFlags.Instance |
      BindingFlags.FlattenHierarchy;

    #region Dynamic Method Cache

    private readonly static Dictionary<MethodCacheKey, DynamicMethodHandle> _methodCache = new Dictionary<MethodCacheKey, DynamicMethodHandle>();

    private static DynamicMethodHandle GetCachedMethod(object obj, System.Reflection.MethodInfo info, params object[] parameters)
    {
      var key = new MethodCacheKey(obj.GetType().FullName, info.Name, GetParameterTypes(parameters));
      DynamicMethodHandle mh = null;
      var found = false;
      try
      {
        found = _methodCache.TryGetValue(key, out mh);
      }
      catch
      { /* failure will drop into !found block */ }
      if (!found)
      {
        lock (_methodCache)
        {
          if (!_methodCache.TryGetValue(key, out mh))
          {
            mh = new DynamicMethodHandle(info, parameters);
            _methodCache.Add(key, mh);
          }
        }
      }
      return mh;
    }

    private static DynamicMethodHandle GetCachedMethod(object obj, string method, params object[] parameters)
    {
      return GetCachedMethod(obj, method, true, parameters);
    }

    private static DynamicMethodHandle GetCachedMethod(object obj, string method, bool hasParameters, params object[] parameters)
    {
      var key = new MethodCacheKey(obj.GetType().FullName, method, GetParameterTypes(hasParameters, parameters));
      if (!_methodCache.TryGetValue(key, out DynamicMethodHandle mh))
      {
        lock (_methodCache)
        {
          if (!_methodCache.TryGetValue(key, out mh))
          {
            var info = GetMethod(obj.GetType(), method, hasParameters, parameters);
            mh = new DynamicMethodHandle(info, parameters);
            _methodCache.Add(key, mh);
          }
        }
      }
      return mh;
    }

    #endregion

    #region Dynamic Constructor Cache

    private readonly static Dictionary<Type, DynamicCtorDelegate> _ctorCache = new Dictionary<Type, DynamicCtorDelegate>();

    private static DynamicCtorDelegate GetCachedConstructor(Type objectType) 
    {
      if (objectType == null)
        throw new ArgumentNullException(nameof(objectType));

      DynamicCtorDelegate result = null;
      var found = false;
      try
      {
        found = _ctorCache.TryGetValue(objectType, out result);
      }
      catch
      { /* failure will drop into !found block */ }
      if (!found)
      {
        lock (_ctorCache)
        {
          if (!_ctorCache.TryGetValue(objectType, out result))
          {
            ConstructorInfo info = objectType.GetConstructor(ctorFlags, null, Type.EmptyTypes, null);
            if (info == null)
              throw new NotSupportedException(string.Format(
                CultureInfo.CurrentCulture,
                "Cannot create instance of Type '{0}'. No public parameterless constructor found.",
                objectType));

            result = DynamicMethodHandlerFactory.CreateConstructor(info);
            _ctorCache.Add(objectType, result);
          }
        }
      }
      return result;
    }

    #endregion

    #region GetType

    /// <summary>
    /// Gets a Type object based on the type name.
    /// </summary>
    /// <param name="typeName">Type name including assembly name.</param>
    /// <param name="throwOnError">true to throw an exception if the type can't be found.</param>
    /// <param name="ignoreCase">true for a case-insensitive comparison of the type name.</param>
    public static Type GetType(string typeName, bool throwOnError, bool ignoreCase)
    {
      string fullTypeName;
        fullTypeName = typeName;
        return Type.GetType(fullTypeName, throwOnError, ignoreCase);
    }

    /// <summary>
    /// Gets a Type object based on the type name.
    /// </summary>
    /// <param name="typeName">Type name including assembly name.</param>
    /// <param name="throwOnError">true to throw an exception if the type can't be found.</param>
    public static Type GetType(string typeName, bool throwOnError)
    {
      return GetType(typeName, throwOnError, false);
    }

    /// <summary>
    /// Gets a Type object based on the type name.
    /// </summary>
    /// <param name="typeName">Type name including assembly name.</param>
    public static Type GetType(string typeName)
    {
      return GetType(typeName, true, false);
    }

    #endregion

    #region Create Instance

    /// <summary>
    /// Uses reflection to create an object using its 
    /// default constructor.
    /// </summary>
    /// <param name="objectType">Type of object to create.</param>
    public static object CreateInstance(Type objectType)
    {
      var provider = ApplicationContext.CurrentServiceProvider;
      if (provider is null)
        provider = ApplicationContext.DefaultServiceProvider;
      if (provider != null)
        return ActivatorUtilities.CreateInstance(provider, objectType);
      else
        return Activator.CreateInstance(objectType);
    }

    /// <summary>
    /// Creates an object.
    /// </summary>
    /// <param name="objectType">Type of object to create</param>
    /// <param name="parameters">Parameters for constructor</param>
    public static object CreateInstance(Type objectType, params object[] parameters)
    {
      var provider = ApplicationContext.CurrentServiceProvider;
      if (provider is null)
        provider = ApplicationContext.DefaultServiceProvider;
      if (provider != null)
        return ActivatorUtilities.CreateInstance(provider, objectType, parameters);
      else
        return Activator.CreateInstance(objectType, parameters);
    }

    /// <summary>
    /// Creates an instance of a generic type
    /// using its default constructor.
    /// </summary>
    /// <param name="type">Generic type to create</param>
    /// <param name="paramTypes">Type parameters</param>
    /// <returns></returns>
    public static object CreateGenericInstance(Type type, params Type[] paramTypes)
    {
      var genericType = type.GetGenericTypeDefinition();
      var gt = genericType.MakeGenericType(paramTypes);
      return CreateInstance(gt);
    }

    #endregion

    private const BindingFlags propertyFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy;
    private const BindingFlags fieldFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    private static readonly Dictionary<MethodCacheKey, DynamicMemberHandle> _memberCache = new Dictionary<MethodCacheKey, DynamicMemberHandle>();

    internal static DynamicMemberHandle GetCachedProperty(Type objectType, string propertyName)
    {
      var key = new MethodCacheKey(objectType.FullName, propertyName, GetParameterTypes(null));
      if (!_memberCache.TryGetValue(key, out DynamicMemberHandle mh))
      {
        lock (_memberCache)
        {
          if (!_memberCache.TryGetValue(key, out mh))
          {
            PropertyInfo info = objectType.GetProperty(propertyName, propertyFlags);
            if (info == null)
              throw new InvalidOperationException(
                string.Format(Resources.MemberNotFoundException, propertyName));
            mh = new DynamicMemberHandle(info);
            _memberCache.Add(key, mh);
          }
        }
      }
      return mh;
    }

    internal static DynamicMemberHandle GetCachedField(Type objectType, string fieldName)
    {
      var key = new MethodCacheKey(objectType.FullName, fieldName, GetParameterTypes(null));
      if (!_memberCache.TryGetValue(key, out DynamicMemberHandle mh))
      {
        lock (_memberCache)
        {
          if (!_memberCache.TryGetValue(key, out mh))
          {
            FieldInfo info = objectType.GetField(fieldName, fieldFlags);
            if (info == null)
              throw new InvalidOperationException(
                string.Format(Resources.MemberNotFoundException, fieldName));
            mh = new DynamicMemberHandle(info);
            _memberCache.Add(key, mh);
          }
        }
      }
      return mh;
    }

    /// <summary>
    /// Invokes a property getter using dynamic
    /// method invocation.
    /// </summary>
    /// <param name="obj">Target object.</param>
    /// <param name="property">Property to invoke.</param>
    /// <returns></returns>
    public static object CallPropertyGetter(object obj, string property)
    {
      if (ApplicationContext.UseReflectionFallback)
      {
        var propertyInfo = obj.GetType().GetProperty(property);
        return propertyInfo.GetValue(obj);
      }
      else
      {
        if (obj == null)
          throw new ArgumentNullException("obj");
        if (string.IsNullOrEmpty(property))
          throw new ArgumentException("Argument is null or empty.", "property");

        var mh = GetCachedProperty(obj.GetType(), property);
        if (mh.DynamicMemberGet == null)
        {
          throw new NotSupportedException(string.Format(
            CultureInfo.CurrentCulture,
            "The property '{0}' on Type '{1}' does not have a public getter.",
            property,
            obj.GetType()));
        }

        return mh.DynamicMemberGet(obj);
      }
    }

    /// <summary>
    /// Invokes a property setter using dynamic
    /// method invocation.
    /// </summary>
    /// <param name="obj">Target object.</param>
    /// <param name="property">Property to invoke.</param>
    /// <param name="value">New value for property.</param>
    public static void CallPropertySetter(object obj, string property, object value)
    {
      if (obj == null)
        throw new ArgumentNullException("obj");
      if (string.IsNullOrEmpty(property))
        throw new ArgumentException("Argument is null or empty.", "property");

      if (ApplicationContext.UseReflectionFallback)
      {
        var propertyInfo = obj.GetType().GetProperty(property);
        propertyInfo.SetValue(obj, value);
      }
      else
      {
        var mh = GetCachedProperty(obj.GetType(), property);
        if (mh.DynamicMemberSet == null)
        {
          throw new NotSupportedException(string.Format(
            CultureInfo.CurrentCulture,
            "The property '{0}' on Type '{1}' does not have a public setter.",
            property,
            obj.GetType()));
        }

        mh.DynamicMemberSet(obj, value);
      }
    }


    #region Call Method

    /// <summary>
    /// Uses reflection to dynamically invoke a method
    /// if that method is implemented on the target object.
    /// </summary>
    /// <param name="obj">
    /// Object containing method.
    /// </param>
    /// <param name="method">
    /// Name of the method.
    /// </param>
    public static object CallMethodIfImplemented(object obj, string method)
    {
      return CallMethodIfImplemented(obj, method, false, null);
    }

    /// <summary>
    /// Uses reflection to dynamically invoke a method
    /// if that method is implemented on the target object.
    /// </summary>
    /// <param name="obj">
    /// Object containing method.
    /// </param>
    /// <param name="method">
    /// Name of the method.
    /// </param>
    /// <param name="parameters">
    /// Parameters to pass to method.
    /// </param>
    public static object CallMethodIfImplemented(object obj, string method, params object[] parameters)
    {
      return CallMethodIfImplemented(obj, method, true, parameters);
    }

    private static object CallMethodIfImplemented(object obj, string method, bool hasParameters, params object[] parameters)
    {
      if (ApplicationContext.UseReflectionFallback)
      {
        var found = (FindMethod(obj.GetType(), method, GetParameterTypes(hasParameters, parameters)) != null);
        if (found)
          return CallMethod(obj, method, parameters);
        else
          return null;
      }
      else
      {
        var mh = GetCachedMethod(obj, method, parameters);
        if (mh == null || mh.DynamicMethod == null)
          return null;
        return CallMethod(obj, mh, hasParameters, parameters);
      }
    }

    /// <summary>
    /// Detects if a method matching the name and parameters is implemented on the provided object.
    /// </summary>
    /// <param name="obj">The object implementing the method.</param>
    /// <param name="method">The name of the method to find.</param>
    /// <param name="parameters">The parameters matching the parameters types of the method to match.</param>
    /// <returns>True obj implements a matching method.</returns>
    public static bool IsMethodImplemented(object obj, string method, params object[] parameters)
    {
      var mh = GetCachedMethod(obj, method, parameters);
      return mh != null && mh.DynamicMethod != null;
    }

    /// <summary>
    /// Uses reflection to dynamically invoke a method,
    /// throwing an exception if it is not
    /// implemented on the target object.
    /// </summary>
    /// <param name="obj">
    /// Object containing method.
    /// </param>
    /// <param name="method">
    /// Name of the method.
    /// </param>
    public static object CallMethod(object obj, string method)
    {
      return CallMethod(obj, method, false, null);
    }

    /// <summary>
    /// Uses reflection to dynamically invoke a method,
    /// throwing an exception if it is not
    /// implemented on the target object.
    /// </summary>
    /// <param name="obj">
    /// Object containing method.
    /// </param>
    /// <param name="method">
    /// Name of the method.
    /// </param>
    /// <param name="parameters">
    /// Parameters to pass to method.
    /// </param>
    public static object CallMethod(object obj, string method, params object[] parameters)
    {
      return CallMethod(obj, method, true, parameters);
    }

    private static object CallMethod(object obj, string method, bool hasParameters, params object[] parameters)
    {
      if (ApplicationContext.UseReflectionFallback)
      {
        System.Reflection.MethodInfo info = GetMethod(obj.GetType(), method, hasParameters, parameters);
        if (info == null)
          throw new NotImplementedException(obj.GetType().Name + "." + method + " " + Resources.MethodNotImplemented);

        return CallMethod(obj, info, hasParameters, parameters);
      }
      else
      {
        var mh = GetCachedMethod(obj, method, hasParameters, parameters);
        if (mh == null || mh.DynamicMethod == null)
          throw new NotImplementedException(obj.GetType().Name + "." + method + " " + Resources.MethodNotImplemented);
        return CallMethod(obj, mh, hasParameters, parameters);
      }
    }

    /// <summary>
    /// Uses reflection to dynamically invoke a method,
    /// throwing an exception if it is not
    /// implemented on the target object.
    /// </summary>
    /// <param name="obj">
    /// Object containing method.
    /// </param>
    /// <param name="info">
    /// System.Reflection.MethodInfo for the method.
    /// </param>
    /// <param name="parameters">
    /// Parameters to pass to method.
    /// </param>
    public static object CallMethod(object obj, System.Reflection.MethodInfo info, params object[] parameters)
    {
      return CallMethod(obj, info, true, parameters);
    }

    private static object CallMethod(object obj, System.Reflection.MethodInfo info, bool hasParameters, params object[] parameters)
    {
      if (ApplicationContext.UseReflectionFallback)
      {
        var infoParams = info.GetParameters();
        var infoParamsCount = infoParams.Length;
        bool hasParamArray = infoParamsCount > 0 && infoParams[infoParamsCount - 1].GetCustomAttributes(typeof(ParamArrayAttribute), true).Length > 0;
        bool specialParamArray = false;
        if (hasParamArray && infoParams[infoParamsCount - 1].ParameterType.Equals(typeof(string[])))
          specialParamArray = true;
        if (hasParamArray && infoParams[infoParamsCount - 1].ParameterType.Equals(typeof(object[])))
          specialParamArray = true;
        object[] par;
        if (infoParamsCount == 1 && specialParamArray)
        {
          par = new object[] { parameters };
        }
        else if (infoParamsCount > 1 && hasParamArray && specialParamArray)
        {
          par = new object[infoParamsCount];
          for (int i = 0; i < infoParamsCount - 1; i++)
            par[i] = parameters[i];
          par[infoParamsCount - 1] = parameters[infoParamsCount - 1];
        }
        else
        {
          par = parameters;
        }

        object result;
        try
        {
          result = info.Invoke(obj, par);
        }
        catch (Exception e)
        {
          Exception inner;
          if (e.InnerException == null)
            inner = e;
          else
            inner = e.InnerException;
          throw new CallMethodException(obj.GetType().Name + "." + info.Name + " " + Resources.MethodCallFailed, inner);
        }
        return result;
      }
      else
      {
        var mh = GetCachedMethod(obj, info, parameters);
        if (mh == null || mh.DynamicMethod == null)
          throw new NotImplementedException(obj.GetType().Name + "." + info.Name + " " + Resources.MethodNotImplemented);
        return CallMethod(obj, mh, hasParameters, parameters);
      }
    }

    private static object CallMethod(object obj, DynamicMethodHandle methodHandle, bool hasParameters, params object[] parameters)
    {
      object result = null;
      var method = methodHandle.DynamicMethod;

      object[] inParams;
      if (parameters == null)
        inParams = new object[] { null };
      else
        inParams = parameters;

      if (methodHandle.HasFinalArrayParam)
      {
        // last param is a param array or only param is an array
        var pCount = methodHandle.MethodParamsLength;
        var inCount = inParams.Length;
        if (inCount == pCount - 1)
        {
          // no paramter was supplied for the param array
          // copy items into new array with last entry null
          object[] paramList = new object[pCount];
          for (var pos = 0; pos <= pCount - 2; pos++)
            paramList[pos] = parameters[pos];
          paramList[paramList.Length - 1] = hasParameters && inParams.Length == 0 ? inParams : null;

          // use new array
          inParams = paramList;
        }
        else if ((inCount == pCount && inParams[inCount - 1] != null && !inParams[inCount - 1].GetType().IsArray) || inCount > pCount)
        {
          // 1 or more params go in the param array
          // copy extras into an array
          var extras = inParams.Length - (pCount - 1);
          object[] extraArray = GetExtrasArray(extras, methodHandle.FinalArrayElementType);
          Array.Copy(inParams, pCount - 1, extraArray, 0, extras);

          // copy items into new array
          object[] paramList = new object[pCount];
          for (var pos = 0; pos <= pCount - 2; pos++)
            paramList[pos] = parameters[pos];
          paramList[paramList.Length - 1] = extraArray;

          // use new array
          inParams = paramList;
        }
      }
      try
      {
        result = methodHandle.DynamicMethod(obj, inParams);
      }
      catch (Exception ex)
      {
        throw new CallMethodException(obj.GetType().Name + "." + methodHandle.MethodName + " " + Resources.MethodCallFailed, ex);
      }
      return result;
    }

    private static object[] GetExtrasArray(int count, Type arrayType)
    {
      return (object[])(System.Array.CreateInstance(arrayType.GetElementType(), count));
    }
#endregion

#region Get/Find Method

    /// <summary>
    /// Uses reflection to locate a matching method
    /// on the target object.
    /// </summary>
    /// <param name="objectType">
    /// Type of object containing method.
    /// </param>
    /// <param name="method">
    /// Name of the method.
    /// </param>
    public static System.Reflection.MethodInfo GetMethod(Type objectType, string method)
    {
      return GetMethod(objectType, method, true, false, null);
    }

    /// <summary>
    /// Uses reflection to locate a matching method
    /// on the target object.
    /// </summary>
    /// <param name="objectType">
    /// Type of object containing method.
    /// </param>
    /// <param name="method">
    /// Name of the method.
    /// </param>
    /// <param name="parameters">
    /// Parameters to pass to method.
    /// </param>
    public static System.Reflection.MethodInfo GetMethod(Type objectType, string method, params object[] parameters)
    {
      return GetMethod(objectType, method, true, parameters);
    }

    private static System.Reflection.MethodInfo GetMethod(Type objectType, string method, bool hasParameters, params object[] parameters)
    {
      System.Reflection.MethodInfo result;

      object[] inParams;
      if (!hasParameters)
        inParams = new object[] { };
      else if (parameters == null)
        inParams = new object[] { null };
      else
        inParams = parameters;

      // try to find a strongly typed match

      // first see if there's a matching method
      // where all params match types
      result = FindMethod(objectType, method, GetParameterTypes(hasParameters, inParams));

      if (result == null)
      {
        // no match found - so look for any method
        // with the right number of parameters
        try
        {
          result = FindMethod(objectType, method, inParams.Length);
        }
        catch (AmbiguousMatchException)
        {
          // we have multiple methods matching by name and parameter count
          result = FindMethodUsingFuzzyMatching(objectType, method, inParams);
        }
      }

      // no strongly typed match found, get default based on name only
      if (result == null)
      {
        result = objectType.GetMethod(method, allLevelFlags);
      }
      return result;
    }

    private static System.Reflection.MethodInfo FindMethodUsingFuzzyMatching(Type objectType, string method, object[] parameters)
    {
      System.Reflection.MethodInfo result = null;
      Type currentType = objectType;
      do
      {
        System.Reflection.MethodInfo[] methods = currentType.GetMethods(oneLevelFlags);
        int parameterCount = parameters.Length;
        // Match based on name and parameter types and parameter arrays
        foreach (System.Reflection.MethodInfo m in methods)
        {
          if (m.Name == method)
          {
            var infoParams = m.GetParameters();
            var pCount = infoParams.Length;
            if (pCount > 0)
            {
              if (pCount == 1 && infoParams[0].ParameterType.IsArray)
              {
                // only param is an array
                if (parameters.GetType().Equals(infoParams[0].ParameterType))
                {
                  // got a match so use it
                  result = m;
                  break;
                }
              }
              if (infoParams[pCount - 1].GetCustomAttributes(typeof(ParamArrayAttribute), true).Length > 0)
              {
                // last param is a param array
                if (parameterCount == pCount && parameters[pCount - 1].GetType().Equals(infoParams[pCount - 1].ParameterType))
                {
                  // got a match so use it
                  result = m;
                  break;
                }
              }
            }
          }
        }
        if (result == null)
        {
          // match based on parameter name and number of parameters
          foreach (System.Reflection.MethodInfo m in methods)
          {
            if (m.Name == method && m.GetParameters().Length == parameterCount)
            {
              result = m;
              break;
            }
          }
        }
        if (result != null)
          break;
        currentType = currentType.BaseType;
      } while (currentType != null);

      
      return result;
    }

    /// <summary>
    /// Returns information about the specified
    /// method, even if the parameter types are
    /// generic and are located in an abstract
    /// generic base class.
    /// </summary>
    /// <param name="objectType">
    /// Type of object containing method.
    /// </param>
    /// <param name="method">
    /// Name of the method.
    /// </param>
    /// <param name="types">
    /// Parameter types to pass to method.
    /// </param>
    public static System.Reflection.MethodInfo FindMethod(Type objectType, string method, Type[] types)
    {
      System.Reflection.MethodInfo info;
      do
      {
        // find for a strongly typed match
        info = objectType.GetMethod(method, oneLevelFlags, null, types, null);
        if (info != null)
        {
          break; // match found
        }

        objectType = objectType.BaseType;
      } while (objectType != null);

      return info;
    }

    /// <summary>
    /// Returns information about the specified
    /// method, finding the method based purely
    /// on the method name and number of parameters.
    /// </summary>
    /// <param name="objectType">
    /// Type of object containing method.
    /// </param>
    /// <param name="method">
    /// Name of the method.
    /// </param>
    /// <param name="parameterCount">
    /// Number of parameters to pass to method.
    /// </param>
    public static System.Reflection.MethodInfo FindMethod(Type objectType, string method, int parameterCount)
    {
      // walk up the inheritance hierarchy looking
      // for a method with the right number of
      // parameters
      System.Reflection.MethodInfo result = null;
      Type currentType = objectType;
      do
      {
        System.Reflection.MethodInfo info = currentType.GetMethod(method, oneLevelFlags);
        if (info != null)
        {
          var infoParams = info.GetParameters();
          var pCount = infoParams.Length;
          if (pCount > 0 &&
             ((pCount == 1 && infoParams[0].ParameterType.IsArray) ||
             (infoParams[pCount - 1].GetCustomAttributes(typeof(ParamArrayAttribute), true).Length > 0)))
          {
            // last param is a param array or only param is an array
            if (parameterCount >= pCount - 1)
            {
              // got a match so use it
              result = info;
              break;
            }
          }
          else if (pCount == parameterCount)
          {
            // got a match so use it
            result = info;
            break;
          }
        }
        currentType = currentType.BaseType;
      } while (currentType != null);

      return result;
    }

#endregion

    /// <summary>
    /// Returns an array of Type objects corresponding
    /// to the type of parameters provided.
    /// </summary>
    public static Type[] GetParameterTypes()
    {
      return GetParameterTypes(false, null);
    }

    /// <summary>
    /// Returns an array of Type objects corresponding
    /// to the type of parameters provided.
    /// </summary>
    /// <param name="parameters">
    /// Parameter values.
    /// </param>
    public static Type[] GetParameterTypes(object[] parameters)
    {
      return GetParameterTypes(true, parameters);
    }

    private static Type[] GetParameterTypes(bool hasParameters, object[] parameters)
    {
      if (!hasParameters)
        return new Type[] { };

      List<Type> result = new List<Type>();

      if (parameters == null)
      {
        result.Add(typeof(object));

      }
      else
      {
        foreach (object item in parameters)
        {
          if (item == null)
          {
            result.Add(typeof(object));
          }
          else
          {
            result.Add(item.GetType());
          }
        }
      }
      return result.ToArray();
    }

    /// <summary>
    /// Gets a property type descriptor by name.
    /// </summary>
    /// <param name="t">Type of object containing the property.</param>
    /// <param name="propertyName">Name of the property.</param>
    public static PropertyDescriptor GetPropertyDescriptor(Type t, string propertyName)
    {
      var propertyDescriptors = TypeDescriptor.GetProperties(t);
      PropertyDescriptor result = null;
      foreach (PropertyDescriptor desc in propertyDescriptors)
        if (desc.Name == propertyName)
        {
          result = desc;
          break;
        }
      return result;
    }

    /// <summary>
    /// Gets information about a property.
    /// </summary>
    /// <param name="objectType">Object containing the property.</param>
    /// <param name="propertyName">Name of the property.</param>
    public static PropertyInfo GetProperty(Type objectType, string propertyName)
    {
      return objectType.GetProperty(propertyName, propertyFlags);
    }

    /// <summary>
    /// Gets a property value.
    /// </summary>
    /// <param name="obj">Object containing the property.</param>
    /// <param name="info">Property info object for the property.</param>
    /// <returns>The value of the property.</returns>
    public static object GetPropertyValue(object obj, PropertyInfo info)
    {
      object result;
      try
      {
        result = info.GetValue(obj, null);
      }
      catch (Exception e)
      {
        Exception inner;
        if (e.InnerException == null)
          inner = e;
        else
          inner = e.InnerException;
        throw new CallMethodException(obj.GetType().Name + "." + info.Name + " " + Resources.MethodCallFailed, inner);
      }
      return result;
    }

    /// <summary>
    /// Invokes an instance method on an object.
    /// </summary>
    /// <param name="obj">Object containing method.</param>
    /// <param name="info">Method info object.</param>
    /// <returns>Any value returned from the method.</returns>
    public static object CallMethod(object obj, System.Reflection.MethodInfo info)
    {
      object result;
      try
      {
        result = info.Invoke(obj, null);
      }
      catch (Exception e)
      {
        Exception inner;
        if (e.InnerException == null)
          inner = e;
        else
          inner = e.InnerException;
        throw new CallMethodException(obj.GetType().Name + "." + info.Name + " " + Resources.MethodCallFailed, inner);
      }
      return result;
    }

    /// <summary>
    /// Uses reflection to dynamically invoke a method,
    /// throwing an exception if it is not
    /// implemented on the target object.
    /// </summary>
    /// <param name="obj">
    /// Object containing method.
    /// </param>
    /// <param name="method">
    /// Name of the method.
    /// </param>
    /// <param name="parameters">
    /// Parameters to pass to method.
    /// </param>
    public async static Task<object> CallMethodTryAsync(object obj, string method, params object[] parameters)
    {
      return await CallMethodTryAsync(obj, method, true, parameters);
    }

    /// <summary>
    /// Invokes an instance method on an object. If the method
    /// is async returning Task of object it will be invoked using an await statement.
    /// </summary>
    /// <param name="obj">Object containing method.</param>
    /// <param name="method">
    /// Name of the method.
    /// </param>
    public async static Task<object> CallMethodTryAsync(object obj, string method)
    {
      return await CallMethodTryAsync(obj, method, false, null);
    }

    private async static Task<object> CallMethodTryAsync(object obj, string method, bool hasParameters, params object[] parameters)
    {
      try
      {
        if (ApplicationContext.UseReflectionFallback)
        {
          var info = FindMethod(obj.GetType(), method, GetParameterTypes(hasParameters, parameters));
          if (info == null)
            throw new NotImplementedException(obj.GetType().Name + "." + method + " " + Resources.MethodNotImplemented);
          var isAsyncTask = (info.ReturnType == typeof(Task));
          var isAsyncTaskObject = (info.ReturnType.IsGenericType && (info.ReturnType.GetGenericTypeDefinition() == typeof(Task<>)));
          if (isAsyncTask)
          {
            await (Task)CallMethod(obj, method, hasParameters, parameters);
            return null;
          }
          else if (isAsyncTaskObject)
          {
            return await (Task<object>)CallMethod(obj, method, hasParameters, parameters);
          }
          else
          {
            return CallMethod(obj, method, hasParameters, parameters);
          }
        }
        else
        {
          var mh = GetCachedMethod(obj, method, hasParameters, parameters);
          if (mh == null || mh.DynamicMethod == null)
            throw new NotImplementedException(obj.GetType().Name + "." + method + " " + Resources.MethodNotImplemented);
          if (mh.IsAsyncTask)
          {
            await (Task)CallMethod(obj, mh, hasParameters, parameters);
            return null;
          }
          else if (mh.IsAsyncTaskObject)
          {
            return await (Task<object>)CallMethod(obj, mh, hasParameters, parameters);
          }
          else
          {
            return CallMethod(obj, mh, hasParameters, parameters);
          }
        }
      }
      catch (InvalidCastException ex)
      {
        throw new NotSupportedException(
          string.Format(Resources.TaskOfObjectException, obj.GetType().Name + "." + method),
          ex);
      }
    }

    /// <summary>
    /// Returns true if the method provided is an async method returning a Task object.
    /// </summary>
    /// <param name="obj">Object containing method.</param>
    /// <param name="method">Name of the method.</param>
    public static bool IsAsyncMethod(object obj, string method)
    {
      return IsAsyncMethod(obj, method, false, null);
    }

    /// <summary>
    /// Returns true if the method provided is an async method returning a Task object.
    /// </summary>
    /// <param name="obj">Object containing method.</param>
    /// <param name="method">Name of the method.</param>
    /// <param name="parameters">
    /// Parameters to pass to method.
    /// </param>
    public static bool IsAsyncMethod(object obj, string method, params object[] parameters)
    {
      return IsAsyncMethod(obj, method, true, parameters);
    }

    internal static bool IsAsyncMethod(System.Reflection.MethodInfo info)
    {
      var isAsyncTask = (info.ReturnType == typeof(Task));
      var isAsyncTaskObject = (info.ReturnType.IsGenericType && (info.ReturnType.GetGenericTypeDefinition() == typeof(Task<>)));
      return isAsyncTask || isAsyncTaskObject;
    }

    private static bool IsAsyncMethod(object obj, string method, bool hasParameters, params object[] parameters)
    {
      if (ApplicationContext.UseReflectionFallback)
      {
        var info = FindMethod(obj.GetType(), method, GetParameterTypes(hasParameters, parameters));
        if (info == null)
          throw new NotImplementedException(obj.GetType().Name + "." + method + " " + Resources.MethodNotImplemented);
        return IsAsyncMethod(info);
      }
      else
      {
        var mh = GetCachedMethod(obj, method, hasParameters, parameters);
        if (mh == null || mh.DynamicMethod == null)
          throw new NotImplementedException(obj.GetType().Name + "." + method + " " + Resources.MethodNotImplemented);

        return mh.IsAsyncTask || mh.IsAsyncTaskObject;
      }
    }

    /// <summary>
    /// Invokes a generic async static method by name
    /// </summary>
    /// <param name="objectType">Class containing static method</param>
    /// <param name="method">Method to invoke</param>
    /// <param name="typeParams">Type parameters for method</param>
    /// <param name="hasParameters">Flag indicating whether method accepts parameters</param>
    /// <param name="parameters">Parameters for method</param>
    /// <returns></returns>
    public static Task<object> CallGenericStaticMethodAsync(Type objectType, string method, Type[] typeParams, bool hasParameters, params object[] parameters)
    {
      var tcs = new TaskCompletionSource<object>();
      try
      {
        Task task = null;
        if (hasParameters)
        {
          var pTypes = GetParameterTypes(parameters);
          var methodReference = objectType.GetMethod(method, BindingFlags.Static | BindingFlags.Public, null, CallingConventions.Any, pTypes, null);
          if (methodReference == null)
            methodReference = objectType.GetMethod(method, BindingFlags.Static | BindingFlags.Public);
          if (methodReference == null)
            throw new InvalidOperationException(objectType.Name + "." + method);
          var gr = methodReference.MakeGenericMethod(typeParams);
          task = (Task)gr.Invoke(null, parameters);
        }
        else
        {
          var methodReference = objectType.GetMethod(method, BindingFlags.Static | BindingFlags.Public, null, CallingConventions.Any, System.Type.EmptyTypes, null);
          var gr = methodReference.MakeGenericMethod(typeParams);
          task = (Task)gr.Invoke(null, null);
        }
        task.Wait();
        if (task.Exception != null)
          tcs.SetException(task.Exception);
        else
          tcs.SetResult(Csla.Reflection.MethodCaller.CallPropertyGetter(task, "Result"));
      }
      catch (Exception ex)
      {
        tcs.SetException(ex);
      }
      return tcs.Task;
    }

    /// <summary>
    /// Invokes a generic method by name
    /// </summary>
    /// <param name="target">Object containing method to invoke</param>
    /// <param name="method">Method to invoke</param>
    /// <param name="typeParams">Type parameters for method</param>
    /// <param name="hasParameters">Flag indicating whether method accepts parameters</param>
    /// <param name="parameters">Parameters for method</param>
    /// <returns></returns>
    public static object CallGenericMethod(object target, string method, Type[] typeParams, bool hasParameters, params object[] parameters)
    {
      var objectType = target.GetType();
      object result;
      if (hasParameters)
      {
        var pTypes = GetParameterTypes(parameters);
        var methodReference = objectType.GetMethod(method, BindingFlags.Instance | BindingFlags.Public, null, CallingConventions.Any, pTypes, null);
        if (methodReference == null)
          methodReference = objectType.GetMethod(method, BindingFlags.Instance | BindingFlags.Public);
        if (methodReference == null)
          throw new InvalidOperationException(objectType.Name + "." + method);
        var gr = methodReference.MakeGenericMethod(typeParams);
        result = gr.Invoke(target, parameters);
      }
      else
      {
        var methodReference = objectType.GetMethod(method, BindingFlags.Static | BindingFlags.Public, null, CallingConventions.Any, System.Type.EmptyTypes, null);
        if (methodReference == null)
          throw new InvalidOperationException(objectType.Name + "." + method);
        var gr = methodReference.MakeGenericMethod(typeParams);
        result = gr.Invoke(target, null);
      }
      return result;
    }

    /// <summary>
    /// Invokes a static factory method.
    /// </summary>
    /// <param name="objectType">Business class where the factory is defined.</param>
    /// <param name="method">Name of the factory method</param>
    /// <param name="parameters">Parameters passed to factory method.</param>
    /// <returns>Result of the factory method invocation.</returns>
    public static object CallFactoryMethod(Type objectType, string method, params object[] parameters)
    {
      object returnValue;
      System.Reflection.MethodInfo factory = objectType.GetMethod(
           method, factoryFlags, null,
           MethodCaller.GetParameterTypes(parameters), null);

      if (factory == null)
      {
        // strongly typed factory couldn't be found
        // so find one with the correct number of
        // parameters 
        int parameterCount = parameters.Length;
        System.Reflection.MethodInfo[] methods = objectType.GetMethods(factoryFlags);
        foreach (System.Reflection.MethodInfo oneMethod in methods)
          if (oneMethod.Name == method && oneMethod.GetParameters().Length == parameterCount)
          {
            factory = oneMethod;
            break;
          }
      }
      if (factory == null)
      {
        // no matching factory could be found
        // so throw exception
        throw new InvalidOperationException(
          string.Format(Resources.NoSuchFactoryMethod, method));
      }
      try
      {
        returnValue = factory.Invoke(null, parameters);
      }
      catch (Exception ex)
      {
        Exception inner;
        if (ex.InnerException == null)
          inner = ex;
        else
          inner = ex.InnerException;
        throw new CallMethodException(objectType.Name + "." + factory.Name + " " + Resources.MethodCallFailed, inner);
      }
      return returnValue;
    }

    /// <summary>
    /// Gets a System.Reflection.MethodInfo object corresponding to a
    /// non-public method.
    /// </summary>
    /// <param name="objectType">Object containing the method.</param>
    /// <param name="method">Name of the method.</param>
    public static System.Reflection.MethodInfo GetNonPublicMethod(Type objectType, string method)
    {
      var result = FindMethod(objectType, method, privateMethodFlags);
      return result;
    }

    /// <summary>
    /// Returns information about the specified
    /// method.
    /// </summary>
    /// <param name="objType">Type of object.</param>
    /// <param name="method">Name of the method.</param>
    /// <param name="flags">Flag values.</param>
    public static System.Reflection.MethodInfo FindMethod(Type objType, string method, BindingFlags flags)
    {
      System.Reflection.MethodInfo info;
      do
      {
        // find for a strongly typed match
        info = objType.GetMethod(method, flags);
        if (info != null)
          break; // match found
        objType = objType.BaseType;
      } while (objType != null);

      return info;
    }
  }
}