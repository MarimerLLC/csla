using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Csla.Properties;

namespace Csla
{
  internal static class MethodCaller
  {
    const BindingFlags allLevelFlags =
      BindingFlags.FlattenHierarchy |
      BindingFlags.Instance |
      BindingFlags.Public |
      BindingFlags.NonPublic;

    const BindingFlags oneLevelFlags =
      BindingFlags.DeclaredOnly |
      BindingFlags.Instance |
      BindingFlags.Public |
      BindingFlags.NonPublic;

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
    public static MethodInfo GetCreateMethod(Type objectType, object criteria)
    {
      MethodInfo method = null;
      if (criteria is int)
      {
        // an "Integer" criteria is a special flag indicating
        // that criteria is empty and should not be used
        method = MethodCaller.GetMethod(objectType, "DataPortal_Create");
      }
      else
        method = MethodCaller.GetMethod(objectType, "DataPortal_Create", criteria);
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
    public static MethodInfo GetFetchMethod(Type objectType, object criteria)
    {
      MethodInfo method = null;
      if (criteria is int)
      {
        // an "Integer" criteria is a special flag indicating
        // that criteria is empty and should not be used
        method = MethodCaller.GetMethod(objectType, "DataPortal_Fetch");
      }
      else
        method = MethodCaller.GetMethod(objectType, "DataPortal_Fetch", criteria);
      return method;
    }

    /// <summary>
    /// Uses reflection to dynamically invoke a method
    /// if that method is implemented on the target object.
    /// </summary>
    public static object CallMethodIfImplemented(
      object obj, string method, params object[] parameters)
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
    public static object CallMethod(
      object obj, string method, params object[] parameters)
    {
      MethodInfo info = GetMethod(obj.GetType(), method, parameters);
      if (info == null)
        throw new NotImplementedException(
          method + " " + Resources.MethodNotImplemented);
      return CallMethod(obj, info, parameters);
    }

    /// <summary>
    /// Uses reflection to dynamically invoke a method,
    /// throwing an exception if it is not
    /// implemented on the target object.
    /// </summary>
    public static object CallMethod(
      object obj, MethodInfo info, params object[] parameters)
    {
      // call a private method on the object
      object result;
      try
      {
        result = info.Invoke(obj, parameters);
      }
      catch (Exception e)
      {
        throw new Csla.Server.CallMethodException(
          info.Name + " " + Resources.MethodCallFailed, e.InnerException);
      }
      return result;
    }

    /// <summary>
    /// Uses reflection to locate a matching method
    /// on the target object.
    /// </summary>
    public static MethodInfo GetMethod(
      Type objectType, string method, params object[] parameters)
    {
      MethodInfo result = null;

      // try to find a strongly typed match

      // first see if there's a matching method
      // where all params match types
      result = FindMethod(objectType, method, GetParameterTypes(parameters));

      if (result == null)
      {
        // no match found - so look for any method
        // with the right number of parameters
        result = FindMethod(objectType, method, parameters.Length);
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
            if (m.Name == method && m.GetParameters().Length == parameters.Length)
            {
              result = m;
              break;
            }
          if (result == null)
            throw;
        }
      }
      return result;
    }

    internal static Type[] GetParameterTypes(object[] parameters)
    {
      List<Type> result = new List<Type>();
      foreach (object item in parameters)
        if (item == null)
          result.Add(typeof(object));
        else
          result.Add(item.GetType());
      return result.ToArray();
    }

    /// <summary>
    /// Returns a business object type based on
    /// the supplied criteria object.
    /// </summary>
    public static Type GetObjectType(object criteria)
    {
      if (criteria.GetType().IsSubclassOf(typeof(Csla.Server.ICriteria)))
      {
        // get the type of the actual business object
        // from ICriteria
        return ((Csla.Server.ICriteria)criteria).ObjectType;
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
    public static MethodInfo FindMethod(Type objType, string method, Type[] types)
    {
      MethodInfo info = null;
      do
      {
        //find for a strongly typed match
        info = objType.GetMethod(method, oneLevelFlags, null, types, null);
        if (info != null)
          break;  //match found

        objType = objType.BaseType;
      } while (objType != null);

      return info;
    }

    /// <summary>
    /// Returns information about the specified
    /// method, finding the method based purely
    /// on the method name and number of parameters.
    /// </summary>
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
          if (info.GetParameters().Length == parameterCount)
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
  }
}
