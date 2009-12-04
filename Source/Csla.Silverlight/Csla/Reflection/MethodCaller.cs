using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Csla.Properties;
using System.Diagnostics;

namespace Csla.Reflection
{
  /// <summary>
  /// Provides methods to dynamically find and call methods.
  /// </summary>
#if TESTING
  [System.Diagnostics.DebuggerNonUserCode]
#endif
  public static class MethodCaller
  {
    const BindingFlags allLevelFlags =
      BindingFlags.FlattenHierarchy |
      BindingFlags.Instance |
      BindingFlags.Public;

    const BindingFlags oneLevelFlags =
      BindingFlags.DeclaredOnly |
      BindingFlags.Instance |
      BindingFlags.Public;

    private const BindingFlags ctorFlags =
      BindingFlags.Instance |
      BindingFlags.Public;

    private const BindingFlags factoryFlags =
      BindingFlags.Static |
      BindingFlags.Public |
      BindingFlags.FlattenHierarchy;

    private const BindingFlags propertyFlags =
      BindingFlags.Public |
      BindingFlags.Instance |
      BindingFlags.FlattenHierarchy;

    private const BindingFlags privateMethodFlags =
      BindingFlags.Public |
      BindingFlags.NonPublic |
      BindingFlags.Instance |
      BindingFlags.FlattenHierarchy;

    #region Dynamic Constructor Cache

    // TODO: Make dynamic when time permits.
    private static Dictionary<Type, ConstructorInfo> _ctorCache = new Dictionary<Type, ConstructorInfo>();

    private static ConstructorInfo GetCachedConstructor(Type objectType)
    {
      ConstructorInfo result = null;
      if (!_ctorCache.TryGetValue(objectType, out result))
      {
        lock (_ctorCache)
        {
          if (!_ctorCache.TryGetValue(objectType, out result))
          {
            ConstructorInfo info =
              objectType.GetConstructor(ctorFlags, null, Type.EmptyTypes, null);
            result = info; // DynamicMethodHandlerFactory.CreateConstructor(info);
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
      if (typeName.Contains("Version="))
        fullTypeName = typeName;
      else
        fullTypeName = typeName + ", Version=..., Culture=neutral, PublicKeyToken=null";
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
      var ctor = GetCachedConstructor(objectType);
      if (ctor == null)
        throw new NotImplementedException(Resources.DefaultConstructor + " " + Resources.MethodNotImplemented);
      return ctor.Invoke(null);
    }

    #endregion

    /// <summary>
    /// Uses reflection to determine whether a method
    /// is implemented on the target object.
    /// </summary>
    public static bool IsMethodImplemented(object obj, string method, params object[] parameters)
    {
      MethodInfo info = GetMethod(obj.GetType(), method, parameters);
      return info != null;
    }

    /// <summary>
    /// Uses reflection to dynamically invoke a method
    /// if that method is implemented on the target object.
    /// </summary>
    public static object CallMethodIfImplemented(object obj, string method, params object[] parameters)
    {
      MethodInfo info = GetMethod(obj.GetType(), method, parameters);
      if (info != null)
        return CallMethod(obj, info, parameters);
      else
        return null;
    }

    /// <summary>
    /// Uses reflection to dynamically invoke a method,
    /// throwing an exception if it is not
    /// implemented on the target object.
    /// </summary>
    public static object CallMethod(object obj, string method, params object[] parameters)
    {
      MethodInfo info = GetMethod(obj.GetType(), method, parameters);
      if (info == null)
        throw new NotImplementedException(method + " " + Resources.MethodNotImplemented);

      return CallMethod(obj, info, parameters);
    }

    /// <summary>
    /// Uses reflection to dynamically invoke a method,
    /// throwing an exception if it is not implemented
    /// on the target object.
    /// </summary>
    public static object CallMethod(object obj, MethodInfo info, params object[] parameters)
    {
      object result = null;
      var infoParams = info.GetParameters();
      object[] inParams = null;
      if (parameters == null)
        inParams = new object[] { null };
      else
        inParams = parameters;
      var pCount = infoParams.Length;
      if ((pCount > 0 && infoParams[pCount - 1].GetCustomAttributes(typeof(ParamArrayAttribute), true).Length > 0) || (pCount == 1 && infoParams[0].ParameterType.IsArray))
      {
        // last param is a param array or only param is an array
        var extras = inParams.Length - (pCount - 1);
        // 1 or more params go in the param array
        // copy extras into an array
        object[] extraArray = GetExtrasArray(extras, infoParams[pCount - 1].ParameterType);
        Array.Copy(inParams, extraArray, extras);
        //For pos = 0 To extras - 1
        //  extraArray(pos) = inParams(pCount - 1 + pos)
        //Next

        // copy items into new array
        object[] paramList = new object[pCount];
        for (var pos = 0; pos <= pCount - 2; pos++)
        {
          paramList[pos] = parameters[pos];
        }
        paramList[paramList.Length - 1] = extraArray;

        // use new array
        inParams = paramList;
      }
      try
      {
        result = info.Invoke(obj, inParams);
      }
      catch (Exception e)
      {
        Exception inner = null;
        if (e.InnerException == null)
          inner = e;
        else
          inner = e.InnerException;
        throw new CallMethodException(info.Name + " " + Resources.MethodCallFailed, inner);
      }
      return result;
    }

    /// <summary>
    /// Gets a property value.
    /// </summary>
    /// <param name="obj">Object containing the property.</param>
    /// <param name="info">Property info object for the property.</param>
    /// <returns>The value of the property.</returns>
    public static object GetPropertyValue(object obj, PropertyInfo info)
    {
      object result = null;
      try
      {
        result = info.GetValue(obj, null);
      }
      catch (Exception e)
      {
        Exception inner = null;
        if (e.InnerException == null)
          inner = e;
        else
          inner = e.InnerException;
        throw new CallMethodException(info.Name + " " + Resources.MethodCallFailed, inner);
      }
      return result;
    }

    /// <summary>
    /// Invokes an instance method on an object.
    /// </summary>
    /// <param name="obj">Object containing method.</param>
    /// <param name="info">Method info object.</param>
    /// <returns>Any value returned from the method.</returns>
    public static object CallMethod(object obj, MethodInfo info)
    {
      object result = null;
      try
      {
        result = info.Invoke(obj, null);
      }
      catch (Exception e)
      {
        Exception inner = null;
        if (e.InnerException == null)
          inner = e;
        else
          inner = e.InnerException;
        throw new CallMethodException(info.Name + " " + Resources.MethodCallFailed, inner);
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
      MethodInfo factory = objectType.GetMethod(
           method, factoryFlags, null,
           MethodCaller.GetParameterTypes(parameters), null);

      if (factory == null)
      {
        // strongly typed factory couldn't be found
        // so find one with the correct number of
        // parameters 
        int parameterCount = parameters.Length;
        MethodInfo[] methods = objectType.GetMethods(factoryFlags);
        foreach (MethodInfo oneMethod in methods)
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
        Exception inner = null;
        if (ex.InnerException == null)
          inner = ex;
        else
          inner = ex.InnerException;
        throw new CallMethodException(factory.Name + " " + Resources.MethodCallFailed, inner);
      }
      return returnValue;
    }

    private static object[] GetExtrasArray(int count, Type arrayType)
    {
      return (object[])(System.Array.CreateInstance(arrayType.GetElementType(), count));
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
    /// Uses reflection to locate a matching method
    /// on the target object.
    /// </summary>
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

    /// <summary>
    /// Gets a MethodInfo object corresponding to a
    /// non-public method.
    /// </summary>
    /// <param name="objectType">Object containing the method.</param>
    /// <param name="method">Name of the method.</param>
    public static MethodInfo GetNonPublicMethod(Type objectType, string method)
    {

      MethodInfo result = null;

      result = FindMethod(objectType, method, privateMethodFlags);

      return result;
    }

    private static MethodInfo FindMethodUsingFuzzyMatching(Type objectType, string method, object[] parameters)
    {
      MethodInfo result = null;
      Type currentType = objectType;
      do
      {
        MethodInfo[] methods = currentType.GetMethods(oneLevelFlags);
        int parameterCount = parameters.Length;
        // Match based on name and parameter types and parameter arrays
        foreach (MethodInfo m in methods)
        {
          if (m.Name == method)
          {
            var infoParams = m.GetParameters();
            var pCount = infoParams.Length;
            if (pCount > 0 &&
               ((pCount == 1 && infoParams[0].ParameterType.IsArray) ||
               (infoParams[pCount - 1].GetCustomAttributes(typeof(ParamArrayAttribute), true).Length > 0)))
            {
              // last param is a param array or only param is an array
              if (parameterCount >= pCount - 1)
              {
                // got a match so use it
                result = m;
                break;
              }
            }
          }
        }
        if (result == null)
        {
          // match based on parameter name and number of parameters
          foreach (MethodInfo m in methods)
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

    internal static Type[] GetParameterTypes(object[] parameters)
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
    public static Type GetObjectType(object criteria)
    {
      if (criteria.GetType().IsSubclassOf(typeof(CriteriaBase)))
      {
        // get the type of the actual business object
        // from CriteriaBase 
        return ((CriteriaBase)criteria).ObjectType;
      }
      else
      {
        // get the type of the actual business object
        // based on the nested class scheme in the book
        return criteria.GetType().DeclaringType;
      }
    }

    /// <summary>
    /// Returns information about the specified
    /// method, even if the parameter types are
    /// generic and are located in an abstract
    /// generic base class.
    /// </summary>
    /// <param name="objType">Type of object.</param>
    /// <param name="method">Name of the method.</param>
    /// <param name="types">Types of parameters to be passed to method.</param>
    public static MethodInfo FindMethod(Type objType, string method, Type[] types)
    {
      MethodInfo info = null;
      do
      {
        // find for a strongly typed match
        info = objType.GetMethod(method, oneLevelFlags, null, types, null);
        if (info != null)
          break; // match found
        objType = objType.BaseType;
      } while (objType != null);

      return info;
    }

    /// <summary>
    /// Returns information about the specified
    /// method.
    /// </summary>
    /// <param name="objType">Type of object.</param>
    /// <param name="method">Name of the method.</param>
    /// <param name="flags">Flag values.</param>
    public static MethodInfo FindMethod(Type objType, string method, BindingFlags flags)
    {
      MethodInfo info = null;
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

    /// <summary>
    /// Returns information about the specified
    /// method, finding the method based purely
    /// on the method name and number of parameters.
    /// </summary>
    /// <param name="objType">Type of object.</param>
    /// <param name="method">Name of the method.</param>
    /// <param name="parameterCount">Number of parameters passed to method.</param>
    public static MethodInfo FindMethod(Type objType, string method, int parameterCount)
    {
      // walk up the inheritance hierarchy looking
      // for a method with the right number of
      // parameters
      MethodInfo result = null;
      Type currentType = objType;
      do
      {
        MethodInfo info = currentType.GetMethod(method, oneLevelFlags);
        if (info != null)
        {
          var infoParams = info.GetParameters();
          var pCount = infoParams.Length;
          if ((pCount > 0 && infoParams[pCount - 1].GetCustomAttributes(typeof(ParamArrayAttribute), true).Length > 0) || (pCount == 1 && infoParams[0].ParameterType.IsArray))
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

    /// <summary>
    /// Invokes a property getter using dynamic
    /// method invocation.
    /// </summary>
    /// <param name="obj">Target object.</param>
    /// <param name="property">Property to invoke.</param>
    /// <returns></returns>
    public static object CallPropertyGetter(object obj, string property)
    {
      var prop = obj.GetType().GetProperty(property);
      if (prop != null)
        return prop.GetValue(obj, new object[] { });
      else
        throw new MissingMemberException(property);
    }
  }
}
