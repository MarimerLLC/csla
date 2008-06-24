using System;
using System.Collections.Generic;
using System.Reflection;
using Csla.Properties;
using Csla;

namespace Csla.Reflection
{
  /// <summary>
  /// Provides methods to dynamically find and call methods.
  /// </summary>
  public static class MethodCaller
  {

    private const BindingFlags allLevelFlags = BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public;

    private const BindingFlags oneLevelFlags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public;

    private const BindingFlags ctorFlags = BindingFlags.Instance | BindingFlags.Public;

    #region Dynamic Method Cache

    private static Dictionary<MethodCacheKey, DynamicMethodHandle> _methodCache = new Dictionary<MethodCacheKey, DynamicMethodHandle>();

    private static DynamicMethodHandle GetCachedMethod(object obj, MethodInfo info, params object[] parameters)
    {
      var key = new MethodCacheKey(obj.GetType().FullName, info.Name, GetParameterTypes(parameters));
      DynamicMethodHandle mh = null;
      if (!_methodCache.TryGetValue(key, out mh))
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
      var key = new MethodCacheKey(obj.GetType().FullName, method, GetParameterTypes(parameters));
      DynamicMethodHandle mh = null;
      if (!_methodCache.TryGetValue(key, out mh))
      {
        lock (_methodCache)
        {
          if (!_methodCache.TryGetValue(key, out mh))
          {
            MethodInfo info = GetMethod(obj.GetType(), method, parameters);
            mh = new DynamicMethodHandle(info, parameters);
            _methodCache.Add(key, mh);
          }
        }
      }
      return mh;
    }

    #endregion

    #region Dynamic Constructor Cache 

    private static Dictionary<Type, DynamicCtorDelegate> _ctorCache = new Dictionary<Type, DynamicCtorDelegate>();

    private static DynamicCtorDelegate GetCachedConstructor(Type objectType)
    {
      DynamicCtorDelegate result = null;
      if (!_ctorCache.TryGetValue(objectType, out result))
      {
        lock (_ctorCache)
        {
          if (!_ctorCache.TryGetValue(objectType, out result))
          {
            ConstructorInfo info = 
              objectType.GetConstructor(ctorFlags, null, Type.EmptyTypes, null);
            result = DynamicMethodHandlerFactory.CreateConstructor(info);
            _ctorCache.Add(objectType, result);
          }
        }
      }
      return result;
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
      var ctor = GetCachedConstructor(objectType);
      if (ctor == null)
        throw new NotImplementedException("Default constructor " + Resources.MethodNotImplemented);
      return ctor.Invoke();
    }

    #endregion

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
    /// <param name="parameters">
    /// Parameters to pass to method.
    /// </param>
    public static object CallMethodIfImplemented(object obj, string method, params object[] parameters)
    {
      var mh = GetCachedMethod(obj, method, parameters);
      if (mh == null || mh.DynamicMethod == null)
        return null;
      return CallMethod(obj, mh, parameters);
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
      var mh = GetCachedMethod(obj, method, parameters);
      if (mh == null || mh.DynamicMethod == null)
        throw new NotImplementedException(method + " " + Resources.MethodNotImplemented);
      return CallMethod(obj, mh, parameters);
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
    /// MethodInfo for the method.
    /// </param>
    /// <param name="parameters">
    /// Parameters to pass to method.
    /// </param>
    public static object CallMethod(object obj, MethodInfo info, params object[] parameters)
    {
      var mh = GetCachedMethod(obj, info, parameters);
      if (mh == null || mh.DynamicMethod == null)
        throw new NotImplementedException(info.Name + " " + Resources.MethodNotImplemented);
      return CallMethod(obj, mh, parameters);
    }

    /// <summary>
    /// Uses reflection to dynamically invoke a method,
    /// throwing an exception if it is not implemented
    /// on the target object.
    /// </summary>
    /// <param name="obj">
    /// Object containing method.
    /// </param>
    /// <param name="methodHandle">
    /// MethodHandle for the method.
    /// </param>
    /// <param name="parameters">
    /// Parameters to pass to method.
    /// </param>
    private static object CallMethod(object obj, DynamicMethodHandle methodHandle, params object[] parameters)
    {
      object result = null;
      var method = methodHandle.DynamicMethod;

      object[] inParams = null;
      if (parameters == null)
        inParams = new object[] { null };
      else
        inParams = parameters;

      if (methodHandle.HasFinalArrayParam)
      {
        var pCount = methodHandle.MethodParamsLength;
        // last param is a param array or only param is an array
        var extras = inParams.Length - (pCount - 1);

        // 1 or more params go in the param array
        // copy extras into an array
        object[] extraArray = GetExtrasArray(extras, methodHandle.FinalArrayElementType);
        Array.Copy(inParams, extraArray, extras);

        // copy items into new array
        object[] paramList = new object[pCount];
        for (var pos = 0; pos <= pCount - 2; pos++)
          paramList[pos] = parameters[pos];
        paramList[paramList.Length - 1] = extraArray;

        // use new array
        inParams = paramList;
      }
      try
      {
        result = methodHandle.DynamicMethod(obj, inParams);
      }
      catch (Exception ex)
      {
        throw new CallMethodException(methodHandle.MethodName + " " + Resources.MethodCallFailed, ex);
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
    /// <param name="parameters">
    /// Parameters to pass to method.
    /// </param>
    public static MethodInfo GetMethod(Type objectType, string method, params object[] parameters)
    {

      MethodInfo result = null;

      object[] inParams = null;
      if (parameters == null)
        inParams = new object[] { null };
      else
        inParams = parameters;

      // try to find a strongly typed match

      // first see if there's a matching method
      // where all params match types
      result = FindMethod(objectType, method, GetParameterTypes(inParams));

      if (result == null)
      {
        // no match found - so look for any method
        // with the right number of parameters
        result = FindMethod(objectType, method, inParams.Length);
      }

      // no strongly typed match found, get default
      if (result == null)
      {
        try
        {
          result = objectType.GetMethod(method, allLevelFlags);
        }
        catch (AmbiguousMatchException)
        {
          MethodInfo[] methods = objectType.GetMethods();
          foreach (MethodInfo m in methods)
          {
            if (m.Name == method && m.GetParameters().Length == inParams.Length)
            {
              result = m;
              break;
            }
          }
          if (result == null)
            throw;
        }
      }

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
    public static MethodInfo FindMethod(Type objectType, string method, Type[] types)
    {
      MethodInfo info = null;
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
    public static MethodInfo FindMethod(Type objectType, string method, int parameterCount)
    {
      // walk up the inheritance hierarchy looking
      // for a method with the right number of
      // parameters
      MethodInfo result = null;
      Type currentType = objectType;
      do
      {
        MethodInfo info = currentType.GetMethod(method, oneLevelFlags);
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
    /// <param name="parameters">
    /// Parameter values.
    /// </param>
    public static Type[] GetParameterTypes(object[] parameters)
    {
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
    /// Returns a business object type based on
    /// the supplied criteria object.
    /// </summary>
    /// <param name="criteria">
    /// Criteria object.
    /// </param>
    public static Type GetObjectType(object criteria)
    {
      var strong = criteria as CriteriaBase;
      if (strong != null)
      {
        // get the type of the actual business object
        // from the ICriteria
        return strong.ObjectType;
      }
      else
      {
        // get the type of the actual business object
        // based on the nested class scheme in the book
        return criteria.GetType().DeclaringType;
      }
    }
  }
}
